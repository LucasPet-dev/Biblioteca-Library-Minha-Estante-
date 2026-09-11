; Inno Setup script para MinhaEstante (Windows).
; 1. Execute .\build\windows-installer\build-windows.ps1 primeiro.
; 2. Compile este arquivo no Inno Setup (ISCC.exe installer.iss).
; Resultado: build\windows-installer\MinhaEstanteSetup.exe

#define AppName "MinhaEstante"
#define AppVersion "1.0.0"
#define AppExeName "MinhaEstante.Presentation.exe"
#define AppIcon "..\..\src\MinhaEstante.Presentation\Assets\Logo.ico"

[Setup]
AppId={#AppName}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=MinhaEstante
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=.
OutputBaseFilename=MinhaEstanteSetup
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
SetupIconFile={#AppIcon}
UninstallDisplayIcon={app}\{#AppExeName}

[Files]
Source: "publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb"

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na area de trabalho"; GroupDescription: "Atalhos:"

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Executar {#AppName}"; Flags: nowait postinstall skipifsilent