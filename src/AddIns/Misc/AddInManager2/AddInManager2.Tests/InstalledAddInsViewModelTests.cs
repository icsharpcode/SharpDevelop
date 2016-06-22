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
using System.IO;
using System.Collections.Generic;
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
	public class InstalledAddInsViewModelTests
	{
		private const string SharpDevelopAddInTag = " sharpdevelopaddin ";
		
		FakeAddInManagerServices _services;
		
		AddIn _addIn1;
		AddIn _addIn1_new;
		AddIn _addIn2;
		AddIn _addIn_noVersion;
		
		public InstalledAddInsViewModelTests()
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
			
//			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_2_New.addin"))
//			{
//				_addIn2_new = AddIn.Load(_addInTree, streamReader);
//			}
			
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
		public void ShowInstalledOfflineAddIns()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			
			// Empty list of NuGet repositories
			_services.FakeRepositories.RegisteredPackageSources = new List<PackageSource>();
			_services.FakeRepositories.RegisteredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			var viewModel = new InstalledAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(2), "AddIn list must contain 2 items.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1.Name), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(_addIn1.Version), "Version of 1st AddIn");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.True, "1st AddIn must be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.False, "1st AddIn must not be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.False, "1st AddIn must not be 'added'");
			Assert.That(firstAddIn.IsRemoved, Is.False, "1st AddIn must not be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.False, "1st AddIn must not have 'NuGet connection'");
			
			// Check 'externally referenced' status of both AddIns
			// (simulating that IsAddInManifestIinExternalPath() returns true for 1st AddIn and false for 2nd)
			_services.FakeSDAddInManagement.IsAddInManifestInExternalPathCallback = (addIn) => addIn == _addIn1;
			Assert.That(viewModel.AddInPackages[0].IsExternallyReferenced, Is.True, "1st AddIn must be 'externally referenced'");
			Assert.That(viewModel.AddInPackages[1].IsExternallyReferenced, Is.False, "2nd AddIn must not be 'externally referenced'");
		}
		
		[Test]
		public void ShowOfflineAddInsMarkedForUninstallation()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			_addIn1.Action = AddInAction.Uninstall;
			
			// Empty list of NuGet repositories
			_services.FakeRepositories.RegisteredPackageSources = new List<PackageSource>();
			_services.FakeRepositories.RegisteredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			var viewModel = new InstalledAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(2), "AddIn list must contain 2 items.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1.Name), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(_addIn1.Version), "Version of 1st AddIn");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.True, "1st AddIn must be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.False, "1st AddIn must not be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.False, "1st AddIn must not be 'added'");
			Assert.That(firstAddIn.IsRemoved, Is.True, "1st AddIn must be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.False, "1st AddIn must not have 'NuGet connection'");
			
			// Check 'externally referenced' status of both AddIns
			// (simulating that IsAddInManifestIinExternalPath() returns true for 1st AddIn and false for 2nd)
			_services.FakeSDAddInManagement.IsAddInManifestInExternalPathCallback = (addIn) => addIn == _addIn1;
			Assert.That(viewModel.AddInPackages[0].IsExternallyReferenced, Is.True, "1st AddIn must be 'externally referenced'");
			Assert.That(viewModel.AddInPackages[1].IsExternallyReferenced, Is.False, "2nd AddIn must not be 'externally referenced'");
		}
		
		[Test]
		public void ShowInstalledNuGetAddIns()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version),
				Tags = SharpDevelopAddInTag
			};
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage.Version.ToString());
			
			// Empty list of NuGet repositories
			_services.FakeRepositories.RegisteredPackageSources = new List<PackageSource>();
			_services.FakeRepositories.RegisteredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			var viewModel = new InstalledAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(2), "AddIn list must contain 2 items.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1.Name), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(fakePackage.Version.Version), "NuGet (!) version of 1st AddIn");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.True, "1st AddIn must be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.False, "1st AddIn must not be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.False, "1st AddIn must not be 'added'");
			Assert.That(firstAddIn.IsRemoved, Is.False, "1st AddIn must not be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.True, "1st AddIn must have 'NuGet connection'");
		}
		
		[Test]
		public void ShowingNuGetVersionIfAddInVersionIsEmpty()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion("1.0.2.0"),
				Tags = SharpDevelopAddInTag
			};
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage.Version.ToString());
			
			// Empty list of NuGet repositories
			_services.FakeRepositories.RegisteredPackageSources = new List<PackageSource>();
			_services.FakeRepositories.RegisteredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			
			var viewModel = new InstalledAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(1), "AddIn list must contain 1 item.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1.Manifest.PrimaryIdentity), "Primary identity of AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1.Name), "Name of AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(fakePackage.Version.Version), "NuGet (!) version of AddIn");
			Assert.That(firstAddIn.IsInstalled, Is.True, "AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.True, "AddIn must be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.False, "AddIn must not be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.False, "AddIn must not be 'added'");
			Assert.That(firstAddIn.IsRemoved, Is.False, "AddIn must not be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.True, "AddIn must have 'NuGet connection'");
		}
		
		[Test]
		public void ShowNuGetAddInMarkedForInstallation()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version),
				Tags = SharpDevelopAddInTag
			};
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage.Version.ToString());
			
			// Empty list of NuGet repositories
			_services.FakeRepositories.RegisteredPackageSources = new List<PackageSource>();
			_services.FakeRepositories.RegisteredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			// Simulate marked AddIn (usually set up by AddInSetup service)
			_services.FakeSetup.AddInsMarkedForInstallList.Add(
				new ManagedAddIn(_addIn1)
				{
					IsTemporary = true,
					InstallationSource = AddInInstallationSource.NuGetRepository
				}
			);
			
			var viewModel = new InstalledAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(2), "AddIn list must contain 2 items.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1.Name), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(fakePackage.Version.Version), "NuGet (!) Version of 1st AddIn");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.True, "1st AddIn must be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.False, "1st AddIn must not be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.True, "1st AddIn must be 'added'");;
			Assert.That(firstAddIn.IsRemoved, Is.False, "1st AddIn must not be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.True, "1st AddIn must have 'NuGet connection'");
		}
		
		[Test]
		public void ShowNuGetAddInMarkedForUpdate()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			_addIn1_new.Enabled = true;
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version),
				Tags = SharpDevelopAddInTag
			};
			FakePackage fakePackage_new = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1_new.Version),
				Tags = SharpDevelopAddInTag
			};
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage.Version.ToString());
			_addIn1_new.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage_new.Id);
			_addIn1_new.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage_new.Version.ToString());
			
			// Empty list of NuGet repositories
			_services.FakeRepositories.RegisteredPackageSources = new List<PackageSource>();
			_services.FakeRepositories.RegisteredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			// Simulate marked AddIn (usually set up by AddInSetup service)
			_services.FakeSetup.AddInsMarkedForInstallList.Add(
				new ManagedAddIn(_addIn1_new)
				{
					IsUpdate = true,
					OldVersion = _addIn1.Version,
					InstallationSource = AddInInstallationSource.NuGetRepository,
					IsTemporary = true
				}
			);
			
			var viewModel = new InstalledAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(2), "AddIn list must contain 2 items.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1.Name), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(_addIn1_new.Version), "Version of 1st AddIn must be the one of the update");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.True, "1st AddIn must be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.True, "1st AddIn must be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.True, "1st AddIn must be 'added'");
			Assert.That(firstAddIn.IsRemoved, Is.False, "1st AddIn must not be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.True, "1st AddIn must have 'NuGet connection'");
		}
		
		[Test]
		public void ShowNuGetAddInMarkedForUninstallation()
		{
			CreateAddIns();
			_addIn1.Enabled = true;
			_addIn1.Action = AddInAction.Uninstall;
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version),
				Tags = SharpDevelopAddInTag
			};
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage.Version.ToString());
			
			// Empty list of NuGet repositories
			_services.FakeRepositories.RegisteredPackageSources = new List<PackageSource>();
			_services.FakeRepositories.RegisteredPackageRepositories = new List<IPackageRepository>();
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_services.FakeNuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			var viewModel = new InstalledAddInsViewModel(_services);
			viewModel.ReadPackagesAndWaitForUpdate();
			
			Assert.That(viewModel.AddInPackages.Count, Is.EqualTo(2), "AddIn list must contain 2 items.");
			
			AddInPackageViewModelBase firstAddIn = viewModel.AddInPackages[0];
			Assert.That(firstAddIn.Id, Is.EqualTo(_addIn1.Manifest.PrimaryIdentity), "Primary identity of 1st AddIn");
			Assert.That(firstAddIn.Name, Is.EqualTo(_addIn1.Name), "Name of 1st AddIn");
			Assert.That(firstAddIn.Version, Is.EqualTo(_addIn1.Version), "Version of 1st AddIn must be the one of the update");
			Assert.That(firstAddIn.IsInstalled, Is.True, "1st AddIn must be 'installed''");
			Assert.That(firstAddIn.IsOffline, Is.True, "1st AddIn must be 'offline'");
			Assert.That(firstAddIn.IsEnabled, Is.True, "1st AddIn must be 'enabled'");
			Assert.That(firstAddIn.IsUpdate, Is.False, "1st AddIn must not be 'update'");
			Assert.That(firstAddIn.IsAdded, Is.False, "1st AddIn must not be 'added'");;
			Assert.That(firstAddIn.IsRemoved, Is.True, "1st AddIn must be 'removed'");
			Assert.That(firstAddIn.HasNuGetConnection, Is.True, "1st AddIn must have 'NuGet connection'");
		}
	}
}
