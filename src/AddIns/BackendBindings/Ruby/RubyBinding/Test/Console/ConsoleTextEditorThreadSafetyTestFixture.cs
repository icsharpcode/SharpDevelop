// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using RubyBinding.Tests.Utils.Tests;

namespace RubyBinding.Tests.Console
{
	[TestFixture]
	public class ConsoleTextEditorThreadSafetyTestFixture
	{
		RubyConsoleTextEditor consoleTextEditor;
		TextEditor avalonEditTextEditor;
		MockControlDispatcher dispatcher;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			avalonEditTextEditor = new TextEditor();
			dispatcher = new MockControlDispatcher();
			consoleTextEditor = new RubyConsoleTextEditor(avalonEditTextEditor, dispatcher);
		}
		
		[Test]
		public void IfDispatcherCheckAccessReturnsFalseWriteMethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			consoleTextEditor.Write("abc");
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void IfDispatcherCheckAccessReturnsFalseWriteMethodIsInvokedWithTextAsArg()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			
			consoleTextEditor.Write("abc");
			object[] expectedArgs = new object[] { "abc" };
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void IfDispatcherCheckAccessReturnsFalseGetLineMethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			consoleTextEditor.GetLine(0);
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void IfDispatcherCheckAccessReturnsFalseGetLineMethodIsInvokedWithLineNumberAsArg()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			
			consoleTextEditor.GetLine(0);
			object[] expectedArgs = new object[] { 0 };
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void IfDispatcherCheckAccessReturnsFalseReplaceMethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			avalonEditTextEditor.Text = "abcd";
			
			consoleTextEditor.Replace(0, 2, "12");
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void IfDispatcherCheckAccessReturnsFalseReplaceethodIsInvokedWithThreeArgs()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			avalonEditTextEditor.Text = "abcd";
			
			consoleTextEditor.Replace(0, 2, "12");
			object[] expectedArgs = new object[] { 0, 2, "12" };
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
		
		[Test]
		public void IfDispatcherCheckAccessReturnsFalseMakeCurrentContentReadOnlyIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvoked = null;
			
			consoleTextEditor.MakeCurrentContentReadOnly();
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
	}
}
