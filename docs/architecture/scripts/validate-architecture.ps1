param(
    [string]$Workspace = "docs/architecture/workspace.dsl"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "../../..")
$workspacePath = Resolve-Path (Join-Path $repoRoot $Workspace)
$workspaceDirectory = Split-Path $workspacePath -Parent
$workspaceFile = Split-Path $workspacePath -Leaf

function Invoke-StructurizrCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string]$CommandName
    )

    $output = docker run `
        --rm `
        -v "${workspaceDirectory}:/workspace" `
        structurizr/structurizr `
        $CommandName `
        -workspace "/workspace/${workspaceFile}" 2>&1

    $output | ForEach-Object { Write-Output $_ }

    if ($LASTEXITCODE -ne 0 -or ($output -match "\bERROR\b")) {
        throw "Structurizr ${CommandName} failed."
    }
}

Invoke-StructurizrCommand -CommandName "validate"
Invoke-StructurizrCommand -CommandName "inspect"
