// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakeNuGetCorePackageManager : NuGet.IPackageManager
	{
		public FakeNuGetCorePackageManager()
		{
		}
		
		#pragma warning disable 0067
		public event EventHandler<NuGet.PackageOperationEventArgs> PackageInstalled;
		
		public event EventHandler<NuGet.PackageOperationEventArgs> PackageInstalling;
		
		public event EventHandler<NuGet.PackageOperationEventArgs> PackageUninstalled;
		
		public event EventHandler<NuGet.PackageOperationEventArgs> PackageUninstalling;
		#pragma warning restore 0067
		
		public NuGet.IFileSystem FileSystem
		{
			get;
			set;
		}
		
		public NuGet.IPackageRepository LocalRepository
		{
			get;
			set;
		}
		
		public NuGet.ILogger Logger
		{
			get;
			set;
		}
		
		public NuGet.IPackageRepository SourceRepository
		{
			get;
			set;
		}
		
		public NuGet.IPackagePathResolver PathResolver
		{
			get
			{
				return null;
			}
		}
		
		public Action<IPackage, bool, bool> InstallPackageCallback
		{
			get;
			set;
		}
		
		public Action<string, SemanticVersion, bool, bool> InstallPackageWithVersionCallback
		{
			get;
			set;
		}
		
		public void InstallPackage(NuGet.IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			if (InstallPackageCallback != null)
			{
				InstallPackageCallback(package, ignoreDependencies, allowPrereleaseVersions);
			}
		}
		
		public void InstallPackage(string packageId, NuGet.SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			if (InstallPackageWithVersionCallback != null)
			{
				InstallPackageWithVersionCallback(packageId, version, ignoreDependencies, allowPrereleaseVersions);
			}
		}
		
		public Action<IPackage, bool, bool> UpdatePackageCallback
		{
			get;
			set;
		}
		
		public void UpdatePackage(NuGet.IPackage newPackage, bool updateDependencies, bool allowPrereleaseVersions)
		{
			if (UpdatePackageCallback != null)
			{
				UpdatePackageCallback(newPackage, updateDependencies, allowPrereleaseVersions);
			}
		}
		
		public Action<string, SemanticVersion, bool, bool> UpdatePackageWithVersionCallback
		{
			get;
			set;
		}
		
		public void UpdatePackage(string packageId, NuGet.SemanticVersion version, bool updateDependencies, bool allowPrereleaseVersions)
		{
			if (UpdatePackageWithVersionCallback != null)
			{
				UpdatePackageWithVersionCallback(packageId, version, updateDependencies, allowPrereleaseVersions);
			}
		}
		
		public Action<string, IVersionSpec, bool, bool> UpdatePackageWithVersionSpecCallback
		{
			get;
			set;
		}
		
		public void UpdatePackage(string packageId, NuGet.IVersionSpec versionSpec, bool updateDependencies, bool allowPrereleaseVersions)
		{
			if (UpdatePackageWithVersionSpecCallback != null)
			{
				UpdatePackageWithVersionSpecCallback(packageId, versionSpec, updateDependencies, allowPrereleaseVersions);
			}
		}
		
		public Action<IPackage, bool, bool> UninstallPackageCallback
		{
			get;
			set;
		}
		
		public void UninstallPackage(NuGet.IPackage package, bool forceRemove, bool removeDependencies)
		{
			if (UninstallPackageCallback != null)
			{
				UninstallPackageCallback(package, forceRemove, removeDependencies);
			}
		}
		
		public Action<string, SemanticVersion, bool, bool> UninstallPackageWithVersionCallback
		{
			get;
			set;
		}
		
		public void UninstallPackage(string packageId, NuGet.SemanticVersion version, bool forceRemove, bool removeDependencies)
		{
			if (UninstallPackageWithVersionCallback != null)
			{
				UninstallPackageWithVersionCallback(packageId, version, forceRemove, removeDependencies);
			}
		}
		
		public void InstallPackage(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions, bool ignoreWalkInfo)
		{
			throw new NotImplementedException();
		}
	}
}
