%windir%\microsoft.net\framework\v4.0.20506\msbuild /m SharpDevelop.sln /p:Configuration=Debug "/p:FSharpBuildTasksPath=%CD%\src\AddIns\BackendBindings\FSharp\RequiredLibraries" "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
rem %windir%\microsoft.net\framework\v4.0.20506\msbuild /m AddIns\Misc\Profiler\AutomatedBuild.proj /p:Configuration=Debug
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1