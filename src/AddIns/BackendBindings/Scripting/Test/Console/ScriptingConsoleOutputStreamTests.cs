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
