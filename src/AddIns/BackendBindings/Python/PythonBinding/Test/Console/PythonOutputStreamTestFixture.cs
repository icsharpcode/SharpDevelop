// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;

using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	[TestFixture]
	public class PythonOutputStreamTestFixture
	{
		PythonOutputStream stream;
		MockTextEditor textEditor;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditor = new MockTextEditor();
			stream = new PythonOutputStream(textEditor);
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
