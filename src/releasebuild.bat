@ECHO OFF
%windir%\microsoft.net\framework\v2.0.41115\msbuild /property:Configuration=Release
IF %ERRORLEVEL% NEQ 0 PAUSE
