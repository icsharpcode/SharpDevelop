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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class WixDocumentEditorReplaceTextTestFixture
	{
		IDocument document;
		MockTextEditor textEditor;
		string originalXml;
		string initialDocumentRegionText;
		
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
			
			document = textEditor.Document;

			textEditor.Document.Text = originalXml;
					
			// Replace the <child> element
			WixDocumentEditor editor = new WixDocumentEditor(textEditor);
			
			string xmlToInsert = 
				"<new-child>\r\n" +
				"</new-child>";
			
			int line = 2;
			int column = 2;
			int endLine = 3;
			
			// End column is the column containing the '>' of the </child> element.
			int endColumn = 9;
			
			var region = new DomRegion(line, column, endLine, endColumn);
			
			WixDocumentLineSegment lineSegment = WixDocumentLineSegment.ConvertRegionToSegment(textEditor.Document, region);
			initialDocumentRegionText = textEditor.Document.GetText(lineSegment.Offset, lineSegment.Length);

			editor.Replace(region, xmlToInsert);			
		}
		
		[Test]
		public void ExpectedDocumentXmlAfterReplace()
		{
			string expectedXml = 
				"<root>\r\n" +
				"\t<new-child>\r\n" +
				"\t</new-child>\r\n" +
				"</root>";
				
			Assert.AreEqual(expectedXml, document.Text, document.Text);
		}
		
		[Test]
		public void LocationJumpedToAfterInsert()
		{
			int column = 1;
			int line = 2;
			var expectedLocation = new TextLocation(column, line);
			Assert.AreEqual(expectedLocation, textEditor.LocationJumpedTo);
		}
		
		[Test]
		public void TextInsertedIsSelected()
		{
			string expectedText = 
				"<new-child>\r\n" +
				"\t</new-child>";
			
			Assert.AreEqual(expectedText, textEditor.SelectedText);
		}
		
		[Test]
		public void InsertCanBeUndoneInOneStep()
		{
			textEditor.Undo();
			
			Assert.AreEqual(originalXml, document.Text, document.Text);
		}
		
		[Test]
		public void InitialDocumentTextRegionCoversInnerChildXmlElement()
		{
			string expectedText = 
				"<child>\r\n" +
				"\t</child>";
			
			Assert.AreEqual(expectedText, initialDocumentRegionText);
		}
	}
}
