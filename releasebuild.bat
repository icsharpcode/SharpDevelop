%windir%\microsoft.net\framework\v4.0.21006\msbuild /m SharpDevelop.sln /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1