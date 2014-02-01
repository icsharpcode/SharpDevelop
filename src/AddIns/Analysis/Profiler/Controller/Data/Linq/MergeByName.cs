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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
		
		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			QueryNode newTarget = (QueryNode)visitor.Visit(Target);
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
