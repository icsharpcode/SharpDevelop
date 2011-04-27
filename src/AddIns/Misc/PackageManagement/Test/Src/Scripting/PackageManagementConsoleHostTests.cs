// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageManagementConsoleHostTests
	{
		TestablePackageManagementConsoleHost host;
		FakeScriptingConsoleWithLinesToRead scriptingConsole;
		FakePowerShellHost powerShellHost;
		FakePackageManagementSolution fakeSolution;
		FakePackageManagementProjectService fakeProjectService;
		
		void CreateHost()
		{
			host = new TestablePackageManagementConsoleHost();
			fakeSolution = host.FakeSolution;
			scriptingConsole = host.FakeScriptingConsole;
			powerShellHost = host.FakePowerShellHostFactory.FakePowerShellHost;
			fakeProjectService = host.FakeProjectService;
		}
		
		void RunHost()
		{
			host.Run();
			host.ThreadStartPassedToCreateThread.Invoke();
		}
		
		TestableProject AddProject(string name)
		{
			var project = ProjectHelper.CreateTestProject(name);
			fakeProjectService.AddFakeProject(project);
			return project;
		}
		
		[Test]
		public void Dispose_ScriptingConsoleIsNotNull_ScriptingConsoleIsDisposed()
		{
			CreateHost();
			host.Dispose();
			
			Assert.IsTrue(scriptingConsole.IsDisposeCalled);
		}
		
		[Test]
		public void Dispose_ScriptingConsoleIsNull_NullReferenceExceptionIsNotThrown()
		{
			CreateHost();
			host.ScriptingConsole = null;
			
			Assert.DoesNotThrow(() => host.Dispose());
		}
		
		[Test]
		public void Run_ConsoleHostIsRun_StartsThreadToProcessCommandsEnteredIntoConsole()
		{
			CreateHost();
			host.Run();
			
			Assert.IsTrue(host.FakeThread.IsStartCalled);
		}
		
		[Test]
		public void Dispose_ConsoleHostRunCalled_ThreadJoinIsCalled()
		{
			CreateHost();
			host.Run();
			host.Dispose();
			
			Assert.IsTrue(host.FakeThread.IsJoinCalled);
		}
		
		[Test]
		public void Dispose_DisposeCalledTwiceAfterConsoleHostIsRun_ThreadJoinIsCalledOnce()
		{
			CreateHost();
			host.Run();
			host.Dispose();
			host.FakeThread.IsJoinCalled = false;
			
			host.Dispose();
			
			Assert.IsFalse(host.FakeThread.IsJoinCalled);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_PowerShellHostIsCreated()
		{
			CreateHost();
			RunHost();
			
			var actualConsole = host.FakePowerShellHostFactory.ScriptingConsolePassedToCreatePowerShellHost;
			var expectedConsole = scriptingConsole;
			
			Assert.AreSame(expectedConsole, actualConsole);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_InitialPowerShellExecutionPolicySet()
		{
			CreateHost();
			RunHost();
			
			Assert.IsTrue(powerShellHost.IsSetRemoteSignedExecutionPolicyCalled);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_PromptTextWrittenToConsole()
		{
			CreateHost();
			RunHost();
			var expectedTextPassedToWrite = new String[] { "PM> "};
			var actualTextPassedToWrite = scriptingConsole.AllTextPassedToWrite;
			
			CollectionAssert.AreEqual(expectedTextPassedToWrite, actualTextPassedToWrite);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_PromptTextWrittenWithPromptyStyleToConsole()
		{
			CreateHost();
			RunHost();
			
			Assert.AreEqual(ScriptingStyle.Prompt, scriptingConsole.ScriptingStylePassedToWrite);
		}
		
		[Test]
		public void Run_OneCommandEntered_CommandExecutedByPowerShellHost()
		{
			CreateHost();
			scriptingConsole.AllTextToReturnFromReadLine.Add("RunThis");
			RunHost();
			
			Assert.AreEqual("RunThis", powerShellHost.CommandPassedToExecuteCommand);
		}
		
		[Test]
		public void Run_OneCommandEntered_CommandPromptDisplayedAgain()
		{
			CreateHost();
			scriptingConsole.AllTextToReturnFromReadLine.Add("RunThis");
			RunHost();
			
			var expectedTextPassedToWrite = new String[] { "PM> ", "PM> "};
			var actualTextPassedToWrite = scriptingConsole.AllTextPassedToWrite;
			
			CollectionAssert.AreEqual(expectedTextPassedToWrite, actualTextPassedToWrite);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_NoCommandsPassedToPowerShellHost()
		{
			CreateHost();
			powerShellHost.CommandPassedToExecuteCommand = "test";
			RunHost();
			
			Assert.AreEqual("test", powerShellHost.CommandPassedToExecuteCommand);
		}
		
		[Test]
		public void Run_TwoCommandsEnteredByUser_BothCommandsExecuted()
		{
			CreateHost();
			var commands = new string[] { "one", "two" };
			scriptingConsole.AllTextToReturnFromReadLine.AddRange(commands);
			
			host.ScriptingConsole = scriptingConsole;
			RunHost();
			
			Assert.AreEqual(commands, powerShellHost.AllCommandsPassedToExecuteCommand);
		}
		
		[Test]
		public void Run_PowerShellHostInitialization_CmdletsAreImported()
		{
			CreateHost();
			string cmdletsAssemblyFileName = 
				@"d:\program files\SharpDevelop\4.0\AddIns\PackageManagement\PackageManagement.Cmdlets.dll";
			host.FakePackageManagementAddInPath.CmdletsAssemblyFileName = cmdletsAssemblyFileName;
			RunHost();
			
			var expectedModules = new string[] {
				cmdletsAssemblyFileName
			};
			var actualModules = powerShellHost.ModulesToImport;
			
			CollectionAssert.AreEqual(expectedModules, actualModules);
		}
		
		[Test]
		public void Run_TwoPowerShellFormattingConfigXmlFilesInAddInFolder_UpdateFormattingCalledWithTwoFormattingFiles()
		{
			CreateHost();
			var files = new string[] {
				@"d:\program files\SharpDevelop\4.0\AddIns\PackageManagement\Scripting\Package.Format.ps1xml",
				@"d:\temp\test\Format.ps1xml"
			};
			host.FakePackageManagementAddInPath.PowerShellFormattingFileNames.AddRange(files);
			
			RunHost();
			
			var actualFiles = powerShellHost.FormattingFilesPassedToUpdateFormatting;
			
			CollectionAssert.AreEqual(files, actualFiles);
		}
		
		[Test]
		public void Run_TextDisplayedBeforeFirstPromptDisplayed_NuGetVersionDisplayed()
		{
			CreateHost();
			powerShellHost.Version = new Version("1.2.0.4");
			RunHost();
			
			string expected = "NuGet 1.2.0.4";
			
			bool contains = scriptingConsole.AllTextPassedToWriteLine.Contains(expected);
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void Run_TextDisplayedBeforeFirstPromptDisplayed_HelpInfoDisplayed()
		{
			CreateHost();
			string expectedHelpMessage = "Type 'get-help NuGet' for more information.";
			host.TextToReturnFromGetHelpInfo = expectedHelpMessage;
			RunHost();
			
			bool contains = scriptingConsole.AllTextPassedToWriteLine.Contains("Type 'get-help NuGet' for more information.");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void Run_TextDisplayedBeforeFirstPromptDisplayed_BlankLineBeforePrompt()
		{
			CreateHost();
			host.TextToReturnFromGetHelpInfo = "abc";
			RunHost();
			
			string actualLastLine = scriptingConsole.LastLinePassedToWriteLine;
			string expectedLastLine = String.Empty;
			
			Assert.AreEqual(expectedLastLine, actualLastLine);
		}
		
		[Test]
		public void GetProject_ProjectNameAndPackageSourcePassed_ProjectNameUsedToGetProject()
		{
			CreateHost();
			string source = "http://sharpdevelop.net";
			string expectedProjectName = "Test";
			
			host.GetProject(source, expectedProjectName);
			
			var actualProjectName = fakeSolution.ProjectNamePassedToGetProject;
			
			Assert.AreEqual(expectedProjectName, actualProjectName);
		}
		
		[Test]
		public void GetProject_ProjectNameAndPackageSourcePassed_PackageSourceUsedToGetProject()
		{
			CreateHost();
			string expectedSource = "http://sharpdevelop.net";
			
			host.GetProject(expectedSource, "Test");
			
			var actualSource = fakeSolution.PackageSourcePassedToGetProject.Source;
			
			Assert.AreEqual(expectedSource, actualSource);
		}
		
		[Test]
		public void GetProject_ProjectNameAndPackageSourcePassed_ReturnsProject()
		{
			CreateHost();
			string source = "http://sharpdevelop.net";
			
			var project = host.GetProject(source, "Test");
			
			var expectedProject = fakeSolution.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_NullPackageSourcePassed_UsesDefaultSourceToCreateProject()
		{
			CreateHost();
			var expectedSource = new PackageSource("http://sharpdevelop.net");
			host.ActivePackageSource = expectedSource;
			string packageSource = null;
			
			host.GetProject(packageSource, "Test");
			
			var actualSource = fakeSolution.PackageSourcePassedToGetProject;
			
			Assert.AreEqual(expectedSource, actualSource);
		}
		
		[Test]
		public void GetProject_NullProjectPassed_UsesDefaultProjectToCreateProject()
		{
			CreateHost();
			var source = "http://sharpdevelop.net";
			var project = ProjectHelper.CreateTestProject("Test");
			host.DefaultProject = project;
			
			host.GetProject(source, null);
			
			var projectName = fakeSolution.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("Test", projectName);
		}
		
		[Test]
		public void GetProject_ProjectNameAndRepositoryPassed_ProjectNameUsedToGetProject()
		{
			CreateHost();
			var repository = new FakePackageRepository();
			string expectedProjectName = "Test";
			
			host.GetProject(repository, expectedProjectName);
			
			var actualProjectName = fakeSolution.ProjectNamePassedToGetProject;
			
			Assert.AreEqual(expectedProjectName, actualProjectName);
		}
		
		[Test]
		public void GetProject_ProjectNameAndRepositoryPassed_RepositoryUsedToGetProject()
		{
			CreateHost();
			var repository = new FakePackageRepository();
			
			host.GetProject(repository, "Test");
			
			var actualRepository = fakeSolution.RepositoryPassedToGetProject;
			
			Assert.AreEqual(repository, actualRepository);
		}
		
		[Test]
		public void GetProject_ProjectNameAndRepositoryPassed_ReturnsProject()
		{
			CreateHost();
			var repository = new FakePackageRepository();
			
			var project = host.GetProject(repository, "Test");
			
			var expectedProject = fakeSolution.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_NullProjectNameAndNonNullRepositoryPassed_UsesDefaultProjectNameToCreateProject()
		{
			CreateHost();
			host.DefaultProject = ProjectHelper.CreateTestProject("MyProject");
			var repository = new FakePackageRepository();
			
			var project = host.GetProject(repository, null);
			
			var projectName = fakeSolution.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", projectName);
		}
		
		[Test]
		public void GetOpenProjects_TwoProjectsInOpenSolution_ReturnsTwoProjects()
		{
			CreateHost();
			fakeProjectService.AddFakeProject(ProjectHelper.CreateTestProject("A"));
			fakeProjectService.AddFakeProject(ProjectHelper.CreateTestProject("B"));
			
			var projects = host.GetOpenProjects();
			var expectedProjects = fakeProjectService.FakeOpenProjects;
			
			CollectionAssert.AreEqual(expectedProjects, projects);
		}
	}
}
