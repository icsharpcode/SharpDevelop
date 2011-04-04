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
		
		public ProcessPackageAction(IPackageManagementService packageManagementService)
		{
			this.packageManagementService = packageManagementService;
		}
		
		public ISharpDevelopPackageManager PackageManager { get; set; }
		public IPackage Package { get; set; }
		public Version PackageVersion { get; set; }
		public string PackageId { get; set; }
		public IPackageRepository PackageRepository { get; set; }
		public PackageSource PackageSource { get; set; }
		public MSBuildBasedProject Project { get; set; }
		
		protected void OnParentPackageInstalled()
		{
			packageManagementService.OnParentPackageInstalled(Package);
		}
		
		protected void OnParentPackageUninstalled()
		{
			packageManagementService.OnParentPackageUninstalled(Package);
		}
		
		public void Execute()
		{
			BeforeExecute();
			ExecuteCore();
		}
		
		protected virtual void BeforeExecute()
		{
			CreatePackageManagerIfMissing();
			GetPackageIfMissing();
		}
		
		protected virtual void ExecuteCore()
		{
		}
		
		void CreatePackageManagerIfMissing()
		{
			if (PackageManager == null) {
				CreatePackageManager();
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
		
		void GetPackageIfMissing()
		{
			if (Package == null) {
				Package = PackageManager.SourceRepository.FindPackage(PackageId, PackageVersion);
			}
		}
	}
}
