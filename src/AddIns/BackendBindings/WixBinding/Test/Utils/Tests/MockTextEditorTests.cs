// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			Location expectedLocation = new Location(col, line);
		
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
			textEditor.Caret.Position = new Location(col, line);
			
			Location expectedLocation = new Location(col, line);
			Assert.AreEqual(expectedLocation, textEditor.Caret.Position);
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
