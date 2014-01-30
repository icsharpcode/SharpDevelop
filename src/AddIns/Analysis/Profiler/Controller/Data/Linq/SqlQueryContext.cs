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
using System.Globalization;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	sealed class SqlQueryContext
	{
		public readonly ProfilingDataSQLiteProvider.SQLiteDataSet StartDataSet;
		public readonly ProfilingDataSQLiteProvider.SQLiteDataSet EndDataSet;
		
		public CallTreeNodeSqlNameSet CurrentNameSet { get; private set; }
		
		/// <summary>
		/// The type of the table currently being accessed (the current FROM clause).
		/// Is 'None' when reading from an inner query.
		/// </summary>
		public SqlTableType CurrentTable { get; private set; }
		
		/// <summary>
		/// Passed down the query tree to signalize that the ID list is required.
		/// </summary>
		public bool RequireIDList;
		
		/// <summary>
		/// Passed up the query tree to signalize whether an ID list is present.
		/// </summary>
		public bool HasIDList { get; private set; }
		
		public void SetCurrent(CallTreeNodeSqlNameSet nameSet, SqlTableType table, bool hasIDList)
		{
			CurrentNameSet = nameSet;
			CurrentTable = table;
			HasIDList = hasIDList;
		}
		
		SQLiteQueryProvider provider;
		
		public SqlQueryContext(SQLiteQueryProvider provider)
		{
			this.provider = provider;
			this.StartDataSet = provider.StartDataSet;
			this.EndDataSet = provider.EndDataSet;
		}
		
		public ProfilingDataSQLiteProvider.SQLiteDataSet FindDataSetById(int id)
		{
			return provider.FindDataSetById(id);
		}
		
		int uniqueVariableIndex;
		
		public string GenerateUniqueVariableName()
		{
			return "v" + (++uniqueVariableIndex).ToString(CultureInfo.InvariantCulture);
		}
	}
	
	enum SqlTableType
	{
		/// <summary>
		/// No direct table
		/// </summary>
		None,
		/// <summary>
		/// The Calls table
		/// </summary>
		Calls
	}
	
	sealed class CallTreeNodeSqlNameSet
	{
		public readonly string ID;
		public readonly string NameID;
		public readonly string CpuCyclesSpent;
		public readonly string CpuCyclesSpentSelf;
		public readonly string CallCount;
		public readonly string HasChildren;
		public readonly string ActiveCallCount;
		
		public CallTreeNodeSqlNameSet(SqlQueryContext c)
		{
			string prefix = c.GenerateUniqueVariableName();
			ID = prefix + "ID";
			NameID = prefix + "NameID";
			CpuCyclesSpent = prefix + "CpuCyclesSpent";
			CpuCyclesSpentSelf = prefix + "CpuCyclesSpentSelf";
			CallCount = prefix + "CallCount";
			HasChildren = prefix + "HasChildren";
			ActiveCallCount = prefix + "ActiveCallCount";
		}
	}
}
