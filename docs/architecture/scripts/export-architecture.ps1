param(
    [string]$Workspace = "docs/architecture/workspace.dsl",
    [string]$Output = "docs/architecture/generated"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "../../..")
$workspacePath = Resolve-Path (Join-Path $repoRoot $Workspace)
$outputPath = Join-Path $repoRoot $Output

New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

function Invoke-StructurizrExport {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Format,

        [Parameter(Mandatory = $true)]
        [string]$FormatOutput
    )

    $output = docker run `
        --rm `
        -v "${repoRoot}:/repo" `
        structurizr/structurizr `
        export `
        -workspace "/repo/${Workspace}" `
        -format $Format `
        -output "/repo/${Output}/${FormatOutput}" 2>&1

    $output | ForEach-Object { Write-Output $_ }

    if ($LASTEXITCODE -ne 0 -or ($output -match "\bERROR\b")) {
        throw "Structurizr export failed for format ${Format}."
    }
}

Invoke-StructurizrExport -Format "mermaid" -FormatOutput "mermaid"
Invoke-StructurizrExport -Format "plantuml/c4plantuml" -FormatOutput "c4plantuml"
Invoke-StructurizrExport -Format "plantuml" -FormatOutput "plantuml"
