// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
		
		protected override ExpressionType NodeTypeImpl()
		{
			return ExpressionType.Extension;
		}
		
		protected override Type TypeImpl()
		{
			return typeof(IQueryable<CallTreeNode>);
		}
		
		protected abstract override Expression VisitChildren(Func<Expression, Expression> visitor);
		
		/// <summary>
		/// SQL construction documentation see SQLiteQueryProvider documentation.
		/// </summary>
		public abstract SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context);
		
		/// <summary>
		/// Wraps the current SQL statement into an inner select, allowing to continue with "WHERE" queries
		/// even after ORDER BY or LIMIT.
		/// </summary>
		protected void WrapSqlIntoNestedStatement(StringBuilder b, SqlQueryContext context)
		{
			CallTreeNodeSqlNameSet oldNames = context.CurrentNameSet;
			CallTreeNodeSqlNameSet newNames = new CallTreeNodeSqlNameSet(context, false);
			b.Insert(0, "SELECT " + SqlAs(oldNames.ID, newNames.ID) + ", "
			         + SqlAs(oldNames.NameID, newNames.NameID) + ", "
			         + SqlAs(oldNames.TimeSpent, newNames.TimeSpent) + ", "
			         + SqlAs(oldNames.CallCount, newNames.CallCount) + ", "
			         + SqlAs(oldNames.HasChildren, newNames.HasChildren) + ", "
			         + SqlAs(oldNames.ActiveCallCount, newNames.ActiveCallCount)
			         + Environment.NewLine + "FROM (" + Environment.NewLine);
			b.AppendLine(")");
			context.CurrentNameSet = newNames;
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
		
		public virtual IQueryable<CallTreeNode> Execute(SQLiteQueryProvider provider)
		{
			StringBuilder b = new StringBuilder();
			BuildSql(b, new SqlQueryContext(provider));
			Console.WriteLine(b.ToString());
			return provider.RunSQL(b.ToString()).AsQueryable();
		}
	}
	
	sealed class AllCalls : QueryNode
	{
		public static readonly AllCalls Instance = new AllCalls();
		
		private AllCalls() : base(null)
		{
		}
		
		protected override Expression VisitChildren(Func<Expression, Expression> visitor)
		{
			return this;
		}
		
		public override string ToString()
		{
			return "AllCalls";
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			CallTreeNodeSqlNameSet newNames = context.CurrentNameSet = new CallTreeNodeSqlNameSet(context, true);
			
			b.AppendLine("SELECT "
			             + SqlAs("id", newNames.ID) + ", "
			             + SqlAs("nameid", newNames.NameID) + ", "
			             + SqlAs("timespent", newNames.TimeSpent) + ", "
			             + SqlAs("callcount", newNames.CallCount) + ", "
			             + SqlAs("(id != endid)", newNames.HasChildren) + ", "
			             + SqlAs("((datasetid = " + context.StartDataSetID + ") AND isActiveAtStart)", newNames.ActiveCallCount));
			b.AppendLine("FROM FunctionData");
			return SqlStatementKind.Select;
		}
	}
	
	sealed class MergeByName : QueryNode
	{
		public MergeByName(QueryNode target)
			: base(target)
		{
			Debug.Assert(target != null);
		}
		
		protected override Expression VisitChildren(Func<Expression, Expression> visitor)
		{
			QueryNode newTarget = (QueryNode)visitor(Target);
			if (newTarget == Target)
				return this;
			else
				return new MergeByName(newTarget);
		}
		
		public override string ToString()
		{
			return Target + ".MergeByName()";
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			Target.BuildSql(b, context);
			
			CallTreeNodeSqlNameSet oldNames = context.CurrentNameSet;
			CallTreeNodeSqlNameSet newNames = new CallTreeNodeSqlNameSet(context, false);
			b.Insert(0, "SELECT "
			         + SqlAs("GROUP_CONCAT(" + oldNames.ID + ")", newNames.ID) + ", "
			         + SqlAs(oldNames.NameID, newNames.NameID) + ", "
			         + SqlAs("SUM(" + oldNames.TimeSpent + ")", newNames.TimeSpent) + ", "
			         + SqlAs("SUM(" + oldNames.CallCount + ")", newNames.CallCount) + ", "
			         + SqlAs("MAX(" + oldNames.HasChildren + ")", newNames.HasChildren) + ", "
			         + SqlAs("SUM(" + oldNames.ActiveCallCount + ")", newNames.ActiveCallCount)
			         + Environment.NewLine + "FROM (" + Environment.NewLine);
			b.AppendLine(") GROUP BY " + oldNames.NameID);
			context.CurrentNameSet = newNames;
			
			return SqlStatementKind.SelectGroupBy;
		}
	}
	
	sealed class Filter : QueryNode
	{
		/// <summary>
		/// List of conditions. The operator between these is AND.
		/// </summary>
		public readonly ReadOnlyCollection<LambdaExpression> Conditions;
		
		public Filter(QueryNode target, params LambdaExpression[] conditions)
			: base(target)
		{
			Debug.Assert(target != null);
			foreach (LambdaExpression l in conditions) {
				Debug.Assert(l.Parameters.Count == 1);
			}
			this.Conditions = Array.AsReadOnly(conditions);
		}
		
		protected override Expression VisitChildren(Func<Expression, Expression> visitor)
		{
			QueryNode newTarget = (QueryNode)visitor(Target);
			LambdaExpression[] newConditions = new LambdaExpression[Conditions.Count];
			bool unchanged = (newTarget == Target);
			for (int i = 0; i < newConditions.Length; i++) {
				newConditions[i] = (LambdaExpression)visitor(Conditions[i]);
				unchanged &= newConditions[i] == Conditions[i];
			}
			if (unchanged)
				return this;
			else
				return new Filter(newTarget, newConditions);
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.Append(Target.ToString());
			b.Append(".Filter(");
			for (int i = 0; i < Conditions.Count; i++) {
				if (i > 0)
					b.Append(" && ");
				b.Append(Conditions[i].ToString());
			}
			b.Append(')');
			return b.ToString();
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			SqlStatementKind kind = Target.BuildSql(b, context);
			SqlStatementKind resultKind;
			switch (kind) {
				case SqlStatementKind.Select:
					b.Append(" WHERE ");
					resultKind = SqlStatementKind.SelectWhere;
					break;
				case SqlStatementKind.SelectGroupBy:
					b.Append(" HAVING ");
					resultKind = SqlStatementKind.SelectGroupByHaving;
					break;
				default:
					WrapSqlIntoNestedStatement(b, context);
					b.Append(" WHERE ");
					resultKind = SqlStatementKind.SelectWhere;
					break;
			}
			for (int i = 0; i < Conditions.Count; i++) {
				if (i > 0)
					b.Append(" AND ");
				BuildSqlForCondition(b, context, Conditions[i]);
			}
			b.AppendLine();
			return resultKind;
		}
		
		void BuildSqlForCondition(StringBuilder b, SqlQueryContext context, LambdaExpression condition)
		{
			Debug.Assert(condition.Parameters.Count == 1);
			StringWriter w = new StringWriter();
			ExpressionSqlWriter writer = new ExpressionSqlWriter(w, context.CurrentNameSet, condition.Parameters[0]);
			writer.Write(condition.Body);
			b.Append(w.ToString());
		}
	}
}
