// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class FindExpressionFromImportWithoutImportedNameTestFixture
	{
		ExpressionResult expressionResult;
		
		[SetUp]
		public void Init()
		{
			string code = "from System import ";
			int offset = 19;
			PythonExpressionFinder expressionFinder = new PythonExpressionFinder();
			expressionResult = expressionFinder.FindExpression(code, offset);
		}
		
		[Test]
		public void ExpressionResultContextIsImportExpression()
		{
			Assert.IsNotNull(expressionResult.Context as PythonImportExpressionContext);
		}
		
		[Test]
		public void ExpressionIsFullFromImportStatement()
		{
			Assert.AreSame("from System import ", expressionResult.Expression);
		}
	}
}
