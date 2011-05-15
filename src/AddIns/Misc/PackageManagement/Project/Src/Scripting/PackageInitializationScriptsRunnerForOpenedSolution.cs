// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializationScriptsRunnerForOpenedSolution
	{
		IPackageInitializationScriptsFactory scriptsFactory;
		IPackageManagementConsoleHost consoleHost;
		
		public PackageInitializationScriptsRunnerForOpenedSolution(
			IPackageManagementProjectService projectService,
			IPackageManagementConsoleHost consoleHost)
			: this(projectService, consoleHost, new PackageInitializationScriptsFactory())
		{
		}
		
		public PackageInitializationScriptsRunnerForOpenedSolution(
			IPackageManagementProjectService projectService,
			IPackageManagementConsoleHost consoleHost,
			IPackageInitializationScriptsFactory scriptsFactory)
		{
			projectService.SolutionLoaded += SolutionLoaded;
			this.consoleHost = consoleHost;
			this.scriptsFactory = scriptsFactory;
		}
		
		void SolutionLoaded(object sender, SolutionEventArgs e)
		{
			RunScripts(e.Solution);
		}
		
		void RunScripts(Solution solution)
		{
			IPackageInitializationScripts scripts = CreatePackageInitializationScripts(solution);
			scripts.Run();
		}
		
		IPackageInitializationScripts CreatePackageInitializationScripts(Solution solution)
		{
			PowerShellPackageScriptSession session = CreateScriptSession();
			return scriptsFactory.CreatePackageInitializationScripts(solution, session);
		}
		
		PowerShellPackageScriptSession CreateScriptSession()
		{
			IPowerShellHost powerShellHost = consoleHost.PowerShellHost;
			return new PowerShellPackageScriptSession(powerShellHost, consoleHost.ScriptingConsole);
		}
	}
}
