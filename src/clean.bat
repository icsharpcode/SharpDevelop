@ECHO OFF
%windir%\microsoft.net\framework\v2.0.50215\msbuild /t:clean
IF %ERRORLEVEL% NEQ 0 PAUSE
