// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
		
		public void Run(IPackageAction action)
		{
			var actions = new List<IPackageAction>();
			actions.Add(action);
			Run(actions);
		}
		
		public void Run(IEnumerable<IPackageAction> actions)
		{
			CreateConsolePadIfConsoleHostIsNotRunning();
			ShowConsolePad();
			AddNewActionsToRun(actions);
			InvokeProcessPackageActionsCmdlet();
		}
		
		void CreateConsolePadIfConsoleHostIsNotRunning()
		{
			if (!consoleHost.IsRunning) {
				workbench.CreateConsolePad();
			}
		}
		
		void ShowConsolePad()
		{
			workbench.ShowConsolePad();
		}
		
		void AddNewActionsToRun(IEnumerable<IPackageAction> actions)
		{
			foreach (IPackageAction action in actions) {
				AddNewActionToRun(action);
			}
		}
		
		void AddNewActionToRun(IPackageAction action)
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
