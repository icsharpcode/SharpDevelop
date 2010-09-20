// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	/// <summary>
	/// Query node that filters the input using conditions. Produces WHERE or HAVING in SQL.
	/// </summary>
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
		
		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			QueryNode newTarget = (QueryNode)visitor.Visit(Target);
			LambdaExpression[] newConditions = new LambdaExpression[Conditions.Count];
			bool unchanged = (newTarget == Target);
			for (int i = 0; i < newConditions.Length; i++) {
				newConditions[i] = (LambdaExpression)visitor.Visit(Conditions[i]);
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
		
		static void BuildSqlForCondition(StringBuilder b, SqlQueryContext context, LambdaExpression condition)
		{
			Debug.Assert(condition.Parameters.Count == 1);
			StringWriter w = new StringWriter(CultureInfo.InvariantCulture);
			ExpressionSqlWriter writer = new ExpressionSqlWriter(w, context, condition.Parameters[0]);
			writer.Write(condition.Body);
			b.Append(w.ToString());
		}
	}
}
