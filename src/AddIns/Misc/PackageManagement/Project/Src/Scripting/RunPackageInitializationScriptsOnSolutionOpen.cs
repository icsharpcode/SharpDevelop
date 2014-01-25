// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			projectService.SolutionOpened += SolutionOpened;
			this.scriptsConsole = scriptsConsole;
			this.scriptsFactory = scriptsFactory;
		}
		
		void SolutionOpened(object sender, SolutionEventArgs e)
		{
			RunPackageInitializationScripts(e.Solution);
		}
		
		void RunPackageInitializationScripts(ISolution solution)
		{
			if (SolutionHasPackageInitializationScripts(solution)) {
				RunInitializePackagesCmdlet();
			}
		}
		
		bool SolutionHasPackageInitializationScripts(ISolution solution)
		{
			IPackageInitializationScripts scripts = CreatePackageInitializationScripts(solution);
			return scripts.Any();
		}
		
		void RunInitializePackagesCmdlet()
		{
			string command = "Invoke-InitializePackages";
			scriptsConsole.ExecuteCommand(command);
		}
		
		IPackageInitializationScripts CreatePackageInitializationScripts(ISolution solution)
		{
			return scriptsFactory.CreatePackageInitializationScripts(solution);
		}
	}
}
