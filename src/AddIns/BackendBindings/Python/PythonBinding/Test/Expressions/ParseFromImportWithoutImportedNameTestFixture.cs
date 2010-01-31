// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
