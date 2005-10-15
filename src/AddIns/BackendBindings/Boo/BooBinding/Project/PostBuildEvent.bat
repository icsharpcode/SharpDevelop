REM TODO: Replace this batch file with a ".booproj" MsBuild project
pushd "%1\..\..\RequiredLibraries"
booc Boo.Microsoft.Build.Tasks.boo -t:library -o:Boo.Microsoft.Build.Tasks.dll -r:Microsoft.Build.Framework -r:Microsoft.Build.Tasks -r:Microsoft.Build.Utilities
@IF %ERRORLEVEL% NEQ 0 GOTO BooPostBuildEventEnd
popd
copy "%1\..\..\RequiredLibraries\booc.*" .
copy "%1\..\..\RequiredLibraries\*.targets" .
copy "%1\..\..\RequiredLibraries\Boo.Microsoft.Build.Tasks.dll" .
:BooPostBuildEventEnd
