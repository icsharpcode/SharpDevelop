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
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using Rhino.Mocks;

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
			RunPackageScriptsAction = new RunPackageScriptsAction(
				project,
				scriptRunner,
				new PackageScriptFactory(),
				MockRepository.GenerateStub<IGlobalMSBuildProjectCollection>());
			return RunPackageScriptsAction;
		}
		
		public bool IsRunPackageScriptsActionDisposed {
			get { return RunPackageScriptsAction.IsDisposed; }
		}
		
		public bool IsExecuteCoreCalled { get; set; }
		
		protected override void ExecuteCore()
		{
			IsExecuteCoreCalled = true;
		}
	}
}
