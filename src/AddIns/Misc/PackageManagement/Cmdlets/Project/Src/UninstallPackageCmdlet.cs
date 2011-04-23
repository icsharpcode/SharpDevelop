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
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public UninstallPackageCmdlet(
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
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
			string source = null; 
			IPackageManagementProject project = ConsoleHost.GetProject(source, ProjectName);
			UninstallPackageAction action = project.CreateUninstallPackageAction();
			action.PackageId = Id;
			action.PackageVersion = Version;
			action.ForceRemove = Force.IsPresent;
			action.RemoveDependencies = RemoveDependencies.IsPresent;
			
			return action;
		}
	}
}
