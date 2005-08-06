@echo off
echo Installing NUnit assemblies into the GAC
echo.
echo NUnit.Core.dll
..\tools\gacutil2.exe /i ..\nunit.core.dll
echo.
echo NUnit.Framework.dll
..\tools\gacutil2.exe /i ..\nunit.framework.dll
echo.
echo ICSharpCode.SharpZipLib.dll
..\tools\gacutil2.exe /i ..\ICSharpCode.SharpZipLib.dll
echo.
echo Installing and configuring help system
cd help
call register.bat
@IF %ERRORLEVEL% NEQ 0 PAUSE