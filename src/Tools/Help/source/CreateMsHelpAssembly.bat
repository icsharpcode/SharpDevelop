@echo off
setlocal
set AxImp="%ProgramFiles%\Microsoft.NET\SDK\v2.0\Bin\AxImp.exe"
set Help2Library="%CommonProgramFiles%\Microsoft Shared\help\hxvz.dll"


if exist Ax*.dll del Ax*.dll
if exist MSHelp*.dll del MSHelp*.dll

%AxImp% %Help2Library%

del Ax*.dll
del MSHelpControls.dll

set Help2Library=
set AxImp=

pause