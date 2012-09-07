%windir%\microsoft.net\framework\v4.0.30319\msbuild /Target:Rebuild /property:Configuration=Release nunit-console\nunit-console.sln 
copy nunit-console.exe nunit-console-x86.exe
corflags /32bit+ nunit-console-x86.exe
@IF %ERRORLEVEL% NEQ 0 GOTO err
copy nunit-console.exe nunit-console-dotnet2.exe
copy nunit-console-x86.exe nunit-console-dotnet2-x86.exe
@exit /B 0
:err
@PAUSE
@exit /B 1