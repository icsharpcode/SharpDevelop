// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageInitializationScriptsFactory : IPackageInitializationScriptsFactory
	{
		public FakePackageInitializationScripts FakePackageInitializationScripts = 
			new FakePackageInitializationScripts();
		
		public Solution SolutionPassedToCreatePackageInitializationScripts;
		
		public IPackageInitializationScripts CreatePackageInitializationScripts(
			Solution solution)
		{
			SolutionPassedToCreatePackageInitializationScripts = solution;
			return FakePackageInitializationScripts;
		}
	}
}
