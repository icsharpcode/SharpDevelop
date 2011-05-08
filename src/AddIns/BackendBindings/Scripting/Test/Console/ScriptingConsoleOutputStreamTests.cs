// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class ScriptingConsoleOutputStreamTests
	{
		ScriptingConsoleOutputStream stream;
		FakeConsoleTextEditor textEditor;
		FakeControlDispatcher dispatcher;
		
		[SetUp]
		public void Init()
		{
			textEditor = new FakeConsoleTextEditor();
			dispatcher = new FakeControlDispatcher();
			dispatcher.CheckAccessReturnValue = true;
			
			stream = new ScriptingConsoleOutputStream(textEditor, dispatcher);
		}
		
		[Test]
		public void CanRead_NewInstance_ReturnsFalse()
		{
			Assert.IsFalse(stream.CanRead);
		}

		[Test]
		public void CanSeek_NewInstance_ReturnsFalse()
		{
			Assert.IsFalse(stream.CanSeek);
		}

		[Test]
		public void CanWrite_NewInstance_ReturnsTrue()
		{
			Assert.IsTrue(stream.CanWrite);
		}
		
		[Test]
		public void Write_UTF8ByteArrayPassed_AddsTextToTextEditor()
		{
			byte[] bytes = UTF8Encoding.UTF8.GetBytes("test");
			stream.Write(bytes, 0, bytes.Length);
			
			string text = textEditor.TextPassedToAppend;
			
			string expectedText = "test";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Write_UTF8ByteArrayPassed_OffsetAndLengthUsedInWriteMethod()
		{
			textEditor.Text = String.Empty;
			byte[] bytes = UTF8Encoding.UTF8.GetBytes("0output1");
			stream.Write(bytes, 1, bytes.Length - 2);
			
			string text = textEditor.TextPassedToAppend;
			string expectedText = "output";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Write_DispatcherCheckAccessReturnsFalse_WriteMethodIsInvoked()
		{
			dispatcher.CheckAccessReturnValue = false;
			
			byte[] bytes = UTF8Encoding.UTF8.GetBytes("test");
			stream.Write(bytes, 0, bytes.Length);
			
			Assert.IsNotNull(dispatcher.MethodInvoked);
		}
		
		[Test]
		public void Write_DispatcherCheckAccessReturnsFalse_WriteMethodIsInvokedWithTextAsArgument()
		{
			dispatcher.CheckAccessReturnValue = false;
			
			byte[] bytes = UTF8Encoding.UTF8.GetBytes("test");
			stream.Write(bytes, 0, bytes.Length);
			
			object[] expectedArgs = new object[] { "test" };
			
			Assert.AreEqual(expectedArgs, dispatcher.MethodInvokedArgs);
		}
	}
}
