// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTextBufferTests
	{
		[Test]
		public void CanGetTextFromTextBufferTextProperty()
		{
			string expectedText = "abc";
			MockTextBuffer textBuffer = new MockTextBuffer(expectedText);
			Assert.AreEqual(expectedText, textBuffer.Text);
		}
		
		[Test]
		public void CanGetTextFromReaderReturnedFromTextBufferCreateReader()
		{
			string expectedText = "abc";
			MockTextBuffer textBuffer = new MockTextBuffer("abc");
			
			StringBuilder text = new StringBuilder();
			using (TextReader reader = textBuffer.CreateReader()) {
				Assert.AreEqual(expectedText, reader.ReadToEnd());
			}
		}
	}
}
