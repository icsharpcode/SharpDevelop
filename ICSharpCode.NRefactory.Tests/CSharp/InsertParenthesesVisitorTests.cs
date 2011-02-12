// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp
{
	[TestFixture]
	public class InsertParenthesesVisitorTests
	{
		string InsertReadable(Expression expr)
		{
			expr = expr.Clone();
			expr.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true }, null);
			StringWriter w = new StringWriter();
			expr.AcceptVisitor(new OutputVisitor(w, new CSharpFormattingPolicy()), null);
			return w.ToString();
		}
		
		string InsertRequired(Expression expr)
		{
			expr = expr.Clone();
			expr.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = false }, null);
			StringWriter w = new StringWriter();
			expr.AcceptVisitor(new OutputVisitor(w, new CSharpFormattingPolicy()), null);
			return w.ToString();
		}
		
		[Test]
		public void EqualityInAssignment()
		{
			Expression expr = new AssignmentExpression(
				new IdentifierExpression("cond"),
				new BinaryOperatorExpression(
					new IdentifierExpression("a"),
					BinaryOperatorType.Equality,
					new IdentifierExpression("b")
				)
			);
			
			Assert.AreEqual("cond = a == b", InsertRequired(expr));
			Assert.AreEqual("cond = (a == b)", InsertReadable(expr));
		}
	}
}
