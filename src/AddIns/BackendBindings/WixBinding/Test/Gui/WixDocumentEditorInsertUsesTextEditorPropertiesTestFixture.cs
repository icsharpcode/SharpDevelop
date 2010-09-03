// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class WixDocumentEditorInsertUsesTextEditorPropertiesTestFixture
	{
		IDocument document;
		MockTextEditor textEditor;
		string originalXml;
		
		[SetUp]
		public void Init()
		{
			originalXml = 
				"<root>\r\n" +
				"    <child>\r\n" +
				"    </child>\r\n" +
				"</root>";
			
			textEditor = new MockTextEditor();
			textEditor.OptionsConvertTabsToSpaces = true;
			textEditor.OptionsIndentationSize = 4;
			textEditor.OptionsIndentationString = " ";
			
			textEditor.Document.Text = originalXml;
					
			// Insert new xml as child element of <child>.
			// Insert position is just before the start of </root> end element.
			WixDocumentEditor editor = new WixDocumentEditor(textEditor);
			
			string xmlToInsert = 
				"<new-child>\r\n" +
				"</new-child>\r\n";
			
			int line = 3;
			int column = 5;
			editor.InsertIndented(line, column, xmlToInsert);
			
			document = textEditor.Document;
		}
		
		[Test]
		public void ExpectedDocumentXmlAfterInsert()
		{
			string expectedXml = 
				"<root>\r\n" +
				"    <child>\r\n" +
				"        <new-child>\r\n" +
				"        </new-child>\r\n" +
				"    </child>\r\n" +
				"</root>";
				
			Assert.AreEqual(expectedXml, document.Text, document.Text);
		}
		
		[Test]
		public void TextInsertedIsSelected()
		{
			string expectedText = 
				"    <new-child>\r\n" +
				"        </new-child>\r\n" +
				"    ";
			
			Assert.AreEqual(expectedText, textEditor.SelectedText);
		}
	}
}
