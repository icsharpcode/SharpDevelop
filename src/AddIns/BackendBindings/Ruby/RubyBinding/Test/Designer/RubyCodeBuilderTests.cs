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
