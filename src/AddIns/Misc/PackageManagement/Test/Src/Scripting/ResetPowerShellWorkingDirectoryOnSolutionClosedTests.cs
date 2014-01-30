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
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class ResetPowerShellWorkingDirectoryOnSolutionClosedTests
	{
		FakePackageManagementProjectService fakeProjectService;
		FakePackageManagementConsoleHost fakeConsoleHost;
		FakeScriptingConsole fakeScriptingConsole;
		ResetPowerShellWorkingDirectoryOnSolutionClosed reset;
		
		void CreateReset()
		{
			fakeProjectService = new FakePackageManagementProjectService();
			fakeConsoleHost = new FakePackageManagementConsoleHost();
			fakeScriptingConsole = new FakeScriptingConsole();
			fakeConsoleHost.ScriptingConsole = fakeScriptingConsole;
			reset = new ResetPowerShellWorkingDirectoryOnSolutionClosed(fakeProjectService, fakeConsoleHost);
		}
		
		bool IsWorkingDirectoryUpdated()
		{
			return fakeConsoleHost.CommandsExecuted.Contains("Invoke-UpdateWorkingDirectory");
		}
		
		[Test]
		public void Instance_SolutionClosedWhenConsoleHostIsRunning_WorkingDirectoryIsUpdated()
		{
			CreateReset();
			fakeConsoleHost.IsRunning = true;
			fakeProjectService.FireSolutionClosedEvent(fakeProjectService.OpenSolution);
			
			bool workingDirectoryUpdated = IsWorkingDirectoryUpdated();
			
			Assert.IsTrue(workingDirectoryUpdated);
		}
		
		[Test]
		public void Instance_SolutionClosedWhenConsoleHostIsNotRunning_WorkingDirectoryIsNotUpdated()
		{
			CreateReset();
			fakeConsoleHost.IsRunning = false;
			fakeProjectService.FireSolutionClosedEvent(fakeProjectService.OpenSolution);
			
			bool workingDirectoryUpdated = IsWorkingDirectoryUpdated();
			
			Assert.IsFalse(workingDirectoryUpdated);
		}
	}
}
