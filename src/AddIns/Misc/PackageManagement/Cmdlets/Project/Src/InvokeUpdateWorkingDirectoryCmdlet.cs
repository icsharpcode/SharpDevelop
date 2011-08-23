// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsLifecycle.Invoke, "UpdateWorkingDirectory", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class InvokeUpdateWorkingDirectoryCmdlet : PackageManagementCmdlet
	{
		IPackageManagementProjectService projectService;
		
		public InvokeUpdateWorkingDirectoryCmdlet()
			: this(
				PackageManagementServices.ProjectService,
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public InvokeUpdateWorkingDirectoryCmdlet(
			IPackageManagementProjectService projectService,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
		{
			this.projectService = projectService;
		}
		
		protected override void ProcessRecord()
		{
			UpdateWorkingDirectory();
		}
		
		void UpdateWorkingDirectory()
		{
			string directory = GetWorkingDirectory();
			UpdateWorkingDirectory(directory);
		}
		
		string GetWorkingDirectory()
		{
			var workingDirectory = new PowerShellWorkingDirectory(projectService);
			return workingDirectory.GetWorkingDirectory();
		}
		
		void UpdateWorkingDirectory(string directory)
		{
			string command = String.Format("Set-Location {0}", directory);
			InvokeScript(command);
		}
	}
}
