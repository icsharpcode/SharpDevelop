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
	public class PackageInitializationScriptsConsoleTests
	{
		TestablePackageInitializationScriptsConsole console;
		FakePackageManagementConsoleHost fakeConsoleHost;
		FakeScriptingConsole fakeScriptingConsole;
		
		void CreateConsole()
		{
			console = new TestablePackageInitializationScriptsConsole();
			fakeConsoleHost = console.FakeConsoleHost;
			fakeScriptingConsole = console.FakeScriptingConsole;
		}
		
		[Test]
		public void ExecuteCommand_ConsoleHostAlreadyRunning_CommandSentToScriptingConsole()
		{
			CreateConsole();
			fakeConsoleHost.IsRunning = true;
			console.ExecuteCommand("Test");
			
			string command = fakeScriptingConsole.TextPassedToSendLine;
			
			Assert.AreEqual("Test", command);
		}
		
		[Test]
		public void ExecuteCommand_ConsoleHostAlreadyRunning_ConsolePadIsNotCreated()
		{
			CreateConsole();
			fakeConsoleHost.IsRunning = true;
			console.ExecuteCommand("Test");
			
			bool created = console.IsCreateConsolePadCalled;
			
			Assert.IsFalse(created);
		}
		
		[Test]
		public void ExecuteCommand_ConsoleHostNotRunning_ConsolePadIsCreated()
		{
			CreateConsole();
			fakeConsoleHost.IsRunning = false;
			console.ExecuteCommand("Test");
			
			bool created = console.IsCreateConsolePadCalled;
			
			Assert.IsTrue(created);
		}
	}
}
