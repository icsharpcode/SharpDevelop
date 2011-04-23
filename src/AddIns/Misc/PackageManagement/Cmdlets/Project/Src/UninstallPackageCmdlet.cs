// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsLifecycle.Uninstall, "Package", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class UninstallPackageCmdlet : PackageManagementCmdlet
	{
		public UninstallPackageCmdlet()
			: this(
				PackageManagementServices.Solution,
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public UninstallPackageCmdlet(
			IPackageManagementSolution solution,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(solution, consoleHost, terminatingError)
		{
		}
		
		[Parameter(Position = 0, Mandatory = true)]
		public string Id { get; set; }
		
		[Parameter(Position = 1)]
		public string ProjectName { get; set; }
		
		[Parameter(Position = 2)]
		public Version Version { get; set; }
		
		[Parameter]
		public SwitchParameter Force { get; set; }
		
		[Parameter]
		public SwitchParameter RemoveDependencies { get; set; }
		
		protected override void ProcessRecord()
		{
			ThrowErrorIfProjectNotOpen();
			UninstallPackage();
		}
		
		void UninstallPackage()
		{
			UninstallPackageAction action = CreateUninstallPackageAction();
			action.Execute();
		}
		
		UninstallPackageAction CreateUninstallPackageAction()
		{
			IPackageManagementProject project = ConsoleHost.GetProject(null, ProjectName);
			UninstallPackageAction action = project.CreateUninstallPackageAction();
			action.PackageId = Id;
			action.PackageVersion = Version;
			action.ForceRemove = Force.IsPresent;
			action.RemoveDependencies = RemoveDependencies.IsPresent;
			
			return action;
		}
	}
}
