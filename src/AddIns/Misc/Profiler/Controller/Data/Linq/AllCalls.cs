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
	/// Query AST node representing the whole 'FunctionData' table.
	/// This is the source of all queries.
	/// Produces SELECT .. FROM .. in SQL.
	/// </summary>
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
			CallTreeNodeSqlNameSet newNames = new CallTreeNodeSqlNameSet(context);
			context.SetCurrent(newNames, SqlTableType.Calls, true);
			
			b.AppendLine("SELECT "
			             + SqlAs("nameid", newNames.NameID) + ", "
			             + SqlAs("timespent", newNames.CpuCyclesSpent) + ", " // TODO : change to CpuCyclesSpent
			             + SqlAs("callcount", newNames.CallCount) + ", "
			             + SqlAs("(id != endid)", newNames.HasChildren) + ", "
			             + SqlAs("((datasetid = " + context.StartDataSetID + ") AND isActiveAtStart)", newNames.ActiveCallCount) + ", "
			             + SqlAs("id", newNames.ID));
			b.AppendLine("FROM FunctionData");
			return SqlStatementKind.Select;
		}
	}
}
