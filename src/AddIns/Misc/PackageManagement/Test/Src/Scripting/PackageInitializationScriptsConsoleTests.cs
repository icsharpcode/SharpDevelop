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
		PackageInitializationScriptsConsole console;
		FakePackageManagementConsoleHost fakeConsoleHost;
		FakeScriptingConsole fakeScriptingConsole;
		
		void CreateConsole()
		{
			fakeConsoleHost = new FakePackageManagementConsoleHost();
			fakeScriptingConsole = new FakeScriptingConsole();
			fakeConsoleHost.ScriptingConsole = fakeScriptingConsole;
			console = new PackageInitializationScriptsConsole(fakeConsoleHost);
		}
		
		[Test]
		public void ExecuteCommand_ConsoleHostAlreadyRunning_CommandIsExecuted()
		{
			CreateConsole();
			fakeConsoleHost.IsRunning = true;
			console.ExecuteCommand("Test");
			
			string command = fakeConsoleHost.FirstCommandExecuted;
			
			Assert.AreEqual("Test", command);
		}
		
		[Test]
		public void ExecuteCommand_ConsoleHostNotRunning_CommandNotSentToScriptingConsole()
		{
			CreateConsole();
			fakeConsoleHost.IsRunning = false;
			console.ExecuteCommand("Test");
			
			string command = fakeScriptingConsole.TextPassedToSendLine;
			
			Assert.IsNull(command);
		}
	}
}
