// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
