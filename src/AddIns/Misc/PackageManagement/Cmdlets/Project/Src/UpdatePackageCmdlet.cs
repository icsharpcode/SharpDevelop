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
				PackageManagementServices.Solution,
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public UpdatePackageCmdlet(
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
			PackageSource packageSource = GetActivePackageSource(Source);
			MSBuildBasedProject project = GetActiveProject(ProjectName);
			
			UpdatePackageAction action = CreateUpdatePackageAction(packageSource, project);
			action.Execute();
		}
		
		UpdatePackageAction CreateUpdatePackageAction(PackageSource packageSource, MSBuildBasedProject project)
		{
			var action = Solution.CreateUpdatePackageAction();
			action.PackageSource = packageSource;
			action.MSBuildProject = project;
			action.PackageId = Id;
			action.PackageVersion = Version;
			action.UpdateDependencies = !IgnoreDependencies.IsPresent;
			return action;
		}
	}
}
