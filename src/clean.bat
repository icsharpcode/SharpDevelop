@ECHO OFF
%windir%\microsoft.net\framework\v2.0.41115\msbuild /t:clean
IF %ERRORLEVEL% NEQ 0 PAUSE
