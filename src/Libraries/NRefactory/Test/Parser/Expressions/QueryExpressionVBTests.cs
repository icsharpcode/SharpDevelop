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
			        	
			        	Assert.AreEqual(1, fromClause.Sources.Count);
			        	
			        	var var1 = fromClause.Sources.First();
			        	
			        	Assert.AreEqual("o", var1.Identifier);
			        	Assert.IsTrue(var1.Expression is MemberReferenceExpression);
			        	var inExpr = var1.Expression as MemberReferenceExpression;
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
			        	
			        	Assert.AreEqual(1, fromClause.Sources.Count);
			        	
			        	var var1 = fromClause.Sources.First();
			        	
			        	Assert.AreEqual("o", var1.Identifier);
			        	Assert.IsTrue(var1.Expression is MemberReferenceExpression);
			        	var inExpr = var1.Expression as MemberReferenceExpression;
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
		public void MultipleValuesSelectTest()
		{
			RunTest(@"From i In list Select i, x2 = i^2",
			        2, expr => {
			        	var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
			        	var selectClause = expr.Clauses[1] as QueryExpressionSelectVBClause;
			        	
			        	Assert.AreEqual(1, fromClause.Sources.Count);
			        	
			        	var var1 = fromClause.Sources.First();
			        	
			        	Assert.AreEqual("i", var1.Identifier);
			        	Assert.IsTrue(var1.Expression is IdentifierExpression);
			        	Assert.IsTrue((var1.Expression as IdentifierExpression).Identifier == "list");
			        	
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
				
				Assert.AreEqual(1, fromClause.Sources.Count);
				
				var fromVar1 = fromClause.Sources.First();
				
				Assert.AreEqual("p", fromVar1.Identifier);
				Assert.IsTrue(fromVar1.Expression is IdentifierExpression);
				Assert.IsTrue((fromVar1.Expression as IdentifierExpression).Identifier == "products");
				
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
		
		[Test]
		public void LetTest()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				var groupClause = expr.Clauses[1] as QueryExpressionGroupVBClause;
				var letClause = expr.Clauses[2] as QueryExpressionLetVBClause;
				var selectClause = expr.Clauses[3] as QueryExpressionSelectVBClause;
				
				// From
				Assert.AreEqual(1, fromClause.Sources.Count);
				
				var fromVar1 = fromClause.Sources.First();
				
				Assert.AreEqual("p", fromVar1.Identifier);
				Assert.IsTrue(fromVar1.Expression is IdentifierExpression);
				Assert.IsTrue((fromVar1.Expression as IdentifierExpression).Identifier == "products");
				
				// Group By Into
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
				
				// Let
				Assert.AreEqual(1, letClause.Variables.Count);
				
				var letVariable = letClause.Variables.First();
				
				Assert.AreEqual("minPrice", letVariable.Identifier);
				Assert.IsTrue(letVariable.Expression is InvocationExpression &&
				              (letVariable.Expression as InvocationExpression).TargetObject is MemberReferenceExpression &&
				              ((letVariable.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).MemberName == "Min" &&
				              ((letVariable.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).TargetObject is IdentifierExpression &&
				              (((letVariable.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "Group");

				// Select
				Assert.AreEqual(2, selectClause.Variables.Count);
				
				var var1 = selectClause.Variables.First();
				var var2 = selectClause.Variables.Skip(1).First();
				
				Assert.IsTrue(var1.Expression is IdentifierExpression &&
				              (var1.Expression as IdentifierExpression).Identifier == "Category");
				Assert.IsTrue(var2.Expression is InvocationExpression &&
				              (var2.Expression as InvocationExpression).TargetObject is MemberReferenceExpression &&
				              ((var2.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).MemberName == "Where" &&
				              ((var2.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).TargetObject is IdentifierExpression &&
				              (((var2.Expression as InvocationExpression).TargetObject as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "Group");
			};
			
			RunTest(@"From p In products _
            Group p By p.Category Into Group _
            Let minPrice = Group.Min(Function(p) p.UnitPrice) _
            Select Category, CheapestProducts = Group.Where(Function(p) p.UnitPrice = minPrice)", 4, constraint,
			        typeof(QueryExpressionFromClause), typeof(QueryExpressionGroupVBClause), typeof(QueryExpressionLetVBClause), typeof(QueryExpressionSelectVBClause));
		}
		
		[Test]
		public void CrossJoinTest()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				var joinClause = expr.Clauses[1] as QueryExpressionJoinVBClause;
				var selectClause = expr.Clauses[2] as QueryExpressionSelectVBClause;
				
				// From
				Assert.AreEqual(1, fromClause.Sources.Count);
				
				var fromVar1 = fromClause.Sources.First();
				
				Assert.AreEqual("c", fromVar1.Identifier);
				Assert.IsTrue(fromVar1.Expression is IdentifierExpression);
				Assert.IsTrue((fromVar1.Expression as IdentifierExpression).Identifier == "categories");
				
				// Join In On Equals
				var inClause = joinClause.JoinVariable as CollectionRangeVariable;
				
				Assert.AreEqual("p", inClause.Identifier);
				Assert.IsTrue(inClause.Expression is IdentifierExpression &&
				              (inClause.Expression as IdentifierExpression).Identifier == "products");
				
				Assert.IsTrue(joinClause.SubJoin.IsNull);
				
				Assert.AreEqual(1, joinClause.Conditions.Count);
				
				var condition1 = joinClause.Conditions.First();
				
				Assert.IsTrue(condition1.LeftSide is IdentifierExpression && (condition1.LeftSide as IdentifierExpression).Identifier == "c");
				Assert.IsTrue(condition1.RightSide is MemberReferenceExpression &&
				              ((condition1.RightSide as MemberReferenceExpression).TargetObject is IdentifierExpression) &&
				              ((condition1.RightSide as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "p" &&
				              (condition1.RightSide as MemberReferenceExpression).MemberName == "Category");
				// Select
				Assert.AreEqual(2, selectClause.Variables.Count);
				
				var var1 = selectClause.Variables.First();
				var var2 = selectClause.Variables.Skip(1).First();
				
				Assert.AreEqual("Category", var1.Identifier);
				Assert.IsEmpty(var2.Identifier);
				
				Assert.IsTrue(var1.Expression is IdentifierExpression &&
				              (var1.Expression as IdentifierExpression).Identifier == "c");
				Assert.IsTrue(var2.Expression is MemberReferenceExpression &&
				              (var2.Expression as MemberReferenceExpression).MemberName == "ProductName" &&
				              (var2.Expression as MemberReferenceExpression).TargetObject is IdentifierExpression &&
				              ((var2.Expression as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "p");
			};
			
			RunTest(@"From c In categories _
                Join p In products On c Equals p.Category _
                Select Category = c, p.ProductName", 3, constraint,
			        typeof(QueryExpressionFromClause), typeof(QueryExpressionJoinVBClause), typeof(QueryExpressionSelectVBClause));
		}
		
		[Test]
		public void OrderByTest()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				var orderClause = expr.Clauses[1] as QueryExpressionOrderClause;
				
				// From
				Assert.AreEqual(1, fromClause.Sources.Count);
				
				var var1 = fromClause.Sources.First();
				
				Assert.AreEqual("i", var1.Identifier);
				Assert.IsTrue(var1.Expression is IdentifierExpression);
				Assert.IsTrue((var1.Expression as IdentifierExpression).Identifier == "list");
				
				// Order By
				Assert.AreEqual(1, orderClause.Orderings.Count);
				
				var ordering1 = orderClause.Orderings.First();
				
				Assert.IsTrue(ordering1.Criteria is IdentifierExpression &&
				              (ordering1.Criteria as IdentifierExpression).Identifier == "i");
				Assert.AreEqual(QueryExpressionOrderingDirection.None, ordering1.Direction);
			};
			
			RunTest(@"From i In list Order By i", 2, constraint, typeof(QueryExpressionFromClause), typeof(QueryExpressionOrderClause));
		}
		
		[Test]
		public void OrderByTest2()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				var orderClause = expr.Clauses[1] as QueryExpressionOrderClause;
				
				// From
				Assert.AreEqual(1, fromClause.Sources.Count);
				
				var var1 = fromClause.Sources.First();
				
				Assert.AreEqual("i", var1.Identifier);
				Assert.IsTrue(var1.Expression is IdentifierExpression);
				Assert.IsTrue((var1.Expression as IdentifierExpression).Identifier == "list");
				
				// Order By
				Assert.AreEqual(1, orderClause.Orderings.Count);
				
				var ordering1 = orderClause.Orderings.First();
				
				Assert.IsTrue(ordering1.Criteria is IdentifierExpression &&
				              (ordering1.Criteria as IdentifierExpression).Identifier == "i");
				Assert.AreEqual(QueryExpressionOrderingDirection.Ascending, ordering1.Direction);
			};
			
			RunTest(@"From i In list Order By i Ascending", 2, constraint, typeof(QueryExpressionFromClause), typeof(QueryExpressionOrderClause));
		}
		
		[Test]
		public void OrderByTest3()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				var orderClause = expr.Clauses[1] as QueryExpressionOrderClause;
				
				// From
				Assert.AreEqual(1, fromClause.Sources.Count);
				
				var var1 = fromClause.Sources.First();
				
				Assert.AreEqual("i", var1.Identifier);
				Assert.IsTrue(var1.Expression is IdentifierExpression);
				Assert.IsTrue((var1.Expression as IdentifierExpression).Identifier == "list");
				
				// Order By
				Assert.AreEqual(1, orderClause.Orderings.Count);
				
				var ordering1 = orderClause.Orderings.First();
				
				Assert.IsTrue(ordering1.Criteria is IdentifierExpression &&
				              (ordering1.Criteria as IdentifierExpression).Identifier == "i");
				Assert.AreEqual(QueryExpressionOrderingDirection.Descending, ordering1.Direction);
			};
			
			RunTest(@"From i In list Order By i Descending", 2, constraint, typeof(QueryExpressionFromClause), typeof(QueryExpressionOrderClause));
		}
		
		[Test]
		public void OrderByThenByTest()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				var orderClause = expr.Clauses[1] as QueryExpressionOrderClause;
				
				// From
				Assert.AreEqual(1, fromClause.Sources.Count);
				
				var var1 = fromClause.Sources.First();
				
				Assert.AreEqual("d", var1.Identifier);
				Assert.IsTrue(var1.Expression is IdentifierExpression);
				Assert.IsTrue((var1.Expression as IdentifierExpression).Identifier == "digits");
				
				// Order By
				Assert.AreEqual(2, orderClause.Orderings.Count);
				
				var ordering1 = orderClause.Orderings.First();
				var ordering2 = orderClause.Orderings.Skip(1).First();
				
				Assert.IsTrue(ordering1.Criteria is MemberReferenceExpression);
				Assert.IsTrue((ordering1.Criteria as MemberReferenceExpression).MemberName == "Length");
				Assert.IsTrue((ordering1.Criteria as MemberReferenceExpression).TargetObject is IdentifierExpression);
				Assert.IsTrue(((ordering1.Criteria as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "d");

				Assert.IsTrue(ordering2.Criteria is IdentifierExpression &&
				              (ordering2.Criteria as IdentifierExpression).Identifier == "d");
				Assert.AreEqual(QueryExpressionOrderingDirection.None, ordering1.Direction);
				Assert.AreEqual(QueryExpressionOrderingDirection.None, ordering2.Direction);

			};
			
			RunTest(@"From d In digits _
        Order By d.Length, d", 2, constraint,
			        typeof(QueryExpressionFromClause), typeof(QueryExpressionOrderClause));
		}
		
		[Test]
		public void DistinctTest()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				
				// From
				Assert.AreEqual(1, fromClause.Sources.Count);
				
				var var1 = fromClause.Sources.First();
				
				Assert.AreEqual("d", var1.Identifier);
				Assert.IsTrue(var1.Expression is IdentifierExpression);
				Assert.IsTrue((var1.Expression as IdentifierExpression).Identifier == "digits");
			};
			
			RunTest(@"From d In digits Distinct", 2, constraint,
			        typeof(QueryExpressionFromClause), typeof(QueryExpressionDistinctClause));
		}
		
		[Test]
		public void GroupJoinTest()
		{
			
		}
		
		[Test]
		public void SelectManyTest()
		{
			Action<QueryExpressionVB> constraint = expr => {
				var fromClause = expr.Clauses[0] as QueryExpressionFromClause;
				var whereClause = expr.Clauses[1] as QueryExpressionWhereClause;
				var selectClause = expr.Clauses[2] as QueryExpressionSelectVBClause;
				
				// From
				Assert.AreEqual(2, fromClause.Sources.Count);
				
				var fromVar1 = fromClause.Sources.First();
				var fromVar2 = fromClause.Sources.Skip(1).First();
				
				Assert.AreEqual("c", fromVar1.Identifier);
				Assert.IsTrue(fromVar1.Expression is IdentifierExpression);
				Assert.IsTrue((fromVar1.Expression as IdentifierExpression).Identifier == "customers");
				
				Assert.AreEqual("o", fromVar2.Identifier);
				Assert.IsTrue(fromVar2.Expression is MemberReferenceExpression);
				Assert.IsTrue((fromVar2.Expression as MemberReferenceExpression).MemberName == "Orders");
				Assert.IsTrue((fromVar2.Expression as MemberReferenceExpression).TargetObject is IdentifierExpression);
				Assert.IsTrue(((fromVar2.Expression as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "c");
				
				// Where
				Assert.IsTrue(whereClause.Condition is BinaryOperatorExpression);
				Assert.IsTrue((whereClause.Condition as BinaryOperatorExpression).Op == BinaryOperatorType.LessThan);
				Assert.IsTrue((whereClause.Condition as BinaryOperatorExpression).Left is MemberReferenceExpression);
				Assert.IsTrue(((whereClause.Condition as BinaryOperatorExpression).Left as MemberReferenceExpression).MemberName == "Total");
				Assert.IsTrue(((whereClause.Condition as BinaryOperatorExpression).Left as MemberReferenceExpression).TargetObject is IdentifierExpression);
				Assert.IsTrue((((whereClause.Condition as BinaryOperatorExpression).Left as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "o");
				Assert.IsTrue((whereClause.Condition as BinaryOperatorExpression).Right is PrimitiveExpression);
				Assert.IsTrue((double)((whereClause.Condition as BinaryOperatorExpression).Right as PrimitiveExpression).Value == 500.0);
				
				// Select
				foreach (var v in selectClause.Variables) {
					Assert.IsEmpty(v.Identifier);
				}
				
				var var1 = selectClause.Variables.First();
				var var2 = selectClause.Variables.Skip(1).First();
				var var3 = selectClause.Variables.Skip(2).First();
				
				Assert.IsTrue(var1.Expression is MemberReferenceExpression);
				Assert.IsTrue((var1.Expression as MemberReferenceExpression).MemberName == "CustomerID" &&
				              (var1.Expression as MemberReferenceExpression).TargetObject is IdentifierExpression &&
				              ((var1.Expression as MemberReferenceExpression).TargetObject as IdentifierExpression).Identifier == "c");
				CheckMemberReferenceExpression("CustomerID", "c");
			};
			
			RunTest(@"From c In customers, o In c.Orders _
        Where o.Total < 500.0 _
        Select c.CustomerID, o.OrderID, o.Total", 3, constraint, typeof(QueryExpressionFromClause), typeof(QueryExpressionWhereClause), typeof(QueryExpressionSelectVBClause));
		}
		
		void CheckMemberReferenceExpression(string member, string target)
		{
			
		}
	}
}
