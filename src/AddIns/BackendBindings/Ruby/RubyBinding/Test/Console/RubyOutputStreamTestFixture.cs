// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	[TestFixture]
	public class RubyOutputStreamTestFixture
	{
		RubyOutputStream stream;
		MockConsoleTextEditor textEditor;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditor = new MockConsoleTextEditor();
			stream = new RubyOutputStream(textEditor);
		}
		
		[Test]
		public void CanReadIsFalse()
		{
			Assert.IsFalse(stream.CanRead);
		}

		[Test]
		public void CanSeekIsFalse()
		{
			Assert.IsFalse(stream.CanSeek);
		}

		[Test]
		public void CanWriteIsTrue()
		{
			Assert.IsTrue(stream.CanWrite);
		}
		
		[Test]
		public void WriteAddsTextToTextEditor()
		{
			textEditor.Text = String.Empty;
			byte[] bytes = UTF8Encoding.UTF8.GetBytes("test");
			stream.Write(bytes, 0, bytes.Length);
			
			Assert.AreEqual("test", textEditor.Text);
		}
		
		[Test]
		public void OffsetAndLengthUsedInWriteMethod()
		{
			textEditor.Text = String.Empty;
			byte[] bytes = UTF8Encoding.UTF8.GetBytes("0output1");
			stream.Write(bytes, 1, bytes.Length - 2);
			
			Assert.AreEqual("output", textEditor.Text);
		}		
	}
}
