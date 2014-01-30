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
