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
		
		IPackageAction GetNextActionToRun()
		{
			IPackageAction action = null;
			actionsToRun.GetNextAction(out action);
			return action;
		}
		
		List<IPackageAction> GetNextActionsToRun()
		{
			var actions = new List<IPackageAction>();
			IPackageAction action = null;
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
			
			IPackageAction actionAdded = GetNextActionToRun();
			
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
		public void Run_ConsoleHostIsNotRunning_ConsolePadIsBroughtToFront()
		{
			CreateRunner();
			ConsoleHostIsNotRunning();
			RunInstallActionWithOneOperation();
			
			bool shown = fakeWorkbench.IsShowConsolePadCalled;
			
			Assert.IsTrue(shown);
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
		public void Run_ConsoleHostIsRunning_ConsolePadIsBroughtToFront()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunInstallActionWithOneOperation();
			
			bool shown = fakeWorkbench.IsShowConsolePadCalled;
			
			Assert.IsTrue(shown);
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
		public void Run_TwoActionsToRunAndConsoleHostIsNotRunning_ConsolePadIsBroughtToFront()
		{
			CreateRunner();
			ConsoleHostIsNotRunning();
			RunTwoInstallActionsWithOneOperation();
			
			bool shown = fakeWorkbench.IsShowConsolePadCalled;
			
			Assert.IsTrue(shown);
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
		public void Run_TwoActionsToRunAndConsoleHostIsRunning_ConsolePadIsBroughtToFront()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunTwoInstallActionsWithOneOperation();
			
			bool shown = fakeWorkbench.IsShowConsolePadCalled;
			
			Assert.IsTrue(shown);
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
			
			List<IPackageAction> actionsAdded = GetNextActionsToRun();
			
			CollectionAssert.AreEqual(expectedActions, actionsAdded);
		}
	}
}
