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
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.Tests.Fakes;
using ICSharpCode.AddInManager2.ViewModel;
using ICSharpCode.Core;
using NuGet;
using NUnit.Framework;

namespace ICSharpCode.AddInManager2.Tests
{
	public class UpdatedAddInsViewModelTests
	{
		private const string SharpDevelopAddInTag = " sharpdevelopaddin ";
		
		FakeAddInManagerServices _services;
		
		AddIn _addIn1;
		AddIn _addIn1_new;
		AddIn _addIn2;
		AddIn _addIn2_new;
		AddIn _addIn_noVersion;
		
		public UpdatedAddInsViewModelTests()
		{
		}
		
		private void CreateAddIns()
		{
			// Create AddIn objects from *.addin files available in this assembly's output directory
			FakeAddInTree _addInTree = new FakeAddInTree();

			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test.addin"))
			{
				_addIn1 = AddIn.Load(_addInTree, streamReader);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_New.addin"))
			{
				_addIn1_new = AddIn.Load(_addInTree, streamReader);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_2.addin"))
			{
				_addIn2 = AddIn.Load(_addInTree, streamReader);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_2_New.addin"))
			{
				_addIn2_new = AddIn.Load(_addInTree, streamReader);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_noVersion.addin"))
			{
				_addIn_noVersion = AddIn.Load(_addInTree, streamReader);
			}
		}
		
		[SetUp]
		public void SetUp()
		{
			_services = new FakeAddInManagerServices();
			_services.FakeSDAddInManagement = new FakeSDAddInManagement();
			_services.Events = new AddInManagerEvents();
			_services.FakeSetup = new FakeAddInSetup(_services.SDAddInManagement);
			_services.FakeSettings = new FakeAddInManagerSettings();
			_services.FakeRepositories = new FakePackageRepositories();
			_services.FakeNuGet = new FakeNuGetPackageManager();
			
			// Create SynchronizationContext needed for the view model
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}
		
		[Test]
		public void ShowUpdatableAddIns()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			
			// Package to be shown in repository
			FakePackage fakePackage1_old = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version),
				Tags = SharpDevelopAddInTag
			};
			FakePackage fakePackage1_new = new FakePackage()
			{
				Id = _addIn1_new.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1_new.Version),
				Tags = SharpDevelopAddInTag,
				Published = new DateTimeOffset(DateTime.UtcNow)
			};
			FakePackage fakePackage2_old = new FakePackage()
			{
				Id = _addIn2.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn2.Version),
				Tags = SharpDevelopAddInTag
			};
			
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage1_old.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage1_old.Version.ToString());
			_addIn2.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage2_old.Id);
			_addIn2.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage2_old.Version.ToString());
			
			// List of NuGet repositories
			List<PackageSource> registeredPackageSources = new List<PackageSource>();
			registeredPackageSources.Add(new PackageSource("", "Test Repository"));
			_services.FakeRepositories.RegisteredPackageSources = registeredPackageSources;
			
			List<IPackageRepository> registeredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository remoteRepository = new FakeCorePackageRepository();
			remoteRepository.Source = registeredPackageSources[0].Source;
			remoteRepository.ReturnedPackages = (new IPackage[] { fakePackage1_new, fakePackage2_old }).AsQueryable();
			registeredPackageRepositories.Add(remoteRepository);
			_services.FakeRepositories.RegisteredPackageRepositories = registeredPackageRepositories;
			
			// PackageRepository service should return remoteRepository instance
			_services.FakeRepositories.GetRepositoryFromSourceCallback = delegate(PackageSource packageSource)
			{
				return remoteRepository;
			};
			
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1_old, fakePackage2_old }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			// Simulation of resolving AddIns <-> NuGet packages
			_services.FakeSetup.GetAddInForNuGetPackageCallback = delegate(IPackage package, bool withAddInsMarkedForInstallation)
			{
				if (package.Id == _addIn1.Properties[ManagedAddIn.NuGetPackageIDManifestAttribute])
				{
					return _addIn1;
				}
				else if (package.Id == _addIn2.Properties[ManagedAddIn.NuGetPackageIDManifestAttribute])
				{
					return _addIn2;
				}
				
				return null;
			};
			
			var viewModel = new UpdatedAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(1), "AddIn list must contain 1 item.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1_new.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1_new.Manifest.PrimaryIdentity), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(_addIn1_new.Version), "Version of 1st AddIn");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.False, "1st AddIn must not be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.True, "1st AddIn must be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.False, "1st AddIn must not be 'added'");
			Assert.That(firstAddIn.IsRemoved, Is.False, "1st AddIn must not be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.False, "1st AddIn must not have 'NuGet connection'");
			Assert.That(viewModel.AddInPackages[0].IsExternallyReferenced, Is.False, "1st AddIn must not be 'externally referenced'");
		}
		
		[Test]
		public void ShowUpdatablePreReleaseAddIns()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			
			// Package to be shown in repository
			FakePackage fakePackage1_old = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version),
				Tags = SharpDevelopAddInTag
			};
			FakePackage fakePackage1_new = new FakePackage()
			{
				Id = _addIn1_new.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1_new.Version),
				Tags = SharpDevelopAddInTag,
				Published = new DateTimeOffset(DateTime.UtcNow)
			};
			FakePackage fakePackage2_old = new FakePackage()
			{
				Id = _addIn2.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn2.Version),
				Tags = SharpDevelopAddInTag
			};
			FakePackage fakePackage2_new = new FakePackage()
			{
				Id = _addIn2_new.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn2_new.Version, "12345"),
				Tags = SharpDevelopAddInTag,
				Published = new DateTimeOffset(DateTime.UtcNow)
			};
			
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage1_old.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage1_old.Version.ToString());
			_addIn2.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage2_old.Id);
			_addIn2.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage2_old.Version.ToString());
			
			// Setting allows to show pre-releases
			_services.FakeSettings.ShowPrereleases = true;
			
			// List of NuGet repositories
			List<PackageSource> registeredPackageSources = new List<PackageSource>();
			registeredPackageSources.Add(new PackageSource("", "Test Repository"));
			_services.FakeRepositories.RegisteredPackageSources = registeredPackageSources;
			
			List<IPackageRepository> registeredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository remoteRepository = new FakeCorePackageRepository();
			remoteRepository.Source = registeredPackageSources[0].Source;
			remoteRepository.ReturnedPackages = (new IPackage[] { fakePackage1_new, fakePackage2_new }).AsQueryable();
			registeredPackageRepositories.Add(remoteRepository);
			_services.FakeRepositories.RegisteredPackageRepositories = registeredPackageRepositories;
			
			// PackageRepository service should return remoteRepository instance
			_services.FakeRepositories.GetRepositoryFromSourceCallback = delegate(PackageSource packageSource)
			{
				return remoteRepository;
			};
			
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1_old, fakePackage2_old }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			// Simulation of resolving AddIns <-> NuGet packages
			_services.FakeSetup.GetAddInForNuGetPackageCallback = delegate(IPackage package, bool withAddInsMarkedForInstallation)
			{
				if (package.Id == _addIn1.Properties[ManagedAddIn.NuGetPackageIDManifestAttribute])
				{
					return _addIn1;
				}
				else if (package.Id == _addIn2.Properties[ManagedAddIn.NuGetPackageIDManifestAttribute])
				{
					return _addIn2;
				}
				
				return null;
			};
			
			var viewModel = new UpdatedAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(2), "AddIn list must contain 2 items.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1_new.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1_new.Manifest.PrimaryIdentity), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(_addIn1_new.Version), "Version of 1st AddIn");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.False, "1st AddIn must not be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.True, "1st AddIn must be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.False, "1st AddIn must not be 'added'");
			Assert.That(firstAddIn.IsRemoved, Is.False, "1st AddIn must not be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.False, "1st AddIn must not have 'NuGet connection'");
			Assert.That(viewModel.AddInPackages[0].IsExternallyReferenced, Is.False, "1st AddIn must not be 'externally referenced'");
			
			AddInPackageViewModelBase secondAddIn = viewModel.AddInPackages[1];
			Assert.That(secondAddIn.Id, Is.EqualTo(_addIn2_new.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(secondAddIn.Name, Is.EqualTo(_addIn2_new.Manifest.PrimaryIdentity), "Name of 2nd AddIn");
			Assert.That(secondAddIn.Version, Is.EqualTo(_addIn2_new.Version), "Version of 1st AddIn");
		}
		
		[Test]
		public void ShowUpdatableAddInsOnPendingUpdate()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			
			// Package to be shown in repository
			FakePackage fakePackage1_old = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version),
				Tags = SharpDevelopAddInTag
			};
			FakePackage fakePackage1_new = new FakePackage()
			{
				Id = _addIn1_new.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1_new.Version),
				Tags = SharpDevelopAddInTag,
				Published = new DateTimeOffset(DateTime.UtcNow)
			};
			FakePackage fakePackage2_old = new FakePackage()
			{
				Id = _addIn2.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn2.Version),
				Tags = SharpDevelopAddInTag
			};
			
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage1_old.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage1_old.Version.ToString());
			_addIn1_new.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage1_new.Id);
			_addIn1_new.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage1_new.Version.ToString());
			_addIn2.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage2_old.Id);
			_addIn2.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage2_old.Version.ToString());
			
			// List of NuGet repositories
			List<PackageSource> registeredPackageSources = new List<PackageSource>();
			registeredPackageSources.Add(new PackageSource("", "Test Repository"));
			_services.FakeRepositories.RegisteredPackageSources = registeredPackageSources;
			
			List<IPackageRepository> registeredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository remoteRepository = new FakeCorePackageRepository();
			remoteRepository.Source = registeredPackageSources[0].Source;
			remoteRepository.ReturnedPackages = (new IPackage[] { fakePackage1_new, fakePackage2_old }).AsQueryable();
			registeredPackageRepositories.Add(remoteRepository);
			_services.FakeRepositories.RegisteredPackageRepositories = registeredPackageRepositories;
			
			// PackageRepository service should return remoteRepository instance
			_services.FakeRepositories.GetRepositoryFromSourceCallback = delegate(PackageSource packageSource)
			{
				return remoteRepository;
			};
			
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1_old, fakePackage1_new, fakePackage2_old }).AsQueryable();
			
			// Simulate list of AddIns
			_addIn1_new.Action = AddInAction.Update;
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			// Simulation of resolving AddIns <-> NuGet packages
			_services.FakeSetup.GetAddInForNuGetPackageCallback = delegate(IPackage package, bool withAddInsMarkedForInstallation)
			{
				if (package.Id == _addIn1_new.Properties[ManagedAddIn.NuGetPackageIDManifestAttribute])
				{
					return withAddInsMarkedForInstallation ? _addIn1_new : _addIn1;
				}
				else if (package.Id == _addIn2.Properties[ManagedAddIn.NuGetPackageIDManifestAttribute])
				{
					return _addIn2;
				}
				
				return null;
			};
			
			var viewModel = new UpdatedAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(1), "AddIn list must contain 1 item.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1_new.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1_new.Manifest.PrimaryIdentity), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(_addIn1_new.Version), "Version of 1st AddIn");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.False, "1st AddIn must not be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.True, "1st AddIn must be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.True, "1st AddIn must be 'added'");
			Assert.That(firstAddIn.IsRemoved, Is.False, "1st AddIn must not be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.False, "1st AddIn must not have 'NuGet connection'");
			Assert.That(viewModel.AddInPackages[0].IsExternallyReferenced, Is.False, "1st AddIn must not be 'externally referenced'");
		}
	}
}
