// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScript : IPackageScript
	{
		public bool IsExecuted;
		
		public void Execute()
		{
			IsExecuted = true;
		}
		
		public IPackageManagementProject Project { get; set; }
	}
}
