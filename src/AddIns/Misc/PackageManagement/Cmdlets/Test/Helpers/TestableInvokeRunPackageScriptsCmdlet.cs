// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Scripting;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableInvokeRunPackageScriptsCmdlet : InvokeRunPackageScriptsCmdlet
	{
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public PackageScriptsToRun ScriptsToBeRun;
		
		public TestableInvokeRunPackageScriptsCmdlet()
			: this(
				new PackageScriptsToRun(),
				new FakePackageManagementConsoleHost(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableInvokeRunPackageScriptsCmdlet(
			PackageScriptsToRun scriptsToBeRun,
			FakePackageManagementConsoleHost consoleHost,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(scriptsToBeRun, consoleHost, cmdletTerminatingError)
		{
			this.ScriptsToBeRun = scriptsToBeRun;
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCmdletTerminatingError = cmdletTerminatingError;
		}
		
		public void CallProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}
