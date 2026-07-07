workspace "Oficina Mecanica API" "Arquitetura C4 da API de oficina mecanica, com Clean Architecture, monolito modular, Kubernetes local e infraestrutura AWS em evolucao." {
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
        github = softwareSystem "GitHub" "Repositorio privado usado para versionamento e Pull Requests." "GitHub"
        dockerHub = softwareSystem "Docker Hub" "Registry usado hoje pelos manifestos Kubernetes locais para a imagem gbsousadev/oficina-api:1.0." "Registro de Containers"
        terraform = softwareSystem "Terraform dev" "Codigo de infraestrutura que provisiona VPC, subnets publicas/privadas, route tables, internet gateway e ECR." "Terraform"
        dockerImage = softwareSystem "Imagem Docker oficina-api" "Artefato OCI gerado pelo Dockerfile multi-stage da API." "Docker image"
        ecr = softwareSystem "Amazon ECR oficina-api" "Repositorio privado com tags imutaveis e scan on push, provisionado pelo modulo Terraform registry." "Amazon ECR" {
            tags "Amazon Web Services - Elastic Container Registry" "Registry"
        }
        k8sManifests = softwareSystem "Manifestos Kubernetes" "Deployment, Service, Ingress, HPA, ConfigMap, Secrets, PVC e namespace oficina." "Kubernetes YAML"
        aws = softwareSystem "AWS" "Conta de desenvolvimento com VPC e ECR provisionados por Terraform; EKS e RDS estao planejados nas proximas etapas." "Provedor de Nuvem" {
            tags "Amazon Web Services - AWS Cloud" "Planned"
        }

        oficina = softwareSystem "Oficina Mecanica API" "Sistema de atendimento, execucao e acompanhamento de ordens de servico para uma oficina mecanica." {
            tags "Core System"

            api = container "OficinaMecanica.API" "Monolito modular ASP.NET Core que expoe endpoints REST, Swagger, autenticacao JWT, webhook de orcamento e inicializacao do banco." "ASP.NET Core Web API / .NET 10" {
                tags "API"

                controllers = component "Controllers HTTP" "Adaptadores de entrada para Identidade, Atendimento, Administrativo, Gestao de Estoque e Gestao de Ordem de Servico." "ASP.NET Core MVC Controllers" {
                    tags "API"
                }

                security = component "Seguranca e middlewares" "Configura autenticacao JWT, autorizacao por perfis, token tecnico de webhook, Swagger e tratamento global de excecoes." "ASP.NET Core middleware" {
                    tags "API"
                }

                application = component "Casos de Uso da Aplicacao" "Orquestra casos de uso, validacoes, mapeamentos e transacoes de aplicacao." "C# / FluentValidation / AutoMapper" {
                    tags "Application"
                }

                domain = component "Modelo de Dominio" "Contem agregados, entidades, value objects, enums, regras de negocio e contratos de repositories." "C# domain model" {
                    tags "Domain"
                }

                infrastructure = component "Adaptadores de Infraestrutura" "Implementa repositories, EF Core, Unit of Work, migrations, seed, servicos JWT e usuario demo." "C# / EF Core / SQL Server" {
                    tags "Infrastructure"
                }

                dbContext = component "OficinaMecanicaDbContext" "Mapeia os agregados e entidades para tabelas SQL Server." "Entity Framework Core DbContext" {
                    tags "Infrastructure"
                }

                workOrderController = component "OrdensServicoController" "Endpoint REST do fluxo principal de ordem de servico." "ASP.NET Core Controller" {
                    tags "Code"
                }

                openWorkOrderUseCase = component "AbrirOrdemServicoUseCase" "Valida cliente, veiculo, mecanico, catalogos e estoque antes de abrir uma OS." "C# use case" {
                    tags "Code"
                }

                budgetDecisionUseCase = component "NotificarDecisaoOrcamentoUseCase" "Processa aprovacao ou recusa de orcamento recebida por webhook tecnico." "C# use case" {
                    tags "Code"
                }

                workOrderAggregate = component "OrdemServico aggregate" "Controla transicoes de estado, orcamento, execucao, finalizacao, entrega e cancelamento." "C# aggregate" {
                    tags "Code" "Domain"
                }

                stockAggregate = component "Estoque aggregate" "Controla disponibilidade, reserva, baixa, estorno e entrada de pecas/insumos." "C# aggregate" {
                    tags "Code" "Domain"
                }

                repositories = component "Implementacoes de Repositorio" "Implementa repositories dos contextos Administrativo, Atendimento, Estoque e Ordem de Servico." "EF Core repositories" {
                    tags "Code" "Infrastructure"
                }

                unitOfWork = component "UnitOfWork" "Confirma transacoes de persistencia envolvendo OS e estoque." "EF Core transaction boundary" {
                    tags "Code" "Infrastructure"
                }
            }

            database = container "OficinaMecanicaDb" "Banco relacional da aplicacao, criado localmente por Docker Compose/Kubernetes e acessado via EF Core." "SQL Server 2022" {
                tags "Database"
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
        dockerImage -> dockerHub "Imagem publicada para uso local atual" "Docker push"
        dockerImage -> ecr "Imagem planejada para pipeline AWS" "Docker push"
        k8sManifests -> dockerHub "Referencia imagem atual gbsousadev/oficina-api:1.0" "imagePull"

        oficina.api -> oficina.database "Le e grava dados operacionais" "EF Core / TDS"
        oficina.api.controllers -> oficina.api.security "Aplica autenticacao, autorizacao, Swagger e tratamento de erros" "ASP.NET Core pipeline"
        oficina.api.controllers -> oficina.api.application "Invoca use cases" "C# method calls"
        oficina.api.application -> oficina.api.domain "Executa regras de negocio" "C# method calls"
        oficina.api.application -> oficina.api.infrastructure "Usa repositories e Unit of Work via DI" "C# interfaces / DI"
        oficina.api.infrastructure -> oficina.api.dbContext "Executa consultas e persistencia" "Entity Framework Core"
        oficina.api.dbContext -> oficina.database "Mapeia entidades e aplica migrations" "EF Core / TDS"
        oficina.api.infrastructure -> oficina.api.domain "Materializa agregados e aplica contratos do dominio" "C# references"

        oficina.api.workOrderController -> oficina.api.openWorkOrderUseCase "Chama abertura de OS" "C# method call"
        oficina.api.workOrderController -> oficina.api.budgetDecisionUseCase "Chama notificacao de decisao de orcamento" "C# method call"
        atendente -> oficina.api.workOrderController "Abre ordens de servico" "HTTPS/JSON"
        canalOrcamento -> oficina.api.workOrderController "Notifica decisao de orcamento" "Webhook HTTP"
        oficina.api.workOrderController -> oficina.api.security "Valida JWT, perfis e token tecnico" "ASP.NET Core filters/middleware"
        oficina.api.openWorkOrderUseCase -> oficina.api.workOrderAggregate "Cria OS e calcula orcamento" "C# domain calls"
        oficina.api.openWorkOrderUseCase -> oficina.api.stockAggregate "Verifica e reserva pecas/insumos" "C# domain calls"
        oficina.api.budgetDecisionUseCase -> oficina.api.workOrderAggregate "Aprova ou recusa orcamento" "C# domain calls"
        oficina.api.budgetDecisionUseCase -> oficina.api.stockAggregate "Estorna estoque quando necessario" "C# domain calls"
        oficina.api.openWorkOrderUseCase -> oficina.api.repositories "Busca cliente, veiculo, mecanico, catalogos, estoque e OS" "Repository interfaces"
        oficina.api.budgetDecisionUseCase -> oficina.api.repositories "Carrega OS e estoque" "Repository interfaces"
        oficina.api.openWorkOrderUseCase -> oficina.api.unitOfWork "Confirma alteracoes" "C# transaction boundary"
        oficina.api.budgetDecisionUseCase -> oficina.api.unitOfWork "Confirma alteracoes" "C# transaction boundary"
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
                        tags "Amazon Web Services - RDS" "Planned"
                    }
                    eksPlanned = infrastructureNode "Amazon EKS" "Cluster Kubernetes planejado para as proximas etapas." "Amazon EKS" {
                        tags "Amazon Web Services - Elastic Kubernetes Service" "Planned"
                        -> ecrRepo "Puxara imagens da API" "OCI image pull"
                        -> rdsPlanned "Usara banco gerenciado" "TDS"
                    }
                }
            }
        }

    }

    views {
        themes amazon-web-services-2025.07 kubernetes

        systemLandscape "SystemLandscape" "Nivel 0 - paisagem de sistemas e plataformas relacionadas." {
            include *
        }

        systemContext oficina "SystemContext" "Nivel 1 - contexto do sistema Oficina Mecanica API." {
            include *
        }

        container oficina "Containers" "Nivel 2 - containers executaveis e banco de dados." {
            include *
        }

        component oficina.api "ComponentsApi" "Nivel 3 - componentes principais do monolito modular." {
            include oficina.api.controllers
            include oficina.api.security
            include oficina.api.application
            include oficina.api.domain
            include oficina.api.infrastructure
            include oficina.api.dbContext
            include oficina.database
        }

        component oficina.api "CodeWorkOrder" "Nivel 4 - detalhe de codigo do fluxo critico de Ordem de Servico." {
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

        dynamic oficina.api "DynamicOpenWorkOrder" "Fluxo dinamico - abertura de ordem de servico com servicos e pecas opcionais." {
            atendente -> oficina.api.workOrderController "POST /api/v1/gestao-ordem-servico/ordens-servico/cadastrar"
            oficina.api.workOrderController -> oficina.api.openWorkOrderUseCase "Executa abertura"
            oficina.api.openWorkOrderUseCase -> oficina.api.repositories "Valida cliente, veiculo, mecanico e catalogos"
            oficina.api.repositories -> oficina.api.dbContext "Consulta dados"
            oficina.api.openWorkOrderUseCase -> oficina.api.stockAggregate "Verifica disponibilidade e reserva itens"
            oficina.api.openWorkOrderUseCase -> oficina.api.workOrderAggregate "Cria OS e registra itens iniciais"
            oficina.api.openWorkOrderUseCase -> oficina.api.unitOfWork "Confirma transacao"
            oficina.api.dbContext -> oficina.database "Persiste OS, itens e reservas"
        }

        dynamic oficina.api "DynamicBudgetDecision" "Fluxo dinamico - decisao externa de orcamento." {
            canalOrcamento -> oficina.api.workOrderController "POST /ordens-servico/{id}/orcamento/notificacoes com X-Webhook-Token"
            oficina.api.workOrderController -> oficina.api.security "Valida token tecnico"
            oficina.api.workOrderController -> oficina.api.budgetDecisionUseCase "Processa decisao"
            oficina.api.budgetDecisionUseCase -> oficina.api.repositories "Carrega OS e estoque"
            oficina.api.budgetDecisionUseCase -> oficina.api.workOrderAggregate "Aprova para execucao ou cancela por reprovacao"
            oficina.api.budgetDecisionUseCase -> oficina.api.stockAggregate "Estorna reservas quando reprovado"
            oficina.api.budgetDecisionUseCase -> oficina.api.unitOfWork "Confirma transacao"
            oficina.api.dbContext -> oficina.database "Atualiza status e estoque"
        }

        deployment oficina local "DeploymentLocalCompose" "Deployment - execucao local via Docker Compose." {
            include *
        }

        deployment oficina kubernetesLocal "DeploymentKubernetesLocal" "Deployment - Kubernetes local no Docker Desktop." {
            include *
        }

        deployment oficina awsDev "DeploymentAwsDev" "Deployment - infraestrutura AWS dev atual e proximos passos planejados." {
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

            element "Core System" {
                background #0b7285
                color #ffffff
            }

            element "Support System" {
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

            element "Application" {
                background #6f42c1
                color #ffffff
            }

            element "Domain" {
                background #2f9e44
                color #ffffff
            }

            element "Infrastructure" {
                background #495057
                color #ffffff
            }

            element "Code" {
                background #f59f00
                color #000000
            }

            element "Database" {
                shape Cylinder
                background #7048e8
                color #ffffff
            }

            element "AWS" {
                background #ff9900
                color #000000
            }

            element "Registry" {
                background #cc7a00
                color #000000
            }

            element "Artifact" {
                background #7950f2
                color #ffffff
            }

            element "Planned" {
                opacity 50
            }

            relationship "Relationship" {
                color #495057
            }
        }
    }

    configuration {
        scope softwaresystem
    }
}
