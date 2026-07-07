# 0001 - Architecture as code with Structurizr MCP

## Status

Accepted

## Context

The project needs architecture documentation for all C4 levels and deployment views. The documentation must stay close to the codebase because the API, Kubernetes manifests and Terraform modules are evolving quickly.

## Decision

Use Structurizr DSL as the source of truth for architecture documentation under `docs/architecture/workspace.dsl`.

Use the Structurizr MCP server as an AI-assisted validation layer:

- validate DSL syntax;
- parse the workspace;
- inspect modeling issues;
- export views to Mermaid, PlantUML and C4-PlantUML.

## Consequences

- Architecture changes become reviewable in Git.
- Diagrams can be regenerated instead of manually edited.
- The AI review loop has concrete tools instead of free-form diagram critique.
- Any future infrastructure change should update the corresponding deployment view.
