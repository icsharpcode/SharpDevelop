// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class TestableProcessPackageAction : ProcessPackageAction
	{
		public FakePackageManagementProject FakeProject;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage = new FakePackage("Test");
		
		public TestableProcessPackageAction()
			: this(new FakePackageManagementProject(), new FakePackageManagementEvents())
		{
		}
		
		public TestableProcessPackageAction(
			FakePackageManagementProject project,
			FakePackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
			FakeProject = project;
			FakePackageManagementEvents = packageManagementEvents;
			this.Package = FakePackage;
		}
		
		public void CallBeforeExecute()
		{
			base.BeforeExecute();
		}
		
		public bool IsRunPackageScriptsActionCreated;
		public IPackageScriptRunner ScriptRunnerPassedToCreateRunPackageScriptsAction;
		public IPackageManagementProject ProjectPassedToCreateRunPackageScriptsAction;
		public RunPackageScriptsAction RunPackageScriptsAction;
		
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
