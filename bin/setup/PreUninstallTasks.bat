@echo off
echo Removing assemblies from the GAC
echo.
..\tools\gacutil2.exe /u ..\nunit.framework.dll
echo.
..\tools\gacutil2.exe /u ..\ICSharpCode.SharpZipLib.dll
echo.
..\tools\gacutil2.exe /u ..\tools\MbUnit\Refly.dll
..\tools\gacutil2.exe /u ..\tools\MbUnit\TestFu.dll
..\tools\gacutil2.exe /u ..\tools\MbUnit\QuickGraph.dll
..\tools\gacutil2.exe /u ..\tools\MbUnit\QuickGraph.Algorithms.dll
echo.
..\tools\gacutil2.exe /u ..\tools\MbUnit\MbUnit.Framework.dll
echo.
rem echo Uninstalling help system
rem cd help
rem call unregister.bat
@IF %ERRORLEVEL% NEQ 0 PAUSE