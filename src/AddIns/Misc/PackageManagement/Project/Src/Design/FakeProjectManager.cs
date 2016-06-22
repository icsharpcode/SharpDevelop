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

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeProjectManager : ISharpDevelopProjectManager
	{
		public FakePackageRepository FakeLocalRepository {
			get { return LocalRepository as FakePackageRepository; }
			set { LocalRepository = value; }
		}
		
		public FakePackageRepository FakeSourceRepository {
			get { return SourceRepository as FakePackageRepository; }
			set { SourceRepository = value; }
		}
		
		public bool IsInstalledReturnValue;
		
		public FakeProjectManager()
		{
			LocalRepository = new FakePackageRepository();
			SourceRepository = new FakePackageRepository();
		}
		
		public event EventHandler<PackageOperationEventArgs> PackageReferenceAdded;
		
		protected virtual void OnPackageReferenceAdded(IPackage package)
		{
			if (PackageReferenceAdded != null) {
				PackageReferenceAdded(this, new PackageOperationEventArgs(package, null, String.Empty));
			}
		}

		public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoving;
		
		protected virtual void OnPackageReferenceRemoving(IPackage package)
		{
			if (PackageReferenceRemoving != null) {
				PackageReferenceRemoving(this, new PackageOperationEventArgs(package, null, String.Empty));
			}
		}
		
		#pragma warning disable 67
		public event EventHandler<PackageOperationEventArgs> PackageReferenceAdding;
		public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved;
		#pragma warning restore 67
		
		public IPackageRepository LocalRepository { get; set; }
		public ILogger Logger { get; set; }
		public IPackageRepository SourceRepository { get; set; }
		public IPackagePathResolver PathResolver { get; set; }

		public IProjectSystem Project {
			get { return FakeProjectSystem; }
			set { FakeProjectSystem = value as FakeProjectSystem; }
		}
		
		public FakeProjectSystem FakeProjectSystem = new FakeProjectSystem();
		
		public void RemovePackageReference(string packageId, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
		
		public IPackage PackagePassedToIsInstalled;
		
		public bool IsInstalled(IPackage package)
		{
			PackagePassedToIsInstalled = package;
			return IsInstalledReturnValue;
		}
		
		public string PackageIdPassedToIsInstalled;
		
		public bool IsInstalled(string packageId)
		{
			PackageIdPassedToIsInstalled = packageId;
			return IsInstalledReturnValue;
		}
		
		public void FirePackageReferenceAdded(IPackage package)
		{
			OnPackageReferenceAdded(package);
		}
		
		public void FirePackageReferenceRemoving(IPackage package)
		{
			OnPackageReferenceRemoving(package);
		}
		
		public void AddPackageReference(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void RemovePackageReference(IPackage package, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void AddPackageReference(string packageId, SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackageReference(string packageId, SemanticVersion version, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackageReference(string packageId, IVersionSpec versionSpec, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public IPackage PackagePassedToHasOlderPackageInstalled;
		public bool HasOlderPackageInstalledReturnValue;
		
		public bool HasOlderPackageInstalled(IPackage package)
		{
			PackagePassedToHasOlderPackageInstalled = package;
			return HasOlderPackageInstalledReturnValue;
		}
		
		public DependencyVersion DependencyVersion {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool WhatIf {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public void UpdatePackageReference(IPackage remotePackage, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
	}
}
