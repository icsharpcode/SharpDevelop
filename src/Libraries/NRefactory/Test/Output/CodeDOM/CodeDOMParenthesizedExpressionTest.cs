// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.NRefactory.Tests.Output.CodeDOM.Tests
{
	[TestFixture]
	public class CodeDOMParenthesizedExpressionTest
	{
		[Test]
		public void TestParenthesizedExpression()
		{
			object output = new ParenthesizedExpression(new PrimitiveExpression(5, "5")).AcceptVisitor(new CodeDomVisitor(), null);
			Assert.IsTrue(output is CodePrimitiveExpression);
		}
	}
}
