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
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class WixDocumentEditorInsertTextTestFixture
	{
		IDocument document;
		MockTextEditor textEditor;
		string originalXml;
		
		[SetUp]
		public void Init()
		{
			originalXml = 
				"<root>\r\n" +
				"\t<child>\r\n" +
				"\t</child>\r\n" +
				"</root>";
			
			textEditor = new MockTextEditor();
			textEditor.OptionsConvertTabsToSpaces = false;
			textEditor.OptionsIndentationSize = 4;
			textEditor.OptionsIndentationString = "\t";

			textEditor.Document.Text = originalXml;
					
			// Insert new xml as child element of <child>.
			// Insert position is just before the start of </root> end element.
			WixDocumentEditor editor = new WixDocumentEditor(textEditor);
			
			string xmlToInsert = 
				"<new-child>\r\n" +
				"</new-child>\r\n";
			
			int line = 3;
			int column = 2;
			editor.InsertIndented(line, column, xmlToInsert);
			
			document = textEditor.Document;
		}
		
		[Test]
		public void ExpectedDocumentXmlAfterInsert()
		{
			string expectedXml = 
				"<root>\r\n" +
				"\t<child>\r\n" +
				"\t\t<new-child>\r\n" +
				"\t\t</new-child>\r\n" +
				"\t</child>\r\n" +
				"</root>";
				
			Assert.AreEqual(expectedXml, document.Text, document.Text);
		}
		
		[Test]
		public void LocationJumpedToAfterInsert()
		{
			int column = 1;
			int line = 3;
			var expectedLocation = new TextLocation(column, line);
			Assert.AreEqual(expectedLocation, textEditor.LocationJumpedTo);
		}
		
		[Test]
		public void TextInsertedIsSelected()
		{
			string expectedText = 
				"\t<new-child>\r\n" +
				"\t\t</new-child>\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.SelectedText);
		}
		
		[Test]
		public void InsertCanBeUndoneInOneStep()
		{
			textEditor.Undo();
			
			Assert.AreEqual(originalXml, document.Text, document.Text);
		}
	}
}
