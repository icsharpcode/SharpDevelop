// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializationScriptsRunnerForOpenedSolution
	{
		IPackageInitializationScriptsFactory scriptsFactory;
		
		public PackageInitializationScriptsRunnerForOpenedSolution(IPackageManagementProjectService projectService)
			: this(projectService, new PackageInitializationScriptsFactory())
		{
		}
		
		public PackageInitializationScriptsRunnerForOpenedSolution(
			IPackageManagementProjectService projectService,
			IPackageInitializationScriptsFactory scriptsFactory)
		{
			projectService.SolutionLoaded += SolutionLoaded;
			this.scriptsFactory = scriptsFactory;
		}
		
		void SolutionLoaded(object sender, SolutionEventArgs e)
		{
			RunScripts(e.Solution);
		}
		
		void RunScripts(Solution solution)
		{
			IPackageInitializationScripts scripts = scriptsFactory.CreatePackageInitializationScripts(solution);
			scripts.Run();
		}
	}
}
