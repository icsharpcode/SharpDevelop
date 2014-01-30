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
using System.Linq;
using System.Linq.Expressions;
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
		
		AllCalls() : base(null)
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
