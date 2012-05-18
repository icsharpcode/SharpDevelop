// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunTestWithDebuggerCommandTestFixture : RunTestWithDebuggerCommandTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			base.InitBase();
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
		}
		
		[Test]
		public void DebuggerStartsUnitTestApplication()
		{
			string expectedFileName = 
				@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			string actualFileName = debuggerService.MockDebugger.ProcessStartInfo.FileName;
			
			Assert.AreEqual(expectedFileName, actualFileName);
		}
		
		[Test]
		public void DebuggerStartsUnitTestApplicationWithCorrectCommandLineArguments()
		{
			string expectedArguments =
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\" /noxml";
			string actualArguments = debuggerService.MockDebugger.ProcessStartInfo.Arguments;
			
			Assert.AreEqual(expectedArguments, actualArguments);
		}
		
		[Test]
		public void DebuggerStartsUnitTestApplicationInUnitTestApplicationWorkingDirectory()
		{
			string expectedWorkingDirectory = @"D:\SharpDevelop\bin\Tools\NUnit";
			string actualWorkingDirectory = debuggerService.MockDebugger.ProcessStartInfo.WorkingDirectory;
			
			Assert.AreEqual(expectedWorkingDirectory, actualWorkingDirectory);
		}
		
		[Test]
		public void UnitTestApplicationCommandLineWrittenToUnitTestCategory()
		{
			string expectedText = 
				"\"D:\\SharpDevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\" /noxml\r\n";
			string actualText = context.UnitTestCategory.Text;
			
			Assert.AreEqual(expectedText, actualText);
		}
		
		[Test]
		public void TestRunCompletedCalledAsynchronouslyAfterDebugStoppedEventFires()
		{
			context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls.Clear();
			debuggerService.MockDebugger.FireDebugStoppedEvent();
			Action expectedAction = runCommand.GetCallRunTestCompletedAction();
			Action actualAction = context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls[0];
			
			Assert.AreEqual(expectedAction, actualAction);
		}
		
		[Test]
		public void DebugStopEventHandlerRemovedAfterFirstDebugStopEventOccurs()
		{
			context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls.Clear();
			debuggerService.MockDebugger.FireDebugStoppedEvent();
			debuggerService.MockDebugger.FireDebugStoppedEvent();
			
			Assert.AreEqual(1, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls.Count);
		}
		
		[Test]
		public void StopMethodStopsDebuggerIfDebuggerIsRunning()
		{
			debuggerService.MockDebugger.IsDebugging = true;
			runCommand.Stop();
			
			Assert.IsTrue(debuggerService.MockDebugger.IsStopCalled);
		}
		
		[Test]
		public void StopMethodDoesNotStopDebuggerIfDebuggerIsNotRunning()
		{
			debuggerService.MockDebugger.IsDebugging = false;
			runCommand.Stop();
			
			Assert.IsFalse(debuggerService.MockDebugger.IsStopCalled);
		}
	}
}
