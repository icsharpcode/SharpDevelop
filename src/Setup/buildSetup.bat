del "bin\SharpDevelop.msi"
"..\Tools\UpdateAssemblyInfo\bin\Debug\UpdateAssemblyInfo.exe"
%windir%\microsoft.net\framework\v4.0.30319\msbuild SharpDevelop.Setup.sln "/p:SharpDevelopBinPath=%CD%\..\..\bin"
@IF %ERRORLEVEL% NEQ 0 PAUSE