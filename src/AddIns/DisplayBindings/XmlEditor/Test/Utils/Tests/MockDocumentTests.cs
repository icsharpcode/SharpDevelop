// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
