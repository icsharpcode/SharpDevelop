%windir%\microsoft.net\framework\v4.0.20506\msbuild /m /property:Configuration=Release SharpDevelop.sln "/p:BooBinPath=%CD%\AddIns\BackendBindings\Boo\RequiredLibraries" "/p:FSharpBuildTasksPath=%CD%\AddIns\BackendBindings\FSharp\RequiredLibraries" "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
%windir%\microsoft.net\framework\v4.0.20506\msbuild /m AddIns\Misc\Profiler\AutomatedBuild.proj /p:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1