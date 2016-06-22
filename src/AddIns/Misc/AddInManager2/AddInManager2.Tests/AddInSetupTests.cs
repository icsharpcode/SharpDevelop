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
		AddIn _addIn_noIdentity;
		AddIn _addIn_noVersion;
		
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
		
		private void ClearAddInNuGetProperties(AddIn addIn)
		{
			addIn.Properties.Remove(ManagedAddIn.NuGetPackageIDManifestAttribute);
			addIn.Properties.Remove(ManagedAddIn.NuGetPackageVersionManifestAttribute);
		}
		
		private void CreateAddIns()
		{
			// Create AddIn objects from *.addin files available in this assembly's output directory
			FakeAddInTree _addInTree = new FakeAddInTree();

			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test.addin"))
			{
				_addIn1 = AddIn.Load(_addInTree, streamReader);
				ClearAddInNuGetProperties(_addIn1);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_New.addin"))
			{
				_addIn1_new = AddIn.Load(_addInTree, streamReader);
				ClearAddInNuGetProperties(_addIn1_new);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_2.addin"))
			{
				_addIn2 = AddIn.Load(_addInTree, streamReader);
				ClearAddInNuGetProperties(_addIn2);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_2_New.addin"))
			{
				_addIn2_new = AddIn.Load(_addInTree, streamReader);
				ClearAddInNuGetProperties(_addIn2_new);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_noIdentity.addin"))
			{
				_addIn_noIdentity = AddIn.Load(_addInTree, streamReader);
				ClearAddInNuGetProperties(_addIn_noIdentity);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_noVersion.addin"))
			{
				_addIn_noVersion = AddIn.Load(_addInTree, streamReader);
				ClearAddInNuGetProperties(_addIn_noVersion);
			}
		}
		
		[Test]
		public void CompareAddInToPackageVersionWithRealService()
		{
			CreateAddIns();
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			CompareAddInToPackageVersionForAddInSetupInstance(_addInSetup);
		}
		
		[Test]
		public void CompareAddInToPackageVersionWithFakeService()
		{
			CreateAddIns();
			
			// Prepare all (fake) services and the FakeAddInSetup to test
			PrepareAddInSetup();
			FakeAddInSetup fakeAddInSetup = new FakeAddInSetup(_sdAddInManagement);
			
			CompareAddInToPackageVersionForAddInSetupInstance(fakeAddInSetup);
		}
		
		private void CompareAddInToPackageVersionForAddInSetupInstance(IAddInSetup addInSetup)
		{
			// Create fake packages
			FakePackage fakePackageEqual = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version)
			};
			FakePackage fakePackageGreater = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion("9.9.9.9")
			};
			FakePackage fakePackageLess = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion("0.0.0.0")
			};
			
			// Ensure we have no __nuGet... attributes in manifest
			if (_addIn1.Properties.Contains(ManagedAddIn.NuGetPackageVersionManifestAttribute))
			{
				_addIn1.Properties.Remove(ManagedAddIn.NuGetPackageVersionManifestAttribute);
			}
			
			Assert.That(addInSetup.CompareAddInToPackageVersion(_addIn1, fakePackageEqual), Is.EqualTo(0), "Comparing AddIn 1.0.0.0 and NuGet package 1.0.0.0");
			Assert.That(addInSetup.CompareAddInToPackageVersion(_addIn1, fakePackageGreater), Is.LessThan(0), "Comparing AddIn 1.0.0.0 and NuGet package 9.9.9.9");
			Assert.That(addInSetup.CompareAddInToPackageVersion(_addIn1, fakePackageLess), Is.GreaterThan(0), "Comparing AddIn 1.0.0.0 and NuGet package 0.0.0.0");
			
			// Comparison if there's no version in manifest
			Assert.That(addInSetup.CompareAddInToPackageVersion(_addIn_noVersion, fakePackageEqual), Is.LessThan(0), "Comparing AddIn <null> and NuGet package 1.0.0.0");
			
			// Comparison if there's no regular version in manifest, but there is one in __nuGet... attribute
			_addIn_noVersion.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackageEqual.Version.ToString());
			Assert.That(addInSetup.CompareAddInToPackageVersion(_addIn_noVersion, fakePackageEqual), Is.EqualTo(0), "Comparing AddIn <null> (NuGet: 1.0.0.0) and NuGet package 1.0.0.0");
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
		
		[Test, Description("AddIn without identity should not be installed, but must be handled with an error event.")]
		public void InstallInvalidAddInFromManifest()
		{
			CreateAddIns();

			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Prepare event handlers
			bool addInOperationErrorReceived = false;
			_events.AddInOperationError += delegate(object sender, AddInOperationErrorEventArgs e)
			{
				addInOperationErrorReceived = true;
			};
			
			// Install the AddIn from external .addin manifest
			_sdAddInManagement.AddInToLoad = _addIn_noIdentity;
			AddIn installedAddIn = _addInSetup.InstallAddIn(@"TestResources\AddInManager2Test_noIdentity.addin");
			
			// The AddIn must have been added to AddInTree
			Assert.That(installedAddIn, Is.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns.Contains(_addIn1), Is.False, "AddIn object added to AddInTree");
			
			Assert.That(addInOperationErrorReceived, "AddInOperationError event sent");
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
		
		[Test, Description("AddIn installed from a NuGet package must be updated from an offline *.sdaddin file. Pending update must be cancellable.")]
		public void UpdateAddInFromDownloadedNuGetPackageWithOfflineAddInAndCancel()
		{
			CreateAddIns();
			
			// Create fake packages
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version)
			};
			_addIn1.Properties[ManagedAddIn.NuGetPackageIDManifestAttribute] = _addIn1.Manifest.PrimaryIdentity;
			_addIn1.Properties[ManagedAddIn.NuGetPackageVersionManifestAttribute] = _addIn1.Version.ToString();
			
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
				if ((package == fakePackage) && forceRemove && !removeDependencies)
				{
					nuGetPackageUninstalled = true;
				}
			};
			
			// This AddIn is already installed
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_nuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
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
			Assert.That(nuGetPackageUninstalled, Is.False, "Already installed NuGet package must not be removed.");
		}
		
		[Test, Description("AddIn installed from a NuGet package must be updated from an offline *.sdaddin file with an older version. Pending update must be cancellable.")]
		public void UpdateAddInFromDownloadedNuGetPackageWithOlderOfflineAddInAndCancel()
		{
			CreateAddIns();
			
			// Create fake packages
			FakePackage fakePackage = new FakePackage()
			{
				Id = _addIn1_new.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1_new.Version)
			};
			_addIn1_new.Properties[ManagedAddIn.NuGetPackageIDManifestAttribute] = _addIn1_new.Manifest.PrimaryIdentity;
			_addIn1_new.Properties[ManagedAddIn.NuGetPackageVersionManifestAttribute] = _addIn1_new.Version.ToString();
			
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
				if ((package == fakePackage) && forceRemove && !removeDependencies)
				{
					nuGetPackageUninstalled = true;
				}
			};
			
			// This AddIn is already installed
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1_new);
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_nuGet.FakeCorePackageManager.LocalRepository = localRepository;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage }).AsQueryable();
			
			// Install the new version of AddIn from manifest
			_sdAddInManagement.AddInToLoad = _addIn1;
			AddIn installedAddIn = _addInSetup.InstallAddIn(@"TestResources\AddInManager2Test.sdaddin");
			
			// Test updated AddIn in AddInTree
			Assert.That(installedAddIn, Is.Not.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1_new), "Old AddIn object still in AddInTree");
			Assert.That(_sdAddInManagement.RegisteredAddIns.Contains(_addIn1), Is.Not.True,
			            "New AddIn object not in AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			var foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1_new.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn found in AddInsWithMarkedForInstallation");
			
			var foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.AddIn.Version, Is.EqualTo(_addIn1.Version), "ManagedAddIn must have new version");
			Assert.That(foundAddIn.OldVersion, Is.EqualTo(_addIn1_new.Version), "ManagedAddIn must know installed (old) version");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.True, "ManagedAddIn is an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.Offline), "ManagedAddIn's installation source is 'offline'");
			
			Assert.That(addInInstalledEventReceived, "AddInInstalled event sent with correct AddIn");
			
			// Cancel the update
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			_addInSetup.CancelUpdate(_addIn1);
			
			// Now check AddInTree, again
			foundAddIns =
				_addInSetup.AddInsWithMarkedForInstallation.Where(ma => ma.AddIn.Manifest.PrimaryIdentity == _addIn1_new.Manifest.PrimaryIdentity);
			
			Assert.That(foundAddIns.Any(), "ManagedAddIn still found in AddInsWithMarkedForInstallation after cancel");
			
			foundAddIn = foundAddIns.First();
			Assert.That(foundAddIn.AddIn.Version, Is.EqualTo(_addIn1_new.Version), "ManagedAddIn now has old version");
			
			Assert.That(addInUninstalledEventReceived, "AddInUninstalled event sent with correct AddIn");
			Assert.That(nuGetPackageUninstalled, Is.False, "Already installed NuGet package must not be removed.");
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
			Assert.That(_addIn1.Action, Is.EqualTo(AddInAction.Uninstall), "AddIn action must be set to 'Uninstall'");
			
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
			Assert.That(_addIn1.Action, Is.EqualTo(AddInAction.Uninstall), "AddIn action must be set to 'Uninstall'");
			
			// Simulate removing unreferenced NuGet packages
			_addInSetup.RemoveUnreferencedNuGetPackages();
			
			Assert.That(nuGetPackageUninstalled, Is.True, "NuGet package must be removed after restart.");
		}
		
		[Test]
		public void RemoveUnreferencedNuGetPackage()
		{
			CreateAddIns();
			
			// Create a fake package
			FakePackage fakePackage1 = new FakePackage()
			{
				Id = _addIn1.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1.Version)
			};
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage1.Id);
			_addIn1.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage1.Version.ToString());
			FakePackage fakePackage1_new = new FakePackage()
			{
				Id = _addIn1_new.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn1_new.Version)
			};
			_addIn1_new.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage1_new.Id);
			_addIn1_new.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage1_new.Version.ToString());
			FakePackage fakePackage2 = new FakePackage()
			{
				Id = _addIn2.Manifest.PrimaryIdentity,
				Version = new SemanticVersion(_addIn2.Version)
			};
			_addIn2.Properties.Set(ManagedAddIn.NuGetPackageIDManifestAttribute, fakePackage2.Id);
			_addIn2.Properties.Set(ManagedAddIn.NuGetPackageVersionManifestAttribute, fakePackage2.Version.ToString());

			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Simulate an installed AddIn, which is not related to installed NuGet package
			_sdAddInManagement.TempInstallDirectory = "";
			_sdAddInManagement.UserInstallDirectory = "";
			
			// Simulate the installed NuGet package in local repository
			FakeCorePackageRepository localRepository = new FakeCorePackageRepository();
			_nuGet.FakeCorePackageManager.LocalRepository = localRepository;
			bool nuGetPackageUninstalled = false;
			IPackage packageForUninstallEvent = null;
			_nuGet.FakeCorePackageManager.UninstallPackageCallback = delegate(IPackage package, bool forceRemove, bool removeDependencies)
			{
				if ((package == packageForUninstallEvent) && forceRemove && !removeDependencies)
				{
					nuGetPackageUninstalled = true;
				}
			};
			
			// Case 1: AddIn of local NuGet package is completely same as installed
			nuGetPackageUninstalled = false;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1 }).AsQueryable();
			packageForUninstallEvent = fakePackage1;
			_sdAddInManagement.RegisteredAddIns.Clear();
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			_addInSetup.RemoveUnreferencedNuGetPackages();
			Assert.That(nuGetPackageUninstalled, Is.False, "fakePackage1 must not be removed, because identical to installed.");
			
			// Case 2: AddIn of local NuGet package not installed at all
			nuGetPackageUninstalled = false;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1 }).AsQueryable();
			packageForUninstallEvent = fakePackage1;
			_sdAddInManagement.RegisteredAddIns.Clear();
			_sdAddInManagement.RegisteredAddIns.Add(_addIn2);
			_addInSetup.RemoveUnreferencedNuGetPackages();
			Assert.That(nuGetPackageUninstalled, Is.True, "fakePackage1 must be removed, because unreferenced.");
			
			// Case 3a: AddIn of local NuGet package is older than the one installed
			nuGetPackageUninstalled = false;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1 }).AsQueryable();
			packageForUninstallEvent = fakePackage1;
			_sdAddInManagement.RegisteredAddIns.Clear();
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1_new);
			_addInSetup.RemoveUnreferencedNuGetPackages();
			Assert.That(nuGetPackageUninstalled, Is.False, "fakePackage1 must not be removed, because older but the only one.");
			
			// Case 3b: There exists a local NuGet package identical to installed AddIn and an older one
			nuGetPackageUninstalled = false;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1, fakePackage1_new }).AsQueryable();
			packageForUninstallEvent = fakePackage1;
			_sdAddInManagement.RegisteredAddIns.Clear();
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1_new);
			_addInSetup.RemoveUnreferencedNuGetPackages();
			Assert.That(nuGetPackageUninstalled, Is.True, "fakePackage1 must be removed, because older and a better fitting package exists.");
			
			// Case 4a: AddIn of local NuGet package is newer than the one installed
			nuGetPackageUninstalled = false;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1_new }).AsQueryable();
			packageForUninstallEvent = fakePackage1_new;
			_sdAddInManagement.RegisteredAddIns.Clear();
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			_addInSetup.RemoveUnreferencedNuGetPackages();
			Assert.That(nuGetPackageUninstalled, Is.False, "fakePackage1_new must not be removed, because newer but the only one.");
			
			// Case 4b: There exists a local NuGet package identical to installed AddIn and a newer one
			nuGetPackageUninstalled = false;
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1, fakePackage1_new }).AsQueryable();
			packageForUninstallEvent = fakePackage1_new;
			_sdAddInManagement.RegisteredAddIns.Clear();
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			_addInSetup.RemoveUnreferencedNuGetPackages();
			Assert.That(nuGetPackageUninstalled, Is.True, "fakePackage1_new must be removed, because newer and a better fitting package exists.");
			
			// Case 5: Installed AddIn has no NuGet version tag in manifest, and there are two versions of local NuGet packages
			nuGetPackageUninstalled = false;
			_addIn1.Properties.Remove(ManagedAddIn.NuGetPackageVersionManifestAttribute);
			localRepository.ReturnedPackages = (new IPackage[] { fakePackage1, fakePackage1_new }).AsQueryable();
			packageForUninstallEvent = fakePackage1;
			_sdAddInManagement.RegisteredAddIns.Clear();
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			_addInSetup.RemoveUnreferencedNuGetPackages();
			Assert.That(nuGetPackageUninstalled, Is.True, "fakePackage1 must be removed, only the latest package is left for AddIn without version info.");
		}
	}
}
