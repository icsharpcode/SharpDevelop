// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageManagementOptionsViewModelTests
	{
		PackageManagementOptionsViewModel viewModel;
		FakeRecentPackageRepository fakeRecentRepository;
		FakeMachinePackageCache fakeMachineCache;
		FakeProcess fakeProcess;
		List<string> propertiesChanged;
		
		void CreateRecentRepository()
		{
			fakeRecentRepository = new FakeRecentPackageRepository();			
		}
		
		void CreateMachineCache()
		{
			fakeMachineCache = new FakeMachinePackageCache();
		}
		
		void CreateViewModelUsingCreatedMachineCache()
		{
			CreateRecentRepository();
			fakeProcess = new FakeProcess();
			viewModel = new PackageManagementOptionsViewModel(fakeRecentRepository, fakeMachineCache, fakeProcess);			
		}
		
		void CreateViewModelUsingCreatedRecentRepository()
		{
			CreateMachineCache();
			fakeProcess = new FakeProcess();
			viewModel = new PackageManagementOptionsViewModel(fakeRecentRepository, fakeMachineCache, fakeProcess);			
		}
		
		void AddPackageToRecentRepository()
		{
			fakeRecentRepository.FakePackages.Add(new FakePackage());
			fakeRecentRepository.HasRecentPackages = true;
		}
		
		void AddPackageToMachineCache()
		{
			fakeMachineCache.FakePackages.Add(new FakePackage());
		}
		
		void RecordPropertyChanges()
		{
			propertiesChanged = new List<string>();
			viewModel.PropertyChanged += (sender, e) => propertiesChanged.Add(e.PropertyName);
		}
		
		[Test]
		public void HasNoRecentPackages_RecentPackageRepositoryHasNoPackages_ReturnsTrue()
		{
			CreateRecentRepository();
			CreateViewModelUsingCreatedRecentRepository();
			fakeRecentRepository.HasRecentPackages = false;
			
			bool hasPackages = viewModel.HasNoRecentPackages;
			
			Assert.IsTrue(hasPackages);
		}
		
		[Test]
		public void HasNoRecentPackages_RecentPackageRepositoryHasOnePackage_ReturnsFalse()
		{
			CreateRecentRepository();
			fakeRecentRepository.HasRecentPackages = true;
			AddPackageToRecentRepository();
			CreateViewModelUsingCreatedRecentRepository();
			
			bool hasPackages = viewModel.HasNoRecentPackages;
			
			Assert.IsFalse(hasPackages);
		}
		
		[Test]
		public void HasNoCachedPackages_MachinePackageCacheHasNoPackages_ReturnsTrue()
		{
			CreateMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			bool hasPackages = viewModel.HasNoCachedPackages;
			
			Assert.IsTrue(hasPackages);
		}
		
		[Test]
		public void HasNoCachedPackages_MachinePackageCacheHasOnePackage_ReturnsFalse()
		{
			CreateMachineCache();
			AddPackageToMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			bool hasPackages = viewModel.HasNoCachedPackages;
			
			Assert.IsFalse(hasPackages);
		}
		
		[Test]
		public void ClearRecentPackagesCommandCanExecute_OneRecentPackage_CanExecuteReturnsTrue()
		{
			CreateRecentRepository();
			AddPackageToRecentRepository();
			CreateViewModelUsingCreatedRecentRepository();
			
			bool canExecute = viewModel.ClearRecentPackagesCommand.CanExecute(null);
			
			Assert.IsTrue(canExecute);
		}
		
		[Test]
		public void ClearRecentPackagesCommandCanExecute_NoRecentPackages_CanExecuteReturnsFalse()
		{
			CreateRecentRepository();
			CreateViewModelUsingCreatedRecentRepository();
			
			bool canExecute = viewModel.ClearRecentPackagesCommand.CanExecute(null);
			
			Assert.IsFalse(canExecute);
		}
		
		[Test]
		public void ClearCachedPackagesCommandCanExecute_OneCachedPackage_CanExecuteReturnsTrue()
		{
			CreateMachineCache();
			AddPackageToMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			bool canExecute = viewModel.ClearCachedPackagesCommand.CanExecute(null);
			
			Assert.IsTrue(canExecute);
		}
		
		[Test]
		public void ClearCachedPackagesCommandCanExecute_NoCachedPackages_CanExecuteReturnsFalse()
		{
			CreateMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			bool canExecute = viewModel.ClearCachedPackagesCommand.CanExecute(null);
			
			Assert.IsFalse(canExecute);
		}
		
		[Test]
		public void ClearCachedPackagesCommandExecute_OneCachedPackage_ClearsPackagesFromCache()
		{
			CreateMachineCache();
			AddPackageToMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			viewModel.ClearCachedPackagesCommand.Execute(null);
			
			Assert.IsTrue(fakeMachineCache.IsClearCalled);
		}
		
		[Test]
		public void ClearRecentPackagesCommandExecute_OneRecentPackage_ClearsPackages()
		{
			CreateMachineCache();
			AddPackageToRecentRepository();
			CreateViewModelUsingCreatedRecentRepository();
			
			viewModel.ClearRecentPackagesCommand.Execute(null);
			
			Assert.IsTrue(fakeRecentRepository.IsClearCalled);
		}
		
		[Test]
		public void ClearRecentPackages_OneRecentPackage_HasNoRecentPackagesIsTrue()
		{
			CreateRecentRepository();
			AddPackageToRecentRepository();
			CreateViewModelUsingCreatedRecentRepository();
			
			RecordPropertyChanges();
			viewModel.ClearRecentPackages();
			fakeRecentRepository.HasRecentPackages = false;
			
			bool hasPackages = viewModel.HasNoRecentPackages;
			
			Assert.IsTrue(hasPackages);
		}
		
		[Test]
		public void ClearCachedPackages_OneCachedPackage_HasNoCachedPackagesReturnsTrue()
		{
			CreateMachineCache();
			AddPackageToMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			RecordPropertyChanges();
			viewModel.ClearCachedPackages();
			
			bool hasPackages = viewModel.HasNoCachedPackages;
			
			Assert.IsTrue(hasPackages);
		}
		
		[Test]
		public void ClearRecentPackages_OneRecentPackage_HasNoRecentPackagesPropertyChangedEventFired()
		{
			CreateRecentRepository();
			AddPackageToRecentRepository();
			CreateViewModelUsingCreatedMachineCache();
			
			RecordPropertyChanges();
			viewModel.ClearRecentPackages();
			
			bool fired = propertiesChanged.Contains("HasNoRecentPackages");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void ClearCachedPackages_OneCachedPackage_HasNoCachedPackagesPropertyChangedEventFired()
		{
			CreateMachineCache();
			AddPackageToMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			RecordPropertyChanges();
			viewModel.ClearCachedPackages();
			
			bool fired = propertiesChanged.Contains("HasNoCachedPackages");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void BrowseCachedPackagesCommandCanExecute_OneCachedPackage_ReturnsTrue()
		{
			CreateMachineCache();
			AddPackageToMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			bool canExecute = viewModel.BrowseCachedPackagesCommand.CanExecute(null);
			
			Assert.IsTrue(canExecute);
		}
		
		[Test]
		public void BrowseCachedPackagesCommandCanExecute_NoCachedPackages_ReturnsFalse()
		{
			CreateMachineCache();
 			CreateViewModelUsingCreatedMachineCache();
			
			bool canExecute = viewModel.BrowseCachedPackagesCommand.CanExecute(null);
			
			Assert.IsFalse(canExecute);
		}
		
		[Test]
		public void BrowseCachedPackagesCommandExecute_OneCachedPackage_StartsProcessToOpenMachineCacheFolder()
		{
			CreateMachineCache();
			CreateViewModelUsingCreatedMachineCache();
			
			string expectedFileName = @"d:\projects\nugetpackages";
			fakeMachineCache.Source = expectedFileName;
			
			viewModel.BrowseCachedPackagesCommand.Execute(null);
			
			string fileName = fakeProcess.FileNamePassedToStart;
			
			Assert.AreEqual(expectedFileName, fileName);
		}
	}
}
