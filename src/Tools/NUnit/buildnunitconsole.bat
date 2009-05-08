%windir%\microsoft.net\framework\v3.5\msbuild /property:Configuration=Release nunit-console\nunit-console.sln 
copy nunit-console.exe nunit-console-x86.exe
"%programfiles%\Microsoft.net\sdk\v2.0\bin\corflags" /32bit+ nunit-console-x86.exe
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1