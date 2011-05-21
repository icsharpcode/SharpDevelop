// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class ConsolePackageScriptRunnerTests
	{
		ConsolePackageScriptRunner runner;
		PackageScriptsToRun packageScriptsToRun;
		FakePackageManagementConsoleHost fakeConsoleHost;
		FakeScriptingConsole fakeScriptingConsole;
		FakePackageManagementWorkbench fakeWorkbench;
		
		void CreateRunner()
		{
			packageScriptsToRun = new PackageScriptsToRun();
			fakeConsoleHost = new FakePackageManagementConsoleHost();
			fakeScriptingConsole = new FakeScriptingConsole();
			fakeConsoleHost.ScriptingConsole = fakeScriptingConsole;
			fakeWorkbench = new FakePackageManagementWorkbench();
			runner = new ConsolePackageScriptRunner(fakeConsoleHost, packageScriptsToRun, fakeWorkbench);
		}
		
		FakePackageScript RunScriptThatExists()
		{
			var script = new FakePackageScript();
			script.ExistsReturnValue = true;
			return RunScript(script);
		}
		
		FakePackageScript RunScriptThatDoesNotExist()
		{
			var script = new FakePackageScript();
			script.ExistsReturnValue = false;
			return RunScript(script);
		}
		
		FakePackageScript RunScript(FakePackageScript script)
		{
			runner.Run(script);
			return script;
		}
		
		void ConsoleHostIsRunning()
		{
			fakeConsoleHost.IsRunning = true;
		}
		
		void ConsoleHostIsNotRunning()
		{
			fakeConsoleHost.IsRunning = false;
		}
		
		IPackageScript GetNextScriptToBeRun()
		{
			return packageScriptsToRun.GetNextScript();
		}
		
		[Test]
		public void Run_ConsoleHostIsRunningAndScriptExists_ScriptAddedToPackageScriptsToBeRun()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			FakePackageScript expectedScript = RunScriptThatExists();
			
			IPackageScript scriptAdded = GetNextScriptToBeRun();
			
			Assert.AreEqual(expectedScript, scriptAdded);
		}
		
		[Test]
		public void Run_ConsoleHostIsRunningAndScriptDoesNotExist_ScriptIsNotAddedToPackageScriptsToBeRun()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			FakePackageScript script = RunScriptThatDoesNotExist();
			script.ExistsReturnValue = false;
			
			IPackageScript scriptAdded = GetNextScriptToBeRun();
			
			Assert.IsNull(scriptAdded);
		}
		
		[Test]
		public void Run_ConsoleHostIsRunningAndScriptExists_CommandPassedToConsoleHostToRunPackageScripts()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunScriptThatExists();
			
			string command = fakeScriptingConsole.TextPassedToSendLine;
			string expectedCommand = "Invoke-RunPackageScripts";
			
			Assert.AreEqual(expectedCommand, command);
		}
		
		[Test]
		public void Run_ConsoleHostIsRunningAndScriptDoesNotExist_CommandIsNotPassedToConsoleHostToRunPackageScripts()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunScriptThatDoesNotExist();
			
			string command = fakeScriptingConsole.TextPassedToSendLine;
			
			Assert.IsNull(command);
		}
		
		[Test]
		public void Run_ConsoleHostIsNotRunningAndScriptExists_ConsolePadIsCreated()
		{
			CreateRunner();
			ConsoleHostIsNotRunning();
			RunScriptThatExists();
			
			bool created = fakeWorkbench.IsCreateConsolePadCalled;
			
			Assert.IsTrue(created);
		}
		
		[Test]
		public void Run_ConsoleHostIsNotRunningAndScriptDoesNotExist_ConsolePadIsNotCreated()
		{
			CreateRunner();
			ConsoleHostIsNotRunning();
			RunScriptThatDoesNotExist();
			
			bool created = fakeWorkbench.IsCreateConsolePadCalled;
			
			Assert.IsFalse(created);
		}
		
		[Test]
		public void Run_ConsoleHostIsRunningAndScriptExists_ConsolePadIsNotCreated()
		{
			CreateRunner();
			ConsoleHostIsRunning();
			RunScriptThatExists();
			
			bool created = fakeWorkbench.IsCreateConsolePadCalled;
			
			Assert.IsFalse(created);
		}
	}
}
