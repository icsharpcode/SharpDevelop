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
	public class CodeDOMPrimitiveExpressionsTests
	{
		[Test]
		public void TestPrimitiveExpression()
		{
			object output = new PrimitiveExpression(5, "5").AcceptVisitor(new CodeDomVisitor(), null);
			Assert.IsTrue(output is CodePrimitiveExpression);
			Assert.AreEqual(((CodePrimitiveExpression)output).Value, 5);
		}
	}
}
