@echo off

pushd ..

echo Installing assemblies into the GAC
echo.
echo NUnit.Framework.dll
tools\gacutil2.exe /i tools\NUnit\nunit.framework.dll
echo NUnit.Core.dll
tools\gacutil2.exe /i tools\NUnit\nunit.core.dll
echo.
echo.
echo.
echo Compiling SharpDevelop to native code to improve startup time...
echo.
%windir%\Microsoft.NET\Framework\v2.0.50727\ngen install SharpDevelop.exe

popd

rem echo Installing and configuring help system
rem cd help
rem call register.bat
echo.
@IF %ERRORLEVEL% NEQ 0 PAUSE