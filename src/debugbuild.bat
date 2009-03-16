%windir%\microsoft.net\framework\v3.5\msbuild SharpDevelop.sln /p:Configuration=Debug "/p:BooBinPath=%CD%\AddIns\BackendBindings\Boo\RequiredLibraries" "/p:FSharpBuildTasksPath=%CD%\AddIns\BackendBindings\FSharp\RequiredLibraries"
@IF %ERRORLEVEL% NEQ 0 GOTO err
%windir%\microsoft.net\framework\v3.5\msbuild AddIns\Misc\Profiler\AutomatedBuild.proj /p:Configuration=Debug
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1