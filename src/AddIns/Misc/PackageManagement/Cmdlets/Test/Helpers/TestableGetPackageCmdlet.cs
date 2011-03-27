// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Management.Automation;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableGetPackageCmdlet : GetPackageCmdlet
	{
		public FakePackageManagementService FakePackageManagementService;
		public FakeCommandRuntime FakeCommandRuntime;
		public List<IPackage> PackagesPassedToWriteObject = new List<IPackage>();
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakeErrorRecordFactory FakeErrorRecordFactory;
		
		public TestableGetPackageCmdlet()
			: this(
				new FakePackageManagementService(),
				new FakePackageManagementConsoleHost(),
				new FakeCommandRuntime(),
				new FakeErrorRecordFactory())
		{
		}
		
		public TestableGetPackageCmdlet(
			FakePackageManagementService packageManagementService,
			FakePackageManagementConsoleHost consoleHost,
			FakeCommandRuntime commandRuntime,
			FakeErrorRecordFactory errorRecordFactory)
			: base(packageManagementService, consoleHost, errorRecordFactory)
		{
			this.FakePackageManagementService = packageManagementService;
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCommandRuntime = commandRuntime;
			this.CommandRuntime = commandRuntime;
			this.FakeErrorRecordFactory = errorRecordFactory;
		}
		
		public void RunProcessRecord()
		{
			base.ProcessRecord();
		}
		
		public bool IsThrowTerminatingErrorCalled { get; set; }
		public ErrorRecord ErrorRecordPassedToThrowTerminatingError;
		
		protected override void CmdletThrowTerminatingError(ErrorRecord errorRecord)
		{
			IsThrowTerminatingErrorCalled = true;
			ErrorRecordPassedToThrowTerminatingError = errorRecord;
		}
	}
}
