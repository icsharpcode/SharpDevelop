// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class ConsolePackageScriptRunner : IPackageScriptRunner
	{
		IPackageManagementConsoleHost consoleHost;
		PackageScriptsToRun packageScriptsToRun;
		IPackageManagementWorkbench workbench;
		
		public ConsolePackageScriptRunner(
			IPackageManagementConsoleHost consoleHost,
			PackageScriptsToRun packageScriptsToRun)
			: this( consoleHost, packageScriptsToRun, new PackageManagementWorkbench())
		{
		}
		
		public ConsolePackageScriptRunner(
			IPackageManagementConsoleHost consoleHost,
			PackageScriptsToRun packageScriptsToRun,
			IPackageManagementWorkbench workbench)
		{
			this.consoleHost = consoleHost;
			this.packageScriptsToRun = packageScriptsToRun;
			this.workbench = workbench;
		}
		
		public void Run(IPackageScript script)
		{
			if (script.Exists()) {
				CreateConsolePadIfConsoleHostIsNotRunning();
				RunScriptThatExists(script);
			}
		}
		
		void CreateConsolePadIfConsoleHostIsNotRunning()
		{
			if (!consoleHost.IsRunning) {
				workbench.CreateConsolePad();
			}
		}
		
		void RunScriptThatExists(IPackageScript script)
		{
			AddNewScriptToRun(script);
			InvokeRunPackageScriptsCmdlet();
		}
		
		void AddNewScriptToRun(IPackageScript script)
		{
			packageScriptsToRun.AddScript(script);
		}
		
		void InvokeRunPackageScriptsCmdlet()
		{
			string command = "Invoke-RunPackageScripts";
			consoleHost.ExecuteCommand(command);
		}
	}
}
