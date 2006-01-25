@echo off

pushd ..

echo Removing assemblies from the GAC...
tools\gacutil2.exe /u nunit.framework.dll
tools\gacutil2.exe /u tools\MbUnit\Refly.dll
tools\gacutil2.exe /u tools\MbUnit\TestFu.dll
tools\gacutil2.exe /u tools\MbUnit\QuickGraph.dll
tools\gacutil2.exe /u tools\MbUnit\QuickGraph.Algorithms.dll
tools\gacutil2.exe /u tools\MbUnit\MbUnit.Framework.dll
echo.
echo.
echo Uninstalling native images...
echo.
ngen uninstall SharpDevelop.exe

popd

rem echo Uninstalling help system
rem cd help
rem call unregister.bat
echo.
@IF %ERRORLEVEL% NEQ 0 PAUSE