// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class RunPackageInitializationScriptsOnSolutionOpen
	{
		IPackageInitializationScriptsFactory scriptsFactory;
		PackageInitializationScriptsConsole scriptsConsole;
		
		public RunPackageInitializationScriptsOnSolutionOpen(
			IPackageManagementProjectService projectService)
			: this(
				projectService,
				new PackageInitializationScriptsConsole(PackageManagementServices.ConsoleHost),
				new PackageInitializationScriptsFactory())
		{
		}
		
		public RunPackageInitializationScriptsOnSolutionOpen(
			IPackageManagementProjectService projectService,
			PackageInitializationScriptsConsole scriptsConsole,
			IPackageInitializationScriptsFactory scriptsFactory)
		{
			projectService.SolutionLoaded += SolutionLoaded;
			this.scriptsConsole = scriptsConsole;
			this.scriptsFactory = scriptsFactory;
		}
		
		void SolutionLoaded(object sender, SolutionEventArgs e)
		{
			RunPackageInitializationScripts(e.Solution);
		}
		
		void RunPackageInitializationScripts(Solution solution)
		{
			if (SolutionHasPackageInitializationScripts(solution)) {
				RunInitializePackagesCmdlet();
			}
		}
		
		bool SolutionHasPackageInitializationScripts(Solution solution)
		{
			IPackageInitializationScripts scripts = CreatePackageInitializationScripts(solution);
			return scripts.Any();
		}
		
		void RunInitializePackagesCmdlet()
		{
			string command = "Invoke-InitializePackages";
			scriptsConsole.ExecuteCommand(command);
		}
		
		IPackageInitializationScripts CreatePackageInitializationScripts(Solution solution)
		{
			return scriptsFactory.CreatePackageInitializationScripts(solution);
		}
	}
}
