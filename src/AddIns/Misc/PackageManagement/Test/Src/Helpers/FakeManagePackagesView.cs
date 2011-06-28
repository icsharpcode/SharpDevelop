// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakeManagePackagesView : IManagePackagesView
	{
		public bool IsShowDialogCalled;
		
		public bool? ShowDialog()
		{
			IsShowDialogCalled = true;
			return true;
		}
		
		public bool IsDisposeCalled;
		
		public void Dispose()
		{
			IsDisposeCalled = true;
		}
	}
}
