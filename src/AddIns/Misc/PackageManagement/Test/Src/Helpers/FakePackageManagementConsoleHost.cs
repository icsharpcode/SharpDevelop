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
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;
using Rhino.Mocks;

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
		
		public bool IsRunning { get; set; }
		
		public bool IsShutdownConsoleCalled;
		
		public void ShutdownConsole()
		{
			IsShutdownConsoleCalled = true;
		}
		
		public List<string> CommandsExecuted = new List<string>();
		
		public string FirstCommandExecuted {
			get { return CommandsExecuted[0]; }
		}
		
		public string LastCommandExecuted {
			get { return CommandsExecuted[CommandsExecuted.Count - 1]; }
		}
		
		public void ExecuteCommand(string command)
		{
			CommandsExecuted.Add(command);
		}
		
		public FakePackageRepository FakePackageRepository = 
			new FakePackageRepository();
		
		public PackageSource PackageSourcePassedToGetRepository;
		
		public IPackageRepository GetPackageRepository(PackageSource packageSource)
		{
			PackageSourcePassedToGetRepository = packageSource;
			return FakePackageRepository;
		}
		
		public bool IsSetDefaultRunspaceCalled;
		
		public void SetDefaultRunspace()
		{
			IsSetDefaultRunspaceCalled = true;
		}
		
		public IConsoleHostFileConflictResolver FakeFileConflictResolver =
			MockRepository.GenerateStub<IConsoleHostFileConflictResolver>();
		
		public FileConflictAction LastFileConflictActionUsedWhenCreatingResolver = 
			(FileConflictAction)(-1);
		
		public IConsoleHostFileConflictResolver CreateFileConflictResolver(FileConflictAction fileConflictAction)
		{
			LastFileConflictActionUsedWhenCreatingResolver = fileConflictAction;
			return FakeFileConflictResolver;
		}
		
		public void AssertFileConflictResolverIsDisposed()
		{
			FakeFileConflictResolver.AssertWasCalled(resolver => resolver.Dispose());
		}
		
		public IDisposable FakeConsoleHostLogger =
			MockRepository.GenerateStub<IDisposable>();
		
		public void AssertLoggerIsDisposed()
		{
			FakeConsoleHostLogger.AssertWasCalled(logger => logger.Dispose());
		}
		
		public ICmdletLogger CmdletLoggerUsedToCreateLogger;
		
		public IDisposable CreateLogger(ICmdletLogger logger)
		{
			CmdletLoggerUsedToCreateLogger = logger;
			return FakeConsoleHostLogger;
		}
	}
}
