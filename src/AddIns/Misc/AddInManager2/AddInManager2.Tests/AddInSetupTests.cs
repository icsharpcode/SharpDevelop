// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.AddInManager2;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.Tests.Fakes;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NuGet;
using NUnit.Framework;

namespace ICSharpCode.AddInManager2.Tests
{
	[TestFixture]
	[Category("AddInSetupTests")]
	public class AddInSetupTests
	{
		AddInManagerEvents _events;
		FakeNuGetPackageManager _nuGet;
		FakeSDAddInManagement _sdAddInManagement;
		AddInSetup _addInSetup;
		
		AddIn _addIn1;
		AddIn _addIn1_new;
		AddIn _addIn2;
		AddIn _addIn2_new;
		
		public AddInSetupTests()
		{
		}
		
		private void PrepareAddInSetup()
		{
			_events = new AddInManagerEvents();
			_nuGet = new FakeNuGetPackageManager();
			_sdAddInManagement = new FakeSDAddInManagement();
			_addInSetup = new AddInSetup(_events, _nuGet, _sdAddInManagement);
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
		}
		
		[Test, Description("AddIn must be installed from external *.addin manifest file. Pending installation must be cancellable.")]
		public void InstallValidAddInFromManifestAndCancel()
		{
			CreateAddIns();

			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInInstalledEventReceived = false;
			_events.AddInInstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInInstalledEventReceived = true;
				}
			};
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInUninstalledEventReceived = true;
				}
			};
			
			// Install the AddIn from external .addin manifest
			_sdAddInManagement.AddInToLoad = _addIn1;
			AddIn installedAddIn = _addInSetup.InstallAddIn(@"TestResources\AddInManager2Test.addin");
			
			// The AddIn must have been added to AddInTree
			Assert.That(installedAddIn, Is.Not.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			Assert.That(_sdAddInManagement.AddedExternalAddIns, Contains.Item(_addIn1), "AddIn object added as external");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			var foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn found in AddInsWithMarkedForInstallation");
			
			var foundAddIn = foundAddIns.First();
			
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.Offline), "ManagedAddIn's installation source is 'offline'");
			
			Assert.That(addInInstalledEventReceived, "AddInInstalled event sent with correct AddIn");
			
			// Cancel the installation
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			_addInSetup.CancelInstallation(_addIn1);
			
			// Now check AddInTree, again
			foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), Is.False, "ManagedAddIn not found in AddInsWithMarkedForInstallation after cancel");
			Assert.That(_sdAddInManagement.RemovedExternalAddIns, Contains.Item(_addIn1), "External AddIn has been removed.");
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
		}
		
		[Test, Description("AddIn must be installed from offline *.sdaddin package. Pending installation must be cancellable.")]
		public void InstallValidAddInFromOfflinePackageAndCancel()
		{
			CreateAddIns();
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInInstalledEventReceived = false;
			_events.AddInInstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInInstalledEventReceived = true;
				}
			};
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInUninstalledEventReceived = true;
				}
			};
			
			// Install the AddIn from *.sdaddin package
			_sdAddInManagement.AddInToLoad = _addIn1;
			AddIn installedAddIn = _addInSetup.InstallAddIn(@"TestResources\AddInManager2Test.sdaddin");
			
			// The AddIn must have been added to AddInTree
			Assert.That(installedAddIn, Is.Not.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			var foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn found in AddInsWithMarkedForInstallation");
			
			var foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.Offline), "ManagedAddIn's installation source is 'offline'");
			
			Assert.That(addInInstalledEventReceived, "AddInInstalled event sent with correct AddIn");
			
			// Cancel the installation
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			_addInSetup.CancelInstallation(_addIn1);
			
			// Now check AddInTree, again
			foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), Is.False, "ManagedAddIn not found in AddInsWithMarkedForInstallation after cancel");
			Assert.That(_sdAddInManagement.RemovedExternalAddIns, Contains.Item(_addIn1), "External AddIn has been removed.");
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
		}
		
		[Test, Description("Installed AddIn must be updated from offline *.sdaddin package. Pending update must be cancellable.")]
		public void UpdateValidAddInFromOfflinePackageAndCancel()
		{
			CreateAddIns();
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInInstalledEventReceived = false;
			_events.AddInInstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if ((e.AddIn.Manifest.PrimaryIdentity == _addIn1_new.Manifest.PrimaryIdentity)
				    && (e.AddIn.Version == _addIn1_new.Version))
				{
					addInInstalledEventReceived = true;
				}
			};
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if ((e.AddIn.Manifest.PrimaryIdentity == _addIn1_new.Manifest.PrimaryIdentity)
				    && (e.AddIn.Version == _addIn1_new.Version))
				{
					addInUninstalledEventReceived = true;
				}
			};
			
			// This AddIn is already installed
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			
			// Install the new version of AddIn from manifest
			_sdAddInManagement.AddInToLoad = _addIn1_new;
			AddIn installedAddIn = _addInSetup.InstallAddIn(@"TestResources\AddInManager2Test_New.sdaddin");
			
			// Test updated AddIn in AddInTree
			Assert.That(installedAddIn, Is.Not.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "Old AddIn object still in AddInTree");
			Assert.That(_sdAddInManagement.RegisteredAddIns.Contains(_addIn1_new), Is.Not.True,
			            "New AddIn object not in AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			var foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn found in AddInsWithMarkedForInstallation");
			
			var foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.AddIn.Version, Is.EqualTo(_addIn1_new.Version), "ManagedAddIn must have new version");
			Assert.That(foundAddIn.OldVersion, Is.EqualTo(_addIn1.Version), "ManagedAddIn must know installed (old) version");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.True, "ManagedAddIn is an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.Offline), "ManagedAddIn's installation source is 'offline'");
			
			Assert.That(addInInstalledEventReceived, "AddInInstalled event sent with correct AddIn");
			
			// Cancel the update
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			_addInSetup.CancelUpdate(_addIn1_new);
			
			// Now check AddInTree, again
			foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn still found in AddInsWithMarkedForInstallation after cancel");
			
			foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.AddIn.Version, Is.EqualTo(_addIn1.Version), "ManagedAddIn now has old version");
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
		}
		
		[Test, Description("AddIn must be installed from downloaded, but not installed NuGet package. Pending installation must be cancellable.")]
		public void InstallValidAddInFromIncompletelyInstalledNuGetPackageAndCancel()
		{
			CreateAddIns();
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version)
			};
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInInstalledEventReceived = false;
			_events.AddInInstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInInstalledEventReceived = true;
				}
			};
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInUninstalledEventReceived = true;
				}
			};
			bool nuGetPackageUninstalled = false;
			_nuGet.FakeCorePackageManager.UninstallPackageCallback = delegate(IPackage package, bool forceRemove, bool removeDependencies)
			{
				if ((package == fakePackage) && forceRemove && !removeDependencies)
				{
					nuGetPackageUninstalled = true;
				}
			};
			
			// Install the AddIn from an extracted NuGet package
			_sdAddInManagement.AddInToLoad = _addIn1;
			AddIn installedAddIn = _addInSetup.InstallAddIn(fakePackage, "TestResources");
			
			// The AddIn must have been added to AddInTree
			Assert.That(installedAddIn, Is.Not.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			var foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn found in AddInsWithMarkedForInstallation");
			
			var foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.NuGetRepository), "ManagedAddIn's installation source is 'NuGet'");
			
			Assert.That(addInInstalledEventReceived, "AddInInstalled event sent with correct AddIn");
			
			// Cancel the installation
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_nuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			_addInSetup.CancelInstallation(_addIn1);
			
			// Now check AddInTree, again
			foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), Is.False, "ManagedAddIn not found in AddInsWithMarkedForInstallation after cancel");
			Assert.That(_sdAddInManagement.RemovedExternalAddIns, Contains.Item(_addIn1), "External AddIn has been removed.");
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
			Assert.That(nuGetPackageUninstalled, "Downloaded NuGet package should be uninstalled.");
		}
		
		[Test, Description("AddIn must be installed from downloaded NuGet package. Pending installation must be cancellable.")]
		public void InstallValidAddInFromDownloadedNuGetPackageAndCancel()
		{
			CreateAddIns();
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version)
			};
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInInstalledEventReceived = false;
			_events.AddInInstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInInstalledEventReceived = true;
				}
			};
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInUninstalledEventReceived = true;
				}
			};
			bool nuGetPackageUninstalled = false;
			_nuGet.FakeCorePackageManager.UninstallPackageCallback = delegate(IPackage package, bool forceRemove, bool removeDependencies)
			{
				if ((package == fakePackage) && forceRemove && !removeDependencies)
				{
					nuGetPackageUninstalled = true;
				}
			};
			
			// Simulate download by NuGet Core PackageManager
			_sdAddInManagement.AddInToLoad = _addIn1;
			_events.OnAddInPackageDownloaded(new PackageOperationEventArgs(fakePackage, null, "TestResources"));
			
			// The AddIn must have been added to AddInTree
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			var foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn found in AddInsWithMarkedForInstallation");
			
			var foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.NuGetRepository), "ManagedAddIn's installation source is 'NuGet'");
			
			Assert.That(addInInstalledEventReceived, "AddInInstalled event sent with correct AddIn");
			
			// Cancel the installation
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_nuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			_addInSetup.CancelInstallation(_addIn1);
			
			// Now check AddInTree, again
			foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), Is.False, "ManagedAddIn not found in AddInsWithMarkedForInstallation after cancel");
			Assert.That(_sdAddInManagement.RemovedExternalAddIns, Contains.Item(_addIn1), "External AddIn has been removed.");
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
			Assert.That(nuGetPackageUninstalled, "Downloaded NuGet package should be uninstalled.");
		}
		
		[Test, Description("Installed AddIn must be updated from downloaded NuGet package. Pending update must be cancellable.")]
		public void UpdateValidAddInFromDownloadedNuGetPackageAndCancel()
		{
			CreateAddIns();
			
			// Create fake packages
			FakePackage fakePackage_new = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1_new.Version)
			};
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInInstalledEventReceived = false;
			_events.AddInInstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1_new.Manifest.PrimaryIdentity)
				{
					addInInstalledEventReceived = true;
				}
			};
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1_new.Manifest.PrimaryIdentity)
				{
					addInUninstalledEventReceived = true;
				}
			};
			bool nuGetPackageUninstalled = false;
			_nuGet.FakeCorePackageManager.UninstallPackageCallback = delegate(IPackage package, bool forceRemove, bool removeDependencies)
			{
				if ((package == fakePackage_new) && forceRemove && !removeDependencies)
				{
					nuGetPackageUninstalled = true;
				}
			};
			
			// This AddIn is already installed
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			
			// Simulate download of new version by NuGet Core PackageManager
			_sdAddInManagement.AddInToLoad = _addIn1_new;
			_events.OnAddInPackageDownloaded(new PackageOperationEventArgs(fakePackage_new, null, "TestResources"));
			
			// Test updated AddIn in AddInTree
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "Old AddIn object still in AddInTree");
			Assert.That(_sdAddInManagement.RegisteredAddIns.Contains(_addIn1_new), Is.Not.True,
			            "New AddIn object not in AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			var foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn found in AddInsWithMarkedForInstallation");
			
			var foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.AddIn.Version, Is.EqualTo(_addIn1_new.Version), "ManagedAddIn must have new version");
			Assert.That(foundAddIn.OldVersion, Is.EqualTo(_addIn1.Version), "ManagedAddIn must know installed (old) version");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.True, "ManagedAddIn is an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.NuGetRepository), "ManagedAddIn's installation source is 'NuGet'");
			
			Assert.That(addInInstalledEventReceived, "AddInInstalled event sent with correct AddIn");
			
			// Cancel the update
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_nuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage_new }).AsQueryable();
			
			_addInSetup.CancelUpdate(_addIn1_new);
			
			// Now check AddInTree, again
			foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn still found in AddInsWithMarkedForInstallation after cancel");
			
			foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.AddIn.Version, Is.EqualTo(_addIn1.Version), "ManagedAddIn now has old version");
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
			Assert.That(nuGetPackageUninstalled, "Downloaded NuGet package should be uninstalled.");
		}
		
		[Test, Description("AddIn must be installed from downloaded NuGet package. Cancellation of update is not a valid operation and should be handled.")]
		public void InstallAddInFromNuGetPackageAndInvalidUpdateCancel()
		{
			CreateAddIns();
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version)
			};
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInInstalledEventReceived = false;
			_events.AddInInstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInInstalledEventReceived = true;
				}
			};
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInUninstalledEventReceived = true;
				}
			};
			bool nuGetPackageUninstalled = false;
			_nuGet.FakeCorePackageManager.UninstallPackageCallback = delegate(IPackage package, bool forceRemove, bool removeDependencies)
			{
				if ((package == fakePackage) && forceRemove && !removeDependencies)
				{
					nuGetPackageUninstalled = true;
				}
			};
			
			// Simulate download by NuGet Core PackageManager
			_sdAddInManagement.AddInToLoad = _addIn1;
			_events.OnAddInPackageDownloaded(new PackageOperationEventArgs(fakePackage, null, "TestResources"));
			
			// The AddIn must have been added to AddInTree
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			var foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn found in AddInsWithMarkedForInstallation");
			
			var foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.NuGetRepository), "ManagedAddIn's installation source is 'NuGet'");
			
			Assert.That(addInInstalledEventReceived, "AddInInstalled event sent with correct AddIn");
			
			// Cancel the installation
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_nuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			_addInSetup.CancelUpdate(_addIn1);
			
			// Now check AddInTree, again
			foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn still found in AddInsWithMarkedForInstallation");
			Assert.That(_sdAddInManagement.RemovedExternalAddIns.Contains(_addIn1), Is.False, "External AddIn has not been removed.");
			
			Assert.That(addInUninstalledEventReceived, Is.False, "AddInUninstalled event sent with correct AddIn");
			Assert.That(nuGetPackageUninstalled, Is.False, "Downloaded NuGet package should be uninstalled.");
		}
		
		[Test, Description("External AddIn must be uninstalled. Pending uninstallation must be cancellable.")]
		public void UninstallValidAddInFromManifestAndCancel()
		{
			CreateAddIns();

			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInUninstalledEventReceived = true;
				}
			};
			
			// Simulate an installed external AddIn
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			
			// Remove the AddIn
			_addInSetup.UninstallAddIn(_addIn1);
			
			Assert.That(_sdAddInManagement.RemovedExternalAddIns, Contains.Item(_addIn1), "AddIn must have been removed from external AddIns list.");
			Assert.That(_sdAddInManagement.AddInsMarkedForRemoval, Contains.Item(_addIn1.Manifest.PrimaryIdentity), "AddIn must have been marked for removal on next startup.");
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
			
			// Cancel the uninstallation
			_addInSetup.CancelUninstallation(_addIn1);
			
			// Check if uninstallation has been reverted
			Assert.That(_sdAddInManagement.AddInsMarkedForRemoval.Contains(_addIn1.Manifest.PrimaryIdentity), Is.False, "AddIn must not be marked for removal any more.");
		}
		
		[Test, Description("AddIn installed from a NuGet package must be uninstalled.")]
		public void UninstallValidNuGetAddIn()
		{
			CreateAddIns();
			
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version)
			};

			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInUninstalledEventReceived = false;
			_events.AddInUninstalled += delegate(object sender, AddInInstallationEventArgs e)
			{
				if (e.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					addInUninstalledEventReceived = true;
				}
			};
			
			// Simulate an installed AddIn
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			
			// Simulate the installed NuGet package in local repository
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_nuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			bool nuGetPackageUninstalled = false;
			_nuGet.FakeCorePackageManager.UninstallPackageCallback = delegate(IPackage package, bool forceRemove, bool removeDependencies)
			{
				if ((package == fakePackage) && forceRemove && !removeDependencies)
				{
					nuGetPackageUninstalled = true;
				}
			};
			
			// Remove the AddIn
			_addInSetup.UninstallAddIn(_addIn1);
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
			
			// Simulate removing unreferenced NuGet packages
			_addInSetup.RemoveUnreferencedNuGetPackages();
			
			Assert.That(nuGetPackageUninstalled, Is.True, "NuGet package must be removed after restart.");
		}
	}
}
