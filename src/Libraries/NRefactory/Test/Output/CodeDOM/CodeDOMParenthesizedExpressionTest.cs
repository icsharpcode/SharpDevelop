/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 02.12.2004
 * Time: 12:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.CodeDom;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.Output.CodeDOM.Tests
{
	[TestFixture]
	public class CodeDOMParenthesizedExpressionTest
	{
		[Test]
		public void TestParenthesizedExpression()
		{
			object output = new ParenthesizedExpression(Expression.Null).AcceptVisitor(new CodeDOMVisitor(), null);
			Assert.AreEqual(output, Expression.Null);
		}
	}
}
