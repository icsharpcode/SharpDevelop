// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	/// <summary>
	/// Tests that the RubyFormattingStrategy indents the new line added after method and class definitions.
	/// </summary>
	[TestFixture]
	public class RubyNewMethodIndentationTestFixture
	{
		TextEditor textEditor;
		RubyFormattingStrategy formattingStrategy;
		AvalonEditTextEditorAdapter textEditorAdapter;
		
		[SetUp]
		public void Init()
		{
			MockTextEditorOptions textEditorOptions = new MockTextEditorOptions();
			textEditorOptions.IndentationSize = 4;
			textEditor = new TextEditor();
			textEditor.Options = textEditorOptions;
			
			textEditorAdapter = new AvalonEditTextEditorAdapter(textEditor);
			
			formattingStrategy = new RubyFormattingStrategy();
		}
		
		[Test]
		public void NewMethodDefinition()
		{
			textEditor.Text =
				"def newMethod\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"def newMethod\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void NewMethodDefinitionWithBrackets()
		{
			textEditor.Text =
				"def newMethod()\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"def newMethod()\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void NewClassDefinition()
		{
			textEditor.Text =
				"class MyClass\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"class MyClass\r\n" +
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
		public void ReturnValueStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text =
				"def method1\r\n" +
				"\treturn 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"def method1\r\n" +
				"\treturn 0\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}

		[Test]
		public void ReturnStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text =
				"def method1\r\n" +
				"\treturn\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);

			string expectedText =
				"def method1\r\n" +
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
				"def method1\r\n" +
				"\traise 'a'\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);

			string expectedText =
				"def method1\r\n" +
				"\traise 'a'\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void RaiseStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text =
				"def method1\r\n" +
				"\traise\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);

			string expectedText =
				"def method1\r\n" +
				"\traise\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void StatementIsNotARaiseStatementOnPreviousLine()
		{
			textEditor.Text =
				"def method1\r\n" +
				"\traiseThis\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);

			string expectedText =
				"def method1\r\n" +
				"\traiseThis\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void BreakStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text =
				"def method1\r\n" +
				"\tbreak\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);

			string expectedText =
				"def method1\r\n" +
				"\tbreak\r\n" +
				"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void StatementIsNotABreakStatementOnPreviousLine()
		{
			textEditor.Text =
				"def method1\r\n" +
				"\tbreakThis\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);

			string expectedText =
				"def method1\r\n" +
				"\tbreakThis\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void IfThenStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"if i > 0 then\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);

			string expectedText =
				"if i > 0 then\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void IfStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"if i > 0\r\n" +
				"";

			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);

			string expectedText =
				"if i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void ElseStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"else\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"else\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void ElseIfStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"elsif i > 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"elsif i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void LoopStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"loop do\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"loop do\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void UnlessStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"unless i > 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"unless i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void UntilStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"until i > 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"until i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void ForStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"for i in 1..5\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"for i in 1..5\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void DoStatementAtEndOfLineIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"expr do\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"expr do\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void OpenCurlyBraceAtEndOfLineIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"expr {\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"expr {\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void BeginStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"begin\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"begin\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void RescueStatementWithExceptionIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"rescue Exception => ex\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"rescue Exception => ex\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void RescueStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"rescue\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"rescue\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void EnsureStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"ensure\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"ensure\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void ModuleStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"module Foo\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"module Foo\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void CaseWhenStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"case num\r\n" +
				"\twhen 0\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(3);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"case num\r\n" +
				"\twhen 0\r\n" +
				"\t\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void CaseStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"case num\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"case num\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void CaseStatementInMiddleOfLineIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"value = case num\r\n" +
				"";
			
			IDocumentLine line = textEditorAdapter.Document.GetLine(2);
			formattingStrategy.IndentLine(textEditorAdapter, line);
			
			string expectedText =
				"value = case num\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
	}
}
