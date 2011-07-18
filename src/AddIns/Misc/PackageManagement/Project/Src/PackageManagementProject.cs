// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementProject : IPackageManagementProject
	{
		ISharpDevelopPackageManager packageManager;
		ISharpDevelopProjectManager projectManager;
		IPackageManagementEvents packageManagementEvents;
		MSBuildBasedProject msbuildProject;
		
		public PackageManagementProject(
			IPackageRepository sourceRepository,
			MSBuildBasedProject project,
			IPackageManagementEvents packageManagementEvents,
			IPackageManagerFactory packageManagerFactory)
		{
			SourceRepository = sourceRepository;
			msbuildProject = project;
			this.packageManagementEvents = packageManagementEvents;
			
			packageManager = packageManagerFactory.CreatePackageManager(sourceRepository, project);
			projectManager = packageManager.ProjectManager;
		}
		
		public string Name {
			get { return msbuildProject.Name; }
		}
		
		public IPackageRepository SourceRepository { get; private set; }
		
		public ILogger Logger {
			get { return packageManager.Logger; }
			set {
				packageManager.Logger = value;
				packageManager.FileSystem.Logger = value;
			
				projectManager.Logger = value;
				projectManager.Project.Logger = value;
			}
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageInstalled {
			add { packageManager.PackageInstalled += value; }
			remove { packageManager.PackageInstalled -= value; }
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageUninstalled {
			add { packageManager.PackageUninstalled += value; }
			remove { packageManager.PackageUninstalled -= value; }
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageReferenceAdded {
			add { projectManager.PackageReferenceAdded += value; }
			remove { projectManager.PackageReferenceAdded -= value; }
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved {
			add { projectManager.PackageReferenceRemoved += value; }
			remove { projectManager.PackageReferenceRemoved -= value; }
		}
		
		public bool IsPackageInstalled(IPackage package)
		{
			return projectManager.IsInstalled(package);
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			return projectManager.LocalRepository.GetPackages();
		}
		
		public IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, bool ignoreDependencies)
		{
			return packageManager.GetInstallPackageOperations(package, ignoreDependencies);
		}
		
		public void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations, bool ignoreDependencies)
		{
			packageManager.InstallPackage(package, operations, ignoreDependencies);
		}
		
		public void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
		{
			packageManager.UninstallPackage(package, forceRemove, removeDependencies);
		}
		
		public void UpdatePackage(IPackage package, IEnumerable<PackageOperation> operations, bool updateDependencies)
		{
			packageManager.UpdatePackage(package, operations, updateDependencies);
		}
		
		public InstallPackageAction CreateInstallPackageAction()
		{
			return new InstallPackageAction(this, packageManagementEvents);
		}
		
		public UninstallPackageAction CreateUninstallPackageAction()
		{
			return new UninstallPackageAction(this, packageManagementEvents);
		}
		
		public UpdatePackageAction CreateUpdatePackageAction()
		{
			return new UpdatePackageAction(this, packageManagementEvents);
		}
		
		public Project ConvertToDTEProject()
		{
			return new Project(msbuildProject);
		}
	}
}
