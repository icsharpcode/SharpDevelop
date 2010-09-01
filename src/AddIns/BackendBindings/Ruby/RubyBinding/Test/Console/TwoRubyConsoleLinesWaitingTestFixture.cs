// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;
using Input = System.Windows.Input;

using ICSharpCode.RubyBinding;
using IronRuby.Hosting;
using IronRuby.Runtime;
using NUnit.Framework;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Ensures that both lines of text can be read from the Ruby console if they are written
	/// before ReadLine is called.
	/// </summary>
	[TestFixture]
	public class TwoRubyConsoleLinesWaitingTestFixture
	{
		string line1;
		string line2;
		TestableRubyConsole rubyConsole;
		bool lineAvailableBeforeFirstEnterKey;
		bool lineAvailableAfterFirstEnterKey;
		bool lineAvailableAtEnd;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (rubyConsole = new TestableRubyConsole()) {
				MockConsoleTextEditor textEditor = rubyConsole.MockConsoleTextEditor;
				textEditor.RaisePreviewKeyDownEvent(Input.Key.A);
				textEditor.RaisePreviewKeyDownEvent(Input.Key.B);
				textEditor.RaisePreviewKeyDownEvent(Input.Key.C);
				lineAvailableBeforeFirstEnterKey = rubyConsole.IsLineAvailable;
				textEditor.RaisePreviewKeyDownEventForDialogKey(Input.Key.Enter);
				lineAvailableAfterFirstEnterKey = rubyConsole.IsLineAvailable;
				
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
				
				lineAvailableAtEnd = rubyConsole.IsLineAvailable;
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
			line1 = rubyConsole.ReadLine(0);
			line2 = rubyConsole.ReadLine(0);
		}
	}
}
