// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementConsoleHost : IPackageManagementConsoleHost
	{
		public IProject DefaultProject { get; set; }
		public PackageSource ActivePackageSource { get; set; }
		public IScriptingConsole ScriptingConsole { get; set; }
		
		public IPackageManagementSolution Solution {
			get { return FakeSolution; }
		}
		
		public FakePackageManagementSolution FakeSolution = new FakePackageManagementSolution();
		
		public bool IsDisposeCalled;
		public bool IsClearCalled;
		public bool IsWritePromptCalled;
		public bool IsRunCalled;
		
		public void Dispose()
		{
			IsDisposeCalled = true;
		}
		
		public void Clear()
		{
			IsClearCalled = true;
		}
		
		public void WritePrompt()
		{
			IsWritePromptCalled = true;
		}
		
		public void Run()
		{
			IsRunCalled = true;
		}
		
		public TestableProject AddFakeDefaultProject()
		{
			var project = ProjectHelper.CreateTestProject();
			DefaultProject = project;
			return project;
		}
		
		public PackageSource AddTestPackageSource()
		{
			var source = new PackageSource("Test");
			ActivePackageSource = source;
			return source;
		}
		
		public FakePackageManagementProjectService FakeProjectService = new FakePackageManagementProjectService();

		public string PackageSourcePassedToGetProject;
		public string ProjectNamePassedToGetProject;
		public FakePackageManagementProject FakeProject = new FakePackageManagementProject();
		
		public IPackageManagementProject GetProject(string packageSource, string projectName)
		{
			PackageSourcePassedToGetProject = packageSource;
			ProjectNamePassedToGetProject = projectName;
			return FakeProject;
		}
		
		public IPackageRepository PackageRepositoryPassedToGetProject;
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName)
		{
			PackageRepositoryPassedToGetProject = sourceRepository;
			ProjectNamePassedToGetProject = projectName;
			return FakeProject;
		}
		
		public string PackageSourcePassedToGetActivePackageSource;
		public PackageSource PackageSourceToReturnFromGetActivePackageSource = new PackageSource("http://sharpdevelop.com");
		
		public PackageSource GetActivePackageSource(string source)
		{
			PackageSourcePassedToGetActivePackageSource = source;
			return PackageSourceToReturnFromGetActivePackageSource;
		}
		
		public IPowerShellHost PowerShellHost {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
