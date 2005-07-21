Tools\UpdateAssemblyInfo\bin\Debug\UpdateAssemblyInfo.exe Libraries
@IF %ERRORLEVEL% NEQ 0 PAUSE & EXIT
%windir%\microsoft.net\framework\v2.0.50215\msbuild /property:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 PAUSE