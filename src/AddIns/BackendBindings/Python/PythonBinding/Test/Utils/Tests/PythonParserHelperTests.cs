// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class PythonParserHelperTests
	{
		[Test]
		public void CreateParseInfoReturnsParseInfoWithSingleClass()
		{
			string code =
				"class foo:\r\n" +
				"    pass";
			
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			Assert.AreEqual("foo", parseInfo.CompilationUnit.Classes[0].Name);
		}
		
		[Test]
		public void GetAssignmentStatementReturnsFirstAssignmentStatementInCode()
		{
			string code =
				"i = 10";
			
			AssignmentStatement assignment = PythonParserHelper.GetAssignmentStatement(code);
			NameExpression nameExpression = assignment.Left[0] as NameExpression;
			
			Assert.AreEqual("i", nameExpression.Name);
		}
		
		[Test]
		public void GetCallExpressionReturnsFirstCallStatementInCode()
		{
			string code =
				"run()";
			
			CallExpression call = PythonParserHelper.GetCallExpression(code);
			NameExpression nameExpression = call.Target as NameExpression;
			
			Assert.AreEqual("run", nameExpression.Name);
		}
	}
}
