// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Tests that the PythonFormattingStrategy indents the new line added after pressing the ':' character.
	/// </summary>
	[TestFixture]
	public class PythonIndentationTestFixture
	{
		TextEditor textEditor;
		AvalonEditTextEditorAdapter textEditorAdapter;
		PythonFormattingStrategy formattingStrategy;
		
		[SetUp]
		public void Init()
		{
			MockTextEditorOptions textEditorOptions = new MockTextEditorOptions();
			textEditorOptions.IndentationSize = 4;
			textEditor = new TextEditor();
			textEditor.Options = textEditorOptions;
			
			textEditorAdapter = new AvalonEditTextEditorAdapter(textEditor);
			formattingStrategy = new PythonFormattingStrategy();
		}
		
		[Test]
		public void NewMethodDefinition()
		{
			textEditor.Text =
				"def newMethod:\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"def newMethod:\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void NoExtraIndentationRequired()
		{
			textEditor.Text = 
				"\tprint 'abc'\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"\tprint 'abc'\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void PassStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = 
				"def method1:\r\n" +
				"\tpass\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"def method1:\r\n" +
				"\tpass\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void ReturnValueStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = 
				"def method1:\r\n" +
				"\treturn 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"def method1:\r\n" +
				"\treturn 0\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}

		[Test]
		public void ReturnStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = 
				"def method1:\r\n" +
				"\treturn\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"def method1:\r\n" +	
				"\treturn\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}		
		[Test]
		public void ReturnStatementWithNoIndentOnPreviousLine()
		{
			textEditor.Text = 
				"return\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"return\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);			
		}
		
		[Test]
		public void StatementIsNotAReturnOnPreviousLine()
		{
			textEditor.Text = 
				"\treturnValue\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"\treturnValue\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);			
		}
		
		[Test]
		public void RaiseStatementWithObjectDecreasesIndentOnThirdLine()
		{
			textEditor.Text = 
				"def method1:\r\n" +
				"\traise 'a'\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"def method1:\r\n" +
				"\traise 'a'\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void RaiseStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text =
				"def method1:\r\n" +
				"\traise\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"def method1:\r\n" +
				"\traise\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void StatementIsNotARaiseStatementOnPreviousLine()
		{
			textEditor.Text = 
				"def method1:\r\n" +
				"\traiseThis\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"def method1:\r\n" +
				"\traiseThis\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void BreakStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = 
				"def method1:\r\n" +
				"\tbreak\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"def method1:\r\n" +
				"\tbreak\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void StatementIsNotABreakStatementOnPreviousLine()
		{
			textEditor.Text = 
				"def method1:\r\n" +
				"\tbreakThis\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText = 
				"def method1:\r\n" +
				"\tbreakThis\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void IndentingFirstLineDoesNotThrowArgumentOutOfRangeException()
		{
			textEditor.Text = "print 'hello'";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(1);
			
			Assert.DoesNotThrow(delegate { 
				formattingStrategy.IndentLine(textEditorAdapter, line); });
			
			Assert.AreEqual("print 'hello'", textEditor.Text);
		}
	}
}
