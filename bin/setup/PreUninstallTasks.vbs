Dim WshShell
Set WshShell = CreateObject("WScript.Shell")
WshShell.CurrentDirectory = GetSetupPath()

WScript.Echo "Removing shared assemblies from the GAC" & vbCrLf

WScript.Echo "ICSharpCode.SharpZipLib.dll"
RemoveFromGAC "..\ICSharpCode.SharpZipLib.dll"

WScript.Echo "NUnit.Core.dll"
RemoveFromGAC "..\nunit.core.dll"

WScript.Echo "NUnit.Framework.dll"
RemoveFromGAC "..\nunit.framework.dll"

' SHARED FUNCTIONS

Public Sub RemoveFromGAC(strAssemblyRelativePath)
  Execute "..\tools\gacutil2.exe /u:" & strAssemblyRelativePath
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
