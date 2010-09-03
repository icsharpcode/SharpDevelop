// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq.Expressions;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	/// <summary>
	/// Query AST node representing the whole 'Functions' table.
	/// This is the source of all queries.
	/// Produces SELECT .. FROM .. in SQL.
	/// </summary>
	sealed class AllFunctions : QueryNode
	{
		public readonly int StartDataSet, EndDataSet;
		
		public AllFunctions() : base(null)
		{
			this.StartDataSet = -1;
			this.EndDataSet = -1;
		}
		
		public AllFunctions(int startDataSet, int endDataSet) : base(null)
		{
			this.StartDataSet = startDataSet;
			this.EndDataSet = endDataSet;
		}
		
		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			return this;
		}
		
		public override string ToString()
		{
			if (StartDataSet < 0)
				return "AllFunctions";
			else
				return "AllFunctions(" + StartDataSet + ", " + EndDataSet + ")";
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			if (context.RequireIDList)
				throw new NotSupportedException();
			
			CallTreeNodeSqlNameSet newNames = new CallTreeNodeSqlNameSet(context);
			context.SetCurrent(newNames, SqlTableType.None, false);
			
			b.AppendLine("SELECT "
			             + SqlAs("nameid", newNames.NameID) + ", "
			             + SqlAs("SUM(cpucyclesspent)", newNames.CpuCyclesSpent) + ", "
			             + SqlAs("SUM(cpucyclesspentself)", newNames.CpuCyclesSpentSelf) + ", "
			             + SqlAs("SUM(callcount)", newNames.CallCount) + ", "
			             + SqlAs("MAX(hasChildren)", newNames.HasChildren) + ", "
			             + SqlAs("SUM(CASE WHEN datasetid = " + context.StartDataSet.ID + " THEN activecallcount ELSE 0 END)", newNames.ActiveCallCount));
			b.AppendLine("FROM Functions");
			if (StartDataSet >= 0)
				b.AppendLine("WHERE datasetid BETWEEN " + StartDataSet + " AND " + EndDataSet);
			b.AppendLine("GROUP BY nameid");
			return SqlStatementKind.SelectGroupBy;
		}
	}
}
