# Auditoria NuGet - Vulnerabilidades

Data: 2026-05-20

Comando executado:

```powershell
dotnet list package --vulnerable --include-transitive
```

Fontes consultadas:

- `https://api.nuget.org/v3/index.json`
- `C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\`

Resultado:

| Projeto | Status |
| --- | --- |
| `OficinaMecanica.Domain` | Nenhum pacote vulneravel |
| `OficinaMecanica.Application` | Nenhum pacote vulneravel |
| `OficinaMecanica.Infrastructure` | Nenhum pacote vulneravel |
| `OficinaMecanica.API` | Nenhum pacote vulneravel |
| `OficinaMecanica.Domain.UnitTests` | Nenhum pacote vulneravel |
| `OficinaMecanica.Application.UnitTests` | Nenhum pacote vulneravel |
| `OficinaMecanica.API.IntegrationTests` | Nenhum pacote vulneravel |

Conclusao: nao foram encontradas vulnerabilidades conhecidas nos pacotes diretos ou transitivos considerando as fontes atuais.
