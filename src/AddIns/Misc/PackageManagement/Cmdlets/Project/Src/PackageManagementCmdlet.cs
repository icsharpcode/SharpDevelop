// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	public abstract class PackageManagementCmdlet : PSCmdlet, ITerminatingCmdlet
	{
		IPackageManagementSolution solution;
		IPackageManagementConsoleHost consoleHost;
		ICmdletTerminatingError terminatingError;
		
		public PackageManagementCmdlet(
			IPackageManagementSolution solution,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
		{
			this.solution = solution;
			this.consoleHost = consoleHost;
			this.terminatingError = terminatingError;
		}
		
		protected IPackageManagementSolution Solution {
			get { return solution; }
		}
		
		protected IPackageManagementConsoleHost ConsoleHost {
			get { return consoleHost; }
		}
		
		protected MSBuildBasedProject DefaultProject {
			get { return consoleHost.DefaultProject as MSBuildBasedProject; }
		}

		protected ICmdletTerminatingError TerminatingError {
			get {
				if (terminatingError == null) {
					terminatingError = new CmdletTerminatingError(this);
				}
				return terminatingError;
			}
		}
		
		protected void ThrowErrorIfProjectNotOpen()
		{
			if (DefaultProject == null) {
				ThrowProjectNotOpenTerminatingError();
			}
		}
			
		protected void ThrowProjectNotOpenTerminatingError()
		{
			TerminatingError.ThrowNoProjectOpenError();
		}
		
		protected PackageSource GetActivePackageSource(string source)
		{
			if (source != null) {
				return new PackageSource(source);
			}
			return ConsoleHost.ActivePackageSource;
		}
		
		protected MSBuildBasedProject GetActiveProject(string projectName)
		{
			if (projectName != null) {
				return ConsoleHost.GetProject(projectName);
			}
			return ConsoleHost.DefaultProject as MSBuildBasedProject;
		}
	}
}
