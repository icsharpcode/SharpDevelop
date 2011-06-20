// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageReferencesForProject : IPackageReferencesForProject
	{
		MSBuildBasedProject project;
		IPackageReferenceFileFactory packageReferenceFileFactory;
		IPackageReferenceInstaller packageReferenceInstaller;
		IPackageReferenceFile packageReferenceFile;
		List<PackageReference> packageReferences;

		public PackageReferencesForProject(
			MSBuildBasedProject project,
			IPackageRepositoryCache packageRepositoryCache)
			: this(
				project,
				new PackageReferenceInstaller(packageRepositoryCache),
				new PackageReferenceFileFactory())
		{
		}
		
		public PackageReferencesForProject(
			MSBuildBasedProject project,
			IPackageReferenceInstaller packageReferenceInstaller,
			IPackageReferenceFileFactory packageReferenceFileFactory)
		{
			this.project = project;
			this.packageReferenceFileFactory = packageReferenceFileFactory;
			this.packageReferenceInstaller = packageReferenceInstaller;
		}
		
		public void RemovePackageReferences()
		{
			GetPackageReferences();
			IPackageReferenceFile file = PackageReferenceFile;
			file.Delete();
		}
		
		void GetPackageReferences()
		{
			if (packageReferences == null) {
				IEnumerable<PackageReference> packageReferencesInFile = PackageReferenceFile.GetPackageReferences();
				packageReferences = new List<PackageReference>(packageReferencesInFile);
			}
		}
		
		IPackageReferenceFile PackageReferenceFile {
			get {
				if (packageReferenceFile == null) {
					packageReferenceFile = GetPackageReferenceFile();
				}
				return packageReferenceFile;
			}
		}
		
		IPackageReferenceFile GetPackageReferenceFile()
		{
			var fileName = new PackageReferenceFileNameForProject(project);
			return packageReferenceFileFactory.CreatePackageReferenceFile(fileName.ToString());
		}
		
		public void InstallPackages()
		{
			GetPackageReferences();
			packageReferenceInstaller.InstallPackages(packageReferences, project);
		}
	}
}
