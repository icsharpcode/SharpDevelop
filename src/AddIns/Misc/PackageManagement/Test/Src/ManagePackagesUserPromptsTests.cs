// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ManagePackagesUserPromptsTests
	{
		ManagePackagesUserPrompts userPrompts;
		IPackageManagementEvents fakeEvents;
		ILicenseAcceptanceService fakeLicenseService;
		ISelectProjectsService fakeSelectProjectsService;
		IFileConflictResolver fakeFileConflictResolver;
		
		void CreateUserPrompts()
		{
			fakeEvents = MockRepository.GenerateStub<IPackageManagementEvents>();
			fakeLicenseService = MockRepository.GenerateStub<ILicenseAcceptanceService>();
			fakeSelectProjectsService = MockRepository.GenerateStub<ISelectProjectsService>();
			fakeFileConflictResolver = MockRepository.GenerateStub<IFileConflictResolver>();
			
			userPrompts = new ManagePackagesUserPrompts(
				fakeEvents,
				fakeLicenseService,
				fakeSelectProjectsService,
				fakeFileConflictResolver);
		}
		
		FileConflictResolution RaiseResolveFileConflict(string message)
		{
			var eventArgs = new ResolveFileConflictEventArgs(message);
			fakeEvents.Raise(events => events.ResolveFileConflict += null, fakeEvents, eventArgs);
			return eventArgs.Resolution;
		}
		
		void RaisePackageOperationsStarting()
		{
			fakeEvents.Raise(events => events.PackageOperationsStarting += null, fakeEvents, new EventArgs());
		}
		
		void ResolverReturns(FileConflictResolution resolution)
		{
			fakeFileConflictResolver
				.Stub(r => r.ResolveFileConflict(Arg<string>.Is.Anything))
				.Return(resolution);
		}
		
		void ResetFakeResolver()
		{
			fakeFileConflictResolver.BackToRecord(BackToRecordOptions.All);
			fakeFileConflictResolver.Replay();
		}
		
		void AssertFileConflictResolverWasNotCalled()
		{
			fakeFileConflictResolver.AssertWasNotCalled(resolver => resolver.ResolveFileConflict(Arg<string>.Is.Anything));
		}
		
		[Test]
		public void OnResolveFileConflict_MessagePassed_MessagePassedToFileConflictService()
		{
			CreateUserPrompts();
			RaisePackageOperationsStarting();
			
			RaiseResolveFileConflict("message");
			
			fakeFileConflictResolver.AssertWasCalled(resolver => resolver.ResolveFileConflict("message"));
		}
		
		[Test]
		public void Dispose_ResolveFileConflictEventRaised_FileConflictServiceNotCalled()
		{
			CreateUserPrompts();
			
			userPrompts.Dispose();
			RaiseResolveFileConflict("message");
			
			AssertFileConflictResolverWasNotCalled();
		}
		
		[Test]
		public void OnResolveFileConflict_FileConflictResolverReturnsOverwrite_OverwriteSetInEventArgs()
		{
			CreateUserPrompts();
			RaisePackageOperationsStarting();
			ResolverReturns(FileConflictResolution.Overwrite);
			
			FileConflictResolution resolution = RaiseResolveFileConflict("message");
			
			Assert.AreEqual(FileConflictResolution.Overwrite, resolution);
		}
		
		[Test]
		public void OnResolveFileConflict_ResolverReturnsIgnoreAllAndThenResolveFileConflictFiredAgain_IgnoreAllReturnedWithoutCallingFileConflictResolverAgain()
		{
			CreateUserPrompts();
			RaisePackageOperationsStarting();
			ResolverReturns(FileConflictResolution.IgnoreAll);
			RaiseResolveFileConflict("message");
			ResetFakeResolver();
			ResolverReturns(FileConflictResolution.Overwrite);
			
			FileConflictResolution resolution = RaiseResolveFileConflict("message");
			
			Assert.AreEqual(FileConflictResolution.IgnoreAll, resolution);
		}
		
		[Test]
		public void OnResolveFileConflict_ResolverReturnsOverwriteAllAndThenResolveFileConflictFiredAgain_OverwriteAllReturnedWithoutCallingFileConflictResolverAgain()
		{
			CreateUserPrompts();
			RaisePackageOperationsStarting();
			ResolverReturns(FileConflictResolution.OverwriteAll);
			RaiseResolveFileConflict("message");
			ResetFakeResolver();
			ResolverReturns(FileConflictResolution.Ignore);
			
			FileConflictResolution resolution = RaiseResolveFileConflict("message");
			
			Assert.AreEqual(FileConflictResolution.OverwriteAll, resolution);
		}
		
		[Test]
		public void OnResolveFileConflict_ResolverReturnsOverwriteAllAndNewPackageInstalledAndThenResolveFileConflictFiredAgain_FileConflictResolverUsedAgainForNewPackage()
		{
			CreateUserPrompts();
			RaisePackageOperationsStarting();
			ResolverReturns(FileConflictResolution.OverwriteAll);
			RaiseResolveFileConflict("message");
			ResetFakeResolver();
			RaisePackageOperationsStarting();
			ResolverReturns(FileConflictResolution.Ignore);
			
			FileConflictResolution resolution = RaiseResolveFileConflict("message");
			
			Assert.AreEqual(FileConflictResolution.Ignore, resolution);
		}
	}
}
