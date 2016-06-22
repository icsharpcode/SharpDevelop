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
