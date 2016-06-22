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
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class ThreadSafeScriptingConsoleTests
	{
		TestableThreadSafeScriptingConsole threadSafeConsole;
		FakeControlDispatcher dispatcher;
		FakeScriptingConsole nonThreadSafeScriptingConsole;
		TestableThreadSafeScriptingConsoleEvents threadSafeConsoleEvents;
		
		void CreateThreadSafeScriptingConsole()
		{
			threadSafeConsole = new TestableThreadSafeScriptingConsole();
			
			dispatcher = threadSafeConsole.Dispatcher;
			dispatcher.CheckAccessReturnValue = true;
			
			nonThreadSafeScriptingConsole = threadSafeConsole.NonThreadSafeScriptingConsole;
			
			threadSafeConsoleEvents = threadSafeConsole.ConsoleEvents;
		}
		
		[Test]
		public void WriteLine_DispatcherCheckAccessReturnsTrue_NonThreadSafeScriptingConsoleWriteLineMethodIsCalled()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = true;
			threadSafeConsole.WriteLine();
			
			Assert.IsTrue(nonThreadSafeScriptingConsole.IsWriteLineCalled);
		}
		
		[Test]
		public void WriteLine_DispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			threadSafeConsole.WriteLine();
			
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void WriteLine_TextAndStyleParametersPassedAndDispatcherCheckAccessReturnsTrue_NonThreadSafeScriptingConsoleWriteLineMethodIsCalled()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = true;
			threadSafeConsole.WriteLine("abc", ScriptingStyle.Out);
			
			object[] parameters = new object[] { 
				nonThreadSafeScriptingConsole.TextPassedToWriteLine,
				nonThreadSafeScriptingConsole.ScriptingStylePassedToWriteLine
			};
			
			object[] expectedParameters = new object[] { 
				"abc", 
				ScriptingStyle.Out
			};
			
			Assert.AreEqual(expectedParameters, parameters);
		}
		
		[Test]
		public void WriteLine_TextAndScriptingStylePassedAndDispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			threadSafeConsole.WriteLine("abc", ScriptingStyle.Out);
			
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void WriteLine_TextAndScriptingStylePassedDispatcherCheckAccessReturnsFalse_MethodIsInvokedWithTextAndStyleAsArguments()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			
			threadSafeConsole.WriteLine("abc", ScriptingStyle.Out);
			
			object[] expectedArgs = new object[] { 
				"abc", 
				ScriptingStyle.Out
			};

			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void Write_TextAndStyleParametersPassedAndDispatcherCheckAccessReturnsTrue_NonThreadSafeScriptingConsoleWriteMethodIsCalled()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = true;
			threadSafeConsole.Write("abc", ScriptingStyle.Out);
			
			object[] parameters = new object[] { 
				nonThreadSafeScriptingConsole.TextPassedToWrite,
				nonThreadSafeScriptingConsole.ScriptingStylePassedToWrite
			};
			
			object[] expectedParameters = new object[] { 
				"abc", 
				ScriptingStyle.Out
			};
			
			Assert.AreEqual(expectedParameters, parameters);
		}
		
		[Test]
		public void Write_TextAndScriptingStylePassedAndDispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			threadSafeConsole.Write("abc", ScriptingStyle.Out);
			
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void Write_TextAndScriptingStylePassedDispatcherCheckAccessReturnsFalse_MethodIsInvokedWithTextAndStyleAsArguments()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			
			threadSafeConsole.Write("abc", ScriptingStyle.Out);
			
			object[] expectedArgs = new object[] { 
				"abc", 
				ScriptingStyle.Out
			};

			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasOneLineWaitingToBeRead_ReturnsLineFromNonThreadSafeConsole()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = "abc";
			
			int indent = 4;
			string line = threadSafeConsole.ReadLine(indent);
			
			string expectedLine = "abc";
			
			Assert.AreEqual(expectedLine, line);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasOneLineWaitingToBeRead_IndentPassedToNonThreadSafeConsole()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = "abc";
			
			int expectedIndent = 4;
			string line = threadSafeConsole.ReadLine(expectedIndent);
			
			Assert.AreEqual(expectedIndent, nonThreadSafeScriptingConsole.AutoIndentSizePassedToReadLine);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasOneLineWaitingToBeReadAndDispatcherCheckAccessReturnsFalse_ReadLineMethodIsInvokedWithArguments()
		{
			CreateThreadSafeScriptingConsole();
			dispatcher.CheckAccessReturnValue = false;
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = "abc";
			
			int expectedIndent = 4;
			string line = threadSafeConsole.ReadLine(expectedIndent);
			
			object[] expectedArgs = new object[] { expectedIndent };
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasOneLineWaitingToBeReadAndDispatcherCheckAccessReturnsFalse_ReadLineMethodIsInvoked()
		{
			CreateThreadSafeScriptingConsole();
			dispatcher.CheckAccessReturnValue = false;
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = "abc";
			
			int expectedIndent = 4;
			string line = threadSafeConsole.ReadLine(expectedIndent);
			
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasNoLinesWaitingToBeRead_MethodWaitsForLine()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = null;
			
			int indent = 4;
			string line = threadSafeConsole.ReadLine(indent);
			
			Assert.IsTrue(threadSafeConsoleEvents.IsWaitForLineCalled);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasNoLinesWaitingToBeReadAndWaitForLineReturnsFalse_ReturnsNull()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = null;
			threadSafeConsoleEvents.WaitForLineReturnValue = false;
			
			int indent = 4;
			string line = threadSafeConsole.ReadLine(indent);
			
			Assert.IsNull(line);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasNoLinesWaitingToBeReadAndWaitForLineReturnsTrue_ReturnsFirstLineFromNonThreadSafeConsole()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = null;
			threadSafeConsoleEvents.WaitForLineReturnValue = true;
			threadSafeConsoleEvents.DispatcherCheckAccessReturnValueAfterWaitForLineCalled = true;
			nonThreadSafeScriptingConsole.TextToReturnFromReadFirstUnreadLine = "abc";
			
			int indent = 4;
			string line = threadSafeConsole.ReadLine(indent);
			
			string expectedLine = "abc";
			Assert.AreEqual(expectedLine, line);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasNoLinesWaitingToBeReadAndWaitForLineReturnsTrueWhenDispatcherCheckAccessReturnsFalse_ReadFirstUnreadLineInvokedAndReturnsLine()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = null;
			threadSafeConsoleEvents.WaitForLineReturnValue = true;
			threadSafeConsoleEvents.DispatcherCheckAccessReturnValueAfterWaitForLineCalled = false;
			nonThreadSafeScriptingConsole.TextToReturnFromReadFirstUnreadLine = null;
			dispatcher.InvokeReturnValue = "abc";
			
			int indent = 4;
			string line = threadSafeConsole.ReadLine(indent);
			
			string expectedLine = "abc";
			Assert.AreEqual(expectedLine, line);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasNoLinesWaitingToBeReadAndWaitForLineReturnsTrueWhenDispatcherCheckAccessReturnsFalse_ReadFirstUnreadLineInvoked()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = null;
			threadSafeConsoleEvents.WaitForLineReturnValue = true;
			threadSafeConsoleEvents.DispatcherCheckAccessReturnValueAfterWaitForLineCalled = false;
			nonThreadSafeScriptingConsole.TextToReturnFromReadFirstUnreadLine = null;
			dispatcher.InvokeReturnValue = "abc";
			
			int indent = 4;
			string line = threadSafeConsole.ReadLine(indent);
			
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void ReadLine_NonThreadSafeConsoleHasNoLinesWaitingToBeRead_LineReceivedEventResetBeforeReadLineCalled()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadLine = null;
			threadSafeConsoleEvents.WaitForLineReturnValue = false;
			
			int indent = 4;
			string line = threadSafeConsole.ReadLine(indent);
			
			Assert.IsTrue(threadSafeConsoleEvents.IsLineReceivedEventResetBeforeReadLineCalled);
		}
		
		[Test]
		public void Dispose_ConsoleToBeShutdown_SetDisposedEventMethodIsCalled()
		{
			CreateThreadSafeScriptingConsole();
			threadSafeConsole.Dispose();
			
			Assert.IsTrue(threadSafeConsoleEvents.IsSetDisposedEventCalled);
		}
		
		[Test]
		public void Constructor_NonThreadSafeScriptingConsoleFiresLineReceivedEvent_SetLineReceivedEventIsCalled()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.FireLineReceivedEvent();
			
			Assert.IsTrue(threadSafeConsoleEvents.IsSetLineReceivedEventCalled);			
		}
		
		[Test]
		public void Dispose_NonThreadSafeScriptingConsoleFiresLineReceivedEvent_SetLineReceivedEventIsNotCalled()
		{
			CreateThreadSafeScriptingConsole();
			threadSafeConsole.Dispose();
			nonThreadSafeScriptingConsole.FireLineReceivedEvent();
			
			Assert.IsFalse(threadSafeConsoleEvents.IsSetLineReceivedEventCalled);
		}
		
		[Test]
		public void SendLine_PassedThreeCharacters_CallsNonThreadSafeConsoleSendLineMethod()
		{
			CreateThreadSafeScriptingConsole();
			threadSafeConsole.SendLine("abc");
			
			string expectedLine = "abc";
			Assert.AreEqual(expectedLine, nonThreadSafeScriptingConsole.TextPassedToSendLine);
		}
		
		[Test]
		public void SendText_PassedThreeCharacters_CallsNonThreadSafeConsoleSendTextMethod()
		{
			CreateThreadSafeScriptingConsole();
			threadSafeConsole.SendText("abc");
			
			string expectedText = "abc";
			Assert.AreEqual(expectedText, nonThreadSafeScriptingConsole.TextPassedToSendText);
		}
		
		[Test]
		public void ReadFirstUnreadLine_NonThreadSafeConsoleHasOneUnreadLine_ReturnsNonThreadSafeConsoleUnreadLine()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.TextToReturnFromReadFirstUnreadLine = "abc";
			string line = threadSafeConsole.ReadFirstUnreadLine();
			
			string expectedLine = "abc";
			Assert.AreEqual(expectedLine, line);
		}
		
		[Test]
		public void ScrollToEndWhenTextWritten_NonThreadSafeConsoleScrollToEndWhenTextWrittenIsTrue_ReturnsTrue()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.ScrollToEndWhenTextWritten = true;
			
			Assert.IsTrue(threadSafeConsole.ScrollToEndWhenTextWritten);
		}
		
		[Test]
		public void ScrollToEndWhenTextWritten_NonThreadSafeConsoleScrollToEndWhenTextWrittenIsFalse_ReturnsFalse()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.ScrollToEndWhenTextWritten = false;
			
			Assert.IsFalse(threadSafeConsole.ScrollToEndWhenTextWritten);
		}
		
		[Test]
		public void GetMaximumVisibleColumns_NonThreadSafeConsoleMaximumVisibleColumnsIsTen_ReturnsTen()
		{
			CreateThreadSafeScriptingConsole();
			nonThreadSafeScriptingConsole.MaximumVisibleColumns = 10;
			
			int columns = threadSafeConsole.GetMaximumVisibleColumns();
			
			Assert.AreEqual(10, columns);
		}
		
		[Test]
		public void GetMaximumVisibleColumns_DispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			dispatcher.InvokeReturnValue = 10;
			
			int columns = threadSafeConsole.GetMaximumVisibleColumns();
			
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void GetMaximumVisibleColumns_DispatcherCheckAccessReturnsFalse_DispatcherReturnValueIsReturned()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			dispatcher.InvokeReturnValue = 10;
			
			int columns = threadSafeConsole.GetMaximumVisibleColumns();
			
			Assert.AreEqual(10, columns);
		}
		
		[Test]
		public void Clear_DispatcherCheckAccessReturnsTrue_NonThreadSafeScriptingConsoleClearMethodIsCalled()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = true;
			threadSafeConsole.Clear();
			
			Assert.IsTrue(nonThreadSafeScriptingConsole.IsClearCalled);
		}
		
		[Test]
		public void Clear_DispatcherCheckAccessReturnsFalse_MethodIsInvoked()
		{
			CreateThreadSafeScriptingConsole();
			
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			threadSafeConsole.Clear();
			
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
	}
}
