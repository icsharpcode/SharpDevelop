// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Tests.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class QueryExpressionVBTests
	{
		void RunTest(string expression, int expectedCount, Action<QueryExpressionVB> constraint, params Type[] expectedTypes)
		{
			var expr = ParseUtilVBNet.ParseExpression<QueryExpressionVB>(expression);
			
			Assert.AreEqual(expectedCount, expr.Clauses.Count);
			
			for (int i = 0; i < expectedTypes.Length; i++) {
				Assert.IsTrue(expectedTypes[i] == expr.Clauses[i].GetType());
			}
			
			constraint(expr);
		}
		
		[Test]
		public void SimpleQueryTest()
		{
			RunTest("From o In db.Orders Select o.OrderID", 2,
			        expr => {
			        	var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
			        	var selectClause = expr.Clauses[1] as QueryExpressionSelectVBClause;
			        	
			        	Assert.AreEqual("o", fromClause.Identifier);
			        	Assert.IsTrue(fromClause.InExpression is MemberReferenceExpression);
			        	var inExpr = fromClause.InExpression as MemberReferenceExpression;
			        	Assert.IsTrue(inExpr.MemberName == "Orders" && inExpr.TargetObject is IdentifierExpression && (inExpr.TargetObject as IdentifierExpression).Identifier == "db");
			        	
			        	Assert.AreEqual(1, selectClause.Variables.Count);
			        	Assert.IsTrue(selectClause.Variables[0].Expression is MemberReferenceExpression);
			        	var member = selectClause.Variables[0].Expression as MemberReferenceExpression;
			        	
			        	Assert.IsTrue(member.MemberName == "OrderID" && member.TargetObject is IdentifierExpression && (member.TargetObject as IdentifierExpression).Identifier == "o");
			        },
			        typeof(QueryExpressionFromClause), typeof(QueryExpressionSelectVBClause)
			       );
		}
		
		[Test]
		public void SkipTakeQueryTest()
		{
			RunTest("From o In db.Orders Select o.OrderID Skip 10 Take 5", 4,
			        expr => {
			        	var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
			        	var selectClause = expr.Clauses[1] as QueryExpressionSelectVBClause;
			        	var skipClause = expr.Clauses[2] as QueryExpressionPartitionVBClause;
			        	var takeClause = expr.Clauses[3] as QueryExpressionPartitionVBClause;
			        	
			        	Assert.AreEqual("o", fromClause.Identifier);
			        	Assert.IsTrue(fromClause.InExpression is MemberReferenceExpression);
			        	var inExpr = fromClause.InExpression as MemberReferenceExpression;
			        	Assert.IsTrue(inExpr.MemberName == "Orders" && inExpr.TargetObject is IdentifierExpression && (inExpr.TargetObject as IdentifierExpression).Identifier == "db");
			        	
			        	Assert.AreEqual(1, selectClause.Variables.Count);
			        	Assert.IsTrue(selectClause.Variables[0].Expression is MemberReferenceExpression);
			        	var member = selectClause.Variables[0].Expression as MemberReferenceExpression;
			        	
			        	Assert.IsTrue(member.MemberName == "OrderID" && member.TargetObject is IdentifierExpression && (member.TargetObject as IdentifierExpression).Identifier == "o");
			        	
			        	Assert.AreEqual(QueryExpressionPartitionType.Skip, skipClause.PartitionType);
			        	Assert.IsTrue(skipClause.Expression is PrimitiveExpression &&
			        	              (skipClause.Expression as PrimitiveExpression).StringValue == "10");
			        	
			        	Assert.AreEqual(QueryExpressionPartitionType.Take, takeClause.PartitionType);
			        	Assert.IsTrue(takeClause.Expression is PrimitiveExpression &&
			        	              (takeClause.Expression as PrimitiveExpression).StringValue == "5");
			        },
			        typeof(QueryExpressionFromClause), typeof(QueryExpressionSelectVBClause),
			        typeof(QueryExpressionPartitionVBClause), typeof(QueryExpressionPartitionVBClause)
			       );
		}
		
		[Test]
		public void MultipleValuesSelect()
		{
			RunTest(@"From i In list Select i, x2 = i^2",
			        2, expr => {
			        	var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
			        	var selectClause = expr.Clauses[1] as QueryExpressionSelectVBClause;
			        	
			        	Assert.AreEqual("i", fromClause.Identifier);
			        	Assert.IsTrue(fromClause.InExpression is IdentifierExpression);
			        	Assert.IsTrue((fromClause.InExpression as IdentifierExpression).Identifier == "list");
			        	
			        	Assert.AreEqual(2, selectClause.Variables.Count);
			        	
			        	var selectExpr1 = selectClause.Variables[0];
			        	var selectExpr2 = selectClause.Variables[1];
			        	
			        	Assert.IsEmpty(selectExpr1.Identifier);
			        	Assert.IsTrue(selectExpr1.Expression is IdentifierExpression &&
			        	              (selectExpr1.Expression as IdentifierExpression).Identifier == "i");
			        	
			        	Assert.AreEqual("x2", selectExpr2.Identifier);
			        	Assert.IsTrue(selectExpr2.Type.IsNull);
			        	Assert.IsTrue(selectExpr2.Expression is BinaryOperatorExpression);
			        	
			        	var binOp = selectExpr2.Expression as BinaryOperatorExpression;
			        	
			        	Assert.AreEqual(BinaryOperatorType.Power, binOp.Op);
			        	Assert.IsTrue(binOp.Left is IdentifierExpression && (binOp.Left as IdentifierExpression).Identifier == "i");
			        	Assert.IsTrue(binOp.Right is PrimitiveExpression && (binOp.Right as PrimitiveExpression).StringValue == "2");
			        },
			        typeof(QueryExpressionFromClause), typeof(QueryExpressionSelectVBClause)
			       );
		}
		
		[Test]
		public void GroupTest()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				var groupClause = expr.Clauses[1] as QueryExpressionGroupVBClause;
				var selectClause = expr.Clauses[2] as QueryExpressionSelectVBClause;
				
				Assert.AreEqual("p", fromClause.Identifier);
				Assert.IsTrue(fromClause.InExpression is IdentifierExpression);
				Assert.IsTrue((fromClause.InExpression as IdentifierExpression).Identifier == "products");
				
				Assert.AreEqual(1, groupClause.GroupVariables.Count);
				Assert.AreEqual(1, groupClause.ByVariables.Count);
				Assert.AreEqual(1, groupClause.IntoVariables.Count);
				
				var gv = groupClause.GroupVariables.First();
				var bv = groupClause.ByVariables.First();
				var iv = groupClause.IntoVariables.First();
				
				Assert.IsTrue(gv.Expression is IdentifierExpression && (gv.Expression as IdentifierExpression).Identifier == "p");
				Assert.IsTrue(bv.Expression is MemberReferenceExpression &&
				              (bv.Expression as MemberReferenceExpression).MemberName == "Category");
				Assert.IsTrue((bv.Expression as MemberReferenceExpression).TargetObject is IdentifierExpression &&
				              ((bv.Expression as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "p");
				Assert.IsTrue(iv.Expression is IdentifierExpression &&
				              (iv.Expression as IdentifierExpression).Identifier == "Group");
				
				Assert.AreEqual(2, selectClause.Variables.Count);
				
				var var1 = selectClause.Variables.First();
				var var2 = selectClause.Variables.Skip(1).First();
				
				Assert.IsTrue(var1.Expression is IdentifierExpression &&
				              (var1.Expression as IdentifierExpression).Identifier == "Category");
				Assert.IsTrue(var2.Expression is InvocationExpression &&
				              (var2.Expression as InvocationExpression).TargetObject is MemberReferenceExpression &&
				              ((var2.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).MemberName == "Average" &&
				              ((var2.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).TargetObject is IdentifierExpression &&
				              (((var2.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "Group");
			};
			
			RunTest(@"From p In products _
            Group p By p.Category Into Group _
            Select Category, AveragePrice = Group.Average(Function(p) p.UnitPrice)", 3, constraint,
			        typeof(QueryExpressionFromClause), typeof(QueryExpressionGroupVBClause), typeof(QueryExpressionSelectVBClause));
		}
	}
}
