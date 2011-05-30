// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializationScriptsFactory : IPackageInitializationScriptsFactory
	{
		public IPackageInitializationScripts CreatePackageInitializationScripts(
			Solution solution)
		{
			var repository = new SolutionPackageRepository(solution);
			var scriptFactory = new PackageScriptFactory();
			return new PackageInitializationScripts(repository, scriptFactory);
		}
	}
}
