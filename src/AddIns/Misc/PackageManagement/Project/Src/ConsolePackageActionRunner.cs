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
