// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
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
		
		[Test, Description("")]
		public void InstallValidAddInFromManifest()
		{
			CreateAddIns();

			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Install the AddIn from *.sdaddin package
			_sdAddInManagement.AddInToLoad = _addIn1;
			AddIn installedAddIn = _addInSetup.InstallAddIn(@"TestResources\AddInManager2Test.addin");
			
			// The AddIn must have been added to AddInTree
			Assert.That(installedAddIn, Is.Not.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			Assert.That(_sdAddInManagement.AddedExternalAddIns, Contains.Item(_addIn1), "AddIn object added as external");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			ManagedAddIn foundAddIn = null;
			foreach (var managedAddIn in _addInSetup.AddInsWithMarkedForInstallation)
			{
				if (managedAddIn.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					// Found!
					foundAddIn = managedAddIn;
					break;
				}
			}
			
			Assert.That(foundAddIn, Is.Not.Null, "ManagedAddIn found in AddInsWithMarkedForInstallation");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.Offline), "ManagedAddIn's installation source is 'offline'");
		}
		
		[Test, Description("")]
		public void InstallValidAddInFromOfflinePackage()
		{
			CreateAddIns();
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Install the AddIn from manifest
			_sdAddInManagement.AddInToLoad = _addIn1;
			AddIn installedAddIn = _addInSetup.InstallAddIn(@"TestResources\AddInManager2Test.sdaddin");
			
			// The AddIn must have been added to AddInTree
			Assert.That(installedAddIn, Is.Not.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			ManagedAddIn foundAddIn = null;
			foreach (var managedAddIn in _addInSetup.AddInsWithMarkedForInstallation)
			{
				if (managedAddIn.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					// Found!
					foundAddIn = managedAddIn;
					break;
				}
			}
			
			Assert.That(foundAddIn, Is.Not.Null, "ManagedAddIn found in AddInsWithMarkedForInstallation");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.Offline), "ManagedAddIn's installation source is 'offline'");
		}
		
		[Test, Description("")]
		public void UpdateValidAddInFromOfflinePackage()
		{
			CreateAddIns();
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
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
			ManagedAddIn foundAddIn = null;
			foreach (var managedAddIn in _addInSetup.AddInsWithMarkedForInstallation)
			{
				if (managedAddIn.AddIn.Manifest.PrimaryIdentity == _addIn1_new.Manifest.PrimaryIdentity)
				{
					// Found!
					foundAddIn = managedAddIn;
					break;
				}
			}
			
			Assert.That(foundAddIn, Is.Not.Null, "ManagedAddIn found in AddInsWithMarkedForInstallation");
			Assert.That(foundAddIn.AddIn.Version, Is.EqualTo(_addIn1_new.Version), "ManagedAddIn must have new version");
			Assert.That(foundAddIn.OldVersion, Is.EqualTo(_addIn1.Version), "ManagedAddIn must know installed (old) version");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.True, "ManagedAddIn is an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.Offline), "ManagedAddIn's installation source is 'offline'");
		}
		
		[Test, Description("")]
		public void InstallValidAddInFromIncompletelyInstalledNuGetPackage()
		{
			CreateAddIns();
			
			// Create a fake package
			FakePackage package = new FakePackage()
			{
				Id = _addIn1.Name,
				Version = new SemanticVersion(_addIn1.Version)
			};
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Install the AddIn from an extracted NuGet package
			_sdAddInManagement.AddInToLoad = _addIn1;
			AddIn installedAddIn = _addInSetup.InstallAddIn(package, "TestResources");
			
			// The AddIn must have been added to AddInTree
			Assert.That(installedAddIn, Is.Not.Null, "InstallAddIn() returns valid AddIn object");
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			ManagedAddIn foundAddIn = null;
			foreach (var managedAddIn in _addInSetup.AddInsWithMarkedForInstallation)
			{
				if (managedAddIn.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					// Found!
					foundAddIn = managedAddIn;
					break;
				}
			}
			
			Assert.That(foundAddIn, Is.Not.Null, "ManagedAddIn found in AddInsWithMarkedForInstallation");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.NuGetRepository), "ManagedAddIn's installation source is 'NuGet'");
		}
		
		[Test, Description("")]
		public void InstallValidAddInFromDownloadedNuGetPackage()
		{
			CreateAddIns();
			
			// Create a fake package
			FakePackage package = new FakePackage()
			{
				Id = _addIn1.Name,
				Version = new SemanticVersion(_addIn1.Version)
			};
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// Simulate download by NuGet Core PackageManager
			_sdAddInManagement.AddInToLoad = _addIn1;
			_events.OnAddInPackageDownloaded(new PackageOperationEventArgs(package, null, "TestResources"));
			
			// The AddIn must have been added to AddInTree
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "AddIn object added to AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			ManagedAddIn foundAddIn = null;
			foreach (var managedAddIn in _addInSetup.AddInsWithMarkedForInstallation)
			{
				if (managedAddIn.AddIn.Manifest.PrimaryIdentity == _addIn1.Manifest.PrimaryIdentity)
				{
					// Found!
					foundAddIn = managedAddIn;
					break;
				}
			}
			
			Assert.That(foundAddIn, Is.Not.Null, "ManagedAddIn found in AddInsWithMarkedForInstallation");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.False, "ManagedAddIn is not an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.NuGetRepository), "ManagedAddIn's installation source is 'NuGet'");
		}
		
		[Test, Description("")]
		public void UpdateValidAddInFromDownloadedNuGetPackage()
		{
			CreateAddIns();
			
			// Create a fake package
			FakePackage package = new FakePackage()
			{
				Id = _addIn1.Name,
				Version = new SemanticVersion(_addIn1_new.Version)
			};
			
			// Prepare all (fake) services needed for AddInSetup and its instance, itself
			PrepareAddInSetup();
			
			// This AddIn is already installed
			_sdAddInManagement.RegisteredAddIns.Add(_addIn1);
			
			// Simulate download of new version by NuGet Core PackageManager
			_sdAddInManagement.AddInToLoad = _addIn1_new;
			_events.OnAddInPackageDownloaded(new PackageOperationEventArgs(package, null, "TestResources"));
			
			// Test updated AddIn in AddInTree
			Assert.That(_sdAddInManagement.RegisteredAddIns, Contains.Item(_addIn1), "Old AddIn object still in AddInTree");
			Assert.That(_sdAddInManagement.RegisteredAddIns.Contains(_addIn1_new), Is.Not.True,
			            "New AddIn object not in AddInTree");
			
			// Look if we find a ManagedAddIn object for the new AddIn
			ManagedAddIn foundAddIn = null;
			foreach (var managedAddIn in _addInSetup.AddInsWithMarkedForInstallation)
			{
				if (managedAddIn.AddIn.Manifest.PrimaryIdentity == _addIn1_new.Manifest.PrimaryIdentity)
				{
					// Found!
					foundAddIn = managedAddIn;
					break;
				}
			}
			
			Assert.That(foundAddIn, Is.Not.Null, "ManagedAddIn found in AddInsWithMarkedForInstallation");
			Assert.That(foundAddIn.AddIn.Version, Is.EqualTo(_addIn1_new.Version), "ManagedAddIn must have new version");
			Assert.That(foundAddIn.OldVersion, Is.EqualTo(_addIn1.Version), "ManagedAddIn must know installed (old) version");
			Assert.That(foundAddIn.IsTemporary, Is.True, "ManagedAddIn is temporary");
			Assert.That(foundAddIn.IsUpdate, Is.True, "ManagedAddIn is an update");
			Assert.That(foundAddIn.InstallationSource, Is.EqualTo(AddInInstallationSource.NuGetRepository), "ManagedAddIn's installation source is 'NuGet'");
		}
	}
}
