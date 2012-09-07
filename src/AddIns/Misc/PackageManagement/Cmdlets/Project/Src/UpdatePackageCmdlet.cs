// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsData.Update, "Package", DefaultParameterSetName = "All")]
	public class UpdatePackageCmdlet : PackageManagementCmdlet
	{
		IUpdatePackageActionsFactory updatePackageActionsFactory;
		
		public UpdatePackageCmdlet()
			: this(
				new UpdatePackageActionsFactory(),
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public UpdatePackageCmdlet(
			IUpdatePackageActionsFactory updatePackageActionsFactory,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
		{
			this.updatePackageActionsFactory = updatePackageActionsFactory;
		}
		
		[Parameter(Position = 0, Mandatory = true, ParameterSetName = "Project")]
		[Parameter(Position = 0, ParameterSetName = "All")]
		public string Id { get; set; }
		
		[Parameter(Position = 1, ParameterSetName = "Project")]
		[Parameter(Position = 1, ParameterSetName = "All")]
		public string ProjectName { get; set; }
		
		[Parameter(Position = 2, ParameterSetName = "Project")]
		public SemanticVersion Version { get; set; }
		
		[Parameter(Position = 3)]
		public string Source { get; set; }
		
		[Parameter]
		public SwitchParameter IgnoreDependencies { get; set; }
		
		[Parameter, Alias("Prerelease")]
		public SwitchParameter IncludePrerelease { get; set; }

		protected override void ProcessRecord()
		{
			ThrowErrorIfProjectNotOpen();
			if (HasPackageId()) {
				if (HasProjectName()) {
					UpdatePackageInSingleProject();
				} else {
					UpdatePackageInAllProjects();
				}
			} else {
				if (HasProjectName()) {
					UpdateAllPackagesInProject();
				} else {
					UpdateAllPackagesInSolution();
				}
			}
		}
		
		bool HasPackageId()
		{
			return !String.IsNullOrEmpty(Id);
		}
		
		bool HasProjectName()
		{
			return !String.IsNullOrEmpty(ProjectName);
		}
		
		void UpdateAllPackagesInProject()
		{
			IUpdatePackageActions actions = CreateUpdateAllPackagesInProject();
			RunActions(actions);
		}
		
		IUpdatePackageActions CreateUpdateAllPackagesInProject()
		{
			IPackageManagementProject project = GetProject();
			return updatePackageActionsFactory.CreateUpdateAllPackagesInProject(project);
		}
		
		void UpdateAllPackagesInSolution()
		{
			IUpdatePackageActions actions = CreateUpdateAllPackagesInSolution();
			RunActions(actions);
		}
		
		IUpdatePackageActions CreateUpdateAllPackagesInSolution()
		{
			IPackageManagementSolution solution = ConsoleHost.Solution;
			IPackageRepository repository = GetActivePackageRepository();
			return updatePackageActionsFactory.CreateUpdateAllPackagesInSolution(solution, repository);
		}
		
		IPackageRepository GetActivePackageRepository()
		{
			PackageSource packageSource = ConsoleHost.GetActivePackageSource(Source);
			return ConsoleHost.GetPackageRepository(packageSource);
		}
		
		void UpdatePackageInSingleProject()
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
			action.UpdateDependencies = UpdateDependencies;
			action.AllowPrereleaseVersions = AllowPreleaseVersions;
			action.PackageScriptRunner = this;
			return action;
		}
		
		bool UpdateDependencies {
			get { return !IgnoreDependencies.IsPresent; }
		}
		
		bool AllowPreleaseVersions {
			get { return IncludePrerelease.IsPresent; }
		}
		
		void RunActions(IUpdatePackageActions updateActions)
		{
			updateActions.UpdateDependencies = UpdateDependencies;
			updateActions.AllowPrereleaseVersions = AllowPreleaseVersions;
			updateActions.PackageScriptRunner = this;
			
			foreach (UpdatePackageAction action in updateActions.CreateActions()) {
				action.Execute();
			}
		}
		
		void UpdatePackageInAllProjects()
		{
			IPackageManagementSolution solution = ConsoleHost.Solution;
			IPackageRepository repository = GetActivePackageRepository();
			PackageReference packageReference = CreatePackageReference();
			IUpdatePackageActions updateActions =
				updatePackageActionsFactory.CreateUpdatePackageInAllProjects(packageReference, solution, repository);
			
			RunActions(updateActions);
		}
		
		PackageReference CreatePackageReference()
		{
			return new PackageReference(Id, Version, null, null);
		}
	}
}
