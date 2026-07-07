# 📄[ADR 005] Definição da Documentação de Arquitetura com Structurizr DSL e MCP

## Status

**Status:** ✅ Aceito **Data:** 07/07/2026 **Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> A Fase 2 adiciona novas decisões de infraestrutura, containers, Kubernetes, Terraform, registro de imagens e preparação para AWS. Precisamos documentar a arquitetura em vários níveis do C4 Model sem transformar a documentação em uma dependência rígida do build, dos testes, do deploy ou da inicialização da aplicação.
>
> Também precisamos de um formato simples de manter, compatível com o Structurizr Playground e adequado para revisão assistida por IA.

## 2. Fatores Decisivos (Drivers)

- Necessidade de manter a arquitetura como código versionado no repositório.
- Necessidade de representar contexto, containers, componentes, fluxos dinâmicos e deployment.
- Preferência por um artefato oficial único, evitando Mermaid, PlantUML e imagens geradas no Git.
- Uso de temas oficiais do Structurizr para melhorar a leitura visual de AWS e Kubernetes.
- Integração opcional com MCP para apoiar revisão por IA sem acoplar a aplicação a essa ferramenta.

## 3. Decisão Proposta

> Adotaremos o **Structurizr DSL** como artefato oficial de documentação de arquitetura do projeto, mantido em `docs/architecture/workspace.dsl`.
>
> O repositório manterá apenas scripts auxiliares e manuais para validação local e subida do Structurizr MCP. O MCP será usado como uma camada opcional de revisão/lint por IA, com foco em validação, inspeção e sugestões sobre o DSL.
>
> Não versionaremos exports em Mermaid, PlantUML, C4-PlantUML, PNG ou SVG como parte do fluxo padrão.

## 4. Justificativa

> O Structurizr DSL permite representar o C4 Model em um arquivo texto versionável e compatível com o Structurizr Playground. Isso reduz a quantidade de artefatos derivados no repositório e preserva uma fonte única de verdade para os diagramas.
>
> Os temas oficiais do Structurizr para AWS e Kubernetes fornecem ícones e estilos padronizados, deixando as views de deployment mais profissionais sem precisarmos manter imagens locais.
>
> A integração com MCP fica limitada ao uso manual e opcional. Dessa forma, a equipe pode usar IA para revisar a modelagem, mas uma falha em Docker, Node, MCP ou scripts de documentação não impede a aplicação de compilar, testar, subir ou ser entregue.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- **Fonte única de verdade:** o `workspace.dsl` concentra os diagramas e reduz divergências entre formatos.
- **Repositório mais limpo:** não há arquivos gerados em múltiplos formatos.
- **Revisão visual simples:** o DSL pode ser colado diretamente no Structurizr Playground.
- **Melhor acabamento visual:** os temas oficiais adicionam ícones e estilos para AWS e Kubernetes.
- **Baixo acoplamento:** scripts e MCP são opcionais e não fazem parte do ciclo operacional da API.

### ❌ Negativo (Perdas/Riscos)

- **Dependência de conhecimento do DSL:** a equipe precisa conhecer o básico da sintaxe do Structurizr.
- **Layout manual eventual:** algumas setas e posições podem precisar de ajuste visual no Playground.
- **Validação opcional:** sem workflow obrigatório, a disciplina de rodar validação local depende da equipe.

## 6. Referências

- **Structurizr.** *Structurizr DSL*. https://docs.structurizr.com/dsl
- **Structurizr.** *C4 model and Structurizr DSL pattern catalog*. https://docs.structurizr.com/dsl/patterns/
- **Structurizr.** *Themes*. https://docs.structurizr.com/server/diagrams/themes
- **Structurizr.** *MCP server*. https://docs.structurizr.com/ai/mcp
- **C4 Model.** https://c4model.com/
- **FIAP, Pós-Tech Software Architecture.** Fase 2 - conteúdos de containers, orquestração, DevOps e infraestrutura em nuvem.
