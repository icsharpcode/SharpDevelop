%windir%\microsoft.net\framework\v3.5\msbuild CopyDependencies.build
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1