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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	/// <summary>
	/// Query node that limits the amount of data selected. Produces LIMIT in SQL.
	/// </summary>
	sealed class Limit : QueryNode
	{
		public int Start { get; private set; }
		public int Length { get; private set; }
		
		public Limit(QueryNode target, int start, int length)
			: base(target)
		{
			this.Start = start;
			this.Length = length;
		}
		
		protected override Expression VisitChildren(Func<Expression, Expression> visitor)
		{
			QueryNode newTarget = (QueryNode)visitor(Target);
			if (newTarget == Target)
				return this;
			else
				return new Limit(newTarget, Start, Length);
		}
		
		public override string ToString()
		{
			return Target + ".Limit(" + Start + "," + Length + ")";
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			SqlStatementKind kind = Target.BuildSql(b, context);
			if (kind == SqlStatementKind.SelectLimit)
				WrapSqlIntoNestedStatement(b, context);
			
			b.Append(" LIMIT " + Length);
			b.AppendLine(" OFFSET " + Start);
			
			return SqlStatementKind.SelectLimit;
		}
	}
}
