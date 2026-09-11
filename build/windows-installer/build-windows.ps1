# Publica o app para Windows (self-contained, single-file) em ./publish/win-x64.
# Uso (PowerShell):  .\build\windows-installer\build-windows.ps1
$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$project = Join-Path $root "src\MinhaEstante.Presentation\MinhaEstante.Presentation.csproj"
$outDir = Join-Path $PSScriptRoot "publish\win-x64"

Write-Host "Publicando para win-x64..."
dotnet publish $project `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $outDir

Write-Host "Artefato gerado em: $outDir"
Write-Host "Para gerar o instalador 'MinhaEstanteSetup.exe', compile 'installer.iss' no Inno Setup (ISCC.exe)."