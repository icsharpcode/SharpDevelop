// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageInitializationScripts : IPackageInitializationScripts
	{
		public IPackageScriptSession SessionPassedToRun;
		
		public void Run(IPackageScriptSession session)
		{
			SessionPassedToRun = session;
		}
		
		public bool AnyReturnValue = true;
		
		public bool Any()
		{
			return AnyReturnValue;
		}
	}
}
