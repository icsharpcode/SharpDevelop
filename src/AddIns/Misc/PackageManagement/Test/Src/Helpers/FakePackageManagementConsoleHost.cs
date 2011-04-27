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
		
		public bool IsDisposeCalled;
		public bool IsClearCalled;
		public bool IsRunCalled;
		
		public void Dispose()
		{
			IsDisposeCalled = true;
		}
		
		public void Clear()
		{
			IsClearCalled = true;
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
		
		public IPackageManagementProjectService ProjectService {
			get { return FakeProjectService; }
		}
		
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
		
		public List<TestableProject> FakeOpenProjects = new List<TestableProject>();
		
		public TestableProject AddFakeProject(string name)
		{
			var project = ProjectHelper.CreateTestProject(name);
			FakeOpenProjects.Add(project);
			return project;
		}
		
		public IEnumerable<IProject> GetOpenProjects()
		{
			return FakeOpenProjects;
		}
	}
}
