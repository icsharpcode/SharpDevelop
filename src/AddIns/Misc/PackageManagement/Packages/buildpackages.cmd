@ECHO OFF
SETLOCAL
SET msbuild=%windir%\microsoft.net\framework\v4.0.30319\msbuild
SET project=..\..\..\..\Libraries\AvalonEdit\ICSharpCode.AvalonEdit\ICSharpCode.AvalonEdit.csproj
SET buildoptions=/t:Rebuild /p:Configuration=Release /p:DebugType=PdbOnly

@ECHO ON
:Clean debug build
%msbuild% /m %project% /t:Clean /p:Configuration=Debug /p:OutputPath=%~dp0\AvalonEdit\lib\Net40

:BUILD .NET 4.0 version
%msbuild% /m %project% %buildoptions% /p:OutputPath=%~dp0\AvalonEdit\lib\Net40

:BUILD .NET 3.5 version
%msbuild% /m %project% %buildoptions% "/p:DefineConstants=TRACE" "/p:TargetFrameworkVersion=v3.5" /p:OutputPath=%~dp0\AvalonEdit\lib\Net35

nuget.exe pack AvalonEdit\AvalonEdit.nuspec -Symbols -BasePath AvalonEdit -OutputDirectory AvalonEdit
nuget.exe pack AvalonEdit.Sample\AvalonEdit.Sample.nuspec -BasePath AvalonEdit.Sample -OutputDirectory AvalonEdit.Sample

@ECHO OFF
ENDLOCAL