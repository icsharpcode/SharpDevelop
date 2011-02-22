// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
				PackageReferenceAdded(this, new PackageOperationEventArgs(package, String.Empty));
			}
		}

		#pragma warning disable 67
		public event EventHandler<PackageOperationEventArgs> PackageReferenceAdding;
		public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved;
		public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoving;
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
		
		public void AddPackageReference(string packageId, Version version, bool ignoreDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void RemovePackageReference(string packageId, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackageReference(string packageId, Version version, bool updateDependencies)
		{
			throw new NotImplementedException();
		}
		
		public bool IsInstalled(IPackage package)
		{
			return IsInstalledReturnValue;
		}
		
		public void FirePackageReferenceAdded(IPackage package)
		{
			OnPackageReferenceAdded(package);
		}
	}
}
