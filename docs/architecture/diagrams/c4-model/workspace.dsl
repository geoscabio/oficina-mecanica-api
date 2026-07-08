workspace "Oficina Mecânica API" "Arquitetura C4 da API de oficina mecânica, focada nos níveis oficiais de contexto, contêineres e componentes." {
    model {
        administrador = person "Administrador" "Mantém cadastros, catálogos, estoque e apoia a operação da oficina."
        atendente = person "Atendente" "Recebe clientes, cadastra veículos, abre ordens de serviço, acompanha orçamentos e registra entregas."
        mecanico = person "Mecânico" "Registra diagnóstico, informa serviços e peças, executa o trabalho e finaliza serviços."
        cliente = person "Cliente" "Acompanha o status público de uma ordem de serviço."

        canalOrcamento = softwareSystem "Canal externo de orçamento" "Canal usado para receber a aprovação ou recusa do orçamento pelo cliente." {
            tags "Sistema Externo"
        }

        oficina = softwareSystem "Oficina Mecânica API" "Sistema que apoia a oficina desde a abertura da ordem de serviço até a entrega ao cliente." {
            tags "Sistema Principal"
            properties {
                structurizr.inspection.model.softwaresystem.documentation ignore
                structurizr.inspection.model.softwaresystem.decisions ignore
            }

            api = container "OficinaMecanica.API" "Aplicação web responsável por atendimentos, cadastros, estoque, orçamentos, execução dos serviços e consulta de status." "ASP.NET Core / .NET 10" {
                tags "API"

                group "Camada API / Entrada" {
                    segurancaApi = component "Segurança e documentação da API" "Autenticação JWT, autorização por perfil, token técnico do webhook, Swagger e tratamento global de erros." "ASP.NET Core middleware, filters e extensions" {
                        tags "Entrada"
                    }

                    entradaHttp = component "Controllers HTTP" "Recebem comandos e consultas dos fluxos da oficina e traduzem a entrada para casos de uso." "ASP.NET Core Controllers" {
                        tags "Entrada"
                    }
                }

                group "Camada de Aplicação" {
                    casosUso = component "Casos de uso" "Orquestram os fluxos de Identidade, Atendimento, Administrativo, Estoque e Ordem de Serviço." "Application use cases" {
                        tags "Aplicação"
                    }

                    validacaoMapeamento = component "Validação e mapeamento" "Valida requisições, monta respostas e converte dados entre API, aplicação e domínio." "FluentValidation e AutoMapper" {
                        tags "Aplicação"
                    }
                }

                group "Camada de Domínio" {
                    dominioOficina = component "Domínio da Oficina" "Agregados, entidades, value objects e enums dos contextos Atendimento, Administrativo, Estoque e Ordem de Serviço." "DDD aggregates, entities, value objects e enums" {
                        tags "Domínio"
                    }

                    contratosRepositorio = component "Contratos de repositório" "Interfaces que protegem os casos de uso contra detalhes de banco de dados e infraestrutura." "Domain repository interfaces" {
                        tags "Domínio"
                    }
                }

                group "Camada de Infraestrutura" {
                    repositoriosUnitOfWork = component "Repositórios e Unit of Work" "Implementam contratos de repositório e coordenam transações de persistência." "EF Core repositories e UnitOfWork" {
                        tags "Infraestrutura"
                    }

                    dbContext = component "OficinaMecanicaDbContext" "Mapeia agregados e entidades para tabelas SQL Server." "EF Core DbContext" {
                        tags "Infraestrutura"
                    }

                    tokenService = component "TokenService" "Gera tokens JWT usados pela autenticação de usuários demo." "JWT" {
                        tags "Infraestrutura"
                    }

                    inicializacaoBanco = component "Inicialização do banco" "Aplica migrations e carrega dados demo conforme a configuração do ambiente." "EF Core migrations e seed" {
                        tags "Infraestrutura"
                    }
                }
            }

            group "Camada de Dados" {
                database = container "OficinaMecanicaDb" "Banco relacional da aplicação." "SQL Server 2022" {
                    tags "Banco de Dados" "Camada de Dados"
                }
            }
        }

        administrador -> oficina "Mantém cadastros, catálogos e estoque" "Uso"
        atendente -> oficina "Abre OS, acompanha orçamentos e registra entregas" "Uso"
        mecanico -> oficina "Registra diagnóstico, serviços e peças" "Uso"
        cliente -> oficina "Consulta o status da OS" "Uso"
        canalOrcamento -> oficina "Envia a decisão do orçamento" "Webhook HTTP"

        administrador -> api "Usa endpoints administrativos e operacionais" "HTTPS/JSON"
        atendente -> api "Usa endpoints de atendimento e ordem de serviço" "HTTPS/JSON"
        mecanico -> api "Usa endpoints de diagnóstico, execução e finalização" "HTTPS/JSON"
        cliente -> api "Consulta status público da ordem de serviço" "HTTPS/JSON"
        canalOrcamento -> api "Envia aprovação ou recusa do orçamento" "Webhook HTTP"

        apiToDatabase = api -> database "Lê e grava dados operacionais" "EF Core"

        segurancaApi -> entradaHttp "Protege chamadas e padroniza respostas" "ASP.NET Core pipeline"
        entradaHttp -> casosUso "Envia comandos e consultas" "C# method call"
        casosUso -> validacaoMapeamento "Valida dados e monta respostas" "FluentValidation / AutoMapper"
        casosUso -> dominioOficina "Executa regras de negócio" "C# domain model"
        casosUso -> contratosRepositorio "Acessa dados por contratos" "C# interfaces"
        casosUso -> tokenService "Solicita emissão de token" "C# service call"
        contratosRepositorio -> repositoriosUnitOfWork "Implementado por" "C# interfaces"
        repositoriosUnitOfWork -> dbContext "Persiste agregados" "EF Core"
        dbContext -> database "Lê e grava tabelas" "EF Core"
        inicializacaoBanco -> dbContext "Aplica migrations e seed demo" "EF Core migrations"
    }

    views {
        systemContext oficina "C4ModelL1SystemContext" {
            title "C4 Model - L1 - Visão de Contexto do Sistema"
            description "Visão para stakeholders: quem usa o sistema, qual problema ele resolve e qual canal externo participa da decisão de orçamento."
            include administrador
            include atendente
            include mecanico
            include cliente
            include canalOrcamento
            include oficina
        }

        container oficina "C4ModelL2Containers" {
            title "C4 Model - L2 - Visão de Contêineres"
            description "Aplicação e banco de dados que compõem o sistema da oficina."
            include administrador
            include atendente
            include mecanico
            include cliente
            include canalOrcamento
            include api
            include database
        }

        component api "C4ModelL3ComponentsApi" {
            title "C4 Model - L3 - Visão de Componentes da API"
            description "Componentes internos da API organizados de fora para dentro: Entrada, Aplicação, Domínio, Infraestrutura e Dados."
            include segurancaApi
            include entradaHttp
            include casosUso
            include validacaoMapeamento
            include dominioOficina
            include contratosRepositorio
            include repositoriosUnitOfWork
            include dbContext
            include tokenService
            include inicializacaoBanco
            include database
            exclude apiToDatabase
        }

        styles {
            element "Person" {
                shape Person
                background #08427b
                color #ffffff
                stroke #073763
                strokeWidth 2
                fontSize 22
            }

            element "Software System" {
                shape RoundedBox
                background #1168bd
                color #ffffff
                stroke #0b4f8a
                strokeWidth 2
                fontSize 22
            }

            element "Sistema Principal" {
                background #1168bd
                color #ffffff
                stroke #0b4f8a
                strokeWidth 2
            }

            element "Sistema Externo" {
                shape RoundedBox
                background #6c757d
                color #ffffff
                stroke #495057
                strokeWidth 2
                fontSize 22
            }

            element "Container" {
                shape RoundedBox
                background #1168bd
                color #ffffff
                stroke #0b4f8a
                strokeWidth 2
                fontSize 22
            }

            element "API" {
                shape RoundedBox
                background #0b7285
                color #ffffff
                stroke #075563
                strokeWidth 2
            }

            element "Component" {
                shape Component
                background #1168bd
                color #ffffff
                stroke #0b4f8a
                strokeWidth 2
                fontSize 20
            }

            element "Entrada" {
                shape Component
                background #0b7285
                color #ffffff
                stroke #075563
                strokeWidth 2
            }

            element "Aplicação" {
                shape Component
                background #6f42c1
                color #ffffff
                stroke #55319a
                strokeWidth 2
            }

            element "Domínio" {
                shape Component
                background #2f9e44
                color #ffffff
                stroke #247a35
                strokeWidth 2
            }

            element "Infraestrutura" {
                shape Component
                background #495057
                color #ffffff
                stroke #343a40
                strokeWidth 2
            }

            element "Banco de Dados" {
                shape Cylinder
                background #438dd5
                color #ffffff
                stroke #2f6da8
                strokeWidth 2
                fontSize 22
            }

            element "Camada de Dados" {
                background #438dd5
                color #ffffff
                stroke #2f6da8
                strokeWidth 2
            }

            relationship "Relationship" {
                color #495057
                style dashed
                routing Orthogonal
                fontSize 18
                width 260
            }
        }

        theme default
    }

    configuration {
        scope softwaresystem
    }
}
