%windir%\microsoft.net\framework\v3.5\msbuild /t:clean SharpDevelop.sln "/p:BooBinPath=%CD%\AddIns\BackendBindings\Boo\RequiredLibraries" "/p:FSharpBuildTasksPath=%CD%\AddIns\BackendBindings\FSharp\RequiredLibraries"
@IF %ERRORLEVEL% NEQ 0 PAUSE
%windir%\microsoft.net\framework\v3.5\msbuild /t:clean AddIns\Misc\Profiler\AutomatedBuild.proj
@IF %ERRORLEVEL% NEQ 0 PAUSE