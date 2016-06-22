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
