@echo off

pushd ..

echo Removing assemblies from the GAC...
tools\gacutil2.exe /u tools\NUnit\nunit.framework.dll
tools\gacutil2.exe /u tools\NUnit\nunit.core.dll
echo.
echo.
echo Uninstalling native images...
echo.
%windir%\Microsoft.NET\Framework\v2.0.50727\ngen uninstall SharpDevelop.exe

popd

rem echo Uninstalling help system
rem cd help
rem call unregister.bat
echo.
@IF %ERRORLEVEL% NEQ 0 PAUSE