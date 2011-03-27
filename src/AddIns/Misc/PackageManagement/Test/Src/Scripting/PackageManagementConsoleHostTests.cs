// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
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
		
		void CreateHost()
		{
			host = new TestablePackageManagementConsoleHost();
			scriptingConsole = host.FakeScriptingConsole;
			
			powerShellHost = host.FakePowerShellHostFactory.FakePowerShellHost;
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
	}
}
