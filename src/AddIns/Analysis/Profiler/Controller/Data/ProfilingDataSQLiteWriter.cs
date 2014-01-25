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
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;


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
			if (isDisposed)
				return;
			
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				// create index at the end (after inserting data), this is faster
				cmd.CommandText = CallsAndFunctionsIndexDefs;
				cmd.ExecuteNonQuery();
			}
			
			Dispose();
		}
		
		/// <summary>
		/// Sets or gets the processor frequency of the computer, where the profiling session was created.
		/// The processor frequency is measured in MHz.
		/// </summary>
		public int ProcessorFrequency {
			get {
				return processorFrequency;
			}
			set {
				processorFrequency = value;
				ProfilingDataSQLiteProvider.SetProperty(connection.CreateCommand(), "processorfrequency", value.ToString(CultureInfo.InvariantCulture));
			}
		}
		
		/// <summary>
		/// Writes a profiling dataset to the database.
		/// </summary>
		public void WriteDataSet(IProfilingDataSet dataSet)
		{
			if (dataSet == null)
				throw new ArgumentNullException("dataSet");
			
			using (SQLiteTransaction transaction = connection.BeginTransaction()) {
				SQLiteCommand cmd = connection.CreateCommand();
				
				if (dataSetCount == -1)
					dataSetCount = 0;
				
				cmd.Parameters.Add(new SQLiteParameter("id", dataSetCount));
				cmd.Parameters.Add(new SQLiteParameter("isfirst", dataSet.IsFirst));
				cmd.Parameters.Add(new SQLiteParameter("rootid", functionInfoCount));
				
				cmd.CommandText = "INSERT INTO DataSets(id, isfirst, rootid)" +
					"VALUES(?,?,?);";
				
				int dataSetStartId = functionInfoCount;
				
				using (SQLiteCommand loopCommand = connection.CreateCommand()) {
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
				
				using (SQLiteCommand functionsCommand = connection.CreateCommand()) {
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
			);
			
			CREATE TABLE PerformanceCounter(
				id INTEGER NOT NULL PRIMARY KEY,
				name TEXT NOT NULL,
				minvalue REAL NULL,
				maxvalue REAL NULL,
				format TEXT NOT NULL,
				unit TEXT NOT NULL
			);
			
			CREATE TABLE CounterData(
				datasetid INTEGER NOT NULL,
				counterid INTEGER NOT NULL,
				value REAL NOT NULL
			);
			
			CREATE TABLE EventData(
				datasetid INTEGER NOT NULL,
				eventtype INTEGER NOT NULL,
				nameid INTEGER NOT NULL,
				data TEXT NULL
			);
";
		
		internal const string CallsAndFunctionsIndexDefs =
			"CREATE INDEX CallsParent ON Calls(parentid ASC);" // required for searching the children
			+ " ANALYZE;"; // make SQLite analyze the indices available; this will help the query planner later
		
		void InitializeTables()
		{
			// NameMapping { Id, ReturnType, Name, Parameters }
			// Calls { id, endid, parentid, nameid, cpucyclesspent, cpucyclesspentself, isactiveatstart, callcount }
			// Functions { datasetid, nameid, cpucyclesspent, cpucyclesspentself, activecallcount, callcount, haschildren }
			// DataSets { Id, IsFirst, RootId }
			//
			// NameMapping.Id <-> FunctionData.NameId 1:N
			// FunctionData.ParentId <-> FunctionData.Id 1:N
			
			SQLiteCommand cmd = connection.CreateCommand();
			
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
					isfirst INTEGER NOT NULL,
					rootid INTEGER NOT NULL
				);
				
				CREATE TABLE Properties(
					name TEXT NOT NULL PRIMARY KEY,
					value TEXT NOT NULL
				);
				
				INSERT INTO Properties(name, value) VALUES('version', '1.2');
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
			
			long cpuCycles = node.CpuCyclesSpent;
			long cpuCyclesSelf = node.CpuCyclesSpentSelf;
			
			// we sometimes saw invalid data with the 0x0080000000000000L bit set
			if (cpuCycles > 0x0007ffffffffffffL || cpuCycles < 0) {
				throw new InvalidOperationException("Too large CpuCyclesSpent - there's something wrong in the data");
			}
			
			if (node.NameMapping.Id != 0 && (cpuCyclesSelf > cpuCycles || cpuCyclesSelf < 0)) {
				throw new InvalidOperationException("Too large/small CpuCyclesSpentSelf (" + cpuCyclesSelf + ") - there's something wrong in the data");
			}
			
			dataParams.callCount.Value = node.RawCallCount;
			dataParams.isActiveAtStart.Value = node.IsActiveAtStart;
			dataParams.cpuCyclesSpent.Value = cpuCycles;
			dataParams.cpuCyclesSpentSelf.Value = cpuCyclesSelf;

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
			using (SQLiteTransaction trans = connection.BeginTransaction()) {
				using (SQLiteCommand cmd = connection.CreateCommand()) {
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
				connection.Close();
			
			isDisposed = true;
		}
		
		/// <inheritdoc/>
		public void WritePerformanceCounterData(IEnumerable<PerformanceCounterDescriptor> counters)
		{
			using (SQLiteTransaction trans = connection.BeginTransaction()) {
				using (SQLiteCommand cmd = connection.CreateCommand()) {
					using (SQLiteCommand cmd2 = connection.CreateCommand()) {

						SQLiteParameter idParam = new SQLiteParameter("id");
						SQLiteParameter nameParam = new SQLiteParameter("name");
						SQLiteParameter dataSetParam = new SQLiteParameter("dataset");
						SQLiteParameter valueParam = new SQLiteParameter("value");
						SQLiteParameter minParam = new SQLiteParameter("min");
						SQLiteParameter maxParam = new SQLiteParameter("max");
						SQLiteParameter unitParam = new SQLiteParameter("unit");
						SQLiteParameter formatParam = new SQLiteParameter("format");
						
						cmd.Parameters.AddRange(new SQLiteParameter[] { idParam, dataSetParam, valueParam });
						cmd2.Parameters.AddRange(new SQLiteParameter[] { idParam, nameParam, minParam, maxParam, unitParam, formatParam });
						
						cmd2.CommandText =
							"INSERT INTO PerformanceCounter(id, name, minvalue, maxvalue, unit, format)" +
							"VALUES(@id,@name,@min,@max,@unit,@format);";
						
						cmd.CommandText =
							"INSERT INTO CounterData(datasetid, counterid, value)" +
							"VALUES(@dataset,@id,@value);";
						
						int id = 0;
						
						foreach (PerformanceCounterDescriptor counter in counters) {
							idParam.Value = id;
							nameParam.Value = counter.Name;
							minParam.Value = counter.MinValue;
							maxParam.Value = counter.MaxValue;
							unitParam.Value = counter.Unit;
							formatParam.Value = counter.Format;
							
							for (int i = 0; i < counter.Values.Count; i++) {
								dataSetParam.Value = i;
								valueParam.Value = counter.Values[i];
								cmd.ExecuteNonQuery();
							}
							
							cmd2.ExecuteNonQuery();
							
							id++;
						}
					}
				}
				trans.Commit();
			}
		}
		
		/// <inheritdoc/>
		public void WriteEventData(IEnumerable<EventDataEntry> events)
		{
			using (SQLiteTransaction trans = connection.BeginTransaction()) {
				using (SQLiteCommand cmd = connection.CreateCommand()) {
					SQLiteParameter dataSetParam = new SQLiteParameter("datasetid");
					SQLiteParameter eventTypeParam = new SQLiteParameter("eventtype");
					SQLiteParameter nameIdParam = new SQLiteParameter("nameid");
					SQLiteParameter dataParam = new SQLiteParameter("data");
					
					cmd.CommandText =
						"INSERT INTO EventData(datasetid,eventtype,nameid,data) " +
						"VALUES(@datasetid,@eventtype,@nameid,@data);";
					
					cmd.Parameters.AddRange(new SQLiteParameter[] { dataSetParam, eventTypeParam, nameIdParam, dataParam });
					
					foreach (EventDataEntry entry in events) {
						dataSetParam.Value = entry.DataSetId;
						eventTypeParam.Value = (int)entry.Type;
						nameIdParam.Value = entry.NameId;
						dataParam.Value = entry.Data;
						cmd.ExecuteNonQuery();
					}
				}
				trans.Commit();
			}
		}
		
		/// <inheritdoc/>
		public int DataSetCount {
			get { return dataSetCount; }
		}
	}
}
