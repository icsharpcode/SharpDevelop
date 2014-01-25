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
		
		public void Run(IEnumerable<IPackageAction> actions)
		{
			if (ShouldRunActionsInConsole(actions)) {
				consolePackageActionRunner.Run(actions);
			} else {
				foreach (ProcessPackageAction action in actions) {
					action.Execute();
				}
			}
		}
		
		bool ShouldRunActionsInConsole(IEnumerable<IPackageAction> actions)
		{
			foreach (IPackageAction action in actions) {
				if (ShouldRunActionInConsole(action)) {
					return true;
				}
			}
			return false;
		}
		
		public void Run(IPackageAction action)
		{
			if (ShouldRunActionInConsole(action)) {
				consolePackageActionRunner.Run(action);
			} else {
				action.Execute();
			}
		}
		
		bool ShouldRunActionInConsole(IPackageAction action)
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
