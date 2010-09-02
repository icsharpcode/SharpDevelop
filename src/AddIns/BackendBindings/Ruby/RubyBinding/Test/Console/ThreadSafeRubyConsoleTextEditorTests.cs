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
using ICSharpCode.Scripting.Tests.Utils.Tests;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using RubyBinding.Tests.Utils.Tests;

namespace RubyBinding.Tests.Console
{
	[TestFixture]
	public class ThreadSafeRubyConsoleTextEditorTests
	{
		ThreadSafeRubyConsoleTextEditor consoleTextEditor;
		MockConsoleTextEditor textEditor;
		MockControlDispatcher dispatcher;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditor = new MockConsoleTextEditor();
			dispatcher = new MockControlDispatcher();
			consoleTextEditor = new ThreadSafeRubyConsoleTextEditor(textEditor, dispatcher);
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
			textEditor.Text = "abcd";
			
			consoleTextEditor.Replace(0, 2, "12");
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void IfDispatcherCheckAccessReturnsFalseReplaceethodIsInvokedWithThreeArgs()
		{
			dispatcher.CheckAccessReturnValue = false;
			dispatcher.MethodInvokedArgs = null;
			textEditor.Text = "abcd";
			
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
