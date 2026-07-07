param(
    [string]$Workspace = "docs/architecture/workspace.dsl",
    [switch]$Strict
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "../../..")
$workspacePath = Resolve-Path (Join-Path $repoRoot $Workspace)
$workspaceDirectory = Split-Path $workspacePath -Parent
$workspaceFile = Split-Path $workspacePath -Leaf

function Complete-OptionalValidation {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if ($Strict) {
        throw $Message
    }

    Write-Warning $Message
    Write-Warning "Validação ignorada; isso não afeta o build, os testes, o deploy ou a inicialização da aplicação."
    exit 0
}

try {
    docker version | Out-Null
}
catch {
    Complete-OptionalValidation -Message "Docker não está disponível para validar o Structurizr DSL."
}

$output = docker run `
    --rm `
    -e "STRUCTURIZR_THEMES=/usr/local/structurizr-themes" `
    -v "${workspaceDirectory}:/workspace" `
    structurizr/structurizr `
    validate `
    -workspace "/workspace/${workspaceFile}" 2>&1

$output | ForEach-Object { Write-Output $_ }

if ($LASTEXITCODE -ne 0 -or ($output -match "\bERROR\b")) {
    throw "Validação do Structurizr DSL falhou."
}

Write-Output "Structurizr DSL validado com sucesso."
