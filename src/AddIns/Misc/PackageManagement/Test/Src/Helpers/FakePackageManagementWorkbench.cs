// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementWorkbench : IPackageManagementWorkbench
	{
		public bool IsCreateConsolePadCalled;
		
		public void CreateConsolePad()
		{
			IsCreateConsolePadCalled = true;
		}
	}
}
