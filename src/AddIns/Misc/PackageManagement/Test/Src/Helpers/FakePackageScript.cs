// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScript : IPackageScript
	{
		public IPackageManagementProject Project { get; set; }
		public IPackage Package { get; set; }
		
		public bool IsRun;		
		public IPackageScriptSession SessionPassedToRun;
		
		public void Run(IPackageScriptSession session)
		{
			IsRun = true;
			SessionPassedToRun = session;
		}
		
		public bool ExistsReturnValue = true;
		
		public bool Exists()
		{
			return ExistsReturnValue;
		}	
	}
}
