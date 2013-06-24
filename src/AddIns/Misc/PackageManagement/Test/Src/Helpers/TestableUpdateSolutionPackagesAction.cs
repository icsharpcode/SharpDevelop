// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class TestableUpdateSolutionPackagesAction : UpdateSolutionPackagesAction
	{
		public bool IsRunPackageScriptsActionCreated;
		public IPackageScriptRunner ScriptRunnerPassedToCreateRunPackageScriptsAction;
		public List<IPackageManagementProject> ProjectsPassedToCreateRunPackageScriptsAction;
		public RunAllProjectPackageScriptsAction RunPackageScriptsAction;
		
		public TestableUpdateSolutionPackagesAction(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents)
			: base(solution, packageManagementEvents)
		{
		}
		
		protected override RunAllProjectPackageScriptsAction CreateRunPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IEnumerable<IPackageManagementProject> projects)
		{
			IsRunPackageScriptsActionCreated = true;
			ScriptRunnerPassedToCreateRunPackageScriptsAction = scriptRunner;
			ProjectsPassedToCreateRunPackageScriptsAction = projects.ToList();
			RunPackageScriptsAction = new RunAllProjectPackageScriptsAction(
				scriptRunner,
				projects,
				new PackageScriptFactory(),
				new NullGlobalMSBuildProjectCollection());
			return RunPackageScriptsAction;
		}
		
		public bool IsRunPackageScriptsActionDisposed {
			get { return RunPackageScriptsAction.IsDisposed; }
		}
	}
}
