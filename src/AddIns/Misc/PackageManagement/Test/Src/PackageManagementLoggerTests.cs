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
	public class PackageManagementLoggerTests
	{
		IPackageManagementEvents fakePackageManagementEvents;
		PackageManagementLogger logger;
		
		void CreateLogger()
		{
			fakePackageManagementEvents = MockRepository.GenerateStub<IPackageManagementEvents>();
			logger = new PackageManagementLogger(fakePackageManagementEvents);
		}
		
		void AssertOnPackageOperationMessageLoggedCalled(MessageLevel level, string message)
		{
			fakePackageManagementEvents.AssertWasCalled(
				events => events.OnPackageOperationMessageLogged(level, message));
		}
		
		void AssertOnPackageOperationMessageLoggedCalled(MessageLevel level, string message, object arg)
		{
			fakePackageManagementEvents.AssertWasCalled(
				events => events.OnPackageOperationMessageLogged(level, message, arg));
		}
		
		void AssertOnResolveFileConflictCalled(string message)
		{
			fakePackageManagementEvents.AssertWasCalled(events => events.OnResolveFileConflict(message));
		}
		
		[Test]
		public void Log_WarningMessageLogged_RaisesMessageLoggedEventWithWarningMessageLevel()
		{
			CreateLogger();
			
			logger.Log(MessageLevel.Warning, "test");
			
			AssertOnPackageOperationMessageLoggedCalled(MessageLevel.Warning, "test");
		}
		
		[Test]
		public void Log_FormattedInfoMessageLogged_RaisesMessageLoggedEventWithFormattedMessage()
		{
			CreateLogger();
			
			string format = "Test {0}";
			logger.Log(MessageLevel.Info, format, "C");
			
			AssertOnPackageOperationMessageLoggedCalled(MessageLevel.Info, format, "C");
		}
		
		[Test]
		public void ResolveFileConflict_MessagePassed_RaisesOnResolveFileConflictEvent()
		{
			CreateLogger();
			
			logger.ResolveFileConflict("message");
			
			AssertOnResolveFileConflictCalled("message");
		}
		
		[Test]
		public void ResolveFileConflict_PackageManagementEventsResolveFileConflictReturnsIgnoreAll_ReturnsIgnoreAll()
		{
			CreateLogger();
			fakePackageManagementEvents
				.Stub(events => events.OnResolveFileConflict("message"))
				.Return(FileConflictResolution.IgnoreAll);
			
			FileConflictResolution resolution = logger.ResolveFileConflict("message");
			
			Assert.AreEqual(FileConflictResolution.IgnoreAll, resolution);
		}
	}
}
