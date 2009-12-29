// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class RubyCodeBuilderTests
	{
		RubyCodeBuilder codeBuilder;
		
		[SetUp]
		public void Init()
		{
			codeBuilder = new RubyCodeBuilder();
			codeBuilder.IndentString = "\t";
		}
		
		[Test]
		public void AppendNewLine()
		{
			codeBuilder.AppendLine();
			Assert.AreEqual("\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendText()
		{
			codeBuilder.Append("abc");
			Assert.AreEqual("abc", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendIndentedText()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			Assert.AreEqual("\tabc", codeBuilder.ToString());
		}
		
		[Test]
		public void IncreaseIndentTwice()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			Assert.AreEqual("\t\tabc", codeBuilder.ToString());
		}
		
		[Test]
		public void DecreaseIndent()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			codeBuilder.AppendLine();
			codeBuilder.DecreaseIndent();			
			codeBuilder.AppendIndented("abc");
			Assert.AreEqual("\tabc\r\nabc", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendIndentedLine()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("abc");
			Assert.AreEqual("\tabc\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void InitialIndentWhenCodeBuilderCreatedIsZero()
		{
			Assert.AreEqual(0, codeBuilder.Indent);
		}
		
		[Test]
		public void IncreaseIndentByOne()
		{
			codeBuilder.IncreaseIndent();
			Assert.AreEqual(1, codeBuilder.Indent);
		}
		
		[Test]
		public void LengthAfterAddingText()
		{
			codeBuilder.Append("abc");
			Assert.AreEqual(3, codeBuilder.Length);
		}
		
		[Test]
		public void IndentPassedToConstructor()
		{
			codeBuilder = new RubyCodeBuilder(2);
			codeBuilder.AppendIndented("abc");
			Assert.AreEqual("\t\tabc", codeBuilder.ToString());
		}
		
		[Test]
		public void InsertIndentedLine()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("def");
			codeBuilder.InsertIndentedLine("abc");
			Assert.AreEqual("\tabc\r\n\tdef\r\n", codeBuilder.ToString());
		}
		
		/// <summary>
		/// Check that the "self._components = System.ComponentModel.Container()" line is generated
		/// the once and before any other lines of code.
		/// </summary>
		[Test]
		public void AppendCreateComponentsContainerTwice()
		{
			codeBuilder.IndentString = "  ";
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("self._listView = System.Windows.Forms.ListView()");
			codeBuilder.InsertCreateComponentsContainer();
			codeBuilder.InsertCreateComponentsContainer();
			
			string expectedCode = "  self._components = System.ComponentModel.Container()\r\n" +
				"  self._listView = System.Windows.Forms.ListView()\r\n";
			
			Assert.AreEqual(expectedCode, codeBuilder.ToString());
		}
		
		[Test]
		public void PreviousLineIsEmptyNewLine()
		{
			codeBuilder.AppendLine();
			Assert.IsFalse(codeBuilder.PreviousLineIsCode);
		}
		
		[Test]
		public void PreviousLineIsComment()
		{
			codeBuilder.AppendIndentedLine("# comment");
			Assert.IsFalse(codeBuilder.PreviousLineIsCode);
		}
		
		[Test]
		public void PreviousLineIsNotEmptyNewLine()
		{
			codeBuilder.AppendIndentedLine("abc");
			Assert.IsTrue(codeBuilder.PreviousLineIsCode);
		}
		
		[Test]
		public void PreviousLineDoesNotExist()
		{
			Assert.IsFalse(codeBuilder.PreviousLineIsCode);
		}
		
		[Test]
		public void PreviousLineIsEmptyAndCurrentLineHasText()
		{
			codeBuilder.AppendLine();
			codeBuilder.Append("abc");
			Assert.IsFalse(codeBuilder.PreviousLineIsCode);
		}
		
		[Test]
		public void PreviousLineIsMadeUpOfWhiteSpace()
		{
			codeBuilder.AppendIndentedLine("  \t  ");
			Assert.IsFalse(codeBuilder.PreviousLineIsCode);			
		}
		
		[Test]
		public void TwoLinesWithPreviousLineMadeUpOfWhiteSpace()
		{
			codeBuilder.AppendIndentedLine("1st");
			codeBuilder.AppendIndentedLine("  \t  ");
			Assert.IsFalse(codeBuilder.PreviousLineIsCode);			
		}
		
		[Test]
		public void TwoLinesWithPreviousEmptyLine()
		{
			codeBuilder.AppendIndentedLine("1st");
			codeBuilder.AppendLine();
			codeBuilder.Append("abc");
			Assert.IsFalse(codeBuilder.PreviousLineIsCode);			
		}
		
		[Test]
		public void TwoLinesWithNoPreviousEmptyLine()
		{
			codeBuilder.AppendIndentedLine("First");
			codeBuilder.AppendIndentedLine("Second");
			Assert.IsTrue(codeBuilder.PreviousLineIsCode);
		}
		
		[Test]
		public void AppendToPreviousLine()
		{
			codeBuilder.AppendIndentedLine("abc");
			codeBuilder.AppendToPreviousLine(" # comment");
			Assert.AreEqual("abc # comment\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void TrimEnd()
		{
			codeBuilder.Append("abc");
			codeBuilder.AppendLine();
			codeBuilder.Append("def");
			codeBuilder.AppendLine();
			codeBuilder.TrimEnd();
			Assert.AreEqual("abc\r\ndef", codeBuilder.ToString());
		}
		
		[Test]
		public void GetFirstLineOfCode()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("abc");
			Assert.AreEqual("\tabc", codeBuilder.GetPreviousLine());
		}
		
		[Test]
		public void GetPreviousLineReturnsEmptyStringIfNoPreviousLine()
		{
			codeBuilder.AppendIndented("def");
			Assert.AreEqual(String.Empty, codeBuilder.GetPreviousLine());
		}
		
		[Test]
		public void AppendLineIfPreviousLineIsEndStatementAppendsNewLineIfPreviousLineIsEndStatement()
		{
			codeBuilder.AppendIndentedLine("end");
			codeBuilder.AppendLineIfPreviousLineIsEndStatement();
			
			Assert.AreEqual("end\r\n\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendLineIfPreviousLineIsEndStatementeIgnoresCaseOfEndStatement()
		{
			codeBuilder.AppendIndentedLine("END");
			codeBuilder.AppendLineIfPreviousLineIsEndStatement();
			
			Assert.AreEqual("END\r\n\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendLineIfPreviousLineIsEndStatementIgnoresWhitespaceWhenCheckingForEndStatement()
		{
			string line = " \t   end    \t  ";
			codeBuilder.AppendIndentedLine(line);
			codeBuilder.AppendLineIfPreviousLineIsEndStatement();
			
			Assert.AreEqual(line + "\r\n\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendLineIfPreviousLineIsEndStatementDoesNotAppendNewLineIfPreviousLineDoesNotContainEndStatement()
		{
			codeBuilder.AppendIndentedLine("abc");
			codeBuilder.AppendLineIfPreviousLineIsEndStatement();
			
			Assert.AreEqual("abc\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendLineIfPreviousLineIsEndStatementAppendsDoesNotAppendNewLineIfNoPreviousLine()
		{
			codeBuilder.AppendLineIfPreviousLineIsEndStatement();
			Assert.AreEqual(String.Empty, codeBuilder.ToString());
		}
	}
}
