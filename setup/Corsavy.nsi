!verbose 3

!define PRODUCT_NAME "SharpDevelop 2.0"
!define PRODUCT_VERSION "Corsavy Tech Preview"
!define PRODUCT_PUBLISHER "ic#code"
!define PRODUCT_WEB_SITE "http://www.icsharpcode.net/opensource/sd/"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\SharpDevelop.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\SharpDevelop2"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

!define MUI_WELCOMEFINISHPAGE_BITMAP "wizard-image.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "wizard-image.bmp"

BrandingText "© 2000-2005 ic#code, http://www.icsharpcode.net/"
SetCompressor lzma
CRCCheck on

; File Association defines
!include "fileassoc.nsh"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "..\doc\license.txt"
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\bin\SharpDevelop.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Run #develop"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

; Language files
!insertmacro MUI_LANGUAGE "English"

; Reserve files
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "Setup.exe"
InstallDir "$PROGRAMFILES\SharpDevelop\2.0\"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

; .NET Framework check
; http://msdn.microsoft.com/netframework/default.aspx?pull=/library/en-us/dnnetdep/html/redistdeploy1_1.asp
; Section "Detecting that the .NET Framework 2.0 Beta 2 is installed"
Function .onInit
	ReadRegDWORD $R0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50215" Install
	StrCmp $R0 "" 0 CheckPreviousVersion
	MessageBox MB_OK "Microsoft .NET Framework 2.0 Beta 2 was not found on this system.$\r$\n$\r$\nUnable to continue this installation."
	Abort

  CheckPreviousVersion:
	ReadRegStr $R0 ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName"
	StrCmp $R0 "" CheckOSVersion 0
	MessageBox MB_OK "An old version of SharpDevelop is installed on this computer, please uninstall first.$\r$\n$\r$\nUnable to continue this installation."
	Abort
	
  CheckOSVersion:
        Call IsSupportedWindowsVersion
        Pop $R0
        StrCmp $R0 "False" NoAbort 0
	MessageBox MB_OK "The operating system you are using is not supported by SharpDevelop (95/98/ME/NT3.x/NT4.x)."
        Abort

  NoAbort:
FunctionEnd

Section "MainSection" SEC01
  SetOverwrite ifnewer

  ; any file that would go in the root
  SetOutPath "$INSTDIR"


  SetOutPath "$INSTDIR\AddIns"
  File /r ..\AddIns\*.*

  SetOutPath "$INSTDIR\bin"
  File /r ..\bin\*.*

  SetOutPath "$INSTDIR\data"
  File /r ..\data\*.*

  SetOutPath "$INSTDIR\doc"
  File /r ..\doc\*.*
  
  CreateDirectory "$SMPROGRAMS\SharpDevelop 2.0"
  CreateShortCut "$SMPROGRAMS\SharpDevelop 2.0\SharpDevelop 2.0.lnk" "$INSTDIR\bin\SharpDevelop.exe"
  CreateShortCut "$DESKTOP\SharpDevelop 2.0.lnk" "$INSTDIR\bin\SharpDevelop.exe"

  ; Add default file associations
  ; CreateFileAssociation extension extType extDef exeCmd defIcon
  ${CreateFileAssociation} ".cmbx"  "SD.cmbxfile" "#Develop Combine" "$INSTDIR\bin\SharpDevelop.exe" "$INSTDIR\data\resources\filetypes\cmbx.ico"
  ${CreateFileAssociation} ".prjx"  "SD.prjxfile" "#Develop Project" "$INSTDIR\bin\SharpDevelop.exe" "$INSTDIR\data\resources\filetypes\prjx.ico"
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\SharpDevelop 2.0\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\SharpDevelop 2.0\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\bin\SharpDevelop.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\bin\SharpDevelop.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  
  ; now finally call our post install tasks
  SetOutPath "$INSTDIR\bin\setup"
  ; return code goes to $0 -> don't fail setup when there are Help2 problems
  ExecWait '"$INSTDIR\bin\setup\PostInstallTasks.bat"' $0
SectionEnd

Section Uninstall
  Delete "$DESKTOP\SharpDevelop 2.0.lnk"
  Delete "$SMPROGRAMS\SharpDevelop 2.0\*.*"

  ; first, remove all dependencies from the GAC etc
  SetOutPath "$INSTDIR\bin\setup"
  ExecWait '"$INSTDIR\bin\setup\PreUninstallTasks.bat"' $0
  ; set OutPath to somewhere else because the current working directory cannot be deleted!
  SetOutPath "$DESKTOP"
  
  RMDir "$SMPROGRAMS\SharpDevelop 2.0"
  RMDir /r "$INSTDIR"
  
  ; NOTE: this application configuration deletion is now deactivated post-1.0 releases
  ; RMDir /r "$APPDATA\.ICSharpCode"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"

  ; File association removal
  ${RemoveFileAssociation} ".cmbx"  "SD.cmbxfile"
  ${RemoveFileAssociation} ".prjx"  "SD.prjxfile"
SectionEnd

; GetWindowsVersion, taken from NSIS help, modified for our purposes
Function IsSupportedWindowsVersion

   Push $R0
   Push $R1

   ReadRegStr $R0 HKLM \
   "SOFTWARE\Microsoft\Windows NT\CurrentVersion" CurrentVersion

   IfErrors 0 lbl_winnt

   ; we are not NT
   ReadRegStr $R0 HKLM \
   "SOFTWARE\Microsoft\Windows\CurrentVersion" VersionNumber

   StrCpy $R1 $R0 1
   StrCmp $R1 '4' 0 lbl_error

   StrCpy $R1 $R0 3

   StrCmp $R1 '4.0' lbl_win32_95
   StrCmp $R1 '4.9' lbl_win32_ME lbl_win32_98

   lbl_win32_95:
     StrCpy $R0 'False'
   Goto lbl_done

   lbl_win32_98:
     StrCpy $R0 'False'
   Goto lbl_done

   lbl_win32_ME:
     StrCpy $R0 'False'
   Goto lbl_done

   lbl_winnt:

   StrCpy $R1 $R0 1

   StrCmp $R1 '3' lbl_winnt_x
   StrCmp $R1 '4' lbl_winnt_x

   StrCpy $R1 $R0 3

   StrCmp $R1 '5.0' lbl_winnt_2000
   StrCmp $R1 '5.1' lbl_winnt_XP
   StrCmp $R1 '5.2' lbl_winnt_2003 lbl_error

   lbl_winnt_x:
     StrCpy $R0 'False'
   Goto lbl_done

   lbl_winnt_2000:
     Strcpy $R0 'True'
   Goto lbl_done

   lbl_winnt_XP:
     Strcpy $R0 'True'
   Goto lbl_done

   lbl_winnt_2003:
     Strcpy $R0 'True'
   Goto lbl_done

   lbl_error:
     Strcpy $R0 'False'
   lbl_done:

   Pop $R1
   Exch $R0

FunctionEnd

