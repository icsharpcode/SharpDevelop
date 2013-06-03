// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class TestableUpdatePackagesAction : UpdatePackagesAction
	{
		public bool IsRunPackageScriptsActionCreated;
		public IPackageScriptRunner ScriptRunnerPassedToCreateRunPackageScriptsAction;
		public IPackageManagementProject ProjectPassedToCreateRunPackageScriptsAction;
		public RunPackageScriptsAction RunPackageScriptsAction;
		
		public TestableUpdatePackagesAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
		}
		
		protected override RunPackageScriptsAction CreateRunPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IPackageManagementProject project)
		{
			IsRunPackageScriptsActionCreated = true;
			ScriptRunnerPassedToCreateRunPackageScriptsAction = scriptRunner;
			ProjectPassedToCreateRunPackageScriptsAction = project;
			RunPackageScriptsAction = base.CreateRunPackageScriptsAction(scriptRunner, project);
			return RunPackageScriptsAction;
		}
		
		public bool IsRunPackageScriptsActionDisposed {
			get { return RunPackageScriptsAction.IsDisposed; }
		}
	}
}
