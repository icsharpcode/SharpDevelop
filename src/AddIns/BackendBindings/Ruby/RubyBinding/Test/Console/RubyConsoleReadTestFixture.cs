// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests the RubyConsole ReadLine method.
	/// </summary>
	[TestFixture]
	public class RubyConsoleReadTestFixture
	{
		RubyConsole RubyConsole;
		int initialAutoIndentSize = 4;
		string readLine;
		int autoIndentSize;
		MockTextEditor mockTextEditor;
		bool raiseKeyPressEvent;
		bool raiseDialogKeyPressEvent;
		
		[TestFixtureSetUp]
		public void Init()
		{
			mockTextEditor = new MockTextEditor();
			RubyConsole = new RubyConsole(mockTextEditor, null);

			autoIndentSize = initialAutoIndentSize;
			Thread thread = new Thread(ReadLineFromConsoleOnDifferentThread);
			thread.Start();
						
			int sleepInterval = 10;
			int maxWait = 2000;
			int currentWait = 0;
			while (mockTextEditor.Text.Length < autoIndentSize && currentWait < maxWait) {
				Thread.Sleep(sleepInterval);
				currentWait += sleepInterval;
			}
			
			raiseKeyPressEvent = mockTextEditor.RaiseKeyPressEvent('a');
			raiseDialogKeyPressEvent = mockTextEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			
			currentWait = 0;
			while ((mockTextEditor.Text.Length < autoIndentSize + 2) && (currentWait < maxWait)) {
				Thread.Sleep(10);
				currentWait += sleepInterval;				
			}
			thread.Join(2000);
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			RubyConsole.Dispose();
		}

		[Test]
		public void ReadLineFromConsole()
		{
			string expectedString = String.Empty.PadLeft(initialAutoIndentSize) + "a";
			Assert.AreEqual(expectedString, readLine);
		}
		
		[Test]
		public void ReadLineWithNonZeroAutoIndentSizeWritesSpacesToTextEditor()
		{
			string expectedString = String.Empty.PadLeft(initialAutoIndentSize) + "a\r\n";
			Assert.AreEqual(expectedString, mockTextEditor.Text);
		}
		
		[Test]
		public void DocumentInsertCalledWhenAutoIndentIsNonZero()
		{
			Assert.IsTrue(mockTextEditor.IsWriteCalled);
		}
		
		[Test]
		public void NoTextWrittenWhenAutoIndentSizeIsZero()
		{
			MockTextEditor textEditor = new MockTextEditor();
			RubyConsole console = new RubyConsole(textEditor, null);
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			
			textEditor.IsWriteCalled = false;
			console.ReadLine(0);
			Assert.IsFalse(textEditor.IsWriteCalled);
		}

		[Test]
		public void TextEditorIndentStyleSetToNone()
		{
			Assert.AreEqual(IndentStyle.None, mockTextEditor.IndentStyle);
		}

		/// <summary>
		/// Should return false for any character that should be handled by the text editor itself.s
		/// </summary>
		[Test]
		public void RaiseKeyPressEventReturnedFalse()
		{
			Assert.IsFalse(raiseKeyPressEvent);
		}
		
		/// <summary>
		/// Should return false for any character that should be handled by the text editor itself.s
		/// </summary>
		[Test]
		public void RaiseDialogKeyPressEventReturnedFalse()
		{
			Assert.IsFalse(raiseDialogKeyPressEvent);
		}
		
		void ReadLineFromConsoleOnDifferentThread()
		{
			System.Console.WriteLine("Reading on different thread");
			readLine = RubyConsole.ReadLine(autoIndentSize);
			System.Console.WriteLine("Finished reading on different thread");
		}
	}
}
