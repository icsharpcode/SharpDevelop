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
	/// Query AST node representing the whole 'Calls' table.
	/// This is the source of all queries.
	/// Produces SELECT .. FROM .. in SQL.
	/// </summary>
	sealed class AllCalls : QueryNode
	{
		public static readonly AllCalls Instance = new AllCalls();
		
		private AllCalls() : base(null)
		{
		}
		
		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			return this;
		}
		
		public override string ToString()
		{
			return "AllCalls";
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			CallTreeNodeSqlNameSet newNames = new CallTreeNodeSqlNameSet(context);
			context.SetCurrent(newNames, SqlTableType.Calls, true);
			
			b.AppendLine("SELECT "
			             + SqlAs("nameid", newNames.NameID) + ", "
			             + SqlAs("cpucyclesspent", newNames.CpuCyclesSpent) + ", "
			             + SqlAs("cpucyclesspentself", newNames.CpuCyclesSpentSelf) + ", "
			             + SqlAs("callcount", newNames.CallCount) + ", "
			             + SqlAs("(id != endid)", newNames.HasChildren) + ", "
			             + SqlAs("((id BETWEEN " + context.StartDataSet.RootID + " AND " + context.StartDataSet.CallEndID
			                     + ") AND isActiveAtStart)", newNames.ActiveCallCount) + ", "
			             + SqlAs("id", newNames.ID));
			b.AppendLine("FROM Calls");
			return SqlStatementKind.Select;
		}
	}
}
