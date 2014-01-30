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
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Gui
{
	[TestFixture]
	public class RubyFormattingStrategyTests
	{
		TextEditor textEditor;
		RubyFormattingStrategy formattingStrategy;
		AvalonEditTextEditorAdapter textEditorAdapter;
		
		void CreateFormattingStrategy()
		{
			MockTextEditorOptions textEditorOptions = new MockTextEditorOptions();
			textEditorOptions.IndentationSize = 4;
			textEditor = new TextEditor();
			textEditor.Options = textEditorOptions;
			
			textEditorAdapter = new AvalonEditTextEditorAdapter(textEditor);
			
			formattingStrategy = new RubyFormattingStrategy();
		}
		
		[Test]
		public void IndentLine_NewMethodDefinitionOnPreviousLine_NextLineIsIndented()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def newMethod\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"def newMethod\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_NewMethodDefinitionWithBracketsOnPreviousLine_NextLineIsIndented()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def newMethod()\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"def newMethod()\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_NewClassDefinitionOnPreviousLine_NextLineIsIndented()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"class MyClass\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"class MyClass\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_PrintStatementOnPreviousLineSoNoExtraIndentationRequired_NextLineIndentedToSameLevelAsPreviousLine()
		{
			CreateFormattingStrategy();
			
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
		public void IndentLine_ReturnValueStatementOnPreviousLine_DecreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def method1\r\n" +
				"\treturn 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"def method1\r\n" +
				"\treturn 0\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}

		[Test]
		public void IndentLine_ReturnStatementOnPreviousLine_DecreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def method1\r\n" +
				"\treturn\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;

			string expectedText =
				"def method1\r\n" +
				"\treturn\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_ReturnStatementOnPreviousLineWithNoIndentOnPreviousLine_NextLineIsNotIndented()
		{
			CreateFormattingStrategy();
			
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
		public void IndentLine_StatementIsNotReturnOnPreviousLine_NextLineIndentedToSameLevelAsPreviousLine()
		{
			CreateFormattingStrategy();
			
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
		public void IndentLine_RaiseStatementWithObjectOnPreviousLine_DecreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def method1\r\n" +
				"\traise 'a'\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;

			string expectedText =
				"def method1\r\n" +
				"\traise 'a'\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_RaiseStatementOnPreviousLine_DecreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def method1\r\n" +
				"\traise\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;

			string expectedText =
				"def method1\r\n" +
				"\traise\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_StatementIsNotRaiseStatementOnPreviousLine_NextLineIsIndentedToSameLevelAsPreviousLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def method1\r\n" +
				"\traiseThis\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;

			string expectedText =
				"def method1\r\n" +
				"\traiseThis\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_BreakStatementOnPreviousLine_DecreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def method1\r\n" +
				"\tbreak\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;

			string expectedText =
				"def method1\r\n" +
				"\tbreak\r\n" +
				"";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_StatementIsNotBreakStatementOnPreviousLine_LineIsIndentedToSameLevelAsPreviousLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"def method1\r\n" +
				"\tbreakThis\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;

			string expectedText =
				"def method1\r\n" +
				"\tbreakThis\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_IfThenStatementOnPreviousLine_IndentIncreasedOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"if i > 0 then\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;

			string expectedText =
				"if i > 0 then\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_IfStatementOnPreviousLine_IndentIncreasedOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"if i > 0\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;

			string expectedText =
				"if i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_ElseStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"else\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"else\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_ElseIfStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"elsif i > 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"elsif i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_LoopStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();

			textEditor.Text =
				"loop do\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"loop do\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_UnlessStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();

			textEditor.Text =
				"unless i > 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"unless i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_UntilStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();

			textEditor.Text =
				"until i > 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"until i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_ForStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();

			textEditor.Text =
				"for i in 1..5\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"for i in 1..5\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_DoStatementAtEndOfPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"expr do\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"expr do\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_OpenCurlyBraceAtEndOfPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"expr {\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"expr {\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_BeginStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"begin\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"begin\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_RescueStatementWithExceptionOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"rescue Exception => ex\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"rescue Exception => ex\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_RescueStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"rescue\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"rescue\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_EnsureStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"ensure\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"ensure\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_ModuleStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"module Foo\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"module Foo\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_CaseWhenStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"case num\r\n" +
				"\twhen 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"case num\r\n" +
				"\twhen 0\r\n" +
				"\t\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_CaseStatementOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"case num\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"case num\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void IndentLine_CaseStatementInMiddleOnPreviousLine_IncreasesIndentOnNextLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text =
				"value = case num\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			string text = textEditor.Text;
			
			string expectedText =
				"value = case num\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void SurroundSelectionWithComment_CursorOnFirstLineNothingSelected_CommentsFirstLine()
		{
			CreateFormattingStrategy();
			
			textEditor.Text = "print 'hello'";
			formattingStrategy.SurroundSelectionWithComment(textEditorAdapter);
			string text = textEditor.Text;
			
			string expectedText = "#print 'hello'";
			
			Assert.AreEqual(expectedText, text);
		}
	}
}
