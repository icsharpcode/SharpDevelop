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
