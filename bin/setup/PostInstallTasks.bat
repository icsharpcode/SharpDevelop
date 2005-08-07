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
rem echo Installing and configuring help system
rem cd help
rem call register.bat
@IF %ERRORLEVEL% NEQ 0 PAUSE