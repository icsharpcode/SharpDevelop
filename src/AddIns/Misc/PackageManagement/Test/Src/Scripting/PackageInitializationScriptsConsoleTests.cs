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
