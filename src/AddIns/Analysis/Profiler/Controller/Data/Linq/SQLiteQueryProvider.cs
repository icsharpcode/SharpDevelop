// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
			AllCalls: represents the whole Calls table
			AllFunctions: represents the whole Functions table
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
					c.CallCount
					c.CpuCyclesSpent
					c.IsThread
					c.IsUserCode
			
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
				
				input.Take(count) -> input.Limit(0, count)
					Note: Skip/Take combinations are not combined (Skip is not supported).
				
				input.OrderBy(x => x.SomeField) -> input.Sort({ [x.SomeField, ascending] })
				input.OrderByDescending(x => x.SomeField) -> input.Sort({ [x.SomeField, descending] })
					Note: OrderBy is not converted into Sort if followed by a ThenBy.
				
				input.MergeByName() -> new MergeByName(input)
			
			Translation rules for expression importer:
				Any valid expressions (as defined in 'valid expressions in QueryAst nodes') are copied over directly.
				Moreover, these expressions are converted into valid expressions:
					c.IsUserCode -> c.NameMapping.ID > 0
					c.IsThread -> Glob(c.NameMapping.Name, "Thread#*")
					s.StartsWith(constantString, StringComparison.Ordinal) -> Glob(s, constantString + "*");
					s.StartsWith(constantString, StringComparison.OrdinalIgnoreCase) -> Like(s, constantString + "%");
					s.EndsWith(constantString, StringComparison.Ordinal) -> Glob(s, "*" + constantString);
					s.EndsWith(constantString, StringComparison.OrdinalIgnoreCase) -> Like(s, "%" + constantString);
		
		Optimization of QueryAst:
			The OptimizeQueryExpressionVisitor is performing these optimizations:
				x.Filter(y).Filter(z) -> x.Filter(y && z)
				x.MergeByName().Filter(criteria) -> x.Filter(criteria).MergeByName() for some safe criterias
					Criterias are safe if they access no CallTreeNode properties except for NameMapping
				x.MergeByName().MergeByName() -> x.MergeByName()
				AllCalls.Filter(criteria).MergeByName() -> AllFunctions.Filter(criteria)
					If criteria accesses no CallTreeNode properties except for NameMapping.
					Criteria of the form 'start <= c.DataSetID && c.DataSetID <= end' will be converted into AllFunctions(start,end)
		
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
		public readonly ProfilingDataSQLiteProvider.SQLiteDataSet StartDataSet;
		public readonly ProfilingDataSQLiteProvider.SQLiteDataSet EndDataSet;
		
		public SQLiteQueryProvider(ProfilingDataSQLiteProvider sqliteProvider, int startDataSetID, int endDataSetID)
		{
			if (sqliteProvider == null)
				throw new ArgumentNullException("sqliteProvider");
			this.sqliteProvider = sqliteProvider;
			
			this.StartDataSet = FindDataSetById(startDataSetID);
			this.EndDataSet = FindDataSetById(endDataSetID);
		}
		
		public ProfilingDataSQLiteProvider.SQLiteDataSet FindDataSetById(int id)
		{
			var dataSets = sqliteProvider.DataSets.Cast<ProfilingDataSQLiteProvider.SQLiteDataSet>();
			return dataSets.Single(d => d.ID == id);
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
		
		public IList<CallTreeNode> RunSQLNodeList(string command, bool hasIdList)
		{
			return sqliteProvider.RunSQLNodeList(this, command, hasIdList);
		}
		
		/// <summary>
		/// Executes an SQL command that returns a list of integers.
		/// </summary>
		public List<int> RunSQLIDList(string command)
		{
			return sqliteProvider.RunSQLIDList(command);
		}
		
		public int[] LoadIDListForFunction(int nameid)
		{
			string command = string.Format(
				CultureInfo.InvariantCulture,
				"SELECT id FROM Calls WHERE (nameid = {0}) AND (id BETWEEN {1} AND {2});",
				nameid, StartDataSet.RootID, EndDataSet.CallEndID);
			return sqliteProvider.RunSQLIDList(command).ToArray();
		}
		
		public IQueryable<CallTreeNode> CreateQuery(QueryNode query)
		{
			return new Query<CallTreeNode>(this, query);
		}
		
		public override string GetQueryText(Expression expression)
		{
			return expression.ToString();
		}
		
		/// <summary>
		/// Optimizes the query without executing it.
		/// Used for unit tests.
		/// </summary>
		public static Expression OptimizeQuery(Expression inputExpression)
		{
			Expression partiallyEvaluatedExpression = PartialEvaluator.Eval(inputExpression, CanBeEvaluatedStatically);
			Expression expression = new ConvertToQueryAstVisitor(new QueryExecutionOptions()).Visit(partiallyEvaluatedExpression);
			return new OptimizeQueryExpressionVisitor().Visit(expression);
		}
		
		public override object Execute(Expression inputExpression)
		{
			Stopwatch watch = Stopwatch.StartNew();
			Expression partiallyEvaluatedExpression = PartialEvaluator.Eval(inputExpression, CanBeEvaluatedStatically);
			
			QueryExecutionOptions options = new QueryExecutionOptions();
			Expression expression = new ConvertToQueryAstVisitor(options).Visit(partiallyEvaluatedExpression);
			// options have been initialized by ConvertToQueryAstVisitor, start logging:
			if (options.HasLoggers) {
				options.WriteLogLine("Input expression: " + inputExpression);
				options.WriteLogLine("Partially evaluated expression: " + partiallyEvaluatedExpression);
				options.WriteLogLine("Converted to Query AST: " + expression);
			}
			
			expression = new OptimizeQueryExpressionVisitor().Visit(expression);
			if (options.HasLoggers) {
				options.WriteLogLine("Optimized Query AST: " + expression);
				options.WriteLogLine("Query preparation time: " + watch.Elapsed);
			}
			
			object result;
			// If the whole query was converted, execute it:
			QueryNode query = expression as QueryNode;
			if (query != null) {
				result = query.Execute(this, options);
			} else {
				// Query not converted completely: we have to use a LINQ-To-Objects / LINQ-To-Profiler mix
				expression = new ExecuteAllQueriesVisitor(this, options).Visit(expression);
				if (expression.Type.IsValueType) {
					expression = Expression.Convert(expression, typeof(object));
				}
				var lambdaExpression = Expression.Lambda<Func<object>>(expression);
				result = lambdaExpression.Compile()();
			}
			watch.Stop();
			options.WriteLogLine("Total query execution time: " + watch.Elapsed);
			return result;
		}
		
		static bool CanBeEvaluatedStatically(Expression expression)
		{
			return expression.NodeType != ExpressionType.Parameter && !(expression is QueryNode);
		}
		
		#region Convert Expression Tree To Query AST
		sealed class ConvertToQueryAstVisitor : System.Linq.Expressions.ExpressionVisitor
		{
			readonly QueryExecutionOptions options;
			
			public ConvertToQueryAstVisitor(QueryExecutionOptions options)
			{
				this.options = options;
			}
			
			protected override Expression VisitExtension(Expression node)
			{
				// We found a query that's already converted, let's keep it as it is.
				QueryNode query = node as QueryNode;
				if (query != null)
					return query;
				else
					return base.VisitExtension(node);
			}
			
			bool IsGenericMethodInfoOfCallTreeNode(MethodCallExpression node, MethodInfo info)
			{
				return node.Method.IsGenericMethod &&
					node.Method.GetGenericMethodDefinition() == info &&
					node.Method.GetGenericArguments()[0] == typeof(CallTreeNode);
			}
			
			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (node.Method == KnownMembers.QueryableOfCallTreeNode_Select && node.Arguments[1].NodeType == ExpressionType.Quote) {
					// CallTreeNode[].Select:
					// selects are not supported by query evaluator, but we will detect and remove
					// degenerate selects (.Select(x => x)).
					UnaryExpression quote = (UnaryExpression)node.Arguments[1];
					if (quote.Operand.NodeType == ExpressionType.Lambda) {
						LambdaExpression lambda = (LambdaExpression)quote.Operand;
						if (lambda.Parameters.Count == 1 && lambda.Body == lambda.Parameters[0])
							return Visit(node.Arguments[0]);
					}
				} else if (node.Method == KnownMembers.QueryableOfCallTreeNode_Where && node.Arguments[1].NodeType == ExpressionType.Quote) {
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
				} else if (node.Method == KnownMembers.Queryable_WithQueryLog && node.Arguments[1].NodeType == ExpressionType.Constant) {
					options.AddLogger((TextWriter)(((ConstantExpression)node.Arguments[1]).Value));
					return Visit(node.Arguments[0]);
				} else if (node.Method == KnownMembers.Queryable_MergeByName) {
					QueryNode target = Visit(node.Arguments[0]) as QueryNode;
					if (target != null)
						return new MergeByName(target);
				} else if (node.Method == KnownMembers.QueryableOfCallTreeNode_Take && node.Arguments[1].NodeType == ExpressionType.Constant) {
					ConstantExpression ce = (ConstantExpression)node.Arguments[1];
					if (ce.Type == typeof(int)) {
						QueryNode target = Visit(node.Arguments[0]) as QueryNode;
						if (target != null) {
							return new Limit(target, 0, (int)ce.Value);
						}
					}
				} else if (IsGenericMethodInfoOfCallTreeNode(node, KnownMembers.Queryable_OrderBy) ||
				           IsGenericMethodInfoOfCallTreeNode(node, KnownMembers.Queryable_OrderByDesc)) {
					SortArgument sortArgument = GetSortArgumentFromSortCall(node);
					if (sortArgument != null) {
						QueryNode target = Visit(node.Arguments[0]) as QueryNode;
						if (target != null) {
							return new Sort(target, new [] { sortArgument });
						}
					}
				} else if (IsThenByCall(node)) {
					// 'ThenBy' is dangerous: we must not translate an OrderBy inside a ThenBy that we do not support
					List<MethodCallExpression> thenByCalls = new List<MethodCallExpression>();
					Expression tmp = node;
					while (IsThenByCall(tmp)) {
						thenByCalls.Add((MethodCallExpression)tmp);
						tmp = ((MethodCallExpression)tmp).Arguments[0];
					}
					MethodCallExpression mc = tmp as MethodCallExpression;
					if (mc != null &&
					    (IsGenericMethodInfoOfCallTreeNode(mc, KnownMembers.Queryable_OrderBy) ||
					     IsGenericMethodInfoOfCallTreeNode(mc, KnownMembers.Queryable_OrderByDesc)))
					{
						// TODO: add support for safe expressions in ThenBy
						
						// this is an unsupported OrderBy/ThenBy sequence
						// skip visiting the sequence and go directly into the base object
						tmp = Visit(mc.Arguments[0]);
						// now reconstruct the OrderBy/ThenBy sequence
						tmp = Expression.Call(mc.Method, new[] { tmp }.Concat(mc.Arguments.Skip(1))); // reconstruct OrderBy
						for (int i = thenByCalls.Count - 1; i >= 0; i--) {
							// reconstruct ThenBy
							tmp = Expression.Call(thenByCalls[i].Method, new[] { tmp }.Concat(thenByCalls[i].Arguments.Skip(1)));
						}
						return tmp;
					}
					// else: we couldn't detect the OrderBy belonging to this ThenBy; so it's probably not one
					// of the OrderBy overloads that we support. Go down recursively
				}
				return base.VisitMethodCall(node);
			}
			
			SortArgument GetSortArgumentFromSortCall(MethodCallExpression node)
			{
				if (node.Arguments[1].NodeType == ExpressionType.Quote) {
					UnaryExpression quote = (UnaryExpression)node.Arguments[1];
					if (quote.Operand.NodeType == ExpressionType.Lambda) {
						LambdaExpression lambda = (LambdaExpression)quote.Operand;
						if (lambda.Parameters.Count == 1) {
							SafeExpressionImporter importer = new SafeExpressionImporter(lambda.Parameters[0]);
							Expression imported = importer.Import(lambda.Body);
							if (imported != null)
								return new SortArgument(
									Expression.Lambda(imported, lambda.Parameters[0]),
									IsGenericMethodInfoOfCallTreeNode(node, KnownMembers.Queryable_OrderByDesc)
									|| IsGenericMethodInfoOfCallTreeNode(node, KnownMembers.Queryable_ThenByDesc)
								);
						}
					}
				}
				return null;
			}
			
			bool IsThenByCall(Expression potentialCall)
			{
				MethodCallExpression mc = potentialCall as MethodCallExpression;
				if (mc != null) {
					return IsGenericMethodInfoOfCallTreeNode(mc, KnownMembers.Queryable_ThenBy) ||
						IsGenericMethodInfoOfCallTreeNode(mc, KnownMembers.Queryable_ThenBy2) ||
						IsGenericMethodInfoOfCallTreeNode(mc, KnownMembers.Queryable_ThenByDesc) ||
						IsGenericMethodInfoOfCallTreeNode(mc, KnownMembers.Queryable_ThenByDesc2);
				} else {
					return false;
				}
			}
		}
		#endregion
		
		#region SafeExpressionImporter
		sealed class SafeExpressionImporter
		{
			readonly ParameterExpression callTreeNodeParameter;
			
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
			
			static object GetConstantValue(Expression expr)
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
								} else if (me.Member == KnownMembers.CallTreeNode_IsThread) {
									return Expression.Call(KnownMembers.Glob,
									                       Expression.Property(Expression.Property(callTreeNodeParameter, KnownMembers.CallTreeNode_NameMapping), KnownMembers.NameMapping_Name),
									                       Expression.Constant("Thread#*"));
								}
								
								if (me.Member == KnownMembers.CallTreeNode_CallCount)
									return me;
								
								if (me.Member == KnownMembers.CallTreeNode_CpuCyclesSpent)
									return me;
							} else if (IsNameMappingOnParameter(me.Expression)) {
								if (me.Member == KnownMembers.NameMapping_ID)
									return me;
								if (me.Member == KnownMembers.NameMapping_Name)
									return me;
							}
						}
						return null;
					case ExpressionType.Call:
						{
							MethodCallExpression mc = (MethodCallExpression)expr;
							
							if (mc.Method == KnownMembers.String_StartsWith)
								return CreateStringMatchCall(mc, (pattern, wildcard) => pattern + wildcard);
							if (mc.Method == KnownMembers.String_EndsWith)
								return CreateStringMatchCall(mc, (pattern, wildcard) => wildcard + pattern);
							
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

			Expression CreateStringMatchCall(MethodCallExpression mc, Func<string, string, string> expressionOf)
			{
				Expression imported = Import(mc.Object);
				if (imported != null && mc.Arguments[0].NodeType == ExpressionType.Constant && mc.Arguments[1].NodeType == ExpressionType.Constant) {
					StringComparison cmp = (StringComparison)GetConstantValue(mc.Arguments[1]);
					string pattern = (string)GetConstantValue(mc.Arguments[0]);
					
					if (cmp == StringComparison.Ordinal && pattern.IndexOfAny(forbiddenGlobChars) == -1)
						return Expression.Call(KnownMembers.Glob, imported, Expression.Constant(expressionOf(pattern, "*")));
					else if (cmp == StringComparison.OrdinalIgnoreCase)
						return Expression.Call(KnownMembers.Like, imported, Expression.Constant(expressionOf(EscapeLikeExpr(pattern, "\\"), "%")));
				}
				
				return null;
			}
			
			static string EscapeLikeExpr(string expression, string escape)
			{
				return expression.Replace(escape, escape + escape)
					.Replace("%", escape + "%")
					.Replace("_", escape + "_");
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
			readonly QueryExecutionOptions options;
			
			public ExecuteAllQueriesVisitor(SQLiteQueryProvider sqliteProvider, QueryExecutionOptions options)
			{
				this.sqliteProvider = sqliteProvider;
				this.options = options;
			}
			
			protected override Expression VisitExtension(Expression node)
			{
				QueryNode query = node as QueryNode;
				if (query != null)
					return Expression.Constant(query.Execute(sqliteProvider, options));
				else
					return base.VisitExtension(node);
			}
		}
		#endregion
	}
	
	sealed class QueryExecutionOptions
	{
		List<TextWriter> loggers = new List<TextWriter>();
		
		public void AddLogger(TextWriter w)
		{
			if (w != null)
				loggers.Add(w);
		}
		
		public bool HasLoggers {
			get { return loggers.Count > 0; }
		}
		
		public void WriteLogLine(string text)
		{
			foreach (TextWriter w in loggers)
				w.WriteLine(text);
		}
	}
}
