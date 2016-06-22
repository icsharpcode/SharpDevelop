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
