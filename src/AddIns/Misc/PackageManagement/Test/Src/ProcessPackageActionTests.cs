// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ProcessPackageActionTests
	{
		TestableProcessPackageAction action;
		FakePackageManagementService fakePackageManagementService;
		FakePackageManager fakePackageManager;
		FakeLogger fakeLogger;
		
		void CreateAction()
		{
			action = new TestableProcessPackageAction();
			fakePackageManagementService = action.FakePackageManagementService;
			fakeLogger = new FakeLogger();
			fakePackageManager = fakePackageManagementService.FakePackageManagerToReturnFromCreatePackageManager;
		}
		
		ILogger AddLoggerToAction()
		{
			var logger = new NullLogger();
			action.Logger = logger;
			return logger;
		}
		
		[Test]
		public void Execute_LoggerIsNull_LoggerUsedByPackageManagerIsPackageManagementLogger()
		{
			CreateAction();
			action.Execute();
			
			ILogger actualLogger = fakePackageManager.Logger;
			Type expectedType = typeof(PackageManagementLogger);
			
			Assert.IsInstanceOf(expectedType, actualLogger);
		}
		
		[Test]
		public void Execute_LoggerIsDefined_LoggerDefinedIsUsedByPackageManager()
		{
			CreateAction();
			ILogger expectedLogger = AddLoggerToAction();
			action.Execute();
			
			ILogger actualLogger = fakePackageManager.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
			
		[Test]
		public void BeforeExecute_LoggerIsDefined_LoggerUsedByPackageManagerIsConfiguredBeforeInstallPackageCalled()
		{
			CreateAction();
			ILogger expectedLogger = AddLoggerToAction();
			action.CallBeforeExecute();
			
			ILogger actualLogger = fakePackageManager.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void Execute_LoggerIsDefined_ProjectManagerUsesLogger()
		{
			CreateAction();
			ILogger expectedLogger = AddLoggerToAction();
			action.Execute();
			
			ILogger actualLogger = fakePackageManager.ProjectManager.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void Execute_LoggerIsDefined_ProjectManagerProjectSystemUsesLogger()
		{
			CreateAction();
			ILogger expectedLogger = AddLoggerToAction();
			action.Execute();
			
			ILogger actualLogger = fakePackageManager.ProjectManager.Project.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void Execute_LoggerIsDefined_PackageManagerFileSystemUsesLogger()
		{
			CreateAction();
			ILogger expectedLogger = AddLoggerToAction();
			action.Execute();
			
			ILogger actualLogger = fakePackageManager.FileSystem.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
	}
}
