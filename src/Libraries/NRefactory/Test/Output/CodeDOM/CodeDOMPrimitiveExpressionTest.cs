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
	public class CodeDOMPrimitiveExpressionsTests
	{
		[Test]
		public void TestPrimitiveExpression()
		{
			object output = new PrimitiveExpression(5, "5").AcceptVisitor(new CodeDOMVisitor(), null);
			Assert.IsTrue(output is CodePrimitiveExpression);
			Assert.AreEqual(((CodePrimitiveExpression)output).Value, 5);
		}
	}
}
