// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageInitializationScripts : IPackageInitializationScripts
	{
		public bool IsRunCalled;
		
		public void Run()
		{
			IsRunCalled = true;
		}
		
		public bool AnyReturnValue = true;
		
		public bool Any()
		{
			return AnyReturnValue;
		}
	}
}
