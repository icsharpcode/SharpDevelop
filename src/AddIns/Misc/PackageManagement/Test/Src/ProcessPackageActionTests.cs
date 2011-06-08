// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ProcessPackageActionTests
	{
		TestableProcessPackageAction action;
		FakePackageManagementProject fakeProject;
		
		void CreateAction()
		{
			action = new TestableProcessPackageAction();
			fakeProject = action.FakeProject;
		}
		
		ILogger AddLoggerToAction()
		{
			var logger = new NullLogger();
			action.Logger = logger;
			return logger;
		}
		
		[Test]
		public void Execute_LoggerIsNull_LoggerUsedByProjectIsPackageManagementLogger()
		{
			CreateAction();
			action.Execute();
			
			ILogger actualLogger = fakeProject.Logger;
			Type expectedType = typeof(PackageManagementLogger);
			
			Assert.IsInstanceOf(expectedType, actualLogger);
		}
		
		[Test]
		public void Execute_LoggerIsDefined_LoggerDefinedIsUsedByProjectManager()
		{
			CreateAction();
			ILogger expectedLogger = AddLoggerToAction();
			action.Execute();
			
			ILogger actualLogger = fakeProject.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
			
		[Test]
		public void BeforeExecute_LoggerIsDefined_LoggerUsedByProjectIsConfiguredBeforeInstallPackageCalled()
		{
			CreateAction();
			ILogger expectedLogger = AddLoggerToAction();
			action.CallBeforeExecute();
			
			ILogger actualLogger = fakeProject.Logger;
			
			Assert.AreEqual(expectedLogger, actualLogger);
		}
		
		[Test]
		public void Execute_MethodCalled_RunPackageScriptsActionCreatedUsingPackageScriptRunner()
		{
			CreateAction();
			var expectedRunner = new FakePackageScriptRunner();
			action.PackageScriptRunner = expectedRunner;
			action.Execute();
			
			IPackageScriptRunner actualRunner = action.ScriptRunnerPassedToCreateRunPackageScriptsAction;
			
			Assert.AreEqual(expectedRunner, actualRunner);
		}
		
		[Test]
		public void Execute_MethodCalled_RunPackageScriptsActionCreatedUsingProject()
		{
			CreateAction();
			var expectedProject = new FakePackageManagementProject();
			action.Project = expectedProject;
			action.PackageScriptRunner = new FakePackageScriptRunner();
			action.Execute();
			
			var actualProject = action.ProjectPassedToCreateRunPackageScriptsAction;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void Execute_MethodCalled_RunPackageScriptsActionIsDisposed()
		{
			CreateAction();
			action.PackageScriptRunner = new FakePackageScriptRunner();
			action.Execute();
			
			Assert.IsTrue(action.IsRunPackageScriptsActionDisposed);
		}
		
		[Test]
		public void Execute_NullSession_RunPackageScriptsActionIsNotCreated()
		{
			CreateAction();
			action.Execute();
			
			Assert.IsFalse(action.IsRunPackageScriptsActionCreated);
		}
	}
}
