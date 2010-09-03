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
	class SortArgument
	{
		readonly LambdaExpression arg;
		readonly bool desc;
		
		public SortArgument(LambdaExpression arg, bool desc)
		{
			this.arg = arg;
			this.desc = desc;
		}
		
		public bool Descending {
			get { return desc; }
		}
		public LambdaExpression Argument {
			get { return arg; }
		}
		
		public override string ToString()
		{
			if (Descending)
				return Argument.ToString() + " DESC";
			else
				return Argument.ToString();
		}
	}
	
	sealed class Sort : QueryNode
	{
		ReadOnlyCollection<SortArgument> arguments;
		
		public Sort(QueryNode target, IList<SortArgument> args)
			: base(target)
		{
			this.arguments = new ReadOnlyCollection<SortArgument>(args);
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			SqlStatementKind kind = Target.BuildSql(b, context);
			if (kind == SqlStatementKind.SelectOrderBy)
				WrapSqlIntoNestedStatement(b, context);
			
			b.Append(" ORDER BY ");
			
			for (int i = 0; i < arguments.Count; i++) {
				StringWriter w = new StringWriter();
				ExpressionSqlWriter writer = new ExpressionSqlWriter(w, context, arguments[i].Argument.Parameters[0]);
				writer.Write(arguments[i].Argument.Body);
				
				if (i == 0)
					b.Append(w.ToString());
				else
					b.Append(", " + w.ToString());
				
				if (arguments[i].Descending)
					b.Append(" DESC");
				else
					b.Append(" ASC");
			}
			
			return SqlStatementKind.SelectOrderBy;
		}
		
		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			QueryNode newTarget = (QueryNode)visitor.Visit(Target);
			if (newTarget == Target)
				return this;
			else
				return new Sort(newTarget, arguments);
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < arguments.Count; i++) {
				if (i > 0)
					builder.Append(", ");
				builder.Append(arguments[i].ToString());
			}
			return Target + ".Sort(" + builder.ToString() + ")";
		}
	}
}
