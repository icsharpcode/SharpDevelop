@rem Resets version information in libraries with "2.0.0.1"
@rem Execute this file to prevent committing changes to the assembly information.
Tools\UpdateAssemblyInfo\bin\Debug\UpdateAssemblyInfo.exe ResetLibraries
@IF %ERRORLEVEL% NEQ 0 PAUSE