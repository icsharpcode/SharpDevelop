// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using Input = System.Windows.Input;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Ensures that both lines of text can be read from the console if they are written
	/// before ReadLine is called.
	/// </summary>
	[TestFixture]
	public class TwoConsoleLinesWaitingTests
	{
		string line1;
		string line2;
		TestableScriptingConsole scriptingConsole;
		bool lineAvailableBeforeFirstEnterKey;
		bool lineAvailableAfterFirstEnterKey;
		bool lineAvailableAtEnd;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (scriptingConsole = new TestableScriptingConsole()) { 
				MockConsoleTextEditor textEditor = scriptingConsole.MockConsoleTextEditor;
				textEditor.RaisePreviewKeyDownEvent(Input.Key.A);
				textEditor.RaisePreviewKeyDownEvent(Input.Key.B);
				textEditor.RaisePreviewKeyDownEvent(Input.Key.C);
				lineAvailableBeforeFirstEnterKey = scriptingConsole.IsLineAvailable;
				textEditor.RaisePreviewKeyDownEventForDialogKey(Input.Key.Enter);
				lineAvailableAfterFirstEnterKey = scriptingConsole.IsLineAvailable;
 				
				textEditor.RaisePreviewKeyDownEvent(Input.Key.D);
				textEditor.RaisePreviewKeyDownEvent(Input.Key.E);
				textEditor.RaisePreviewKeyDownEvent(Input.Key.F);
				textEditor.RaisePreviewKeyDownEventForDialogKey(Input.Key.Enter);

				Thread t = new Thread(ReadLinesOnSeparateThread);
				t.Start();

				int sleepInterval = 20;
				int currentWait = 0;
				int maxWait = 2000;
				
				while (line2 == null && currentWait < maxWait) {
					Thread.Sleep(sleepInterval);
					currentWait += sleepInterval;
				}
				
				lineAvailableAtEnd = scriptingConsole.IsLineAvailable;
			}
		}
		
		[Test]
		public void FirstLineRead()
		{
			Assert.AreEqual("ABC", line1);
		}
		
		[Test]
		public void SecondLineRead()
		{
			Assert.AreEqual("DEF", line2);
		}
		
		[Test]
		public void LineAvailableBeforeEnterKeyPressed()
		{
			Assert.IsFalse(lineAvailableBeforeFirstEnterKey);
		}

		[Test]
		public void LineAvailableAfterEnterKeyPressed()
		{
			Assert.IsTrue(lineAvailableAfterFirstEnterKey);
		}
		
		[Test]
		public void LineAvailableAfterAllLinesRead()
		{
			Assert.IsFalse(lineAvailableAtEnd);
		}
		
		void ReadLinesOnSeparateThread()
		{
			line1 = scriptingConsole.ReadLine(0);
			line2 = scriptingConsole.ReadLine(0);
		}
	}
}
