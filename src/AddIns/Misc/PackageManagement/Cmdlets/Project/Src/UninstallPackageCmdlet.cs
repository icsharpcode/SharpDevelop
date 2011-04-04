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
				ServiceLocator.PackageManagementService,
				ServiceLocator.PackageManagementConsoleHost,
				null)
		{
		}
		
		public UninstallPackageCmdlet(
			IPackageManagementService packageManagementService,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(packageManagementService, consoleHost, terminatingError)
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
			MSBuildBasedProject project = GetActiveProject(ProjectName);
			PackageSource packageSource = GetActivePackageSource();
			
			UninstallPackageAction action = CreateUninstallPackageAction(project, packageSource);
			action.Execute();
		}
		
		PackageSource GetActivePackageSource()
		{
			return GetActivePackageSource(null);
		}
		
		UninstallPackageAction CreateUninstallPackageAction(MSBuildBasedProject project, PackageSource packageSource)
		{
			var action = PackageManagementService.CreateUninstallPackageAction();
			action.PackageId = Id;
			action.PackageVersion = Version;
			action.Project = project;
			action.PackageSource = packageSource;
			action.ForceRemove = Force.IsPresent;
			action.RemoveDependencies = RemoveDependencies.IsPresent;
			
			return action;
		}
	}
}
