// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
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
			
			int line = 1;
			int column = 1;
			int endLine = 2;
			
			// End column is the column containing the '>' of the </child> element.
			int endColumn = 8;
			
			DomRegion region = new DomRegion(line, column, endLine, endColumn);
			
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
			Location expectedLocation = new Location(column, line);
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
		public void CursorAtEndOfSelectedText()
		{
			int col = 14;
			int line = 3;
			Location expectedLocation = new Location(col, line);
			Assert.AreEqual(expectedLocation, textEditor.Caret.Position);
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
