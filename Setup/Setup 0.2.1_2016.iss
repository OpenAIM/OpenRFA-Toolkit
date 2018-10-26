; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "OpenRFA Tools for Revit 2016"
#define MyAppVersion "0.2.1_2016"
#define MyAppPublisher "BIM Extension, LLC"
#define MyAppURL "http://openrfa.org"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{392CB578-CAE0-418B-A589-4E051D489E31}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableDirPage=yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=C:\Users\Jay\DevLab\OpenRFA_WPF_CS\Setup\License.txt
InfoBeforeFile=C:\Users\Jay\DevLab\OpenRFA_WPF_CS\Setup\Pre-Install.rtf
InfoAfterFile=C:\Users\Jay\DevLab\OpenRFA_WPF_CS\Setup\Changelog.txt
OutputBaseFilename=OpenRFA Tools v0.2.1_2016
Compression=lzma
SolidCompression=yes
WizardSmallImageFile=WizOpenRfaLogo.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "C:\Users\Jay\DevLab\OpenRFA_WPF_CS\AboutWindow\bin\Release\OpenRfaAbout.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
Source: "C:\Users\Jay\DevLab\OpenRFA_WPF_CS\LoadParametersToMultiple\bin\Release\OpenRfaLoadToMultiple.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
Source: "C:\Users\Jay\DevLab\OpenRFA_WPF_CS\LoadParametersToMultiple\bin\Release\Newtonsoft.Json.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
Source: "C:\Users\Jay\DevLab\OpenRFA_WPF_CS\OpenRFA_WPF_CS\bin\Release\OpenRfaLoadToFamily.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
Source: "C:\Users\Jay\DevLab\OpenRFA_WPF_CS\OpenRfaRibbon\bin\Release\OpenRfaRibbon.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
Source: "C:\Users\Jay\DevLab\OpenRFA_WPF_CS\OpenRfaRibbon\bin\Release\OpenRfaRibbon.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
Source: "C:\Users\Jay\DevLab\OpenRFA_WPF_CS\SupportWindow\bin\Release\OpenRfaSupport.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

