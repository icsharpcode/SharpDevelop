Dim WshShell
Set WshShell = CreateObject("WScript.Shell")
WshShell.CurrentDirectory = GetSetupPath()

WScript.Echo "Installing shared assemblies into the GAC" & vbCrLf

WScript.Echo "ICSharpCode.SharpZipLib.dll"
RegisterInGAC "..\ICSharpCode.SharpZipLib.dll"

WScript.Echo "NUnit.Core.dll"
RegisterInGAC "..\nunit.core.dll"

WScript.Echo "NUnit.Framework.dll"
RegisterInGAC "..\nunit.framework.dll"

Execute "BuildHelpIndex.exe"

' SHARED FUNCTIONS

Public Sub RegisterInGAC(strAssemblyRelativePath)
  Execute "..\tools\gacutil2.exe /i:" & strAssemblyRelativePath
End Sub

Public Sub Execute(strProgram)
  Dim oExec

  Set oExec = WshShell.Exec(strProgram)

  Do While oExec.Status = 0
    WScript.Sleep 100
  Loop

  WScript.Echo oExec.StdOut.ReadAll
End Sub

Public Function GetSetupPath()
  Dim strSetupDirPath
  strSetupDirPath = Left(WScript.ScriptFullName, InStrRev(WScript.ScriptFullName, "\"))
  GetSetupPath = strSetupDirPath
End Function
