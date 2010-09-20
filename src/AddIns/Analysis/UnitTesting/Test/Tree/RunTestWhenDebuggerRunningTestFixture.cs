// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunTestWhenDebuggerRunningTestFixture : RunTestWithDebuggerCommandTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			base.InitBase();
		}
		
		[Test]
		public void IfDebuggerLoadedAndCurrentDebuggerIsDebuggingUserIsAskedToStopDebugging()
		{
			SetDebuggerIsLoadedAndCurrentDebuggerIsDebuggingToTrue();
			RunTests();
			
			string expectedQuestion = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}";
			string actualQuestion = context.MockMessageService.QuestionPassedToAskQuestion;
			
			Assert.AreEqual(expectedQuestion, actualQuestion);
		}
		
		void SetDebuggerIsLoadedAndCurrentDebuggerIsDebuggingToTrue()
		{
			debuggerService.MockDebugger.IsDebugging = true;
			debuggerService.IsDebuggerLoaded = true;
		}
		
		void RunTests()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
		}
		
		[Test]
		public void IfDebuggerLoadedAndCurrentDebuggerIsDebuggingUserMessageBoxHasStopDebuggingCaption()
		{
			SetDebuggerIsLoadedAndCurrentDebuggerIsDebuggingToTrue();
			RunTests();
			
			string expectedCaption = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}";
			string actualCaption = context.MockMessageService.CaptionPassedToAskQuestion;
			
			Assert.AreEqual(expectedCaption, actualCaption);
		}
		
		[Test]
		public void IfDebuggerLoadedAndCurrentDebuggerIsDebuggingAndUserSaysYesToDialogThenDebuggerIsStopped()
		{
			SetDebuggerIsLoadedAndCurrentDebuggerIsDebuggingToTrue();
			SetMessageServiceAskQuestionToReturnTrue();
			RunTests();
			
			Assert.IsTrue(debuggerService.MockDebugger.IsStopCalled);
		}
		
		void SetMessageServiceAskQuestionToReturnTrue()
		{
			context.MockMessageService.AskQuestionReturnValue = true;
		}
		
		[Test]
		public void IfUserSaysYesToStopDebuggingDialogThenDebuggerIsStarted()
		{
			SetDebuggerIsLoadedAndCurrentDebuggerIsDebuggingToTrue();
			SetMessageServiceAskQuestionToReturnTrue();
			RunTests();
			
			string expectedCommand = 
				@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			string actualCommand = debuggerService.MockDebugger.ProcessStartInfo.FileName;
			
			Assert.AreEqual(expectedCommand, actualCommand);
		}
		
		[Test]
		public void IfUserSaysNoToStopDebuggingDialogThenDebuggerIsNotStarted()
		{
			SetDebuggerIsLoadedAndCurrentDebuggerIsDebuggingToTrue();
			RunTests();
			
			Assert.IsNull(debuggerService.MockDebugger.ProcessStartInfo);
		}
		
		[Test]
		public void IfDebuggerNotLoadedThenDebuggerIsNotStopped()
		{
			SetCurrentDebuggerIsDebuggingToTrue();
			SetMessageServiceAskQuestionToReturnTrue();
			RunTests();
			
			Assert.IsFalse(debuggerService.MockDebugger.IsStopCalled);
		}
		
		void SetCurrentDebuggerIsDebuggingToTrue()
		{
			debuggerService.MockDebugger.IsDebugging = true;
		}

	}
}
