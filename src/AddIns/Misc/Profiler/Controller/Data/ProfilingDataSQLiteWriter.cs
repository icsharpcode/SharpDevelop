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
		bool profileUnitTests;
		string[] unitTestNames;
		
		/// <summary>
		/// Creates a new SQLite profiling data provider and opens or creates a new database stored in a file.
		/// </summary>
		public ProfilingDataSQLiteWriter(string fileName, bool profileUnitTests, string[] unitTestNames)
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
			
			this.profileUnitTests = profileUnitTests;
			this.unitTestNames = unitTestNames;
			
			if (profileUnitTests && unitTestNames == null)
				throw new InvalidOperationException("Please add unit tests to filter!");
		}
		
		/// <summary>
		/// Closes and disposes the database.
		/// </summary>
		public void Close()
		{
			using (SQLiteCommand cmd = this.connection.CreateCommand()) {
				cmd.CommandText = @"CREATE INDEX Parents ON FunctionData(parentid ASC);";
				cmd.ExecuteNonQuery();
			}
			
			this.Dispose();
		}
		
		/// <summary>
		/// Sets or gets the processor frequency of the computer, where the profiling session was created.
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
				
				using (SQLiteCommand loopCommand = this.connection.CreateCommand()) {
					CallTreeNode node = dataSet.RootNode;
					
					loopCommand.CommandText = "INSERT INTO FunctionData(datasetid, id, endid, parentid, nameid, timespent, isactiveatstart, callcount)" +
						"VALUES(?,?,?,?,?,?,?,?);";
					
					FunctionDataParams dataParams = new FunctionDataParams();
					loopCommand.Parameters.Add(dataParams.dataSetId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.functionInfoId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.endId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.parentId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.nameId = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.cpuCyclesSpent = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.isActiveAtStart = new SQLiteParameter());
					loopCommand.Parameters.Add(dataParams.callCount = new SQLiteParameter());
					
					bool addedData = true;
					
					if (profileUnitTests)
						addedData = FindUnitTestsAndInsert(loopCommand, node, dataSet, dataParams);
					else
						InsertTree(loopCommand, node, -1, dataSet, dataParams);
					
					if (addedData) {
						cmd.ExecuteNonQuery();
						dataSetCount++;
					}
				}
				
				transaction.Commit();
			}
		}
		
		bool IsUnitTest(NameMapping name)
		{
			return unitTestNames.Contains(name.Name);
		}
		
		bool FindUnitTestsAndInsert(SQLiteCommand cmd, CallTreeNode node, IProfilingDataSet dataSet, FunctionDataParams dataParams)
		{
			List<CallTreeNode> list = new List<CallTreeNode>();

			FindUnitTests(node, list);
			
			if (list.Count > 0) {
				InsertTree(cmd, new UnitTestRootCallTreeNode(list), -1, dataSet, dataParams);
				return true;
			}
			
			return false;
		}
		
		void FindUnitTests(CallTreeNode parentNode, IList<CallTreeNode> list)
		{
			if (IsUnitTest(parentNode.NameMapping)) {
				list.Add(parentNode);
				return;
			}
			
			foreach (var node in parentNode.Children) {
				FindUnitTests(node, list);
			}
		}
		
		void InitializeTables()
		{
			//NameMapping { Id, ReturnType, Name, Parameters }
			//FunctionData { DataSetId, Id, ParentId, NameId, TimeSpent, CallCount }
			//DataSets { Id, CPUUsage, RootId }
			//
			//NameMapping.Id <-> FunctionData.NameId 1:N
			//FunctionData.ParentId <-> FunctionData.Id 1:N
			
			SQLiteCommand cmd = this.connection.CreateCommand();
			
			cmd.CommandText = @"
				CREATE TABLE NameMapping(
					id INTEGER NOT NULL PRIMARY KEY,
					returntype VARCHAR2(100) NOT NULL,
					name VARCHAR2(255) NOT NULL,
					parameters VARCHAR2(1000) NOT NULL
				);
				
				CREATE TABLE FunctionData(
					datasetid INTEGER NOT NULL,
					id INTEGER NOT NULL PRIMARY KEY,
					endid INTEGER NOT NULL,
					parentid INTEGER NOT NULL,
					nameid INTEGER NOT NULL,
					timespent INT8 NOT NULL,
					isactiveatstart INTEGER NOT NULL,
					callcount INTEGER NOT NULL
				);
				
				CREATE TABLE DataSets(
					id INTEGER NOT NULL PRIMARY KEY,
					cpuusage REAL NOT NULL,
					isfirst INTEGER NOT NULL,
					rootid INTEGER NOT NULL
				);
				
				CREATE TABLE Properties(
					name VARCHAR2(100) NOT NULL PRIMARY KEY,
					value VARCHAR2(100) NOT NULL
				);
				
				INSERT INTO Properties(name, value) VALUES('version', '1.0');
				
				CREATE TABLE PerformanceCounter(
					id INTEGER NOT NULL PRIMARY KEY,
					name VARCHAR2(100) NOT NULL
				);
				
				CREATE TABLE CounterData(
					datasetid INTEGER NOT NULL,
					counterid INTEGER NOT NULL,
					value INTEGER NOT NULL,
					CONSTRAINT PK_couterdata PRIMARY KEY (datasetid, counterid)
				);
	";
			
			cmd.ExecuteNonQuery();
		}
		
		class FunctionDataParams
		{
			public SQLiteParameter dataSetId, functionInfoId,
			parentId, nameId, cpuCyclesSpent, isActiveAtStart, callCount, endId;
		}
		
		void InsertTree(SQLiteCommand cmd, CallTreeNode node, int parentId, IProfilingDataSet dataSet, FunctionDataParams dataParams)
		{
			int thisID = functionInfoCount++;
			
			foreach (CallTreeNode child in node.Children) {
				InsertTree(cmd, child, thisID, dataSet, dataParams);
			}
			
			// we sometimes saw invalid data with the 0x0080000000000000L bit set
			if (node.CpuCyclesSpent > 0x0007ffffffffffffL || node.CpuCyclesSpent < 0) {
				throw new InvalidOperationException("Too large CpuCyclesSpent - there's something wrong in the data");
			}
			
			dataParams.callCount.Value = node.RawCallCount;
			dataParams.isActiveAtStart.Value = node.IsActiveAtStart;
			dataParams.cpuCyclesSpent.Value = node.CpuCyclesSpent;
			dataParams.dataSetId.Value = dataSetCount;
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