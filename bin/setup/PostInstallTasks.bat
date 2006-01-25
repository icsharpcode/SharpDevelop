@echo off

pushd ..

echo Installing assemblies into the GAC
echo.
echo NUnit.Framework.dll
tools\gacutil2.exe /i nunit.framework.dll
echo.
echo MbUnit requirements
tools\gacutil2.exe /i tools\MbUnit\Refly.dll
tools\gacutil2.exe /i tools\MbUnit\TestFu.dll
tools\gacutil2.exe /i tools\MbUnit\QuickGraph.dll
tools\gacutil2.exe /i tools\MbUnit\QuickGraph.Algorithms.dll
echo.
echo MbUnit.Framework.dll
tools\gacutil2.exe /i tools\MbUnit\MbUnit.Framework.dll
echo.
echo.
echo.
echo Compiling SharpDevelop to native code to improve startup time...
echo.
ngen install SharpDevelop.exe

popd

rem echo Installing and configuring help system
rem cd help
rem call register.bat
echo.
@IF %ERRORLEVEL% NEQ 0 PAUSE