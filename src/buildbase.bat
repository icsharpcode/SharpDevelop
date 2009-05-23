cd Main\Base\Project
:loop
%windir%\microsoft.net\framework\v4.0.20506\msbuild ICSharpCode.SharpDevelop.csproj /p:Configuration=Debug "/p:Platform=AnyCPU"
pause
goto loop