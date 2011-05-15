// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializationScriptsFactory : IPackageInitializationScriptsFactory
	{
		IPackageManagementConsoleHost consoleHost;
		
		public PackageInitializationScriptsFactory()
			: this(PackageManagementServices.ConsoleHost)
		{
		}
		
		public PackageInitializationScriptsFactory(IPackageManagementConsoleHost consoleHost)
		{
			this.consoleHost = consoleHost;
		}
		
		public IPackageInitializationScripts CreatePackageInitializationScripts(
			Solution solution,
			IPackageScriptSession scriptSession)
		{
			var repository = new SolutionPackageRepository(solution);
			var scriptFactory = new PackageScriptFactory(scriptSession);
			return new PackageInitializationScripts(repository, scriptFactory);
		}
	}
}
