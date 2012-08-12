// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	}
}
