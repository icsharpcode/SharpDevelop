rem Does a releasebuild, updates the changelog and creates the installer
call releasebuild.bat
@IF %ERRORLEVEL% NEQ 0 EXIT
pushd Tools
%windir%\microsoft.net\framework\v2.0.50215\msbuild /t:publish /property:Configuration=Release Tools.build
@IF %ERRORLEVEL% NEQ 0 PAUSE & EXIT
popd
pushd ..\setup
call buildSetup.bat
@IF %ERRORLEVEL% NEQ 0 EXIT
popd
echo.
echo.
echo.
echo Publish.bat completed successfully.
echo In the directory SharpDevelop\setup, you will find Setup.exe.
echo.
pause