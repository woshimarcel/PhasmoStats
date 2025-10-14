; -------------------------------
; PhasmoStats Inno Setup Installer
; -------------------------------

[Setup]
AppName=PhasmoStats
AppVersion=1.0
AppPublisher=woshi
DefaultDirName={autopf}\PhasmoStats
DefaultGroupName=PhasmoStats
OutputDir=C:\Users\woshi\Downloads\Installer
OutputBaseFilename=PhasmoStatsInstaller
SetupIconFile=C:\Users\woshi\Downloads\Icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
DisableDirPage=no
DisableProgramGroupPage=yes
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin

[Files]
; Main executable and all dependencies
Source: "C:\Users\woshi\source\repos\PhasmoStats\PhasmoStats\bin\Release\net9.0\win-x86\*"; \
    DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; Start Menu shortcut
Name: "{autoprograms}\PhasmoStats"; Filename: "{app}\PhasmoStats.exe"; IconFilename: "{app}\PhasmoStats.exe"

; Desktop shortcut
Name: "{commondesktop}\PhasmoStats"; Filename: "{app}\PhasmoStats.exe"; IconFilename: "{app}\PhasmoStats.exe"

[Run]
; Launch after install
Filename: "{app}\PhasmoStats.exe"; Description: "Launch PhasmoStats"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Clean up leftover files
Type: filesandordirs; Name: "{app}"
