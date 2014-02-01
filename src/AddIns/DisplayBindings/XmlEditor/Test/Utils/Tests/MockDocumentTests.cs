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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockDocumentTests
	{
		MockDocument document;
		
		[SetUp]
		public void Init()
		{
			document = new MockDocument();
		}
		
		[Test]
		public void TextEditorDocumentCanGetOffsetAndLengthUsedAsParametersInGetTextMethod()
		{
			TextSection expectedSection = new TextSection(0, 5);
			
			document.Text = "abcdefghi";
			document.GetText(0, 5);
			
			Assert.AreEqual(expectedSection, document.GetTextSectionUsedWithGetTextMethod());
		}
		
		[Test]
		public void TextEditorDocumentGetTextReturnsCorrectSectionOfText()
		{
			TextSection expectedSection = new TextSection(0, 5);
			
			MockDocument document = new MockDocument();
			document.Text = "abcdefghi";
			
			Assert.AreEqual("def", document.GetText(3, 3));
		}
		
		[Test]
		public void TextSectionWithSameOffsetAndLengthAreEqual()
		{
			TextSection lhs = new TextSection(0, 1);
			TextSection rhs = new TextSection(0, 1);
			
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void TextSectionWithDifferentOffsetAreNotEqual()
		{
			TextSection lhs = new TextSection(0, 1);
			TextSection rhs = new TextSection(1, 1);
			
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void TextSectionWithDifferentLengthAreNotEqual()
		{
			TextSection lhs = new TextSection(0, 15);
			TextSection rhs = new TextSection(0, 1);
			
			Assert.IsFalse(lhs.Equals(rhs));
		}
	}
}
