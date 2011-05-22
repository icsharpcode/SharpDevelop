// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			fakeProjectService.FireSolutionClosedEvent();
			
			bool workingDirectoryUpdated = IsWorkingDirectoryUpdated();
			
			Assert.IsTrue(workingDirectoryUpdated);
		}
		
		[Test]
		public void Instance_SolutionClosedWhenConsoleHostIsNotRunning_WorkingDirectoryIsNotUpdated()
		{
			CreateReset();
			fakeConsoleHost.IsRunning = false;
			fakeProjectService.FireSolutionClosedEvent();
			
			bool workingDirectoryUpdated = IsWorkingDirectoryUpdated();
			
			Assert.IsFalse(workingDirectoryUpdated);
		}
	}
}
