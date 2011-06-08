// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		
		public void Run(ProcessPackageAction action)
		{
			if (RunActionInConsole(action)) {
				consolePackageActionRunner.Run(action);
			} else {
				action.Execute();
			}
		}
		
		bool RunActionInConsole(ProcessPackageAction action)
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
