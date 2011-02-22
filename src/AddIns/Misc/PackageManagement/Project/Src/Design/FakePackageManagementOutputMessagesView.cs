// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementOutputMessagesView : IPackageManagementOutputMessagesView
	{
		public bool IsClearCalled;
		
		public void Clear()
		{
			IsClearCalled = true;
		}
		
		public void Log(MessageLevel level, string message, params object[] args)
		{
			
		}
	}
}
