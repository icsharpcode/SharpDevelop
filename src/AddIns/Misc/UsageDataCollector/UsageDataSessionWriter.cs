// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data.SQLite;
using System.Runtime.Serialization;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Creates an analytics session.
	/// </summary>
	public class AnalyticsSessionWriter : IDisposable
	{
		SQLiteConnection connection;
		long sessionID;
		
		/// <summary>
		/// Opens/Creates the database and starts writing a new session to it.
		/// </summary>
		/// <exception cref="IncompatibleDatabaseException"></exception>
		public AnalyticsSessionWriter(string databaseFileName)
		{
			SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
			conn.Add("Data Source", databaseFileName);
			
			connection = new SQLiteConnection(conn.ConnectionString);
			connection.Open();
			try {
				InitializeTables();
				
				StartSession();
			} catch {
				connection.Close();
				throw;
			}
		}
		
		
		static readonly Version expectedDBVersion = new Version(1, 0, 1);
		
		/// <summary>
		/// Creates or upgrades the database
		/// </summary>
		void InitializeTables()
		{
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = @"
					CREATE TABLE IF NOT EXISTS Properties (
						name TEXT NOT NULL PRIMARY KEY,
						value TEXT NOT NULL
					);
					INSERT OR IGNORE INTO Properties (name, value) VALUES ('dbVersion', '" + expectedDBVersion.ToString() + @"');
					";
					cmd.ExecuteNonQuery();
				}
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "SELECT value FROM Properties WHERE name = 'dbVersion';";
					string version = (string)cmd.ExecuteScalar();
					if (version == null)
						throw new InvalidOperationException("Error retrieving database version");
					Version actualDBVersion = new Version(version);
					if (actualDBVersion != expectedDBVersion) {
						throw new IncompatibleDatabaseException(expectedDBVersion, actualDBVersion);
					}
				}
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = @"
					INSERT OR IGNORE INTO Properties (name, value) VALUES ('userID', '" + Guid.NewGuid().ToString() + @"');
					
					CREATE TABLE IF NOT EXISTS Sessions (
						id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
						startTime TEXT NOT NULL,
						endTime TEXT
					);
					CREATE TABLE IF NOT EXISTS Environment (
						session INTEGER NOT NULL,
						name TEXT NOT NULL,
						value TEXT
					);
					CREATE TABLE IF NOT EXISTS FeatureUses (
						id INTEGER NOT NULL PRIMARY KEY,
						session INTEGER NOT NULL,
						time TEXT NOT NULL,
						endTime TEXT,
						feature TEXT NOT NULL,
						activationMethod TEXT
					);
					CREATE TABLE IF NOT EXISTS Exceptions (
						session INTEGER NOT NULL,
						time TEXT NOT NULL,
						type TEXT NOT NULL,
						stackTrace TEXT
					);
					";
					cmd.ExecuteNonQuery();
				}
				transaction.Commit();
			}
		}
		
		void StartSession()
		{
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "INSERT INTO Sessions (startTime) VALUES (datetime('now'));" +
						"SELECT last_insert_rowid();";
					sessionID = (long)cmd.ExecuteScalar();
				}
				AddEnvironmentData("platform", Environment.OSVersion.Platform.ToString());
				AddEnvironmentData("osVersion", Environment.OSVersion.Version.ToString());
				AddEnvironmentData("processorCount", Environment.ProcessorCount.ToString());
				AddEnvironmentData("dotnetRuntime", Environment.Version.ToString());
				transaction.Commit();
			}
		}
		
		void EndSession()
		{
			using (SQLiteCommand cmd = this.connection.CreateCommand()) {
				cmd.CommandText = "UPDATE Sessions SET endTime = datetime('now') WHERE id = ?;";
				cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
				cmd.ExecuteNonQuery();
			}
		}
		
		public void AddEnvironmentData(string name, string value)
		{
			using (SQLiteCommand cmd = this.connection.CreateCommand()) {
				cmd.CommandText = "INSERT INTO Environment (session, name, value)" +
					" VALUES (?, ?, ?);";
				cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
				cmd.Parameters.Add(new SQLiteParameter { Value = name });
				cmd.Parameters.Add(new SQLiteParameter { Value = value });
				cmd.ExecuteNonQuery();
			}
		}
		
		public long AddFeatureUse(string featureName, string activationMethod)
		{
			long featureRowId;
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "INSERT INTO FeatureUses (session, time, feature, activationMethod)" +
						"    VALUES (?, datetime('now'), ?, ?);" +
						"SELECT last_insert_rowid();";
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.Parameters.Add(new SQLiteParameter { Value = featureName });
					cmd.Parameters.Add(new SQLiteParameter { Value = activationMethod });
					
					featureRowId = (long)cmd.ExecuteScalar();
				}
				transaction.Commit();
			}
			return featureRowId;
		}
		
		public void WriteEndTimeForFeature(long featureID)
		{
			using (SQLiteCommand cmd = this.connection.CreateCommand()) {
				cmd.CommandText = "UPDATE FeatureUses SET endTime = datetime('now') WHERE id = ?;";
				cmd.Parameters.Add(new SQLiteParameter { Value = featureID });
				cmd.ExecuteNonQuery();
			}
		}
		
		public void AddException(string exceptionType, string stacktrace)
		{
			using (SQLiteCommand cmd = this.connection.CreateCommand()) {
				cmd.CommandText = "INSERT INTO Exceptions (session, time, type, stackTrace)" +
					" VALUES (?, datetime('now'), ?, ?);";
				cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
				cmd.Parameters.Add(new SQLiteParameter { Value = exceptionType });
				cmd.Parameters.Add(new SQLiteParameter { Value = stacktrace });
				cmd.ExecuteNonQuery();
			}
		}
		
		public void Dispose()
		{
			EndSession();
			connection.Close();
		}
	}
	
	[Serializable]
	public class IncompatibleDatabaseException : Exception
	{
		public Version ExpectedVersion { get; set; }
		public Version ActualVersion { get; set; }
		
		public IncompatibleDatabaseException() {}
		
		public IncompatibleDatabaseException(Version expectedVersion, Version actualVersion)
			: base("Expected DB version " + expectedVersion + " but found " + actualVersion)
		{
			this.ExpectedVersion = expectedVersion;
			this.ActualVersion = actualVersion;
		}
		
		protected IncompatibleDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null) {
				this.ExpectedVersion = (Version)info.GetValue("ExpectedVersion", typeof(Version));
				this.ActualVersion = (Version)info.GetValue("ActualVersion", typeof(Version));
			}
		}
		
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null) {
				info.AddValue("ExpectedVersion", this.ExpectedVersion, typeof(Version));
				info.AddValue("ActualVersion", this.ActualVersion, typeof(Version));
			}
		}
	}
}