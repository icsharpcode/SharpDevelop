del "bin\SharpDevelop.msi"
"..\Tools\UpdateSetupInfo\bin\UpdateSetupInfo.exe"
%windir%\microsoft.net\framework\v2.0.50727\msbuild SharpDevelop.Setup.sln "/p:SharpDevelopBinPath=%CD%\..\..\bin"
@IF %ERRORLEVEL% NEQ 0 PAUSE