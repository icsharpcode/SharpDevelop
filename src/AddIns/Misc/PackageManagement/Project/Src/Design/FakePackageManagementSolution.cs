// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementSolution : IPackageManagementSolution
	{
		public FakeInstallPackageAction ActionToReturnFromCreateInstallPackageAction =
			new FakeInstallPackageAction();
		
		public virtual InstallPackageAction CreateInstallPackageAction()
		{
			return ActionToReturnFromCreateInstallPackageAction;
		}
		
		public FakeUninstallPackageAction ActionToReturnFromCreateUninstallPackageAction =
			new FakeUninstallPackageAction();
		
		public virtual UninstallPackageAction CreateUninstallPackageAction()
		{
			return ActionToReturnFromCreateUninstallPackageAction;
		}
		
		public void AddPackageToActiveProjectLocalRepository(FakePackage package)
		{
			FakeProject.FakePackages.Add(package);
		}
		
		public FakePackage AddPackageToActiveProjectLocalRepository(string packageId)
		{
			var package = new FakePackage(packageId);
			AddPackageToActiveProjectLocalRepository(package);
			return package;
		}
		
		public FakeUpdatePackageAction ActionToReturnFromCreateUpdatePackageAction =
			new FakeUpdatePackageAction();
		
		public UpdatePackageAction CreateUpdatePackageAction()
		{
			return ActionToReturnFromCreateUpdatePackageAction;
		}
		
		public int GetActiveProjectCallCount;
		public FakePackageManagementProject FakeProject = new FakePackageManagementProject();
		
		public virtual IPackageManagementProject GetActiveProject()
		{
			GetActiveProjectCallCount++;
			
			return FakeProject;
		}
		
		public IPackageRepository RepositoryPassedToCreateProject;
		public MSBuildBasedProject ProjectPassedToCreateProject;
		
		public IPackageManagementProject CreateProject(IPackageRepository sourceRepository, MSBuildBasedProject project)
		{
			RepositoryPassedToCreateProject = sourceRepository;
			ProjectPassedToCreateProject = project;
			return FakeProject;
		}
		
		public IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository)
		{
			RepositoryPassedToCreateProject = sourceRepository;
			return FakeProject;
		}
		
		public PackageSource PackageSourcePassedToCreateProject;
		
		public IPackageManagementProject CreateProject(PackageSource source, MSBuildBasedProject project)
		{
			PackageSourcePassedToCreateProject = source;
			ProjectPassedToCreateProject = project;
			return FakeProject;
		}
	}
}
