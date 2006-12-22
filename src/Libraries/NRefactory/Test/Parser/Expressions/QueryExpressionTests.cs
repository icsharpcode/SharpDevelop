// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class QueryExpressionTests
	{
		[Test]
		public void SimpleExpression()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(
				"from c in customers where c.City == \"London\" select c"
			);
			Assert.AreEqual(1, qe.FromClause.Generators.Count);
			Assert.AreEqual("c", qe.FromClause.Generators[0].Identifier);
			Assert.AreEqual("customers", ((IdentifierExpression)qe.FromClause.Generators[0].InExpression).Identifier);
			Assert.AreEqual(1, qe.FromOrWhereClauses.Count);
			Assert.IsInstanceOfType(typeof(QueryExpressionWhereClause), qe.FromOrWhereClauses[0]);
			QueryExpressionWhereClause wc = (QueryExpressionWhereClause)qe.FromOrWhereClauses[0];
			Assert.IsInstanceOfType(typeof(BinaryOperatorExpression), wc.Condition);
			Assert.IsInstanceOfType(typeof(QueryExpressionSelectClause), qe.SelectOrGroupClause);
		}
		
		[Test]
		public void MultipleGenerators()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(@"
from c in customers
where c.City == ""London""
from o in c.Orders
where o.OrderDate.Year == 2005
select new { c.Name, o.OrderID, o.Total }");
			Assert.AreEqual(1, qe.FromClause.Generators.Count);
			Assert.AreEqual(3, qe.FromOrWhereClauses.Count);
			Assert.IsInstanceOfType(typeof(QueryExpressionWhereClause), qe.FromOrWhereClauses[0]);
			Assert.IsInstanceOfType(typeof(QueryExpressionFromClause), qe.FromOrWhereClauses[1]);
			Assert.IsInstanceOfType(typeof(QueryExpressionWhereClause), qe.FromOrWhereClauses[2]);
			
			Assert.IsInstanceOfType(typeof(QueryExpressionSelectClause), qe.SelectOrGroupClause);
		}
	}
}
