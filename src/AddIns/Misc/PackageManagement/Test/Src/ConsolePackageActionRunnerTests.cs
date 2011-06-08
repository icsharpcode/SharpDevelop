// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ConsolePackageActionRunnerTests
	{
		ConsolePackageActionRunner runner;
		PackageActionsToRun actionsToRun;
		FakePackageManagementConsoleHost fakeConsoleHost;
		FakeScriptingConsole fakeScriptingConsole;
		FakePackageManagementWorkbench fakeWorkbench;
		
		void CreateRunner()
		{
			actionsToRun = new PackageActionsToRun();
			fakeConsoleHost = new FakePackageManagementConsoleHost();
			fakeScriptingConsole = new FakeScriptingConsole();
			fakeConsoleHost.ScriptingConsole = fakeScriptingConsole;
			fakeWorkbench = new FakePackageManagementWorkbench();
			runner = new ConsolePackageActionRunner(fakeConsoleHost, actionsToRun, fakeWorkbench);
		}
		
		FakeInstallPackageAction CreateInstallAction()
		{
			var project = new FakePackageManagementProject();
			var action = new FakeInstallPackageAction(project);
			action.Operations = new PackageOperation[0];
			return action;
		}
		
		FakeInstallPackageAction RunInstallActionWithNoOperations()
		{
			FakeInstallPackageAction action = CreateInstallAction();
			runner.Run(action);
			return action;
		}
		
		FakeInstallPackageAction RunInstallActionWithOneOperation()
		{
			var operations = new PackageOperation[] {
				new PackageOperation(new FakePackage(), PackageAction.Install)
			};
			FakeInstallPackageAction action = CreateInstallAction();			
			action.Operations = operations;
			runner.Run(action);
			return action;
		}
		
		void ConsoleHostIsRunning()
		{
			fakeConsoleHost.IsRunning = true;
		}
		
		void ConsoleHostIsNotRunning()
		{
			fakeConsoleHost.IsRunning = false;
		}
		
		ProcessPackageAction GetNextActionToRun()
		{
			ProcessPackageAction action = null;
			actionsToRun.GetNextAction(out action);
			return action;
		}
		
		[Test]
		public void Run_ConsoleHostIsRunning_ActionAddedToPackageActionsToBeRun()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			FakeInstallPackageAction expectedAction = RunInstallActionWithOneOperation();
			
			ProcessPackageAction actionAdded = GetNextActionToRun();
			
			Assert.AreEqual(expectedAction, actionAdded);
		}
		
		[Test]
		public void Run_ConsoleHostIsRunning_CommandPassedToConsoleHostToProcessPackageActions()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunInstallActionWithOneOperation();
			
			string command = fakeConsoleHost.FirstCommandExecuted;
			string expectedCommand = "Invoke-ProcessPackageActions";
			
			Assert.AreEqual(expectedCommand, command);
		}
		
		[Test]
		public void Run_ConsoleHostIsNotRunning_ConsolePadIsCreated()
		{
			CreateRunner();
			ConsoleHostIsNotRunning();
			RunInstallActionWithOneOperation();
			
			bool created = fakeWorkbench.IsCreateConsolePadCalled;
			
			Assert.IsTrue(created);
		}
		
		[Test]
		public void Run_ConsoleHostIsRunning_ConsolePadIsNotCreated()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunInstallActionWithOneOperation();
			
			bool created = fakeWorkbench.IsCreateConsolePadCalled;
			
			Assert.IsFalse(created);
		}
	}
}
