// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PowerShellMissingConsoleHost : IPackageManagementConsoleHost
	{		
		public IProject DefaultProject { get; set; }
		public PackageSource ActivePackageSource { get; set; }
		public IScriptingConsole ScriptingConsole { get; set; }
		public IPackageManagementSolution Solution { get; private set; }
		
		public bool IsRunning { get; private set; }
		
		public void Clear()
		{
		}
		
		public void WritePrompt()
		{
		}
		
		public void Run()
		{
			WritePowerShellIsNotInstalledMessage();
			IsRunning = true;
		}
		
		void WritePowerShellIsNotInstalledMessage()
		{
			string message = GetPowerShellIsNotInstalledMessage();
			ScriptingConsole.WriteLine(message, ScriptingStyle.Error);
		}
		
		public void ShutdownConsole()
		{
		}
		
		public void ExecuteCommand(string command)
		{
		}
		
		public IPackageManagementProject GetProject(string packageSource, string projectName)
		{
			return null;
		}
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName)
		{
			return null;
		}
		
		public PackageSource GetActivePackageSource(string source)
		{
			return null;
		}
		
		public void Dispose()
		{
		}
		
		protected virtual string GetPowerShellIsNotInstalledMessage()
		{
			return
				"Windows PowerShell 2.0 is not installed.\r\n" +
				"The Package Management Console requires Windows PowerShell 2.0.\r\n" +
				"PowerShell 2.0 can be downloaded from http://support.microsoft.com/kb/968929";
		}
		
		public IPackageRepository GetPackageRepository(PackageSource packageSource)
		{
			return null;
		}
		
		public void SetDefaultRunspace()
		{
		}
		
		public IConsoleHostFileConflictResolver CreateFileConflictResolver(FileConflictAction fileConflictAction)
		{
			return null;
		}
		
		public IDisposable CreateLogger(ICmdletLogger logger)
		{
			return null;
		}
	}
}
