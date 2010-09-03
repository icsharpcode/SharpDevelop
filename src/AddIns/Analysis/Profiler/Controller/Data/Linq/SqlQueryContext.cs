// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			this.CurrentNameSet = nameSet;
			this.CurrentTable = table;
			this.HasIDList = hasIDList;
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
