// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using ICSharpCode.Profiler.Interprocess;
using System.Threading;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Writes data to a SQLite Database.
	/// Instance members of this class are not thread-safe.
	/// </summary>
	public sealed class ProfilingDataSQLiteWriter : IProfilingDataWriter, IDisposable
	{
		SQLiteConnection connection;
		int dataSetCount = -1;
		int functionInfoCount;
		bool isDisposed;
		int processorFrequency;
		
		/// <summary>
		/// Creates a new SQLite profiling data provider and opens or creates a new database stored in a file.
		/// </summary>
		public ProfilingDataSQLiteWriter(string fileName)
		{
			if (File.Exists(fileName))
				throw new IOException("File already exists!");
			
			SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
			conn.Add("Data Source", fileName);
			conn.Add("New", true);
			// Disable protecting the database on crashes - it's a new database,
			// it may go corrupt if we crash during DB creation. Disabling journalling
			// makes Inserts faster.
			conn.Add("Journal Mode", "OFF");
			conn.Add("Synchronous", "OFF");
			this.connection = new SQLiteConnection(conn.ConnectionString);
			
			this.connection.Open();
			
			InitializeTables();
			
			File.SetAttributes(fileName, FileAttributes.Compressed);
		}
		
		/// <summary>
		/// Closes and disposes the database.
		/// </summary>
		public void Close()
		{
			using (SQLiteCommand cmd = this.connection.CreateCommand()) {
				// create index at the end (after inserting data), this is faster
				cmd.CommandText = CallsAndFunctionsIndexDefs;
				cmd.ExecuteNonQuery();
			}
			
			this.Dispose();
		}
		
		/// <summary>
		/// Sets or gets the processor frequency of the computer, where the profiling session was created.
		/// The processor frequency is measured in MHz.
		/// </summary>
		public int ProcessorFrequency {
			get {
				return this.processorFrequency;
			}
			set {
				processorFrequency = value;
				ProfilingDataSQLiteProvider.SetProperty(this.connection.CreateCommand(), "processorfrequency", value.ToString(CultureInfo.InvariantCulture));
			}
		}
		
		/// <summary>
		/// Writes a profiling dataset to the database.
		/// </summary>
		public void WriteDataSet(IProfilingDataSet dataSet)
		{
			if (dataSet == null)
				throw new ArgumentNullException("dataSet");
			
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				SQLiteCommand cmd = this.connection.CreateCommand();
				
				if (dataSetCount == -1)
					dataSetCount = 0;
				
				cmd.Parameters.Add(new SQLiteParameter("id", dataSetCount));
				cmd.Parameters.Add(new SQLiteParameter("cpuuage", dataSet.CpuUsage.ToString(CultureInfo.InvariantCulture)));
				cmd.Parameters.Add(new SQLiteParameter("isfirst", dataSet.IsFirst));
				cmd.Parameters.Add(new SQLiteParameter("rootid", functionInfoCount));
				
				cmd.CommandText = "INSERT INTO DataSets(id, cpuusage, isfirst, rootid)" +
					"VALUES(?,?,?,?);";
				
				int dataSetStartId = functionInfoCount;
				
				using (SQLiteCommand loopCommand = this.connection.CreateCommand()) {
					CallTreeNode node = dataSet.RootNode;
					
					loopCommand.CommandText = "INSERT INTO Calls(id, endid, parentid, nameid, cpucyclesspent, cpucyclesspentself, isactiveatstart, callcount)" +
						"VALUES(?,?,?,?,?,?,?,?);";
					
					CallsParams dataParams = new CallsParams();
					loopCommand.Parameters.Add(dataParams.functionInfoId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.endId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.parentId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.nameId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.cpuCyclesSpent = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.cpuCyclesSpentSelf = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.isActiveAtStart = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.callCount = new SQLiteParameter());

					InsertCalls(loopCommand, node, -1, dataParams);
				}
				
				using (SQLiteCommand functionsCommand = this.connection.CreateCommand()) {
					functionsCommand.CommandText = string.Format(@"
						INSERT INTO Functions
						SELECT {0}, nameid, SUM(cpucyclesspent), SUM(cpucyclesspentself), SUM(isactiveatstart), SUM(callcount), MAX(id != endid)
	 					FROM Calls
	 					WHERE id BETWEEN {1} AND {2}
	 					GROUP BY nameid;", dataSetCount, dataSetStartId, functionInfoCount - 1);
					
					functionsCommand.ExecuteNonQuery();
				}
				
				cmd.ExecuteNonQuery();
				dataSetCount++;
				
				transaction.Commit();
			}
		}
		
		internal const string CallsAndFunctionsTableDefs = @"
			CREATE TABLE Calls (
				id INTEGER NOT NULL PRIMARY KEY,
				endid INTEGER NOT NULL,
				parentid INTEGER NOT NULL,
				nameid INTEGER NOT NULL,
				cpucyclesspent INTEGER NOT NULL,
				cpucyclesspentself INTEGER NOT NULL,
				isactiveatstart INTEGER NOT NULL,
				callcount INTEGER NOT NULL
			);
			CREATE TABLE Functions (
				datasetid INTEGER NOT NULL,
				nameid INTEGER NOT NULL,
				cpucyclesspent INTEGER NOT NULL,
				cpucyclesspentself INTEGER NOT NULL,
				activecallcount INTEGER NOT NULL,
				callcount INTEGER NOT NULL,
				hasChildren INTEGER NOT NULL
			);";
		
		internal const string CallsAndFunctionsIndexDefs =
			"CREATE INDEX CallsParent ON Calls(parentid ASC);" // required for searching the children
			+ " ANALYZE;"; // make SQLite analyze the indices available; this will help the query planner later
		
		void InitializeTables()
		{
			// NameMapping { Id, ReturnType, Name, Parameters }
			// Calls { id, endid, parentid, nameid, cpucyclesspent, cpucyclesspentself, isactiveatstart, callcount }
			// Functions { datasetid, nameid, cpucyclesspent, cpucyclesspentself, activecallcount, callcount, haschildren }
			// DataSets { Id, CPUUsage, RootId }
			//
			// NameMapping.Id <-> FunctionData.NameId 1:N
			// FunctionData.ParentId <-> FunctionData.Id 1:N
			
			SQLiteCommand cmd = this.connection.CreateCommand();
			
			cmd.CommandText = CallsAndFunctionsTableDefs + @"
			
				CREATE TABLE NameMapping(
					id INTEGER NOT NULL PRIMARY KEY,
					returntype TEXT NOT NULL,
					name TEXT NOT NULL,
					parameters TEXT NOT NULL
				);
				
				/* for CREATE TABLE of Calls and Functions see CallsAndFunctionsTableDefs */
				
				CREATE TABLE DataSets(
					id INTEGER NOT NULL PRIMARY KEY,
					cpuusage REAL NOT NULL,
					isfirst INTEGER NOT NULL,
					rootid INTEGER NOT NULL
				);
				
				CREATE TABLE Properties(
					name TEXT NOT NULL PRIMARY KEY,
					value TEXT NOT NULL
				);
				
				INSERT INTO Properties(name, value) VALUES('version', '1.1');
				
				CREATE TABLE PerformanceCounter(
					id INTEGER NOT NULL PRIMARY KEY,
					name TEXT NOT NULL
				);
				
				CREATE TABLE CounterData(
					datasetid INTEGER NOT NULL,
					counterid INTEGER NOT NULL,
					value INTEGER NOT NULL
				);
	";
			
			cmd.ExecuteNonQuery();
		}
		
		class CallsParams
		{
			public SQLiteParameter functionInfoId,
			parentId, nameId, cpuCyclesSpent, cpuCyclesSpentSelf,
			isActiveAtStart, callCount, endId;
		}
		
		void InsertCalls(SQLiteCommand cmd, CallTreeNode node, int parentId, CallsParams dataParams)
		{
			int thisID = functionInfoCount++;
			
			foreach (CallTreeNode child in node.Children) {
				InsertCalls(cmd, child, thisID, dataParams);
			}
			
			// we sometimes saw invalid data with the 0x0080000000000000L bit set
			if (node.CpuCyclesSpent > 0x0007ffffffffffffL || node.CpuCyclesSpent < 0) {
				throw new InvalidOperationException("Too large CpuCyclesSpent - there's something wrong in the data");
			}
			
			if (node.NameMapping.Id != 0 && (node.CpuCyclesSpentSelf > node.CpuCyclesSpent || node.CpuCyclesSpentSelf < 0)) {
				throw new InvalidOperationException("Too large/small CpuCyclesSpentSelf (" + node.CpuCyclesSpentSelf + ") - there's something wrong in the data");
			}
			
			dataParams.callCount.Value = node.RawCallCount;
			dataParams.isActiveAtStart.Value = node.IsActiveAtStart;
			dataParams.cpuCyclesSpent.Value = node.CpuCyclesSpent;
			dataParams.cpuCyclesSpentSelf.Value = node.CpuCyclesSpentSelf;

			dataParams.functionInfoId.Value = thisID;
			dataParams.nameId.Value = node.NameMapping.Id;
			dataParams.parentId.Value = parentId;
			dataParams.endId.Value = functionInfoCount - 1;
			
			cmd.ExecuteNonQuery();
		}
		
		/// <summary>
		/// Writes a name mapping to the database.
		/// </summary>
		public void WriteMappings(IEnumerable<NameMapping> mappings)
		{
			using (SQLiteTransaction trans = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					SQLiteParameter idParam = new SQLiteParameter("id");
					SQLiteParameter retTParam = new SQLiteParameter("returntype");
					SQLiteParameter nameParam = new SQLiteParameter("name");
					SQLiteParameter paramsParam = new SQLiteParameter("parameters");
					
					cmd.CommandText = "INSERT INTO NameMapping(id, returntype, name, parameters)" +
						"VALUES(?,?,?,?);";
					
					cmd.Parameters.AddRange(new SQLiteParameter[] { idParam, retTParam, nameParam, paramsParam });
					
					foreach (NameMapping mapping in mappings) {
						idParam.Value = mapping.Id;
						retTParam.Value = mapping.ReturnType;
						nameParam.Value = mapping.Name;
						paramsParam.Value = ((mapping.Parameters != null) ? string.Join("-", mapping.Parameters.ToArray()) : "");
						
						cmd.ExecuteNonQuery();
					}
				}
				trans.Commit();
			}
		}
		
		/// <summary>
		/// Closes the connection to the database.
		/// </summary>
		public void Dispose()
		{
			if (!isDisposed)
				this.connection.Close();
			
			isDisposed = true;
		}
	}
}