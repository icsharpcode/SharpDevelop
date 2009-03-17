%windir%\microsoft.net\framework\v3.5\msbuild /property:Configuration=Release SharpDevelop.sln "/p:BooBinPath=%CD%\AddIns\BackendBindings\Boo\RequiredLibraries" "/p:FSharpBuildTasksPath=%CD%\AddIns\BackendBindings\FSharp\RequiredLibraries"
@IF %ERRORLEVEL% NEQ 0 GOTO err
%windir%\microsoft.net\framework\v3.5\msbuild AddIns\Misc\Profiler\AutomatedBuild.proj /p:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1