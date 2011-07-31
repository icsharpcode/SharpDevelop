// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageActionRunner : IPackageActionRunner
	{
		IPackageActionRunner consolePackageActionRunner;
		IPackageManagementEvents packageManagementEvents;
		IPowerShellDetection powerShellDetection;
		
		public PackageActionRunner(
			IPackageActionRunner consolePackageActionRunner,
			IPackageManagementEvents packageManagementEvents)
			: this(consolePackageActionRunner, packageManagementEvents, new PowerShellDetection())
		{
		}
		
		public PackageActionRunner(
			IPackageActionRunner consolePackageActionRunner,
			IPackageManagementEvents packageManagementEvents,
			IPowerShellDetection powerShellDetection)
		{
			this.consolePackageActionRunner = consolePackageActionRunner;
			this.packageManagementEvents = packageManagementEvents;
			this.powerShellDetection = powerShellDetection;
		}
		
		public void Run(IEnumerable<ProcessPackageAction> actions)
		{
			if (ShouldRunActionsInConsole(actions)) {
				consolePackageActionRunner.Run(actions);
			} else {
				foreach (ProcessPackageAction action in actions) {
					action.Execute();
				}
			}
		}
		
		bool ShouldRunActionsInConsole(IEnumerable<ProcessPackageAction> actions)
		{
			foreach (ProcessPackageAction action in actions) {
				if (ShouldRunActionInConsole(action)) {
					return true;
				}
			}
			return false;
		}
		
		public void Run(ProcessPackageAction action)
		{
			if (ShouldRunActionInConsole(action)) {
				consolePackageActionRunner.Run(action);
			} else {
				action.Execute();
			}
		}
		
		bool ShouldRunActionInConsole(ProcessPackageAction action)
		{
			if (action.HasPackageScriptsToRun()) {
				if (powerShellDetection.IsPowerShell2Installed()) {
					return true;
				} else {
					ReportPowerShellIsNotInstalled();
				}
			}
			return false;
		}
		
		void ReportPowerShellIsNotInstalled()
		{
			string message = 
				"PowerShell is not installed. PowerShell scripts will not be run for the package.";
			packageManagementEvents.OnPackageOperationMessageLogged(MessageLevel.Warning, message);
		}
	}
}
