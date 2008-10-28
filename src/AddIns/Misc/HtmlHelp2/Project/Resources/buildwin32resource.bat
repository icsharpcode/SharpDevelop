@echo off
setlocal
set RC="K:\Programme\Microsoft.NET\SDK\v2.0\Bin\rc.exe"

if not exist %RC% goto ExitJump
if exist dynamichelp.res del dynamichelp.res
%RC% /fo "dynamichelp.res" "dynamichelp.rc"

set RC=
:ExitJump