@echo off

del *.dll
del *.cs


setlocal
set AxImp="C:\Programme\Microsoft Visual Studio 8\SDK\v2.0\Bin\AxImp.exe"
%AxImp% -source "%CommonProgramFiles%\Microsoft Shared\help\hxvz.dll"
set AxImp=

del *.pdb
del Ax*.dll

pause