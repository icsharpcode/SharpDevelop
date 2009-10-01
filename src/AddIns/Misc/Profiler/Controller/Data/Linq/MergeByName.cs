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
	/// Query node that represents the 'group by nameid and merge' operation. Produces SELECT ... GROUP BY .. in SQL.
	/// </summary>
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
			CallTreeNodeSqlNameSet newNames = new CallTreeNodeSqlNameSet(context);
			
			string query = "SELECT "
				+ SqlAs(oldNames.NameID, newNames.NameID) + ", "
				+ SqlAs("SUM(" + oldNames.CpuCyclesSpent + ")", newNames.CpuCyclesSpent) + ", "
				+ SqlAs("SUM(" + oldNames.CpuCyclesSpentSelf + ")", newNames.CpuCyclesSpentSelf) + ", "
				+ SqlAs("SUM(" + oldNames.CallCount + ")", newNames.CallCount) + ", "
				+ SqlAs("MAX(" + oldNames.HasChildren + ")", newNames.HasChildren) + ", "
				+ SqlAs("SUM(" + oldNames.ActiveCallCount + ")", newNames.ActiveCallCount);
			if (context.HasIDList) {
				query += ", " + SqlAs("GROUP_CONCAT(" + oldNames.ID + ")", newNames.ID);
			}
			query += Environment.NewLine + "FROM (" + Environment.NewLine;
			b.Insert(0, query);
			b.AppendLine(") GROUP BY " + oldNames.NameID);
			context.SetCurrent(newNames, SqlTableType.None, context.HasIDList);
			
			return SqlStatementKind.SelectGroupBy;
		}
	}
}
