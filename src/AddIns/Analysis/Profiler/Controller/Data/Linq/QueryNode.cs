// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	/// <summary>
	/// The SingleCall class is never instanciated; it only used to represent database rows
	/// inside expression trees.
	/// </summary>
	abstract class SingleCall
	{
		// Warning CS0649: Field is never assigned to, and will always have its default value 0
		#pragma warning disable 649
		public int ID;
		public int ParentID;
		public int DataSetID;
		#pragma warning restore 649
		
		public static readonly FieldInfo IDField = typeof(SingleCall).GetField("ID");
		public static readonly FieldInfo ParentIDField = typeof(SingleCall).GetField("ParentID");
		public static readonly FieldInfo DataSetIdField = typeof(SingleCall).GetField("DataSetID");
	}
	
	/// <summary>
	/// Base class for nodes in the Query AST.
	/// </summary>
	abstract class QueryNode : Expression
	{
		public enum SqlStatementKind
		{
			Select,
			SelectWhere,
			SelectGroupBy,
			SelectGroupByHaving,
			SelectOrderBy,
			SelectLimit
		}
		
		public readonly QueryNode Target;
		
		protected QueryNode(QueryNode target)
		{
			this.Target = target;
		}
		
		public override ExpressionType NodeType {
			get { return ExpressionType.Extension; }
		}
		
		public override Type Type {
			get { return typeof(IQueryable<CallTreeNode>); }
		}
		
		protected abstract override Expression VisitChildren(ExpressionVisitor visitor);
		
		/// <summary>
		/// SQL construction documentation see SQLiteQueryProvider documentation.
		/// </summary>
		public abstract SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context);
		
		/// <summary>
		/// Wraps the current SQL statement into an inner select, allowing to continue with "WHERE" queries
		/// even after ORDER BY or LIMIT.
		/// </summary>
		protected static void WrapSqlIntoNestedStatement(StringBuilder b, SqlQueryContext context)
		{
			CallTreeNodeSqlNameSet oldNames = context.CurrentNameSet;
			CallTreeNodeSqlNameSet newNames = new CallTreeNodeSqlNameSet(context);
			
			string query = "SELECT "
				+ SqlAs(oldNames.NameID, newNames.NameID) + ", "
				+ SqlAs(oldNames.CpuCyclesSpent, newNames.CpuCyclesSpent) + ", "
				+ SqlAs(oldNames.CpuCyclesSpentSelf, newNames.CpuCyclesSpentSelf) + ", "
				+ SqlAs(oldNames.CallCount, newNames.CallCount) + ", "
				+ SqlAs(oldNames.HasChildren, newNames.HasChildren) + ", "
				+ SqlAs(oldNames.ActiveCallCount, newNames.ActiveCallCount);
			if (context.HasIDList) {
				query += ", " + SqlAs(oldNames.ID, newNames.ID);
			}
			query += Environment.NewLine + "FROM (" + Environment.NewLine;
			b.Insert(0, query);
			b.AppendLine(")");
			context.SetCurrent(newNames, SqlTableType.None, context.HasIDList);
		}
		
		/// <summary>
		/// Helper function that builds the text 'expression AS newName'
		/// </summary>
		protected static string SqlAs(string expression, string newName)
		{
			if (expression == newName)
				return newName;
			else
				return expression + " AS " + newName;
		}
		
		public IQueryable<CallTreeNode> Execute(SQLiteQueryProvider provider, QueryExecutionOptions options)
		{
			StringBuilder b = new StringBuilder();
			SqlQueryContext context = new SqlQueryContext(provider);
			BuildSql(b, context);
			if (options.HasLoggers)
				options.WriteLogLine(b.ToString());
			Stopwatch w = Stopwatch.StartNew();
			IList<CallTreeNode> result = provider.RunSQLNodeList(b.ToString(), context.HasIDList);
			w.Stop();
			if (options.HasLoggers) {
				options.WriteLogLine("Query returned " + result.Count + " rows in " + w.Elapsed);
			}
			return result.AsQueryable();
		}
	}
}
