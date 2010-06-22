// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the PythonConsole ReadLine method.
	/// </summary>
	[TestFixture]
	public class PythonConsoleReadTestFixture
	{
		PythonConsole pythonConsole;
		int initialAutoIndentSize = 4;
		string readLine;
		int autoIndentSize;
		MockConsoleTextEditor mockTextEditor;
		bool raiseKeyPressEvent;
		bool raiseDialogKeyPressEvent;
		
		[TestFixtureSetUp]
		public void Init()
		{
			mockTextEditor = new MockConsoleTextEditor();
			pythonConsole = new PythonConsole(mockTextEditor, null);

			autoIndentSize = initialAutoIndentSize;
			Thread thread = new Thread(ReadLineFromConsoleOnDifferentThread);
			thread.Start();
						
			int sleepInterval = 10;
			int maxWait = 2000;
			int currentWait = 0;
			while ((mockTextEditor.Text.Length < autoIndentSize) && (currentWait < maxWait)) {
				Thread.Sleep(sleepInterval);
				currentWait += sleepInterval;
			}
			
			raiseKeyPressEvent = mockTextEditor.RaisePreviewKeyDownEvent(Key.A);
			raiseDialogKeyPressEvent = mockTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
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
			pythonConsole.Dispose();
		}

		[Test]
		public void ReadLineFromConsole()
		{
			string expectedString = String.Empty.PadLeft(initialAutoIndentSize) + "A";
			Assert.AreEqual(expectedString, readLine);
		}
		
		[Test]
		public void ReadLineWithNonZeroAutoIndentSizeWritesSpacesToTextEditor()
		{
			string expectedString = String.Empty.PadLeft(initialAutoIndentSize) + "A\r\n";
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
			MockConsoleTextEditor textEditor = new MockConsoleTextEditor();
			PythonConsole console = new PythonConsole(textEditor, null);
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			textEditor.IsWriteCalled = false;
			console.ReadLine(0);
			Assert.IsFalse(textEditor.IsWriteCalled);
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
			readLine = pythonConsole.ReadLine(autoIndentSize);
			System.Console.WriteLine("Finished reading on different thread");
		}
	}
}
