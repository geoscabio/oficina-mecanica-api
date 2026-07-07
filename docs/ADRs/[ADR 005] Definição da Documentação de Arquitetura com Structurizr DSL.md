# 📄[ADR 005] Definição da Documentação de Arquitetura com Structurizr DSL

## Status

**Status:** ✅ Aceito **Data:** 07/07/2026 **Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> A Fase 2 adiciona novas decisões de infraestrutura, containers, Kubernetes, Terraform, registro de imagens e preparação para AWS. Precisamos documentar a arquitetura em vários níveis do C4 Model sem transformar a documentação em uma dependência do build, dos testes, do deploy ou da inicialização da aplicação.
>
> Também precisamos de um formato simples de manter, compatível com o Structurizr Playground e com ferramentas oficiais do Structurizr.

## 2. Fatores Decisivos (Drivers)

- Necessidade de manter a arquitetura como código versionado no repositório.
- Necessidade de representar contexto, containers, componentes, fluxos dinâmicos e deployment.
- Preferência por um artefato oficial único, evitando Mermaid, PlantUML e imagens geradas no Git.
- Uso de temas oficiais do Structurizr para melhorar a leitura visual de AWS e Kubernetes.
- Possibilidade de ajustar visualmente os diagramas fora do DSL, quando necessário, sem adicionar artefatos auxiliares ao repositório.

## 3. Decisão Proposta

> Adotaremos o **Structurizr DSL** como artefato oficial de documentação de arquitetura do projeto, mantido em `docs/architecture/workspace.dsl`.
>
> O repositório manterá apenas o arquivo DSL como fonte dos diagramas. Não versionaremos Mermaid, PlantUML, C4-PlantUML, PNG ou SVG como parte do fluxo padrão.

## 4. Justificativa

> O Structurizr DSL permite representar o C4 Model em um arquivo texto versionável e compatível com o Structurizr Playground. Isso reduz a quantidade de artefatos derivados no repositório e preserva uma fonte única de verdade para os diagramas.
>
> Os temas oficiais do Structurizr para AWS e Kubernetes fornecem ícones e estilos padronizados, deixando as views de deployment mais profissionais sem precisarmos manter imagens locais.
>
> As views não usam layout automático, para permitir ajustes manuais em ambientes Structurizr que suportem edição visual. Dessa forma, a documentação permanece simples e a aplicação continua totalmente independente desses diagramas.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- **Fonte única de verdade:** o `workspace.dsl` concentra os diagramas e reduz divergências entre formatos.
- **Repositório mais limpo:** não há arquivos gerados em múltiplos formatos.
- **Revisão visual simples:** o DSL pode ser colado diretamente no Structurizr Playground.
- **Melhor acabamento visual:** os temas oficiais adicionam ícones e estilos para AWS e Kubernetes.
- **Baixo acoplamento:** a documentação não adiciona serviços auxiliares ao ciclo operacional da API.

### ❌ Negativo (Perdas/Riscos)

- **Dependência de conhecimento do DSL:** a equipe precisa conhecer o básico da sintaxe do Structurizr.
- **Layout manual eventual:** algumas setas e posições podem precisar de ajuste visual em ferramenta Structurizr com editor.
- **Validação manual:** sem workflow obrigatório, a disciplina de validar o DSL depende da equipe.

## 6. Referências

- **Structurizr.** *Structurizr DSL*. https://docs.structurizr.com/dsl
- **Structurizr.** *C4 model and Structurizr DSL pattern catalog*. https://docs.structurizr.com/dsl/patterns/
- **Structurizr.** *Themes*. https://docs.structurizr.com/server/diagrams/themes
- **C4 Model.** https://c4model.com/
- **FIAP, Pós-Tech Software Architecture.** Fase 2 - conteúdos de containers, orquestração, DevOps e infraestrutura em nuvem.
