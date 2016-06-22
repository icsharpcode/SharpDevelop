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
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Designer
{
	[TestFixture]
	public class ScriptingCodeBuilderTests
	{
		ScriptingCodeBuilder codeBuilder;
		
		[SetUp]
		public void Init()
		{
			codeBuilder = new ScriptingCodeBuilder();
			codeBuilder.IndentString = "\t";
		}
		
		[Test]
		public void AppendNewLine()
		{
			codeBuilder.AppendLine();
			string text = codeBuilder.ToString();
			Assert.AreEqual("\r\n", text);
		}
		
		[Test]
		public void AppendText()
		{
			codeBuilder.Append("abc");
			string text = codeBuilder.ToString();
			Assert.AreEqual("abc", text);
		}
		
		[Test]
		public void AppendIndentedText()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			string text = codeBuilder.ToString();
			Assert.AreEqual("\tabc", text);
		}
		
		[Test]
		public void IncreaseIndentTwice()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			string text = codeBuilder.ToString();
			Assert.AreEqual("\t\tabc", text);
		}
		
		[Test]
		public void DecreaseIndent()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			codeBuilder.AppendLine();
			codeBuilder.DecreaseIndent();			
			codeBuilder.AppendIndented("abc");
			string text = codeBuilder.ToString();
			Assert.AreEqual("\tabc\r\nabc", text);
		}
		
		[Test]
		public void AppendIndentedLine()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("abc");
			string text = codeBuilder.ToString();
			Assert.AreEqual("\tabc\r\n", text);
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
			codeBuilder = new ScriptingCodeBuilder(2);
			codeBuilder.AppendIndented("abc");
			string text = codeBuilder.ToString();
			Assert.AreEqual("\t\tabc", text);
		}
		
		[Test]
		public void InsertIndentedLine()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("def");
			codeBuilder.InsertIndentedLine("abc");
			string text = codeBuilder.ToString();			
			Assert.AreEqual("\tabc\r\n\tdef\r\n", text);
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
			string text = codeBuilder.ToString();
			Assert.AreEqual("abc # comment\r\n", text);
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
	}
}
