// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
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
			PythonAst ast = parser.CreateAst(@"snippet.py", code);
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
