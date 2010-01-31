// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
