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
using ICSharpCode.PythonBinding;
using IronPython.Compiler;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class ParseSimpleImportExpressionTestFixture
	{
		PythonExpression expression;
		
		[SetUp]
		public void Init()
		{
			string code = "import";
			ScriptEngine engine = Python.CreateEngine();
			expression = new PythonExpression(engine, code);
		}
		
		[Test]
		public void FirstTokenIsImportKeyword()
		{
			Token token = expression.GetNextToken();
			Assert.AreEqual(TokenKind.KeywordImport, token.Kind);
		}
		
		[Test]
		public void SecondTokenIsEndOfFile()
		{
			expression.GetNextToken();
			Token token = expression.GetNextToken();
			Assert.AreEqual(TokenKind.EndOfFile, token.Kind);
		}
		
		[Test]
		public void CurrentTokenIsNull()
		{
			Assert.IsNull(expression.CurrentToken);
		}
		
		[Test]
		public void GetNextTokenSetsCurrentTokenToKeywordImport()
		{
			expression.GetNextToken();
			Assert.AreEqual(TokenKind.KeywordImport, expression.CurrentToken.Kind);
		}
	}
}
