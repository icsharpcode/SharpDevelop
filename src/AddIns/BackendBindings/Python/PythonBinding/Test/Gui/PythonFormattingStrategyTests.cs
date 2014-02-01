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
using ICSharpCode.AvalonEdit;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using NUnit.Framework;

namespace PythonBinding.Tests.Gui
{
	[TestFixture]
	public class PythonFormattingStrategyTests
	{
		TextEditor textEditor;
		AvalonEditTextEditorAdapter textEditorAdapter;
		PythonFormattingStrategy formattingStrategy;
		
		void CreatePythonFormattingStrategy()
		{
			MockTextEditorOptions textEditorOptions = new MockTextEditorOptions();
			textEditorOptions.IndentationSize = 4;
			textEditor = new TextEditor();
			textEditor.Options = textEditorOptions;
			
			textEditorAdapter = new AvalonEditTextEditorAdapter(textEditor);
			formattingStrategy = new PythonFormattingStrategy();
		}
		
				
		[Test]
		public void IndentLine_IndentLineAfterNewMethodDefinition_LineIsIndentedByOneTab()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text =
				"def newMethod:\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);			
			string text = textEditor.Text;
			
			string expectedText = 
				"def newMethod:\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_NoExtraIndentationRequiredAfterPrintLineStatement_SecondLineIsIndentedToSameLevelAsPrintStatementLine()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = 
				"\tprint 'abc'\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText = 
				"\tprint 'abc'\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_IndentAfterPassStatementOnSecondLine_DecreasesIndentOnThirdLine()
		{
			CreatePythonFormattingStrategy();

			textEditor.Text =
				"def method1:\r\n" +
				"\tpass\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"def method1:\r\n" +
				"\tpass\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_IndentAfterReturnValueStatementOnSecondLine_DecreasesIndentOnThirdLine()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = 
				"def method1:\r\n" +
				"\treturn 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText = 
				"def method1:\r\n" +
				"\treturn 0\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}

		[Test]
		public void IndentLine_IndentAfterReturnStatementOnSecondLine_DecreasesIndentOnThirdLine()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text =
				"def method1:\r\n" +
				"\treturn\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText = 
				"def method1:\r\n" +	
				"\treturn\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}		
		[Test]
		public void IndentLine_ReturnStatementWithNoIndentPreviousLine_SecondLineIsNotIndented()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = 
				"return\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText = 
				"return\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);			
		}
		
		[Test]
		public void IndentLine_StatementIsNotReturnOnPreviousLineAndIsIndentedByOneTab_LineIsIndentedByOneTab()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = 
				"\treturnValue\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText = 
				"\treturnValue\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);			
		}
		
		[Test]
		public void IndentLine_RaiseStatementWithObjectOneSecondLine_DecreasesIndentOnThirdLineByOne()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = 
				"def method1:\r\n" +
				"\traise 'a'\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);		
			string text = textEditor.Text;
			
			string expectedText = 
				"def method1:\r\n" +
				"\traise 'a'\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_RaiseStatementOnSecondLine_IndentOnThirdLineIsDecreasedByOne()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text =
				"def method1:\r\n" +
				"\traise\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);		
			string text = textEditor.Text;
			
			string expectedText = 
				"def method1:\r\n" +
				"\traise\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_StatementIsNotRaiseStatementOnPreviousLine_LineIsIndentedToSameLevelAsPreviouisLine()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = 
				"def method1:\r\n" +
				"\traiseThis\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);		
			string text = textEditor.Text;
			
			string expectedText = 
				"def method1:\r\n" +
				"\traiseThis\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_BreakStatementOnSecondLine_DecreasesIndentOnThirdLine()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = 
				"def method1:\r\n" +
				"\tbreak\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);		
			string text = textEditor.Text;

			string expectedText = 
				"def method1:\r\n" +
				"\tbreak\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_StatementIsNotBreakStatementOnPreviousLine_LineIsIndentedToSameLevelAsPreviousLine()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = 
				"def method1:\r\n" +
				"\tbreakThis\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);		
			string text = textEditor.Text;
			
			string expectedText = 
				"def method1:\r\n" +
				"\tbreakThis\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_LineNumberOutOfRange_DoesNotThrowArgumentOutOfRangeException()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = "print 'hello'";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(1);
			Assert.DoesNotThrow(delegate { 
				formattingStrategy.IndentLine(textEditorAdapter, line); });
			string text = textEditor.Text;
			
			string expectedText = "print 'hello'";
		
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void SurroundSelectionWithComment_CursorOnFirstLineAndNothingSelected_FirstLineIsCommented()
		{
			CreatePythonFormattingStrategy();
			
			textEditor.Text = "print 'hello'";
			formattingStrategy.SurroundSelectionWithComment(textEditorAdapter);
			string text = textEditor.Text;
			
			string expectedText = "#print 'hello'";
			
			Assert.AreEqual(expectedText, text);
		}
	}
}
