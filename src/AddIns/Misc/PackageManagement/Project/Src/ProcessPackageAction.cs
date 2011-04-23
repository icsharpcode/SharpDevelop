// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public abstract class ProcessPackageAction
	{
		IPackageManagementSolution solution;
		IPackageManagementEvents packageManagementEvents;
		
		public ProcessPackageAction(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents)
		{
			this.solution = solution;
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public ProcessPackageAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
		{
			this.Project = project;
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public IPackageManagementProject Project { get; set; }
		public ILogger Logger { get; set; }
		public IPackage Package { get; set; }
		public Version PackageVersion { get; set; }
		public string PackageId { get; set; }
		public IPackageRepository SourceRepository { get; set; }
		public PackageSource PackageSource { get; set; }
		public MSBuildBasedProject MSBuildProject { get; set; }
		
		protected void OnParentPackageInstalled()
		{
			packageManagementEvents.OnParentPackageInstalled(Package);
		}
		
		protected void OnParentPackageUninstalled()
		{
			packageManagementEvents.OnParentPackageUninstalled(Package);
		}
		
		public void Execute()
		{
			BeforeExecute();
			ExecuteCore();
		}
		
		protected virtual void BeforeExecute()
		{
			GetLoggerIfMissing();
			CreateProject();
			ConfigureProjectLogger();
			GetPackageIfMissing();
		}
		
		protected virtual void ExecuteCore()
		{
		}
		
		void GetLoggerIfMissing()
		{
			if (Logger == null) {
				Logger = new PackageManagementLogger(packageManagementEvents);
			}
		}
		
		void CreateProject()
		{
			if (SourceRepository == null) {
				Project = solution.CreateProject(PackageSource, MSBuildProject);
			} else {
				Project = solution.GetActiveProject(SourceRepository);
			}
		}
		
		void ConfigureProjectLogger()
		{
			Project.Logger = Logger;
		}
		
		void GetPackageIfMissing()
		{
			if (Package == null) {
				Package = Project.SourceRepository.FindPackage(PackageId, PackageVersion);
			}
		}
	}
}
