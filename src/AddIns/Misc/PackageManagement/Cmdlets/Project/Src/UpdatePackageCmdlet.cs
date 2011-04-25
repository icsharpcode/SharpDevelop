// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsData.Update, "Package")]
	public class UpdatePackageCmdlet : PackageManagementCmdlet
	{
		public UpdatePackageCmdlet()
			: this(
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public UpdatePackageCmdlet(
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
		
		[Parameter(Position = 3)]
		public string Source { get; set; }
		
		[Parameter]
		public SwitchParameter IgnoreDependencies { get; set; }
		
		protected override void ProcessRecord()
		{
			ThrowErrorIfProjectNotOpen();
			UpdatePackage();
		}
		
		void UpdatePackage()
		{
			IPackageManagementProject project = GetProject();
			UpdatePackageAction action = CreateUpdatePackageAction(project);
			action.Execute();
		}
		
		IPackageManagementProject GetProject()
		{
			return ConsoleHost.GetProject(Source, ProjectName);
		}
		
		UpdatePackageAction CreateUpdatePackageAction(IPackageManagementProject project)
		{
			UpdatePackageAction action = project.CreateUpdatePackageAction();
			action.PackageId = Id;
			action.PackageVersion = Version;
			action.UpdateDependencies = !IgnoreDependencies.IsPresent;
			action.PackageScriptSession = this;
			return action;
		}
	}
}
