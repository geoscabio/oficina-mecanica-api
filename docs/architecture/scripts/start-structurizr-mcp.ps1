param(
    [int]$Port = 3000
)

$ErrorActionPreference = "Stop"

docker run `
    -it `
    --rm `
    -p "${Port}:${Port}" `
    -e "PORT=${Port}" `
    structurizr/mcp `
    -dsl
