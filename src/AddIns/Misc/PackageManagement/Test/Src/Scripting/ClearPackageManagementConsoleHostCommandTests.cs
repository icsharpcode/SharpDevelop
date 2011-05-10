// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class ClearPackageManagementConsoleHostCommandTests
	{
		ClearPackageManagementConsoleHostCommand command;
		FakePackageManagementConsoleHost fakeConsoleHost;
		
		void CreateCommand()
		{
			fakeConsoleHost = new FakePackageManagementConsoleHost();
			command = new ClearPackageManagementConsoleHostCommand(fakeConsoleHost);
		}
		
		[Test]
		public void ClearHost_MethodCalled_ClearsConsoleHost()
		{
			CreateCommand();
			command.ClearHost();
			
			bool called = fakeConsoleHost.IsClearCalled;
			Assert.IsTrue(called);
		}
	}
}
