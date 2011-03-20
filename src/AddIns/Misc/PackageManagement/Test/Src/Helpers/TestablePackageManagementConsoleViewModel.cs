// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageManagementConsoleViewModel : PackageManagementConsoleViewModel
	{
		public PackageManagementConsole FakeConsole = 
			new PackageManagementConsole(new FakeScriptingConsole(), new FakeControlDispatcher());
		
		public TestablePackageManagementConsoleViewModel(
			IPackageManagementService packageManagementService,
			IPackageManagementConsoleHost consoleHost)
			: base(packageManagementService, consoleHost)
		{
		}
		
		protected override PackageManagementConsole CreateConsole()
		{
			return FakeConsole;
		}
	}
}
