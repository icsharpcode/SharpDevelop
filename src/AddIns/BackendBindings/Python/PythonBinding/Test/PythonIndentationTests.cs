// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Indentation
{
	/// <summary>
	/// Tests that the PythonFormattingStrategy indents the new line added after pressing the ':' character.
	/// </summary>
	[TestFixture]
	public class PythonNewMethodIndentationTestFixture
	{
		TextEditorControl textEditor;
		PythonFormattingStrategy formattingStrategy;
		
		[SetUp]
		public void Init()
		{
			MockTextEditorProperties textEditorProperties = new MockTextEditorProperties();
			textEditorProperties.IndentStyle = IndentStyle.Smart;
			textEditorProperties.TabIndent = 4;
			textEditor = new TextEditorControl();
			textEditor.TextEditorProperties = textEditorProperties;
			formattingStrategy = new PythonFormattingStrategy();
		}
		
		[TearDown]
		public void TearDown()
		{
			textEditor.Dispose();
		}
		
		[Test]
		public void NewMethodDefinition()
		{
			textEditor.Text = "def newMethod:\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText = "def newMethod:\r\n" +
								"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void NoExtraIndentationRequired()
		{
			textEditor.Text = "\tprint 'abc'\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText = "\tprint 'abc'\r\n" +
								"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
			Assert.AreEqual(1, indentResult);
		}
		
		[Test]
		public void PassStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = "def method1:\r\n" +
								"\tpass\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText = "def method1:\r\n" +
								"\tpass\r\n" +
								"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void ReturnValueStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = "def method1:\r\n" +
								"\treturn 0\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText = "def method1:\r\n" +
								"\treturn 0\r\n" +
								"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}

		[Test]
		public void ReturnStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = "def method1:\r\n" +
								"\treturn\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText = "def method1:\r\n" +
								"\treturn\r\n" +
								"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}		
		[Test]
		public void ReturnStatementWithNoIndentOnPreviousLine()
		{
			textEditor.Text = "return\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText = "return\r\n" +
								"";
			
			Assert.AreEqual(expectedText, textEditor.Text);			
		}
		
		[Test]
		public void StatementIsNotAReturnOnPreviousLine()
		{
			textEditor.Text = "\treturnValue\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 1);
			string expectedText = "\treturnValue\r\n" +
								"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);			
		}
		
		[Test]
		public void RaiseStatementWithObjectDecreasesIndentOnThirdLine()
		{
			textEditor.Text = "def method1:\r\n" +
								"\traise 'a'\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText = "def method1:\r\n" +
								"\traise 'a'\r\n" +
								"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void RaiseStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = "def method1:\r\n" +
								"\traise\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText = "def method1:\r\n" +
								"\traise\r\n" +
								"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void StatementIsNotARaiseStatementOnPreviousLine()
		{
			textEditor.Text = "def method1:\r\n" +
								"\traiseThis\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText = "def method1:\r\n" +
								"\traiseThis\r\n" +
								"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void BreakStatementDecreasesIndentOnThirdLine()
		{
			textEditor.Text = "def method1:\r\n" +
								"\tbreak\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText = "def method1:\r\n" +
								"\tbreak\r\n" +
								"";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void StatementIsNotABreakStatementOnPreviousLine()
		{
			textEditor.Text = "def method1:\r\n" +
								"\tbreakThis\r\n" +
								"";
			int indentResult = formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 2);
			string expectedText = "def method1:\r\n" +
								"\tbreakThis\r\n" +
								"\t";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void IndentingFirstLineDoesNotThrowArgumentOutOfRangeException()
		{
			textEditor.Text = "print 'hello'";
			
			int indentResult = -1;
			Assert.DoesNotThrow(delegate { indentResult =
				formattingStrategy.IndentLine(textEditor.ActiveTextAreaControl.TextArea, 0); });
			
			Assert.AreEqual("print 'hello'", textEditor.Text);
		}
	}
}
