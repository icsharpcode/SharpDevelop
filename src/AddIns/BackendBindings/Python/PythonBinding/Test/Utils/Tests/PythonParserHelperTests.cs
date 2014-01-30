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
