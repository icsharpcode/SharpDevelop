// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public abstract class ProcessPackageAction
	{
		IPackageManagementService packageManagementService;
		IPackageManagementEvents packageManagementEvents;
		
		public ProcessPackageAction(
			IPackageManagementService packageManagementService,
			IPackageManagementEvents packageManagementEvents)
		{
			this.packageManagementService = packageManagementService;
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public ISharpDevelopPackageManager PackageManager { get; set; }
		public ILogger Logger { get; set; }
		public IPackage Package { get; set; }
		public Version PackageVersion { get; set; }
		public string PackageId { get; set; }
		public IPackageRepository PackageRepository { get; set; }
		public PackageSource PackageSource { get; set; }
		public MSBuildBasedProject Project { get; set; }
		
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
			CreatePackageManager();
			ConfigurePackageManagerLogger();
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
		
		void CreatePackageManager()
		{
			if (PackageRepository == null) {
				PackageManager = packageManagementService.CreatePackageManager(PackageSource, Project);
			} else {
				PackageManager = packageManagementService.CreatePackageManagerForActiveProject(PackageRepository);
			}
		}
		
		void ConfigurePackageManagerLogger()
		{
			PackageManager.Logger = Logger;
			PackageManager.FileSystem.Logger = Logger;
			
			IProjectManager projectManager = PackageManager.ProjectManager;
			projectManager.Logger = Logger;
			projectManager.Project.Logger = Logger;
		}
		
		void GetPackageIfMissing()
		{
			if (Package == null) {
				Package = PackageManager.SourceRepository.FindPackage(PackageId, PackageVersion);
			}
		}
	}
}
