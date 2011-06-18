// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
		
		FakeInstallPackageAction RunInstallActionWithOneOperation()
		{
			FakeInstallPackageAction action = CreateInstallActionWithOneOperation();
			runner.Run(action);
			return action;
		}
		
		FakeInstallPackageAction CreateInstallActionWithOneOperation()
		{
			var operations = new PackageOperation[] {
				new PackageOperation(new FakePackage(), PackageAction.Install)
			};
			FakeInstallPackageAction action = CreateInstallAction();
			action.Operations = operations;
			return action;
		}
		
		List<FakeInstallPackageAction> RunTwoInstallActionsWithOneOperation()
		{
			var actions = new List<FakeInstallPackageAction>();
			actions.Add(CreateInstallActionWithOneOperation());
			actions.Add(CreateInstallActionWithOneOperation());
			
			runner.Run(actions);
			
			return actions;
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
		
		List<ProcessPackageAction> GetNextActionsToRun()
		{
			var actions = new List<ProcessPackageAction>();
			ProcessPackageAction action = null;
			while (actionsToRun.GetNextAction(out action)) {
				actions.Add(action);
			}
			return actions;
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
		
		[Test]
		public void Run_TwoActionsToRunAndConsoleHostIsNotRunning_ConsolePadIsCreated()
		{
			CreateRunner();
			ConsoleHostIsNotRunning();
			RunTwoInstallActionsWithOneOperation();
			
			bool created = fakeWorkbench.IsCreateConsolePadCalled;
			
			Assert.IsTrue(created);
		}
		
		[Test]
		public void Run_TwoActionsToRunAndConsoleHostIsRunning_ConsolePadIsNotCreated()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunTwoInstallActionsWithOneOperation();
			
			bool created = fakeWorkbench.IsCreateConsolePadCalled;
			
			Assert.IsFalse(created);
		}
		
		[Test]
		public void Run_TwoActionsToRunAndConsoleHostIsRunning_CommandPassedToConsoleHostToProcessPackageActions()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunTwoInstallActionsWithOneOperation();
			
			string command = fakeConsoleHost.FirstCommandExecuted;
			string expectedCommand = "Invoke-ProcessPackageActions";
			
			Assert.AreEqual(expectedCommand, command);
		}
		
		[Test]
		public void Run_TwoActionsToRunAndConsoleHostIsRunning_ActionsAddedToPackageActionsToBeRun()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			List<FakeInstallPackageAction> expectedActions = RunTwoInstallActionsWithOneOperation();
			
			List<ProcessPackageAction> actionsAdded = GetNextActionsToRun();
			
			CollectionAssert.AreEqual(expectedActions, actionsAdded);
		}
	}
}
