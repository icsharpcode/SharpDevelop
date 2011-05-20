// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsLifecycle.Invoke, "InitializePackages", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class InvokeInitializePackagesCmdlet : PackageManagementCmdlet
	{
		IPackageManagementProjectService projectService;
		IPackageInitializationScriptsFactory scriptsFactory;
		
		public InvokeInitializePackagesCmdlet()
			: this(
				PackageManagementServices.ProjectService,
				new PackageInitializationScriptsFactory(),
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public InvokeInitializePackagesCmdlet(
			IPackageManagementProjectService projectService,
			IPackageInitializationScriptsFactory scriptsFactory,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
		{
			this.projectService = projectService;
			this.scriptsFactory = scriptsFactory;
		}
		
		protected override void ProcessRecord()
		{
			UpdateWorkingDirectory();
			RunPackageInitializationScripts();
		}
		
		void UpdateWorkingDirectory()
		{
			string command = "Invoke-UpdateWorkingDirectory";
			InvokeScript(command);
		}
		
		void RunPackageInitializationScripts()
		{
			IPackageInitializationScripts scripts = GetPackageInitializationScripts();
			if (scripts.Any()) {
				scripts.Run(this);
			}
		}
		
		IPackageInitializationScripts GetPackageInitializationScripts()
		{
			Solution solution = projectService.OpenSolution;
			return scriptsFactory.CreatePackageInitializationScripts(solution);
		}
	}
}
