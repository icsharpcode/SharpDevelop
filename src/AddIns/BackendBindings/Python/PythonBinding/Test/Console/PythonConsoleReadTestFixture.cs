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
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the PythonConsole ReadLine method.
	/// </summary>
	[TestFixture]
	public class PythonConsoleReadTestFixture : PythonConsoleTestsBase
	{
		int initialAutoIndentSize = 4;
		string readLine;
		int autoIndentSize;
		bool raiseKeyPressEvent;
		bool raiseDialogKeyPressEvent;
		
		[TestFixtureSetUp]
		public void Init()
		{
			base.CreatePythonConsole();
			
			autoIndentSize = initialAutoIndentSize;
			Thread thread = new Thread(ReadLineFromConsoleOnDifferentThread);
			thread.Start();
						
			int sleepInterval = 10;
			int maxWait = 2000;
			int currentWait = 0;
			while ((MockConsoleTextEditor.Text.Length < autoIndentSize) && (currentWait < maxWait)) {
				Thread.Sleep(sleepInterval);
				currentWait += sleepInterval;
			}
			
			raiseKeyPressEvent = MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			raiseDialogKeyPressEvent = MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			currentWait = 0;
			while ((MockConsoleTextEditor.Text.Length < autoIndentSize + 2) && (currentWait < maxWait)) {
				Thread.Sleep(10);
				currentWait += sleepInterval;				
			}
			thread.Join(2000);
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			TestablePythonConsole.Dispose();
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
			Assert.AreEqual(expectedString, MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void DocumentInsertCalledWhenAutoIndentIsNonZero()
		{
			Assert.IsTrue(MockConsoleTextEditor.IsWriteCalled);
		}
		
		[Test]
		public void NoTextWrittenWhenAutoIndentSizeIsZero()
		{
			TestablePythonConsole pythonConsole = new TestablePythonConsole();
			MockConsoleTextEditor textEditor = pythonConsole.MockConsoleTextEditor;
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			textEditor.IsWriteCalled = false;
			pythonConsole.ReadLine(0);
			Assert.IsFalse(textEditor.IsWriteCalled);
		}
		
		/// <summary>
		/// Should return false for any character that should be handled by the text editor itself.
		/// </summary>
		[Test]
		public void RaiseKeyPressEventReturnedFalse()
		{
			Assert.IsFalse(raiseKeyPressEvent);
		}
		
		/// <summary>
		/// Should return false for any character that should be handled by the text editor itself.
		/// </summary>
		[Test]
		public void RaiseDialogKeyPressEventReturnedFalse()
		{
			Assert.IsFalse(raiseDialogKeyPressEvent);
		}
		
		void ReadLineFromConsoleOnDifferentThread()
		{
			System.Console.WriteLine("Reading on different thread");
			readLine = TestablePythonConsole.ReadLine(autoIndentSize);
			System.Console.WriteLine("Finished reading on different thread");
		}
	}
}
