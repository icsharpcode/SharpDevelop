// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.Versioning;
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
		ProjectTargetFramework targetFramework;
		
		public PackageManagementProject(
			IPackageRepository sourceRepository,
			MSBuildBasedProject project,
			IPackageManagementEvents packageManagementEvents,
			IPackageManagerFactory packageManagerFactory)
		{
			SourceRepository = sourceRepository;
			msbuildProject = project;
			targetFramework = new ProjectTargetFramework(project);
			this.packageManagementEvents = packageManagementEvents;
			
			packageManager = packageManagerFactory.CreatePackageManager(sourceRepository, project);
			projectManager = packageManager.ProjectManager;
		}
		
		public string Name {
			get { return msbuildProject.Name; }
		}
		
		public FrameworkName TargetFramework {
			get { return targetFramework.TargetFrameworkName; }
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
		
		public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoving {
			add { projectManager.PackageReferenceRemoving += value; }
			remove { projectManager.PackageReferenceRemoving -= value; }
		}
		
		public bool IsPackageInstalled(IPackage package)
		{
			return projectManager.IsInstalled(package);
		}
		
		public bool IsPackageInstalled(string packageId)
		{
			return projectManager.IsInstalled(packageId);
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			return projectManager.LocalRepository.GetPackages();
		}
		
		public IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, InstallPackageAction installAction)
		{
			return packageManager.GetInstallPackageOperations(package, installAction);
		}
		
		public void InstallPackage(IPackage package, InstallPackageAction installAction)
		{
			packageManager.InstallPackage(package, installAction);
		}
		
		public void UninstallPackage(IPackage package, UninstallPackageAction uninstallAction)
		{
			packageManager.UninstallPackage(package, uninstallAction);
		}
		
		public void UpdatePackage(IPackage package, UpdatePackageAction updateAction)
		{
			packageManager.UpdatePackage(package, updateAction);
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
		
		public IEnumerable<IPackage> GetPackagesInReverseDependencyOrder()
		{
			var packageSorter = new PackageSorter(null);
			return packageSorter
				.GetPackagesByDependencyOrder(projectManager.LocalRepository)
				.Reverse();
		}
		
		public void UpdatePackages(UpdatePackagesAction updateAction)
		{
			packageManager.UpdatePackages(updateAction);
		}
		
		public UpdatePackagesAction CreateUpdatePackagesAction()
		{
			return new UpdatePackagesAction(this, packageManagementEvents);
		}
		
		public IEnumerable<PackageOperation> GetUpdatePackagesOperations(
			IEnumerable<IPackage> packages,
			IUpdatePackageSettings settings)
		{
			return packageManager.GetUpdatePackageOperations(packages, settings);
		}
		
		public void RunPackageOperations(IEnumerable<PackageOperation> operations)
		{
			packageManager.RunPackageOperations(operations);
		}
		
		public bool HasOlderPackageInstalled(IPackage package)
		{
			return projectManager.HasOlderPackageInstalled(package);
		}
		
		public void UpdatePackageReference(IPackage package, IUpdatePackageSettings settings)
		{
			packageManager.UpdatePackageReference(package, settings);
		}
		
		public IPackageConstraintProvider ConstraintProvider {
			get {
				var constraintProvider = projectManager.LocalRepository as IPackageConstraintProvider;
				if (constraintProvider != null) {
					return constraintProvider;
				}
				return NullConstraintProvider.Instance;
			}
		}

		public IPackage FindPackage(string packageId, SemanticVersion version)
		{
			IPackage package = projectManager.LocalRepository.FindPackage(packageId, version);

			if (package != null) {
				return package;
			}

			if (version != null) {
				return packageManager.LocalRepository.FindPackage(packageId, version);
			}

			List<IPackage> packages = packageManager.LocalRepository.FindPackagesById(packageId).ToList();
			if (packages.Count > 1) {
				throw CreateAmbiguousPackageException(packageId);
			}
			return packages.FirstOrDefault();
		}

		InvalidOperationException CreateAmbiguousPackageException(string packageId)
		{
			string message = String.Format("Multiple versions of '{0}' found. Please specify the version.", packageId);
			return new InvalidOperationException(message);
		}
	}
}
