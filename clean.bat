%windir%\microsoft.net\framework\v4.0.20506\msbuild /m /t:clean SharpDevelop.sln "/p:FSharpBuildTasksPath=%CD%\src\AddIns\BackendBindings\FSharp\RequiredLibraries" "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 PAUSE
%windir%\microsoft.net\framework\v4.0.20506\msbuild /m /t:clean AddIns\Misc\Profiler\AutomatedBuild.proj
@IF %ERRORLEVEL% NEQ 0 PAUSE