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
using ICSharpCode.NRefactory;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTextEditorTests
	{
		MockTextEditor textEditor;
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
		}
		
		[Test]
		public void LocationJumpedToIsSaved()
		{
			int line = 1;
			int col = 2;

			textEditor.JumpTo(line, col);
			
			var expectedLocation = new TextLocation(col, line);
		
			Assert.AreEqual(expectedLocation, textEditor.LocationJumpedTo);
		}
		
		[Test]
		public void TextCanBeSelectedInTextEditor()
		{
			int offset = 1;
			int length = 2;
			textEditor.Document.Text = "test";
			textEditor.Select(offset, length);
			
			Assert.AreEqual("es", textEditor.SelectedText);
		}
		
		[Test]
		public void NoTextSelectedWhenTextEditorFirstCreated()
		{
			Assert.AreEqual(String.Empty, textEditor.SelectedText);
		}
		
		[Test]
		public void CursorPositionInTextEditorIsSaved()
		{
			int col = 2;
			int line = 1;
			textEditor.Document.Text = "abc\r\ndef\r\nghi";
			textEditor.Caret.Location = new TextLocation(col, line);
			
			var expectedLocation = new TextLocation(col, line);
			Assert.AreEqual(expectedLocation, textEditor.Caret.Location);
		}
		
		[Test]
		public void TextEditorOptionsIndentationStringIsSaved()
		{
			textEditor.OptionsIndentationString = " ";
			Assert.AreEqual(" ", textEditor.Options.IndentationString);
		}
		
		[Test]
		public void TextEditorOptionsIndentationSizeIsSaved()
		{
			textEditor.OptionsIndentationSize = 2;
			Assert.AreEqual(2, textEditor.Options.IndentationSize);
		}

		[Test]
		public void TextEditorOptionsConvertTabsToSpacesIsSaved()
		{
			textEditor.OptionsConvertTabsToSpaces = true;
			Assert.IsTrue(textEditor.Options.ConvertTabsToSpaces);
		}
		
		[Test]
		public void DefaultTextEditorOptionsIndentationStringIsSingleTabCharacter()
		{
			Assert.AreEqual("\t", textEditor.Options.IndentationString);
		}
		
		[Test]
		public void DefaultTextEditorOptionsIndentationSizeIsFour()
		{
			Assert.AreEqual(4, textEditor.Options.IndentationSize);
		}
		
		[Test]
		public void DefaultTextEditorOptionsConvertTabsToSpacesIsFalse()
		{
			Assert.IsFalse(textEditor.Options.ConvertTabsToSpaces);
		}
		
		[Test]
		public void TextCanBeReplacedInTextEditorDocument()
		{
			textEditor.Document.Text = "abc-def-ghi";
			
			int offset = 4;
			int lengthOfTextToReplace = 3;
			string newText = "replaced";
			textEditor.Document.Replace(offset, lengthOfTextToReplace, newText);
			
			Assert.AreEqual("abc-replaced-ghi", textEditor.Document.Text);
		}
		
		[Test]
		public void NoTextSelectedAfterJumpToMethodIsCalled()
		{
			int offset = 1;
			int length = 2;
			textEditor.Document.Text = "test\r\nsecond line";
			textEditor.Select(offset, length);
			textEditor.JumpTo(2, 1);
			
			Assert.AreEqual(String.Empty, textEditor.SelectedText);
		}
	}
}
