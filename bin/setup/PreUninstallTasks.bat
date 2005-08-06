@echo off
echo Removing NUnit assemblies from the GAC
echo.
..\tools\gacutil2.exe /u ..\nunit.core.dll
echo.
..\tools\gacutil2.exe /u ..\nunit.framework.dll
echo.
..\tools\gacutil2.exe /u ..\ICSharpCode.SharpZipLib.dll
cd help
call unregister.bat
@IF %ERRORLEVEL% NEQ 0 PAUSE