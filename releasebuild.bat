%windir%\microsoft.net\framework\v4.0.21006\msbuild /m /property:Configuration=Release SharpDevelop.sln "/p:FSharpBuildTasksPath=%CD%\src\AddIns\BackendBindings\FSharp\RequiredLibraries" "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1