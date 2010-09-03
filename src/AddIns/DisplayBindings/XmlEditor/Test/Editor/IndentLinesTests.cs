// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class IndentLinesTests
	{
		XmlFormattingStrategy formattingStrategy;
		MockTextEditor textEditor;
		MockTextEditorOptions options;
		AvalonEditDocumentAdapter document;
		TextDocument textDocument;
		
		[SetUp]
		public void Init()
		{
			formattingStrategy = new XmlFormattingStrategy();
			
			options = new MockTextEditorOptions();
			textEditor = new MockTextEditor();
			textEditor.Options = options;
			
			textDocument = new TextDocument();
			document = new AvalonEditDocumentAdapter(textDocument, null);
			textEditor.SetDocument(document);
		}
		
		[Test]
		public void IndentLinesIndentsChildElement()
		{
			document.Text =
				"<root>\r\n" +
				"<child>\r\n" +
				"</child>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"\t<child>\r\n" +
				"\t</child>\r\n" +
				"</root>";
				
			formattingStrategy.IndentLines(textEditor, 1, 4);
			
			Assert.AreEqual(expectedText, document.Text);
		}
		
		public void IndentLinesIndentsChildElementUsingTextEditorIndentationString()
		{
			document.Text =
				"<root>\r\n" +
				"<child>\r\n" +
				"</child>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"    <child>\r\n" +
				"    </child>\r\n" +
				"</root>";
				
			MockTextEditorOptions options = new MockTextEditorOptions();
			options.IndentationString = "    ";
			textEditor.Options = options;
			
			formattingStrategy.IndentLines(textEditor, 1, 4);
			
			Assert.AreEqual(expectedText, document.Text);
		}
		
		[Test]
		public void IndentsChildElementAttributesOnOtherLine()
		{
			document.Text =
				"<root>\r\n" +
				"<child\r\n" +
				"attribute1='1'\r\n" +
				"attribute2='2'>\r\n" +
				"</child>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"\t<child\r\n" +
				"\t\tattribute1='1'\r\n" +
				"\t\tattribute2='2'>\r\n" +
				"\t</child>\r\n" +
				"</root>";
				
			formattingStrategy.IndentLines(textEditor, 1, 6);
			
			Assert.AreEqual(expectedText, document.Text, document.Text);
		}
		
		[Test]
		public void IndentsChildElementAttributeWithSpacesWhenFirstAttributeOnStartElementLine()
		{
			document.Text =
				"<root>\r\n" +
				"<child attribute1='1'\r\n" +
				"attribute2='2'\r\n" +
				"attribute3='3'>\r\n" +
				"</child>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"\t<child attribute1='1'\r\n" +
				"\t       attribute2='2'\r\n" +
				"\t       attribute3='3'>\r\n" +
				"\t</child>\r\n" +
				"</root>";
				
			formattingStrategy.IndentLines(textEditor, 1, 6);
			
			Assert.AreEqual(expectedText, document.Text, document.Text);
		}
		
		[Test]
		public void IndentsChildElementAttributeWithTabsWhenChildElementNameIsLargerThan16Chars()
		{
			document.Text =
				"<root>\r\n" +
				"<verylongchildelementname attribute1='1'\r\n" +
				"attribute2='2'\r\n" +
				"attribute3='3'>\r\n" +
				"</verylongchildelementname>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"\t<verylongchildelementname attribute1='1'\r\n" +
				"\t\tattribute2='2'\r\n" +
				"\t\tattribute3='3'>\r\n" +
				"\t</verylongchildelementname>\r\n" +
				"</root>";
				
			formattingStrategy.IndentLines(textEditor, 1, 6);
			
			Assert.AreEqual(expectedText, document.Text, document.Text);
		}
		
		[Test]
		public void SingleCommentLineIsIndented()
		{
			document.Text =
				"<root>\r\n" +
				"<!-- abc -->\r\n" +
				"<child/>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"\t<!-- abc -->\r\n" +
				"\t<child/>\r\n" +
				"</root>";
				
			formattingStrategy.IndentLines(textEditor, 1, 4);
			
			Assert.AreEqual(expectedText, document.Text, document.Text);
		}
		
		[Test]
		public void MultipleLineCommentOnlyHasFirstLineIndented()
		{
			document.Text =
				"<root>\r\n" +
				"<!-- 1st\r\n" +
				"2nd\r\n" +
				"3rd -->\r\n" +
				"<child/>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"\t<!-- 1st\r\n" +
				"2nd\r\n" +
				"3rd -->\r\n" +
				"\t<child/>\r\n" +
				"</root>";
				
			formattingStrategy.IndentLines(textEditor, 1, 6);
			
			Assert.AreEqual(expectedText, document.Text, document.Text);
		}
		
		[Test]
		public void SingleCDataIsIndented()
		{
			document.Text =
				"<root>\r\n" +
				"<![CDATA[ abc ]]>\r\n" +
				"<child/>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"\t<![CDATA[ abc ]]>\r\n" +
				"\t<child/>\r\n" +
				"</root>";
				
			formattingStrategy.IndentLines(textEditor, 1, 4);
			
			Assert.AreEqual(expectedText, document.Text, document.Text);
		}
		
		[Test]
		public void MultipleLineCDataOnlyHasFirstLineIndented()
		{
			document.Text =
				"<root>\r\n" +
				"<![CDATA[ 1st\r\n" +
				"2nd\r\n" +
				"3rd ]]>\r\n" +
				"<child/>\r\n" +
				"</root>";
			
			string expectedText = 
				"<root>\r\n" +
				"\t<![CDATA[ 1st\r\n" +
				"2nd\r\n" +
				"3rd ]]>\r\n" +
				"\t<child/>\r\n" +
				"</root>";
				
			formattingStrategy.IndentLines(textEditor, 1, 6);
			
			Assert.AreEqual(expectedText, document.Text, document.Text);
		}
	}
}
