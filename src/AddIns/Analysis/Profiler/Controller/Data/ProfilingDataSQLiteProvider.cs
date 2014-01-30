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
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using ICSharpCode.Profiler.Controller.Data.Linq;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// A profiling data provider based on a SQLite database stored in a file.
	/// </summary>
	public sealed class ProfilingDataSQLiteProvider : ProfilingDataProvider, IDisposable
	{
		SQLiteConnection connection;
		bool isDisposed;
		Dictionary<int, NameMapping> nameMappingCache;
		ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
		ReadOnlyCollection<IProfilingDataSet> dataSets;
		
		/// <summary>
		/// Creates a new SQLite profiling data provider and opens a database stored in a file.
		/// </summary>
		ProfilingDataSQLiteProvider(string fileName, bool allowUpgrade)
		{
			this.nameMappingCache = new Dictionary<int, NameMapping>();
			
			SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
			conn.Add("Data Source", fileName);
			this.connection = new SQLiteConnection(conn.ConnectionString);
			
			try {
				this.connection.Open();
				
				CheckFileVersion(allowUpgrade);
			} catch {
				try {
					connection.Dispose();
				} catch {
					// ignore errors during cleanup, rethrow the first error instead
				}
				throw;
			}
		}
		
		const string currentVersion = "1.2";
		
		void CheckFileVersion(bool allowUpgrade)
		{
			string version = GetProperty("version");
			
			if (version == "1.0" && allowUpgrade) {
				try {
					using (SQLiteCommand cmd = connection.CreateCommand()) {
						cmd.CommandText = "DROP TABLE CounterData;" +
							"DROP TABLE PerformanceCounter;" +
							"ALTER TABLE DataSets ADD COLUMN isfirst INTEGER NOT NULL DEFAULT(0);";
						cmd.ExecuteNonQuery();
					}
				} catch (SQLiteException) {
					// some 1.0 DBs already have that column, ignore 'duplicate column' error
				}
				using (SQLiteTransaction transaction = connection.BeginTransaction()) {
					try {
						using (SQLiteCommand cmd = connection.CreateCommand()) {
							cmd.CommandText = ProfilingDataSQLiteWriter.CallsAndFunctionsTableDefs + @"
								INSERT OR REPLACE INTO Properties(name, value) VALUES('version', '" + currentVersion + @"');
								
								INSERT INTO Calls
								SELECT f1.id, f1.endid, f1.parentid, f1.nameid, f1.timespent,
								   (f1.timespent - (SELECT TOTAL(f2.timespent) FROM FunctionData AS f2 WHERE f2.parentid = f1.id)),
								   f1.isactiveatstart, f1.callcount
								   FROM FunctionData f1;

								INSERT INTO Functions
								SELECT f.datasetid, c.nameid, SUM(c.cpucyclesspent), SUM(c.cpucyclesspentself),
										SUM(c.isactiveatstart), SUM(c.callcount), MAX(c.id != c.endid)
									FROM Calls c
									JOIN FunctionData f
									  ON c.id = f.id
									GROUP BY c.nameid, f.datasetid;
									
								INSERT INTO PerformanceCounters(id, name, minvalue, maxvalue, unit)
								VALUES(0, '% Processor Time', 0, 100, '%');
								
								INSERT INTO CounterData
								SELECT id, 0, cpuusage
								FROM DataSets;
								"
								+ "DELETE FROM FunctionData;" +
								"DROP TABLE FunctionData;" // I would like to do DROP TABLE, but that causes locking errors
								+ ProfilingDataSQLiteWriter.CallsAndFunctionsIndexDefs;
							cmd.ExecuteNonQuery();
						}
						transaction.Commit();
					} catch (Exception ex) {
						Console.WriteLine(ex.ToString());
						throw;
					}
				}
				version = currentVersion; // new version that was upgraded to
				try {
					// VACUUM must be run outside the transaction
					using (SQLiteCommand cmd = connection.CreateCommand()) {
						cmd.CommandText = "VACUUM;"; // free the space used by the old FunctionData table
						cmd.ExecuteNonQuery();
					}
				} catch (SQLiteException ex) {
					Console.WriteLine(ex.ToString());
				}
			}
			
			if (version != currentVersion)
				throw new IncompatibleDatabaseException(new Version(1, 0), new Version(version));
		}
		
		/// <summary>
		/// Creates a new SQLite profiling data provider from a file.
		/// </summary>
		public static ProfilingDataSQLiteProvider FromFile(string fileName)
		{
			return new ProfilingDataSQLiteProvider(fileName, false);
		}
		
		/// <summary>
		/// Creates a new SQLite profiling data provider from a file.
		/// </summary>
		public static ProfilingDataSQLiteProvider UpgradeFromOldVersion(string fileName)
		{
			return new ProfilingDataSQLiteProvider(fileName, true);
		}
		
		/// <inheritdoc/>
		public override void Close()
		{
			Dispose();
		}
		
		/// <inheritdoc/>
		public override NameMapping GetMapping(int nameId)
		{
			lock (nameMappingCache) {
				if (nameMappingCache.ContainsKey(nameId))
					return nameMappingCache[nameId];
				
				SQLiteCommand cmd;
				using (LockAndCreateCommand(out cmd)) {
					cmd.CommandText = @"SELECT name, returntype, parameters
										FROM NameMapping
										WHERE id = " + nameId + ";";
					
					using (SQLiteDataReader reader = cmd.ExecuteReader()) {
						
						string name = null, returnType = null;
						IList<string> parameters = null;
						
						while (reader.Read()) {
							name = reader.GetString(0);
							returnType = reader.GetString(1);
							parameters = reader.GetString(2).Split('-').ToList();
							if (parameters.Count == 1 && string.IsNullOrEmpty(parameters[0]))
								parameters = new List<string>();
						}
						
						NameMapping map = new NameMapping(nameId, returnType, name, parameters);
						nameMappingCache.Add(nameId, map);
						return map;
					}
				}
			}
		}
		
		/// <inheritdoc/>
		public override ReadOnlyCollection<IProfilingDataSet> DataSets {
			get {
				if (dataSets == null) {
					List<IProfilingDataSet> list = new List<IProfilingDataSet>();
					
					SQLiteCommand cmd;
					
					using (LockAndCreateCommand(out cmd)) {
						SQLiteDataReader reader;
						cmd.CommandText = @"SELECT d.id, d.isfirst, d.rootid, c.endid
											FROM DataSets d
											JOIN Calls c ON c.id = d.rootid
											ORDER BY d.id;";
						
						reader = cmd.ExecuteReader();
						
						while (reader.Read()) {
							list.Add(new SQLiteDataSet(this, reader.GetInt32(0), reader.GetBoolean(1),
							                           reader.GetInt32(2), reader.GetInt32(3)));
						}
					}
					
					dataSets = new ReadOnlyCollection<IProfilingDataSet>(list);
				}
				
				return dataSets;
			}
		}
		
		internal sealed class SQLiteDataSet : IProfilingDataSet
		{
			ProfilingDataSQLiteProvider provider;
			public readonly int ID;
			bool isFirst;
			public readonly int RootID, CallEndID;
			
			public SQLiteDataSet(ProfilingDataSQLiteProvider provider, int id, bool isFirst, int rootID, int callEndID)
			{
				this.provider = provider;
				this.ID = id;
				this.isFirst = isFirst;
				this.RootID = rootID;
				this.CallEndID = callEndID;
			}
			
			public CallTreeNode RootNode {
				get {
					return provider.GetRoot(ID, ID);
				}
			}
			
			public bool IsFirst {
				get {
					return isFirst;
				}
			}
		}
		
		/// <summary>
		/// Closes the connection to the database.
		/// </summary>
		public void Dispose()
		{
			rwLock.EnterWriteLock();
			try {
				if (!isDisposed)
					connection.Close();
				
				isDisposed = true;
			} finally {
				rwLock.ExitWriteLock();
			}
		}
		
		/// <inheritdoc/>
		public override CallTreeNode GetRoot(int startIndex, int endIndex)
		{
			if (startIndex > endIndex) {
				int help = startIndex;
				startIndex = endIndex;
				endIndex = help;
			}
			
			SQLiteQueryProvider queryProvider = new SQLiteQueryProvider(this, startIndex, endIndex);
			Expression<Func<SingleCall, bool>> filterLambda = c => c.ParentID == -1;
			return queryProvider.CreateQuery(new Filter(AllCalls.Instance, DataSetFilter(startIndex, endIndex), filterLambda)).Merge();
		}
		
		#region Properties
		/// <inheritdoc/>
		public override void SetProperty(string name, string value)
		{
			SQLiteCommand cmd;
			using (LockAndCreateCommand(out cmd))
				SetProperty(cmd, name, value);
		}
		
		internal static void SetProperty(SQLiteCommand cmd, string name, string value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			cmd.Parameters.Add(new SQLiteParameter("@name", name));
			if (value == null) {
				cmd.CommandText = "DELETE FROM Properties WHERE name=@name;";
			} else {
				cmd.Parameters.Add(new SQLiteParameter("@value", value));
				cmd.CommandText = "INSERT OR REPLACE INTO Properties(name, value) VALUES(@name, @value);";
			}
			cmd.ExecuteNonQuery();
		}
		
		/// <inheritdoc/>
		public override string GetProperty(string name)
		{
			SQLiteCommand cmd;
			using (LockAndCreateCommand(out cmd))
				return GetProperty(cmd, name);
		}
		
		internal static string GetProperty(SQLiteCommand cmd, string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			cmd.Parameters.Add(new SQLiteParameter("@name", name));
			cmd.CommandText = "SELECT value FROM Properties WHERE name = ?;";
			SQLiteDataReader reader = cmd.ExecuteReader();
			
			if (reader.Read()) {
				return reader.GetString(0);
			}
			
			return null;
		}
		#endregion
		
		int processorFrequency = -1;
		
		/// <inheritdoc/>
		public override int ProcessorFrequency {
			get {
				if (processorFrequency == -1) {
					string value = GetProperty("processorfrequency");
					if (value != null)
						processorFrequency = int.Parse(value, CultureInfo.InvariantCulture);
					else
						throw new ProfilerException("processorfrequency was not found!");
				}
				
				return processorFrequency;
			}
		}
		
		/// <inheritdoc/>
		public override IQueryable<CallTreeNode> GetFunctions(int startIndex, int endIndex)
		{
			if (startIndex < 0 || startIndex >= DataSets.Count)
				throw new ArgumentOutOfRangeException("startIndex", startIndex, "Value must be between 0 and " + endIndex);
			if (endIndex < startIndex || endIndex >= DataSets.Count)
				throw new ArgumentOutOfRangeException("endIndex", endIndex, "Value must be between " + startIndex + " and " + (DataSets.Count - 1));
			
			SQLiteQueryProvider queryProvider = new SQLiteQueryProvider(this, startIndex, endIndex);
			
			var query = queryProvider.CreateQuery(new Filter(AllCalls.Instance, DataSetFilter(startIndex, endIndex)));
			return query.Where(c => c.NameMapping.Id != 0 && !c.IsThread).MergeByName();
		}
		
		/// <inheritdoc/>
		public override PerformanceCounterDescriptor[] GetPerformanceCounters()
		{
			SQLiteCommand cmd;			
			using (LockAndCreateCommand(out cmd)) {
				cmd.CommandText = "SELECT name, minvalue, maxvalue, unit, format " +
					"FROM PerformanceCounter " +
					"ORDER BY id ASC;";
				
				List<PerformanceCounterDescriptor> list = new List<PerformanceCounterDescriptor>();
				
				var reader = cmd.ExecuteReader();
				
				while (reader.Read()) {
					list.Add(
						new PerformanceCounterDescriptor(
							reader.GetString(0), 
							reader.IsDBNull(1) ? null : new Nullable<float>(reader.GetFloat(1)),
							reader.IsDBNull(2) ? null :  new Nullable<float>(reader.GetFloat(2)),
							reader.GetString(3),
							reader.GetString(4)
						)
					);
				}
				
				return list.ToArray();
			}
		}
		
		/// <inheritdoc/>
		public override float[] GetPerformanceCounterValues(int index)
		{
			SQLiteCommand cmd;			
			using (LockAndCreateCommand(out cmd)) {
				cmd.CommandText = "SELECT value " +
					"FROM CounterData " +
					"WHERE counterid = " + index + " " +
					"ORDER BY datasetid ASC;";
				
				List<float> list = new List<float>();
				
				var reader = cmd.ExecuteReader();
				
				while (reader.Read())
					list.Add(reader.GetFloat(0));
				
				return list.ToArray();
			}
		}
		
		/// <inheritdoc/>
		public override EventDataEntry[] GetEventDataEntries(int index)
		{
			SQLiteCommand cmd;			
			using (LockAndCreateCommand(out cmd)) {
				cmd.CommandText = "SELECT eventtype, nameid, data " +
					"FROM EventData " +
					"WHERE datasetid = " + index;
				
				List<EventDataEntry> list = new List<EventDataEntry>();
				
				var reader = cmd.ExecuteReader();
				
				while (reader.Read())
					list.Add(new EventDataEntry() { Data = reader.GetString(2), DataSetId = index, NameId = reader.GetInt32(1), Type = (EventType)reader.GetInt32(0) });
				
				return list.ToArray();
			}
		}
		
		Expression<Func<SingleCall, bool>> DataSetFilter(int startIndex, int endIndex)
		{
			return c => startIndex <= c.DataSetID && c.DataSetID <= endIndex;
		}
		
		internal IList<CallTreeNode> RunSQLNodeList(SQLiteQueryProvider queryProvider, string command, bool hasIdList)
		{
			List<CallTreeNode> result = new List<CallTreeNode>();
			
			SQLiteCommand cmd;
			using (LockAndCreateCommand(out cmd)) {
				cmd.CommandText = command;
				
				using (SQLiteDataReader reader = cmd.ExecuteReader()) {
					while (reader.Read()) {
						SQLiteCallTreeNode node = new SQLiteCallTreeNode(reader.GetInt32(0), null, queryProvider);
						node.callCount = reader.GetInt32(3);
						node.cpuCyclesSpent = reader.GetInt64(1);
						node.cpuCyclesSpentSelf = reader.GetInt64(2);
						if (hasIdList) {
							object ids = reader.GetValue(6);
							if (ids is long) {
								node.IdList = new int[] { (int)(long)ids };
							} else {
								int[] idList = ids.ToString().Split(',').Select(s => int.Parse(s)).ToArray();
								Array.Sort(idList);
								node.IdList = idList;
							}
						}
						node.hasChildren = reader.GetBoolean(4);
						node.activeCallCount = reader.GetInt32(5);
						result.Add(node);
					}
				}
			}
			
			return result;
		}
		
		/// <summary>
		/// Executes an SQL command that returns a list of integers.
		/// </summary>
		internal List<int> RunSQLIDList(string command)
		{
			List<int> result = new List<int>();
			SQLiteCommand cmd;
			using (LockAndCreateCommand(out cmd)) {
				cmd.CommandText = command;
				
				using (SQLiteDataReader reader = cmd.ExecuteReader()) {
					while (reader.Read()) {
						result.Add(reader.GetInt32(0));
					}
				}
			}
			return result;
		}
		
		LockObject LockAndCreateCommand(out SQLiteCommand cmd)
		{
			rwLock.EnterReadLock();
			
			if (isDisposed) {
				rwLock.ExitReadLock();
				throw new ObjectDisposedException("ProfilingDataSQLiteProvider", "The provider was already closed!");
			}
			
			cmd = connection.CreateCommand();
			return new LockObject(cmd, rwLock);
		}
		
		struct LockObject : IDisposable
		{
			SQLiteCommand cmd;
			ReaderWriterLockSlim rwLock;
			
			public LockObject(SQLiteCommand cmd, ReaderWriterLockSlim rwLock)
			{
				this.rwLock = rwLock;
				this.cmd = cmd;
			}
			
			public void Dispose()
			{
				cmd.Dispose();
				rwLock.ExitReadLock();
			}
		}
	}
}
