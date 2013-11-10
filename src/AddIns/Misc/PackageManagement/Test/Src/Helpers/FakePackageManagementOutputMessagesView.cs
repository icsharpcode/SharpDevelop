// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Workbench;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementOutputMessagesView : IPackageManagementOutputMessagesView
	{
		public bool IsClearCalled;
		
		public void Clear()
		{
			IsClearCalled = true;
		}
		
		public void AppendLine(string message)
		{
		}
		
		public IOutputCategory OutputCategory { get; set; }
	}
}
