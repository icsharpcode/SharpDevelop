// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests
{
	/// <summary>
	/// Tests that the RubyFormattingStrategy indents the new line added after method and class definitions.
	/// </summary>
	[TestFixture]
	public class RubyNewMethodIndentationTestFixture
	{
		TextEditorControl textEditor;
		RubyFormattingStrategy formattingStrategy;
		
		[SetUp]
		public void Init()
		{
			MockTextEditorProperties textEditorProperties = new MockTextEditorProperties();
			textEditorProperties.IndentStyle = IndentStyle.Smart;
			textEditorProperties.TabIndent = 4;
			textEditor = new TextEditorControl();
			textEditor.TextEditorProperties = textEditorProperties;
			formattingStrategy = new RubyFormattingStrategy();
		}
		
		[TearDown]
		public void TearDown()
		{
			textEditor.Dispose();
		}
		
		[Test]
		public void NewMethodDefinition()
		{
			textEditor.Text =
				"def newMethod\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"def newMethod\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void NewMethodDefinitionWithBrackets()
		{
			textEditor.Text =
				"def newMethod()\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"def newMethod()\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void NewClassDefinition()
		{
			textEditor.Text =
				"class MyClass\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"class MyClass\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void NoExtraIndentationRequired()
		{
			textEditor.Text =
				"\tprint 'abc'\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"\tprint 'abc'\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void ReturnValueStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text =
				"def method1\r\n" +
				"\treturn 0\r\n" +
				"";
			
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
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
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"if i > 0 then\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void IfStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"if i > 0\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"if i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void ElseStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"else\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"else\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void ElseIfStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"elsif i > 0\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"elsif i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void LoopStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"loop do\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"loop do\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void UnlessStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"unless i > 0\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"unless i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void UntilStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"until i > 0\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"until i > 0\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void ForStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"for i in 1..5\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"for i in 1..5\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void DoStatementAtEndOfLineIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"expr do\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"expr do\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void OpenCurlyBraceAtEndOfLineIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"expr {\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"expr {\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void BeginStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"begin\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"begin\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void RescueStatementWithExceptionIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"rescue Exception => ex\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"rescue Exception => ex\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void RescueStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"rescue\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"rescue\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void EnsureStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"ensure\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"ensure\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void ModuleStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"module Foo\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"module Foo\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void CaseWhenStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"case num\r\n" +
				"\twhen 0\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText =
				"case num\r\n" +
				"\twhen 0\r\n" +
				"\t\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(2, indentResult);
		}
		
		[Test]
		public void CaseStatementIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"case num\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"case num\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void CaseStatementInMiddleOfLineIncreasesIndentOnNextLine()
		{
			textEditor.Text =
				"value = case num\r\n" +
				"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText =
				"value = case num\r\n" +
				"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
	}
}
