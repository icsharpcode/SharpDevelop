// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	/// <summary>
	/// Description of FakeNuGetCorePackageManager.
	/// </summary>
	public class FakeNuGetCorePackageManager : NuGet.IPackageManager
	{
		public FakeNuGetCorePackageManager()
		{
		}
		
		public event EventHandler<NuGet.PackageOperationEventArgs> PackageInstalled;
		
		public event EventHandler<NuGet.PackageOperationEventArgs> PackageInstalling;
		
		public event EventHandler<NuGet.PackageOperationEventArgs> PackageUninstalled;
		
		public event EventHandler<NuGet.PackageOperationEventArgs> PackageUninstalling;
		
		public NuGet.IFileSystem FileSystem
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		
		public NuGet.IPackageRepository LocalRepository
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		public NuGet.ILogger Logger
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		
		public NuGet.IPackageRepository SourceRepository
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		public NuGet.IPackagePathResolver PathResolver
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		public void InstallPackage(NuGet.IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void InstallPackage(string packageId, NuGet.SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(NuGet.IPackage newPackage, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(string packageId, NuGet.SemanticVersion version, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(string packageId, NuGet.IVersionSpec versionSpec, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UninstallPackage(NuGet.IPackage package, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UninstallPackage(string packageId, NuGet.SemanticVersion version, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
	}
}
