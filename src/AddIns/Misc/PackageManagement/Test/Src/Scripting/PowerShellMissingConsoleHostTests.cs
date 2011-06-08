// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PowerShellMissingConsoleHostTests
	{
		TestablePowerShellMissingConsoleHost consoleHost;
		FakeScriptingConsole fakeScriptingConsole;
		
		void CreateConsoleHost()
		{
			consoleHost = new TestablePowerShellMissingConsoleHost();
			fakeScriptingConsole = consoleHost.FakeScriptingConsole;
		}
		
		[Test]
		public void Run_ConsoleHostIsRun_PowerShellNotInstalledMessageIsDisplayed()
		{
			CreateConsoleHost();
			string expectedMessage = "PowerShell is not installed.";
			consoleHost.GetPowerShellIsNotInstalledMessageReturnValue = expectedMessage;
			consoleHost.Run();
			
			string message = fakeScriptingConsole.TextPassedToWriteLine;
			
			Assert.AreEqual(expectedMessage, message);
		}
		
		[Test]
		public void Run_ConsoleHostIsRun_PowerShellNotInstalledMessageTextHasErrorScriptingStyle()
		{
			CreateConsoleHost();
			consoleHost.Run();
			
			ScriptingStyle style = fakeScriptingConsole.ScriptingStylePassedToWriteLine;
			ScriptingStyle expectedStyle = ScriptingStyle.Error;
			
			Assert.AreEqual(expectedStyle, style);
		}
		
		[Test]
		public void IsRunning_ConsoleHostHasNotBeenRun_ReturnsFalse()
		{
			CreateConsoleHost();
			bool running = consoleHost.IsRunning;
			
			Assert.IsFalse(running);
		}
		
		[Test]
		public void IsRunning_ConsoleHostHasBeenRun_ReturnsTrue()
		{
			CreateConsoleHost();
			consoleHost.Run();
			bool running = consoleHost.IsRunning;
			
			Assert.IsTrue(running);
		}
	}
}
