// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using IQToolkit;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	/// <summary>
	/// "LINQ-To-Profiler" QueryProvider implementation for SQLiteCallTreeNode.
	/// 
	/// Input to a LINQ QueryProvider is a System.Linq.Expressions tree describing the query that should be executed.
	/// 
	/// Query execution is done as:
	/// 1. Partial evaluation
	/// 2. Translation of expression tree to QueryAst
	/// 3. Optimization of QueryAst
	/// 4. Execution of Queries (by converting them to SQL and running it on the DB)
	/// 5. Execution of remaining query using LINQ-to-Objects
	/// </summary>
	sealed class SQLiteQueryProvider : QueryProvider
	{
		readonly ProfilingDataSQLiteProvider sqliteProvider;
		
		public SQLiteQueryProvider(ProfilingDataSQLiteProvider sqliteProvider)
		{
			if (sqliteProvider == null)
				throw new ArgumentNullException("sqliteProvider");
			this.sqliteProvider = sqliteProvider;
		}
		
		public override string GetQueryText(Expression expression)
		{
			return expression.ToString();
		}
		
		public override object Execute(Expression expression)
		{
			Console.WriteLine("Input expression: " + expression);
			
			expression = PartialEvaluator.Eval(expression, CanBeEvaluatedStatically);
			Console.WriteLine("Partially evaluated expression: " + expression);
			
			expression = new ConvertToQueryAstVisitor().Visit(expression);
			Console.WriteLine("Converted to Query AST: " + expression);
			
			expression = new OptimizeQueryExpressionVisitor().Visit(expression);
			Console.WriteLine("Optimized Query AST: " + expression);
			
			// If the whole query was converted, execute it:
			QueryNode query = expression as QueryNode;
			if (query != null)
				return query.Execute(sqliteProvider);
			
			// Query not converted completely: we have to use a LINQ-To-Objects / LINQ-To-Profiler mix
			expression = new ExecuteAllQueriesVisitor(sqliteProvider).Visit(expression);
			var lambdaExpression = Expression.Lambda<Func<object>>(expression);
			return lambdaExpression.Compile()();
		}
		
		static bool CanBeEvaluatedStatically(Expression expression)
		{
			return expression.NodeType != ExpressionType.Parameter && !(expression is QueryNode);
		}
		
		#region Convert Expression Tree To Query AST
		sealed class ConvertToQueryAstVisitor : System.Linq.Expressions.ExpressionVisitor
		{
			protected override Expression VisitExtension(Expression node)
			{
				// The .NET ExpressionVisitor cannot recurse into our query nodes -
				// but those don't need conversion anymore.
				QueryNode query = node as QueryNode;
				if (query != null)
					return query;
				else
					return base.VisitExtension(node);
			}
			
			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (node.Method == KnownMembers.QuerableOfCallTreeNode_Select && node.Arguments[1].NodeType == ExpressionType.Quote) {
					// CallTreeNode[].Select:
					// selects are not supported by query evaluator, but we will detect and remove
					// degenerate selects (.Select(x => x)).
					UnaryExpression quote = (UnaryExpression)node.Arguments[1];
					if (quote.Operand.NodeType == ExpressionType.Lambda) {
						LambdaExpression lambda = (LambdaExpression)quote.Operand;
						if (lambda.Parameters.Count == 1 && lambda.Body == lambda.Parameters[0])
							return Visit(node.Arguments[0]);
					}
				} else if (node.Method == KnownMembers.QuerableOfCallTreeNode_Where && node.Arguments[1].NodeType == ExpressionType.Quote) {
					// CallTreeNode[].Where:
					// if the target is a QueryNode and the condition can be safely imported, convert the Where call to Filter
					UnaryExpression quote = (UnaryExpression)node.Arguments[1];
					if (quote.Operand.NodeType == ExpressionType.Lambda) {
						LambdaExpression lambda = (LambdaExpression)quote.Operand;
						if (lambda.Parameters.Count == 1) {
							QueryNode target = Visit(node.Arguments[0]) as QueryNode;
							if (target != null) {
								SafeExpressionImporter importer = new SafeExpressionImporter(lambda.Parameters[0]);
								List<LambdaExpression> conditions = new List<LambdaExpression>();
								if (importer.AddConditionsToFilterList(lambda.Body, conditions)) {
									return new Filter(target, conditions.ToArray());
								}
							}
						}
					}
				}
				return base.VisitMethodCall(node);
			}
		}
		
		sealed class SafeExpressionImporter
		{
			ParameterExpression callTreeNodeParameter;
			
			public SafeExpressionImporter(ParameterExpression callTreeNodeParameter)
			{
				this.callTreeNodeParameter = callTreeNodeParameter;
			}
			
			public bool AddConditionsToFilterList(Expression expr, List<LambdaExpression> filters)
			{
				if (expr.NodeType == ExpressionType.AndAlso) {
					BinaryExpression binary = (BinaryExpression)expr;
					return AddConditionsToFilterList(binary.Left, filters)
						&& AddConditionsToFilterList(binary.Right, filters);
				} else {
					Expression imported = Import(expr);
					if (imported != null) {
						filters.Add(Expression.Lambda(imported, callTreeNodeParameter));
						return true;
					} else {
						return false;
					}
				}
			}
			
			public Expression Import(Expression expr)
			{
				switch (expr.NodeType) {
					case ExpressionType.Constant:
						// Only integer constants are supported
						if (expr.Type == typeof(int))
							return expr;
						else
							return null;
					case ExpressionType.MemberAccess:
						{
							MemberExpression me = (MemberExpression)expr;
							if (me.Expression == callTreeNodeParameter) {
								if (me.Member == KnownMembers.CallTreeNode_IsUserCode) {
									return Expression.GreaterThan(
										Expression.Property(
											Expression.Property(callTreeNodeParameter, KnownMembers.CallTreeNode_NameMapping),
											KnownMembers.NameMapping_ID),
										Expression.Constant(0));
								}
							} else if (IsNameMappingOnParameter(me.Expression)) {
								if (me.Member == KnownMembers.NameMapping_ID)
									return me;
							}
							return null;
						}
					case ExpressionType.LessThan:
					case ExpressionType.LessThanOrEqual:
					case ExpressionType.GreaterThan:
					case ExpressionType.GreaterThanOrEqual:
					case ExpressionType.Equal:
					case ExpressionType.NotEqual:
						{
							BinaryExpression binary = (BinaryExpression)expr;
							Expression left = Import(binary.Left);
							Expression right = Import(binary.Right);
							if (left != null && right != null)
								return Expression.MakeBinary(expr.NodeType, left, right);
							else
								return null;
						}
					default:
						return null;
				}
			}
			
			bool IsNameMappingOnParameter(Expression expr)
			{
				if (expr.NodeType == ExpressionType.MemberAccess) {
					var me = (MemberExpression)expr;
					return me.Expression == callTreeNodeParameter && me.Member == KnownMembers.CallTreeNode_NameMapping;
				} else {
					return false;
				}
			}
		}
		#endregion
		
		#region ExecuteAllQueriesVisitor
		sealed class ExecuteAllQueriesVisitor : System.Linq.Expressions.ExpressionVisitor
		{
			readonly ProfilingDataSQLiteProvider sqliteProvider;
			
			public ExecuteAllQueriesVisitor(ProfilingDataSQLiteProvider sqliteProvider)
			{
				this.sqliteProvider = sqliteProvider;
			}
			
			protected override Expression VisitExtension(Expression node)
			{
				QueryNode query = node as QueryNode;
				if (query != null)
					return Expression.Constant(query.Execute(sqliteProvider));
				else
					return base.VisitExtension(node);
			}
		}
		#endregion
	}
}
