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
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
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
		FakeRegisteredPackageRepositories fakeRegisteredPackageRepositories;
		
		void CreateHost()
		{
			host = new TestablePackageManagementConsoleHost();
			fakeSolution = host.FakeSolution;
			scriptingConsole = host.FakeScriptingConsole;
			powerShellHost = host.FakePowerShellHostFactory.FakePowerShellHost;
			fakeRegisteredPackageRepositories = host.FakeRegisteredPackageRepositories;
		}
		
		void RunHost()
		{
			host.Run();
			host.ThreadStartPassedToCreateThread.Invoke();
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
		public void Dispose_ConsoleHostRunCalled_ThreadJoinIsCalledWithTimeout()
		{
			CreateHost();
			host.Run();
			host.Dispose();
			
			int timeout = host.FakeThread.TimeoutPassedToJoin;
			int expectedTimeout = 100; //ms
			
			Assert.AreEqual(expectedTimeout, timeout);
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
		public void Dispose_DisposeCalledTwiceAfterConsoleHostIsRunAndThreadDoesNotFinishOnFirstCall_ThreadJoinIsCalledTwice()
		{
			CreateHost();
			host.Run();
			host.FakeThread.JoinReturnValue = false;
			host.Dispose();
			host.FakeThread.IsJoinCalled = false;
			
			host.Dispose();
			
			Assert.IsTrue(host.FakeThread.IsJoinCalled);
		}
		
		[Test]
		public void IsRunning_DisposedAndThreadFinishes_ReturnsFalse()
		{
			CreateHost();
			host.Run();
			host.FakeThread.JoinReturnValue = true;
			host.Dispose();
			
			Assert.IsFalse(host.IsRunning);
		}
		
		[Test]
		public void IsRunning_DisposedButThreadDidNotFinish_ReturnsTrue()
		{
			CreateHost();
			host.Run();
			host.FakeThread.JoinReturnValue = false;
			host.Dispose();
			
			Assert.IsTrue(host.IsRunning);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_PowerShellHostIsCreated()
		{
			CreateHost();
			RunHost();
			
			IScriptingConsole scriptingConsole = host.FakePowerShellHostFactory.ScriptingConsolePassedToCreatePowerShellHost;
			IScriptingConsole expectedScriptingConsole = scriptingConsole;
			
			Assert.AreSame(expectedScriptingConsole, scriptingConsole);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_PrivateDataIsSetToClearConsoleHostCommandObject()
		{
			CreateHost();
			RunHost();
			
			object privateDataObject = host.FakePowerShellHostFactory.PrivateDataPassedToCreatePowerShellHost;
			
			Assert.IsInstanceOf(typeof(ClearPackageManagementConsoleHostCommand), privateDataObject);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_DteObjectIsSet()
		{
			CreateHost();
			RunHost();
			
			object dte = host.FakePowerShellHostFactory.DtePassedToCreatePowerShellHost;
			
			Assert.IsInstanceOf(typeof(DTE), dte);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_PowerShellVersionIsSetToNuGetVersion()
		{
			CreateHost();
			var expectedVersion = new Version("1.0.1.3");
			host.NuGetVersionToReturn = expectedVersion;
			RunHost();
			
			Version version = host.FakePowerShellHostFactory.VersionPassedToCreatePowerShellHost;
			
			Assert.AreEqual(expectedVersion, version);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_InitialPowerShellExecutionPolicySet()
		{
			CreateHost();
			RunHost();
			
			Assert.IsTrue(powerShellHost.IsSetRemoteSignedExecutionPolicyCalled);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_ClearHostFunctionIsRedefined()
		{
			CreateHost();
			RunHost();
			
			string expectedExecutedScript = 
				"function Clear-Host { $host.PrivateData.ClearHost() }";
			
			bool executed = powerShellHost.AllCommandsPassedToExecuteCommand.Contains(expectedExecutedScript);
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_PromptTextWrittenToConsole()
		{
			CreateHost();
			RunHost();
			var expectedTextPassedToWrite = new String[] { "PM> "};
			List<string> actualTextPassedToWrite = scriptingConsole.AllTextPassedToWrite;
			
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
			List<string> actualTextPassedToWrite = scriptingConsole.AllTextPassedToWrite;
			
			CollectionAssert.AreEqual(expectedTextPassedToWrite, actualTextPassedToWrite);
		}
		
		[Test]
		public void Run_TwoCommandsEnteredByUser_FirstCommandExecuted()
		{
			CreateHost();
			var commands = new string[] { "one", "two" };
			scriptingConsole.AllTextToReturnFromReadLine.AddRange(commands);
			
			host.ScriptingConsole = scriptingConsole;
			RunHost();
			
			bool executed = powerShellHost.AllCommandsPassedToExecuteCommand.Contains("one");
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void Run_TwoCommandsEnteredByUser_SecondCommandExecuted()
		{
			CreateHost();
			var commands = new string[] { "one", "two" };
			scriptingConsole.AllTextToReturnFromReadLine.AddRange(commands);
			
			host.ScriptingConsole = scriptingConsole;
			RunHost();
			
			bool executed = powerShellHost.AllCommandsPassedToExecuteCommand.Contains("two");
			
			Assert.IsTrue(executed);
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
			IList<string> actualModules = powerShellHost.ModulesToImport;
			
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
			
			IEnumerable<string> actualFiles = powerShellHost.FormattingFilesPassedToUpdateFormatting;
			
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
			
			string actualProjectName = fakeSolution.ProjectNamePassedToGetProject;
			
			Assert.AreEqual(expectedProjectName, actualProjectName);
		}
		
		[Test]
		public void GetProject_ProjectNameAndPackageSourcePassed_PackageSourceUsedToGetProject()
		{
			CreateHost();
			string expectedSource = "http://sharpdevelop.net";
			
			host.GetProject(expectedSource, "Test");
			
			string actualSource = fakeSolution.PackageSourcePassedToGetProject.Source;
			
			Assert.AreEqual(expectedSource, actualSource);
		}
		
		[Test]
		public void GetProject_ProjectNameAndPackageSourcePassed_ReturnsProject()
		{
			CreateHost();
			string source = "http://sharpdevelop.net";
			
			IPackageManagementProject project = host.GetProject(source, "Test");
			
			FakePackageManagementProject expectedProject = fakeSolution.FakeProjectToReturnFromGetProject;
			
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
			
			PackageSource actualSource = fakeSolution.PackageSourcePassedToGetProject;
			
			Assert.AreEqual(expectedSource, actualSource);
		}
		
		[Test]
		public void GetProject_NullProjectPassed_UsesDefaultProjectToCreateProject()
		{
			CreateHost();
			string source = "http://sharpdevelop.net";
			TestableProject project = ProjectHelper.CreateTestProject("Test");
			host.DefaultProject = project;
			
			host.GetProject(source, null);
			
			string projectName = fakeSolution.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("Test", projectName);
		}
		
		[Test]
		public void GetProject_ProjectNameAndRepositoryPassed_ProjectNameUsedToGetProject()
		{
			CreateHost();
			var repository = new FakePackageRepository();
			string expectedProjectName = "Test";
			
			host.GetProject(repository, expectedProjectName);
			
			string actualProjectName = fakeSolution.ProjectNamePassedToGetProject;
			
			Assert.AreEqual(expectedProjectName, actualProjectName);
		}
		
		[Test]
		public void GetProject_ProjectNameAndRepositoryPassed_RepositoryUsedToGetProject()
		{
			CreateHost();
			var repository = new FakePackageRepository();
			
			host.GetProject(repository, "Test");
			
			IPackageRepository actualRepository = fakeSolution.RepositoryPassedToGetProject;
			
			Assert.AreEqual(repository, actualRepository);
		}
		
		[Test]
		public void GetProject_ProjectNameAndRepositoryPassed_ReturnsProject()
		{
			CreateHost();
			var repository = new FakePackageRepository();
			
			IPackageManagementProject project = host.GetProject(repository, "Test");
			
			FakePackageManagementProject expectedProject = fakeSolution.FakeProjectToReturnFromGetProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void GetProject_NullProjectNameAndNonNullRepositoryPassed_UsesDefaultProjectNameToCreateProject()
		{
			CreateHost();
			host.DefaultProject = ProjectHelper.CreateTestProject("MyProject");
			var repository = new FakePackageRepository();
			
			IPackageManagementProject project = host.GetProject(repository, null);
			
			string projectName = fakeSolution.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", projectName);
		}
		
		[Test]
		public void ActivePackageSource_ConsoleHostCreated_ReturnsRegisteredPackageSourcesActivePackageSource()
		{
			CreateHost();
			var expectedPackageSource = new PackageSource("Test");
			fakeRegisteredPackageRepositories.ActivePackageSource = expectedPackageSource;
			
			PackageSource actualPackageSource = host.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ActivePackageSource_ActivePackageSourceChanged_RegisteredPackageSourcesActivePackageSourceIsUpdated()
		{
			CreateHost();
			var expectedPackageSource = new PackageSource("Test");
			host.ActivePackageSource = expectedPackageSource;
			
			PackageSource actualPackageSource = fakeRegisteredPackageRepositories.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void Clear_ConsoleWindowHasOutputText_ScriptingConsoleIsCleared()
		{
			CreateHost();
			host.Clear();
			
			bool cleared = scriptingConsole.IsClearCalled;
			
			Assert.IsTrue(cleared);
		}
		
		[Test]
		public void WritePrompt_ConsoleWindowHasOutputText_PromptWritten()
		{
			CreateHost();
			host.Clear();
			host.WritePrompt();
			
			var expectedTextPassedToWrite = new String[] { "PM> "};
			List<string> actualTextPassedToWrite = scriptingConsole.AllTextPassedToWrite;
			
			CollectionAssert.AreEqual(expectedTextPassedToWrite, actualTextPassedToWrite);
		}
		
		[Test]
		public void IsRunning_BeforeRunCalled_ReturnsFalse()
		{
			CreateHost();
			bool running = host.IsRunning;
			
			Assert.IsFalse(running);
		}
		
		[Test]
		public void IsRunning_AfterRunCalled_ReturnsFalse()
		{
			CreateHost();
			RunHost();
			bool running = host.IsRunning;
			
			Assert.IsTrue(running);
		}
		
		[Test]
		public void Run_SolutionOpenWhenConsoleRun_InvokeInitializePackagesCmdletIsRun()
		{
			CreateHost();
			fakeSolution.IsOpen = true;
			RunHost();
			
			string expectedExecutedCommand = "Invoke-InitializePackages";
			
			bool executed = powerShellHost.AllCommandsPassedToExecuteCommand.Contains(expectedExecutedCommand);
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void Run_SolutionIsNotOpenWhenConsoleRun_InvokeInitializePackagesCmdletIsNotRun()
		{
			CreateHost();
			fakeSolution.IsOpen = false;
			RunHost();
			
			string command = "Invoke-InitializePackages";
			
			bool executed = powerShellHost.AllCommandsPassedToExecuteCommand.Contains(command);
			
			Assert.IsFalse(executed);
		}
		
		[Test]
		public void Run_SolutionIsNotOpenWhenConsoleRun_PowerShellWorkingDirectoryIsUpdated()
		{
			CreateHost();
			fakeSolution.IsOpen = false;
			RunHost();
			
			string command = "Invoke-UpdateWorkingDirectory";
			
			bool executed = powerShellHost.AllCommandsPassedToExecuteCommand.Contains(command);
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void Run_SolutionIsOpenWhenConsoleRun_PowerShellWorkingDirectoryIsUpdated()
		{
			CreateHost();
			fakeSolution.IsOpen = true;
			RunHost();
			
			string command = "Invoke-UpdateWorkingDirectory";
			
			bool executed = powerShellHost.AllCommandsPassedToExecuteCommand.Contains(command);
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void ShutdownConsole_ScriptingConsoleCreated_DisposesScriptingConsole()
		{
			CreateHost();
			host.ShutdownConsole();
			
			bool disposed = host.FakeScriptingConsole.IsDisposeCalled;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void ShutdownConsole_ScriptingConsoleIsNull_NullReferenceExceptionIsNotThrown()
		{
			CreateHost();
			host.ScriptingConsole = null;
			
			Assert.DoesNotThrow(() => host.ShutdownConsole());
		}
		
		[Test]
		public void ExecuteCommand_ScriptingConsoleCreated_SendsCommandToScriptingConsole()
		{
			CreateHost();
			host.ExecuteCommand("test");
			
			string text = host.FakeScriptingConsole.TextPassedToSendLine;
			string expectedText = "test";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void GetPackageRepository_PackageSourceSpecified_ReturnsPackageRepositoryFromRegisteredRepositories()
		{
			CreateHost();
			
			var packageSource = new PackageSource("Test");
			IPackageRepository repository = host.GetPackageRepository(packageSource);
			FakePackageRepository expectedRepository = fakeRegisteredPackageRepositories.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void GetPackageRepository_PackageSourceSpecified_PackagSourceUsedToGetRepository()
		{
			CreateHost();
			
			var expectedPackageSource = new PackageSource("Test");
			IPackageRepository repository = host.GetPackageRepository(expectedPackageSource);
			PackageSource packageSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void SetDefaultRunspace_AfterHostIsRun_SetsDefaultRunspaceOnPowershellHost()
		{
			CreateHost();
			RunHost();
			
			host.SetDefaultRunspace();
			
			Assert.IsTrue(powerShellHost.IsSetDefaultRunspaceCalled);
		}
		
		[Test]
		public void Run_ConsoleExitsOnFirstRead_TabExpansionFunctionDefined()
		{
			CreateHost();
			RunHost();
			
			string partialExecutedScript = "function TabExpansion";
			
			bool executed = powerShellHost.AllCommandsPassedToExecuteCommand.Any(command => command.Contains(partialExecutedScript));
			
			Assert.IsTrue(executed);
		}
	}
}
