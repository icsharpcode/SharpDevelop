:: -----------------------------------------------------------------
:: Builds the 32bit version of NUnit Console and the AnyCPU version.
:: -----------------------------------------------------------------

%windir%\microsoft.net\framework\v3.5\msbuild nunit-console.sln /p:PlatformTarget=x86 /property:Configuration=Release /t:Rebuild
move ..\nunit-console.exe ..\nunit-console-x86.exe
%windir%\microsoft.net\framework\v3.5\msbuild nunit-console.sln /property:Configuration=Release /t:Rebuild

@IF %ERRORLEVEL% NEQ 0 PAUSE