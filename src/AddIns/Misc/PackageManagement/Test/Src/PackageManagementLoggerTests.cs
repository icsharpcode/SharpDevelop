// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageManagementLoggerTests
	{
		FakePackageManagementEvents fakePackageManagementEvents;
		PackageManagementLogger logger;
		
		void CreateLogger()
		{
			fakePackageManagementEvents = new FakePackageManagementEvents();
			logger = new PackageManagementLogger(fakePackageManagementEvents);
		}
		
		[Test]
		public void Log_WarningMessageLogged_RaisesMessageLoggedEventWithWarningMessageLevel()
		{
			CreateLogger();
			
			logger.Log(MessageLevel.Warning, "test");
			
			Assert.AreEqual(MessageLevel.Warning, fakePackageManagementEvents.MessageLevelPassedToOnPackageOperationMessageLogged);
		}
		
		[Test]
		public void Log_FormattedInfoMessageLogged_RaisesMessageLoggedEventWithFormattedMessage()
		{
			CreateLogger();
			
			string format = "Test {0}";
			logger.Log(MessageLevel.Info, format, "C");
			
			string message = fakePackageManagementEvents.FormattedStringPassedToOnPackageOperationMessageLogged;
			
			Assert.AreEqual("Test C", message);
		}
	}
}
