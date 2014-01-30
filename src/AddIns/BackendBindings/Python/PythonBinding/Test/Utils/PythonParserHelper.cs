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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;

namespace PythonBinding.Tests.Utils
{
	public static class PythonParserHelper
	{
		/// <summary>
		/// Parses the code and returns the first statement as an assignment statement.
		/// </summary>
		public static AssignmentStatement GetAssignmentStatement(string code)
		{
			return GetFirstStatement(code) as AssignmentStatement;
		}
	
		/// <summary>
		/// Parses the code and returns the first statement's expression as call expression.
		/// </summary>
		public static CallExpression GetCallExpression(string code)
		{
			ExpressionStatement expressionStatement = GetFirstStatement(code) as ExpressionStatement;
			return expressionStatement.Expression as CallExpression;
		}
		
		static Statement GetFirstStatement(string code)
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"snippet.py", new StringTextBuffer(code));
			SuiteStatement suiteStatement = (SuiteStatement)ast.Body;
			return suiteStatement.Statements[0];
		}
		
		public static ParseInformation CreateParseInfo(string code)
		{
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			ICompilationUnit compilationUnit = parser.Parse(projectContent, @"C:\test.py", code);
			return new ParseInformation(compilationUnit);
		}
	}
}
