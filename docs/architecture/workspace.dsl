workspace "Oficina Mecanica API" "Arquitetura C4 da API de oficina mecanica, com arquitetura limpa, monolito modular, Kubernetes local e infraestrutura AWS em evolucao." {
    !identifiers hierarchical

    model {
        properties {
            "structurizr.groupSeparator" "/"
        }

        cliente = person "Cliente" "Consulta o status publico de uma ordem de servico."
        atendente = person "Atendente" "Cadastra clientes e veiculos, abre ordens de servico, envia orcamentos e registra entregas."
        mecanico = person "Mecanico" "Executa diagnostico, define servicos, reserva pecas e finaliza trabalhos."
        administrador = person "Administrador" "Mantem cadastros administrativos e apoia a operacao."

        canalOrcamento = softwareSystem "Canal externo de orcamento" "Canal externo usado para enviar orcamento ao cliente e notificar aprovacao ou recusa via webhook." "Sistema Externo"
        github = softwareSystem "GitHub" "Repositorio privado usado para versionamento e revisoes via PR." "GitHub"
        dockerHub = softwareSystem "Docker Hub" "Registro usado hoje pelos manifestos Kubernetes locais para a imagem gbsousadev/oficina-api:1.0." "Registro de Containers"
        terraform = softwareSystem "Terraform dev" "Codigo de infraestrutura que provisiona VPC, subnets publicas/privadas, route tables, internet gateway e ECR." "Terraform"
        dockerImage = softwareSystem "Imagem Docker oficina-api" "Artefato OCI gerado pelo Dockerfile multi-stage da API." "Imagem Docker"
        ecr = softwareSystem "Amazon ECR oficina-api" "Repositorio privado com tags imutaveis e scan on push, provisionado pelo modulo Terraform registry." "Amazon ECR" {
            tags "Amazon Web Services - Elastic Container Registry" "Registro"
        }
        k8sManifests = softwareSystem "Manifestos Kubernetes" "Deployment, Service, Ingress, HPA, ConfigMap, Secrets, PVC e namespace oficina." "Kubernetes YAML"
        aws = softwareSystem "AWS" "Conta de desenvolvimento com VPC e ECR provisionados por Terraform; EKS e RDS estao planejados nas proximas etapas." "Provedor de Nuvem" {
            tags "Amazon Web Services - AWS Cloud" "Planejado"
        }

        oficina = softwareSystem "Oficina Mecanica API" "Sistema de atendimento, execucao e acompanhamento de ordens de servico para uma oficina mecanica." {
            tags "Sistema Principal"

            api = container "OficinaMecanica.API" "Monolito modular ASP.NET Core que expoe endpoints REST, Swagger, autenticacao JWT, webhook de orcamento e inicializacao do banco." "ASP.NET Core Web API / .NET 10" {
                tags "API"

                controllers = component "Controladores HTTP" "Adaptadores de entrada para Identidade, Atendimento, Administrativo, Gestao de Estoque e Gestao de Ordem de Servico." "Controladores MVC ASP.NET Core" {
                    tags "API"
                }

                security = component "Seguranca e middlewares" "Configura autenticacao JWT, autorizacao por perfis, token tecnico de webhook, Swagger e tratamento global de excecoes." "ASP.NET Core middleware" {
                    tags "API"
                }

                application = component "Casos de uso da aplicacao" "Orquestra casos de uso, validacoes, mapeamentos e transacoes de aplicacao." "C# / FluentValidation / AutoMapper" {
                    tags "Aplicacao"
                }

                domain = component "Modelo de dominio" "Contem agregados, entidades, objetos de valor, enums, regras de negocio e contratos de repositorio." "Modelo de dominio C#" {
                    tags "Dominio"
                }

                infrastructure = component "Adaptadores de infraestrutura" "Implementa repositorios, EF Core, unidade de trabalho, migrations, seed, servicos JWT e usuario demo." "C# / EF Core / SQL Server" {
                    tags "Infraestrutura"
                }

                dbContext = component "OficinaMecanicaDbContext" "Mapeia os agregados e entidades para tabelas SQL Server." "Entity Framework Core DbContext" {
                    tags "Infraestrutura"
                }

                workOrderController = component "OrdensServicoController" "Endpoint REST do fluxo principal de ordem de servico." "ASP.NET Core Controller" {
                    tags "Codigo"
                }

                openWorkOrderUseCase = component "AbrirOrdemServicoUseCase" "Valida cliente, veiculo, mecanico, catalogos e estoque antes de abrir uma OS." "Caso de uso C#" {
                    tags "Codigo"
                }

                budgetDecisionUseCase = component "NotificarDecisaoOrcamentoUseCase" "Processa aprovacao ou recusa de orcamento recebida por webhook tecnico." "Caso de uso C#" {
                    tags "Codigo"
                }

                workOrderAggregate = component "Agregado OrdemServico" "Controla transicoes de estado, orcamento, execucao, finalizacao, entrega e cancelamento." "Agregado C#" {
                    tags "Codigo" "Dominio"
                }

                stockAggregate = component "Agregado Estoque" "Controla disponibilidade, reserva, baixa, estorno e entrada de pecas/insumos." "Agregado C#" {
                    tags "Codigo" "Dominio"
                }

                repositories = component "Implementacoes de repositorio" "Implementa repositorios dos contextos Administrativo, Atendimento, Estoque e Ordem de Servico." "Repositorios EF Core" {
                    tags "Codigo" "Infraestrutura"
                }

                unitOfWork = component "Unidade de trabalho" "Confirma transacoes de persistencia envolvendo OS e estoque." "Fronteira transacional EF Core" {
                    tags "Codigo" "Infraestrutura"
                }
            }

            database = container "OficinaMecanicaDb" "Banco relacional da aplicacao, criado localmente por Docker Compose/Kubernetes e acessado via EF Core." "SQL Server 2022" {
                tags "Banco de Dados"
            }
        }

        atendente -> oficina.api "Usa para atendimento, abertura de OS, aprovacoes e entrega" "HTTPS/JSON"
        mecanico -> oficina.api "Usa para diagnostico, reserva de pecas e execucao de servicos" "HTTPS/JSON"
        administrador -> oficina.api "Mantem cadastros administrativos" "HTTPS/JSON"
        cliente -> oficina.api "Consulta status publico da ordem de servico" "HTTPS/JSON"
        atendente -> canalOrcamento "Envia orcamento ao cliente por canal externo" "Manual/externo"
        canalOrcamento -> oficina.api "Notifica decisao do orcamento" "Webhook HTTP com X-Webhook-Token"

        github -> terraform "Versiona codigo de infraestrutura" "Git"
        github -> k8sManifests "Versiona manifestos" "Git"
        terraform -> aws "Provisiona recursos dev" "Terraform AWS provider"
        dockerImage -> dockerHub "Imagem publicada para uso local atual" "Publicacao Docker"
        dockerImage -> ecr "Imagem planejada para pipeline AWS" "Publicacao Docker"
        k8sManifests -> dockerHub "Referencia imagem atual gbsousadev/oficina-api:1.0" "Pull de imagem"

        oficina.api -> oficina.database "Le e grava dados operacionais" "EF Core / TDS"
        oficina.api.controllers -> oficina.api.security "Aplica autenticacao, autorizacao, Swagger e tratamento de erros" "ASP.NET Core pipeline"
        oficina.api.controllers -> oficina.api.application "Invoca casos de uso" "Chamadas C#"
        oficina.api.application -> oficina.api.domain "Executa regras de negocio" "Chamadas C#"
        oficina.api.application -> oficina.api.infrastructure "Usa repositorios e unidade de trabalho via DI" "Interfaces C# / DI"
        oficina.api.infrastructure -> oficina.api.dbContext "Executa consultas e persistencia" "Entity Framework Core"
        oficina.api.dbContext -> oficina.database "Mapeia entidades e aplica migrations" "EF Core / TDS"
        oficina.api.infrastructure -> oficina.api.domain "Materializa agregados e aplica contratos do dominio" "Referencias C#"

        oficina.api.workOrderController -> oficina.api.openWorkOrderUseCase "Chama abertura de OS" "Chamada C#"
        oficina.api.workOrderController -> oficina.api.budgetDecisionUseCase "Chama notificacao de decisao de orcamento" "Chamada C#"
        atendente -> oficina.api.workOrderController "Abre ordens de servico" "HTTPS/JSON"
        canalOrcamento -> oficina.api.workOrderController "Notifica decisao de orcamento" "Webhook HTTP"
        oficina.api.workOrderController -> oficina.api.security "Valida JWT, perfis e token tecnico" "ASP.NET Core filters/middleware"
        oficina.api.openWorkOrderUseCase -> oficina.api.workOrderAggregate "Cria OS e calcula orcamento" "Chamadas de dominio C#"
        oficina.api.openWorkOrderUseCase -> oficina.api.stockAggregate "Verifica e reserva pecas/insumos" "Chamadas de dominio C#"
        oficina.api.budgetDecisionUseCase -> oficina.api.workOrderAggregate "Aprova ou recusa orcamento" "Chamadas de dominio C#"
        oficina.api.budgetDecisionUseCase -> oficina.api.stockAggregate "Estorna estoque quando necessario" "Chamadas de dominio C#"
        oficina.api.openWorkOrderUseCase -> oficina.api.repositories "Busca cliente, veiculo, mecanico, catalogos, estoque e OS" "Interfaces de repositorio"
        oficina.api.budgetDecisionUseCase -> oficina.api.repositories "Carrega OS e estoque" "Interfaces de repositorio"
        oficina.api.openWorkOrderUseCase -> oficina.api.unitOfWork "Confirma alteracoes" "Fronteira transacional C#"
        oficina.api.budgetDecisionUseCase -> oficina.api.unitOfWork "Confirma alteracoes" "Fronteira transacional C#"
        oficina.api.repositories -> oficina.api.dbContext "Consulta e persiste entidades" "Entity Framework Core"

        local = deploymentEnvironment "Local Docker Compose" {
            deploymentNode "Estacao de desenvolvimento" "Estacao Windows com Docker Desktop." "Windows / Docker Desktop" {
                deploymentNode "Docker Compose" "Ambiente definido em docker-compose.yml." "Docker Compose" {
                    deploymentNode "Servico API" "Container oficina-mecanica-api expondo localhost:5093 -> 8080." "Docker container" {
                        apiInstance = containerInstance oficina.api
                    }

                    deploymentNode "Servico SQL Server" "Container oficina-mecanica-sqlserver expondo localhost:14333 -> 1433." "Docker container" {
                        dbInstance = containerInstance oficina.database
                    }
                }
            }
        }

        kubernetesLocal = deploymentEnvironment "Kubernetes Local" {
            deploymentNode "Estacao de desenvolvimento" "Estacao Windows com Docker Desktop Kubernetes." "Windows / Docker Desktop" {
                deploymentNode "Cluster local" "Cluster Kubernetes do Docker Desktop." "Kubernetes" {
                    tags "Kubernetes - control-plane"

                    deploymentNode "Namespace oficina" "Namespace dedicado da aplicacao." "Kubernetes Namespace" {
                        tags "Kubernetes - ns"

                        pvc = infrastructureNode "PVC sqlserver-pvc" "Volume persistente para dados do SQL Server." "PersistentVolumeClaim" {
                            tags "Kubernetes - pvc"
                        }

                        deploymentNode "Deployment sqlserver" "Deployment do SQL Server 2022 Developer." "Kubernetes Deployment" {
                            tags "Kubernetes - deploy"

                            dbK8sInstance = containerInstance oficina.database {
                                -> pvc "Persiste dados" "Volume mount"
                            }
                        }

                        sqlService = infrastructureNode "Service sqlserver" "Service interno para SQL Server." "Kubernetes Service" {
                            tags "Kubernetes - svc"

                            -> oficina.database "Encaminha para Pod SQL Server" "TDS"
                        }

                        deploymentNode "Deployment oficina-api" "Deployment com startup, liveness e readiness probes em /api/health." "Kubernetes Deployment" {
                            tags "Kubernetes - deploy"

                            apiK8sInstance = containerInstance oficina.api {
                                -> sqlService "Acessa banco por connection string em Secret" "TDS"
                            }
                        }

                        apiService = infrastructureNode "Service oficina-api" "Service ClusterIP na porta 8080." "Kubernetes Service" {
                            tags "Kubernetes - svc"

                            -> oficina.api "Balanceia para Pods da API" "HTTP"
                        }

                        ingress = infrastructureNode "Ingress oficina-api" "Roteia oficina.local para o service da API." "NGINX Ingress" {
                            tags "Kubernetes - ing"

                            -> apiService "Encaminha trafego HTTP" "HTTP"
                        }

                        hpa = infrastructureNode "HPA oficina-api-hpa" "Escala a API de 1 a 5 replicas por CPU e memoria." "HorizontalPodAutoscaler" {
                            tags "Kubernetes - hpa"

                            -> oficina.api "Ajusta replicas com base em CPU e memoria" "Kubernetes metrics"
                        }
                    }
                }
            }
        }

        awsDev = deploymentEnvironment "AWS Dev Terraform" {
            deploymentNode "Amazon Web Services" "Conta de desenvolvimento AWS modelada pelo Terraform." "AWS" {
                tags "Amazon Web Services - AWS Cloud"

                deploymentNode "Regiao us-east-1" "Regiao alvo do ambiente dev." "AWS Region" {
                    tags "Amazon Web Services - Region"

                    deploymentNode "VPC oficina-vpc-dev" "CIDR 10.0.0.0/16 com DNS support e hostnames habilitados." "Amazon VPC" {
                        tags "Amazon Web Services - Virtual Private Cloud"

                        publicSubnetA = infrastructureNode "Subnet publica us-east-1a" "CIDR 10.0.1.0/24." "Amazon VPC subnet" {
                            tags "Amazon Web Services - Public subnet"
                        }
                        publicSubnetB = infrastructureNode "Subnet publica us-east-1b" "CIDR 10.0.2.0/24." "Amazon VPC subnet" {
                            tags "Amazon Web Services - Public subnet"
                        }
                        privateSubnetA = infrastructureNode "Subnet privada us-east-1a" "CIDR 10.0.11.0/24." "Amazon VPC subnet" {
                            tags "Amazon Web Services - Private subnet"
                        }
                        privateSubnetB = infrastructureNode "Subnet privada us-east-1b" "CIDR 10.0.12.0/24." "Amazon VPC subnet" {
                            tags "Amazon Web Services - Private subnet"
                        }
                        publicRouteTable = infrastructureNode "Tabela de rotas publica" "Tabela de rotas publica." "Amazon VPC Route Table" {
                            -> publicSubnetA "Associa subnet publica" "AWS route table association"
                            -> publicSubnetB "Associa subnet publica" "AWS route table association"
                        }
                        privateRouteTable = infrastructureNode "Tabela de rotas privada" "Tabela de rotas privada." "Amazon VPC Route Table" {
                            -> privateSubnetA "Associa subnet privada" "AWS route table association"
                            -> privateSubnetB "Associa subnet privada" "AWS route table association"
                        }
                        internetGateway = infrastructureNode "Internet Gateway" "Gateway de internet conectado a VPC." "Amazon VPC Internet Gateway" {
                            -> publicRouteTable "Fornece rota para Internet" "AWS route"
                        }
                    }

                    ecrRepo = infrastructureNode "Amazon ECR oficina-api" "Repositorio privado provisionado com image_tag_mutability IMMUTABLE e scan_on_push." "Amazon ECR" {
                        tags "Amazon Web Services - Elastic Container Registry"
                    }
                    rdsPlanned = infrastructureNode "Amazon RDS SQL Server" "Banco gerenciado planejado para substituir SQL Server em container no ambiente AWS." "Amazon RDS" {
                        tags "Amazon Web Services - RDS" "Planejado"
                    }
                    eksPlanned = infrastructureNode "Amazon EKS" "Cluster Kubernetes planejado para as proximas etapas." "Amazon EKS" {
                        tags "Amazon Web Services - Elastic Kubernetes Service" "Planejado"
                        -> ecrRepo "Puxara imagens da API" "OCI image pull"
                        -> rdsPlanned "Usara banco gerenciado" "TDS"
                    }
                }
            }
        }

    }

    views {
        themes amazon-web-services-2025.07 kubernetes

        systemLandscape "PaisagemSistemas" "Nivel 0 - paisagem de sistemas e plataformas relacionadas." {
            title "Paisagem de sistemas"
            include *
        }

        systemContext oficina "ContextoSistema" "Nivel 1 - contexto do sistema Oficina Mecanica API." {
            title "Contexto da Oficina Mecanica API"
            include *
        }

        container oficina "ContainersAplicacao" "Nivel 2 - containers executaveis e banco de dados." {
            title "Containers da aplicacao"
            include *
        }

        component oficina.api "ComponentesApi" "Nivel 3 - componentes principais do monolito modular." {
            title "Componentes da API"
            include oficina.api.controllers
            include oficina.api.security
            include oficina.api.application
            include oficina.api.domain
            include oficina.api.infrastructure
            include oficina.api.dbContext
            include oficina.database
        }

        component oficina.api "CodigoOrdemServico" "Nivel 4 - detalhe de codigo da Ordem de Servico." {
            title "Detalhe de codigo: Ordem de Servico"
            include oficina.api.workOrderController
            include oficina.api.openWorkOrderUseCase
            include oficina.api.budgetDecisionUseCase
            include oficina.api.workOrderAggregate
            include oficina.api.stockAggregate
            include oficina.api.repositories
            include oficina.api.unitOfWork
            include oficina.api.dbContext
            include oficina.database
        }

        deployment oficina local "ImplantacaoDockerComposeLocal" "Implantacao - execucao local via Docker Compose." {
            title "Implantacao local: Docker Compose"
            include *
        }

        deployment oficina kubernetesLocal "ImplantacaoKubernetesLocal" "Implantacao - Kubernetes local no Docker Desktop." {
            title "Implantacao local: Kubernetes"
            include *
        }

        deployment oficina awsDev "ImplantacaoAwsDesenvolvimento" "Implantacao - infraestrutura AWS de desenvolvimento e proximos passos planejados." {
            title "Implantacao AWS: desenvolvimento"
            include *
        }

        styles {
            element "Person" {
                shape Person
                background #08427b
                color #ffffff
            }

            element "Software System" {
                background #1168bd
                color #ffffff
            }

            element "Sistema Principal" {
                background #0b7285
                color #ffffff
            }

            element "Sistema de Apoio" {
                background #5f6368
                color #ffffff
            }

            element "Container" {
                background #438dd5
                color #ffffff
            }

            element "Component" {
                background #85bbf0
                color #000000
            }

            element "API" {
                background #0b7285
                color #ffffff
            }

            element "Aplicacao" {
                background #6f42c1
                color #ffffff
            }

            element "Dominio" {
                background #2f9e44
                color #ffffff
            }

            element "Infraestrutura" {
                background #495057
                color #ffffff
            }

            element "Codigo" {
                background #f59f00
                color #000000
            }

            element "Banco de Dados" {
                shape Cylinder
                background #7048e8
                color #ffffff
            }

            element "AWS" {
                background #ff9900
                color #000000
            }

            element "Registro" {
                background #cc7a00
                color #000000
            }

            element "Artefato" {
                background #7950f2
                color #ffffff
            }

            element "Planejado" {
                opacity 50
            }

            element "Amazon Web Services - AWS Cloud" {
                width 180
                height 110
                background #ffffff
                color #232f3e
                stroke #ff9900
                strokeWidth 2
                metadata false
                description false
            }

            element "Amazon Web Services - Region" {
                width 180
                height 110
                background #ffffff
                color #232f3e
                stroke #ff9900
                strokeWidth 2
                metadata false
                description false
            }

            element "Amazon Web Services - Virtual Private Cloud" {
                width 220
                height 130
                background #ffffff
                color #232f3e
                stroke #ff9900
                strokeWidth 2
                metadata false
                description false
            }

            element "Amazon Web Services - Public subnet" {
                width 180
                height 100
                background #ffffff
                color #232f3e
                stroke #ff9900
                metadata false
                description false
            }

            element "Amazon Web Services - Private subnet" {
                width 180
                height 100
                background #ffffff
                color #232f3e
                stroke #ff9900
                metadata false
                description false
            }

            element "Amazon Web Services - Elastic Container Registry" {
                width 190
                height 120
                background #ffffff
                color #232f3e
                stroke #ff9900
                strokeWidth 2
                metadata false
                description false
            }

            element "Amazon Web Services - RDS" {
                width 190
                height 120
                background #ffffff
                color #232f3e
                stroke #ff9900
                strokeWidth 2
                metadata false
                description false
            }

            element "Amazon Web Services - Elastic Kubernetes Service" {
                width 190
                height 120
                background #ffffff
                color #232f3e
                stroke #ff9900
                strokeWidth 2
                metadata false
                description false
            }

            element "Kubernetes - control-plane" {
                width 190
                height 110
                background #ffffff
                color #326ce5
                stroke #326ce5
                strokeWidth 2
                metadata false
                description false
            }

            element "Kubernetes - ns" {
                width 180
                height 110
                background #ffffff
                color #326ce5
                stroke #326ce5
                strokeWidth 2
                metadata false
                description false
            }

            element "Kubernetes - deploy" {
                width 180
                height 110
                background #ffffff
                color #326ce5
                stroke #326ce5
                strokeWidth 2
                metadata false
                description false
            }

            element "Kubernetes - svc" {
                width 170
                height 100
                background #ffffff
                color #326ce5
                stroke #326ce5
                strokeWidth 2
                metadata false
                description false
            }

            element "Kubernetes - ing" {
                width 170
                height 100
                background #ffffff
                color #326ce5
                stroke #326ce5
                strokeWidth 2
                metadata false
                description false
            }

            element "Kubernetes - hpa" {
                width 170
                height 100
                background #ffffff
                color #326ce5
                stroke #326ce5
                strokeWidth 2
                metadata false
                description false
            }

            element "Kubernetes - pvc" {
                width 170
                height 100
                background #ffffff
                color #326ce5
                stroke #326ce5
                strokeWidth 2
                metadata false
                description false
            }

            relationship "Relationship" {
                color #495057
            }
        }

        terminology {
            person "Pessoa"
            softwareSystem "Sistema"
            container "Container"
            component "Componente"
            deploymentNode "No de implantacao"
            infrastructureNode "Recurso de infraestrutura"
            relationship "Relacionamento"
        }
    }

    configuration {
        scope softwaresystem
    }
}
