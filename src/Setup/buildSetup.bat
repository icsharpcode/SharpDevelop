del "bin\SharpDevelop.msi"
"..\Tools\UpdateSetupInfo\bin\UpdateSetupInfo.exe"
%windir%\microsoft.net\framework\v4.0.30319\msbuild SharpDevelop.Setup.sln "/p:SharpDevelopBinPath=%CD%\..\..\bin"
@IF %ERRORLEVEL% NEQ 0 PAUSE