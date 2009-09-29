// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using IQToolkit;

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
		/*
		The base class of all QueryAst nodes is QueryNode. A QueryNode represents a query of type IQueryable<CallTreeNode>.
		For QueryNodes that have input, that input must be another QueryNode.
		
		QueryAst nodes:
			AllCalls: represents the whole FunctionData table
			input.Filter(x => condition1(x) && y => condition2(y)): WHERE clause with multiple conditions
			input.MergeByName(): GROUP BY nameid
		
		Valid expressions in QueryAst nodes:
			Only a limited set of expressions are valid in conditions and sort descriptors.
			These are checked by the SafeExpressionImporter.
			The set of valid expressions is:
				- Integer constants
				- String constants
				- Binary operators: < <= > >= == != && || LIKE GLOB
				- Unary operator: !
				- value(List<int>).Contains(validExpr)
				- if c is the lambda parameter, then these expressions are valid:
					c.NameMapping.ID
					c.NameMapping.Name
			
			Additionally, field references on a lambda parameter of type SingleCall are valid inside
			filters that operate directly on "AllCalls" (e.g. AllCalls.Filter()).
			In other cases (other filters, sort descriptors), SingleCall usage is invalid.
			SingleCall usage cannot be imported using SafeExpressionImporter; but is created directly for
			some expressions on SQLiteCallTreeNode (see translation rules below).
		
		Translation rules from CallTreeNode object model to QueryAst:
			Properties serving as query roots:
				sqliteCallTreeNode.Children
					-> AllCalls.Filter((SingleCall c) => sqliteCallTreeNode.ids.Contains(c.ParentID))
				sqliteCallTreeNode.Callers
					-> AllCalls.Filter((SingleCall c) => idList.Contains(c.ID)) where idList is calculated using a manual SQL query
				profilingDataSQLiteProvider.GetFunctions
					-> AllCalls.Filter((SingleCall c) => @start <= c.DataSetId && c.DataSetId <= @end).MergeByName()
				profilingDataSQLiteProvider.GetRoot
					-> AllCalls.Filter((SingleCall c) => @start <= c.DataSetId && c.DataSetId <= @end
							&& c => c.ParentID == -1).Merge()
			
			Translation rules for query nodes:
				input.Where(x => f(x)) -> input.Filter(x => f'(x)), if f(x) is a safe expression
					Note: If the root expression of a filter condition is the '&&' operator, a filter with multiple conditions is created.
					  This allows the optimizer to move around the filter conditions independently.
						input.Where(c => c.CallCount > 10 && c.TimeSpent > 1)
							-> input.Filter(c => c.CallCount > 10 && c => c.TimeSpent > 1)
				
				input.Select(x => x) -> input
					This rule is necessary to remove degenerate selects so that the parts of the query continuing after the select
					can also be represented as QueryNodes.
			
			Translation rules for expression importer:
				Any valid expressions (as defined in 'valid expressions in QueryAst nodes') are copied over directly.
				Moreover, these expressions are be converted into valid expressions:
					c.IsUserCode -> c.NameMapping.ID > 0
					c.NameMapping.Name.StartsWith(constantString, StringComparison.Ordinal) -> Glob(c.NameMapping.Name, constantString);
					c.NameMapping.Name.StartsWith(constantString, StringComparison.OrdinalIgnoreCase) -> Like(c.NameMapping.Name, constantString);
		
		Optimization of QueryAst:
			The OptimizeQueryExpressionVisitor is performing these optimizations:
				x.Filter(y).Filter(z) -> x.Filter(y && z)
				x.MergeByName().Filter(criteria) -> x.Filter(criteria).MergeByName() for some safe criterias
					Criterias are safe if they access no CallTreeNode properties except for NameMapping
				x.MergeByName().MergeByName() -> x.MergeByName()
		
		SQL string building and execution:
			It must be possible to create SQL for every combination of QueryNodes, even if they do strange things like merging multiple times.
			To solve this, we define that every SQL query must have the same set of result fields, which are dynamically named to
			ensure unique names even with nested queries. The current set of names is held in the SqlQueryContext.
			Indeed, conceptually we build a nested query for every QueryNode.
			
			The query is built inside-out: the innermost nested query is appended first to the StringBuilder, the outer queries will
			then insert "SELECT ... FROM (" into the beginning of the StringBuilder and append ") outer query" at the end.
			
			The return value of the QueryNode.BuildSql method contains the kind of SQL statement that is currently in the StringBuilder.
			This allows us to simply append clauses in the majority of cases, only rarely the QueryNode.WrapSqlIntoNestedStatement
			method will be used to create an outer query.
			For example, a Filter will simply append a WHERE to a "SELECT .. FROM .." query. To a "SELECT .. FROM .. GROUP BY .." query,
			a Filter will append HAVING. Only in rare cases like filtering after sorting or after limiting the number of elements,
			a Filter query node will create a nested query.
			
			Because all constructed SELECT queries always select fields with the same meaning in the same order, executing the query is
			a matter of simply filling the SQLiteCallTreeNodes with the query results.
		 */
		
		readonly ProfilingDataSQLiteProvider sqliteProvider;
		internal readonly int startDataSetID;
		
		public SQLiteQueryProvider(ProfilingDataSQLiteProvider sqliteProvider, int startDataSetID)
		{
			if (sqliteProvider == null)
				throw new ArgumentNullException("sqliteProvider");
			this.sqliteProvider = sqliteProvider;
			this.startDataSetID = startDataSetID;
		}
		
		// Implement GetMapping and ProcessorFrequency so that SQLiteQueryProvider can be used in place of
		// ProfilingDataSQLiteProvider.
		public NameMapping GetMapping(int nameID)
		{
			return sqliteProvider.GetMapping(nameID);
		}
		
		public int ProcessorFrequency {
			get { return sqliteProvider.ProcessorFrequency; }
		}
		
		public IList<CallTreeNode> RunSQLNodeList(string command)
		{
			return sqliteProvider.RunSQLNodeList(this, command);
		}
		
		/// <summary>
		/// Executes an SQL command that returns a list of integers.
		/// </summary>
		public List<int> RunSQLIDList(string command)
		{
			return sqliteProvider.RunSQLIDList(command);
		}
		
		public IQueryable<CallTreeNode> CreateQuery(QueryNode query)
		{
			return new Query<CallTreeNode>(this, query);
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
				return query.Execute(this);
			
			// Query not converted completely: we have to use a LINQ-To-Objects / LINQ-To-Profiler mix
			expression = new ExecuteAllQueriesVisitor(this).Visit(expression);
			if (expression.Type.IsValueType) {
				expression = Expression.Convert(expression, typeof(object));
			}
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
				// We found a query that's already converted, let's keep it as it is.
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
			
			/// <summary>
			/// Imports 'expr' and adds the imported conditions to 'filters'.
			/// </summary>
			/// <returns>True if the import was successful.</returns>
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
			
			object GetConstantValue(Expression expr)
			{
				return ((ConstantExpression)expr).Value;
			}
			
			/// <summary>
			/// The list of characters forbidden in GLOB arguments.
			/// </summary>
			static readonly char[] forbiddenGlobChars = new[] { '*', '?', '[', ']' };
			
			/// <summary>
			/// Imports an expresion.
			/// </summary>
			/// <returns>The imported expression; or null if the expression was not safe for import.</returns>
			public Expression Import(Expression expr)
			{
				switch (expr.NodeType) {
					case ExpressionType.Constant:
						// Only integer and string constants are supported
						if (expr.Type == typeof(int))
							return expr;
						else if (expr.Type == typeof(string))
							return expr;
						
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
						}
						return null;
					case ExpressionType.Call:
						{
							MethodCallExpression mc = (MethodCallExpression)expr;
							
							// TODO: accept StartsWith on any object (not only NameMapping.Name)
							// accept NameMapping.Name also in other contexts
							if (IsMemberOnNameMappingOnParameter(mc.Object, KnownMembers.NameMapping_Name)) {
								if (mc.Arguments[0].NodeType == ExpressionType.Constant && mc.Arguments[1].NodeType == ExpressionType.Constant) {
									StringComparison cmp = (StringComparison)GetConstantValue(mc.Arguments[1]);
									string pattern = (string)GetConstantValue(mc.Arguments[0]);
									
									if (mc.Method == KnownMembers.String_StartsWith) {
										if (cmp == StringComparison.Ordinal && pattern.IndexOfAny(forbiddenGlobChars) == -1)
											return Expression.Call(KnownMembers.Glob, mc.Object, Expression.Constant(pattern + "*"));
										else if (cmp == StringComparison.OrdinalIgnoreCase)
											return Expression.Call(KnownMembers.Like, mc.Object, Expression.Constant(EscapeLikeExpr(pattern, "\\") + "%"));
										else
											return null;
									}
								}
							}
							
							return null;
						}
					case ExpressionType.AndAlso:
					case ExpressionType.OrElse:
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
					case ExpressionType.Not:
						UnaryExpression unary = (UnaryExpression)expr;
						Expression imported = Import(unary.Operand);
						if (imported != null)
							return Expression.Not(imported);
						else
							return null;
					default:
						return null;
				}
			}
			
			string EscapeLikeExpr(string expression, string escape)
			{
				return expression.Replace(escape, escape + escape)
					.Replace("%", escape + "%")
					.Replace("_", escape + "_");
			}
			
			string EscapeGlobExpr(string expression, string escape)
			{
				return expression.Replace(escape, escape + escape)
					.Replace("*", escape + "*")
					.Replace("?", escape + "?");
			}
			
			/// <summary>
			/// Tests if expr is 'c.NameMapping'.
			/// </summary>
			bool IsNameMappingOnParameter(Expression expr)
			{
				if (expr.NodeType == ExpressionType.MemberAccess) {
					var me = (MemberExpression)expr;
					return me.Expression == callTreeNodeParameter && me.Member == KnownMembers.CallTreeNode_NameMapping;
				}
				
				return false;
			}
			
			bool IsMemberOnNameMappingOnParameter(Expression expr, MemberInfo member)
			{
				if (expr.NodeType == ExpressionType.MemberAccess) {
					var me = (MemberExpression)expr;
					return IsNameMappingOnParameter(me.Expression) && me.Member == member;
				}
				
				return false;
			}
		}
		#endregion
		
		#region ExecuteAllQueriesVisitor
		sealed class ExecuteAllQueriesVisitor : System.Linq.Expressions.ExpressionVisitor
		{
			readonly SQLiteQueryProvider sqliteProvider;
			
			public ExecuteAllQueriesVisitor(SQLiteQueryProvider sqliteProvider)
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
