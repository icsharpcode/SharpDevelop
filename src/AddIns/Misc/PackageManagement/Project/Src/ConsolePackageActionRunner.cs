// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement
{
	public class ConsolePackageActionRunner : IPackageActionRunner
	{
		IPackageManagementConsoleHost consoleHost;
		PackageActionsToRun packageActionsToRun;
		IPackageManagementWorkbench workbench;
		
		public ConsolePackageActionRunner(
			IPackageManagementConsoleHost consoleHost,
			PackageActionsToRun packageActionsToRun)
			: this(consoleHost, packageActionsToRun, new PackageManagementWorkbench())
		{
		}
		
		public ConsolePackageActionRunner(
			IPackageManagementConsoleHost consoleHost,
			PackageActionsToRun packageActionsToRun,
			IPackageManagementWorkbench workbench)
		{
			this.consoleHost = consoleHost;
			this.packageActionsToRun = packageActionsToRun;
			this.workbench = workbench;
		}
		
		public void Run(ProcessPackageAction action)
		{
			CreateConsolePadIfConsoleHostIsNotRunning();
			RunAction(action);
		}
		
		void CreateConsolePadIfConsoleHostIsNotRunning()
		{
			if (!consoleHost.IsRunning) {
				workbench.CreateConsolePad();
			}
		}
		
		void RunAction(ProcessPackageAction action)
		{
			AddNewActionToRun(action);
			InvokeProcessPackageActionsCmdlet();
		}
		
		void AddNewActionToRun(ProcessPackageAction action)
		{
			packageActionsToRun.AddAction(action);
		}
		
		void InvokeProcessPackageActionsCmdlet()
		{
			string command = "Invoke-ProcessPackageActions";
			consoleHost.ExecuteCommand(command);
		}
	}
}
