// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
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
		
		public AnalyticsSessionWriter(string databaseFileName)
		{
			SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
			conn.Add("Data Source", databaseFileName);
			
			connection = new SQLiteConnection(conn.ConnectionString);
			connection.Open();
			InitializeTables();
			
			StartSession();
		}
		
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
					INSERT OR IGNORE INTO Properties (name, value) VALUES ('dbVersion', '1.0');
					INSERT OR IGNORE INTO Properties (name, value) VALUES ('userID', '" + Guid.NewGuid().ToString() + @"');
					";
					cmd.ExecuteNonQuery();
				}
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "SELECT value FROM Properties WHERE name = 'dbVersion';";
					string version = (string)cmd.ExecuteScalar();
					if (version == null)
						throw new InvalidOperationException("Error retrieving database version");
					if (version != "1.0") {
						throw new DatabaseTooNewException();
					}
				}
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = @"
					CREATE TABLE IF NOT EXISTS Sessions (
						id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
						startTime TEXT NOT NULL,
						endTime TEXT,
						appVersion TEXT,
						platform TEXT,
						osVersion TEXT,
						processorCount INTEGER,
						dotnetRuntime TEXT,
						language TEXT
					);
					CREATE TABLE IF NOT EXISTS FeatureUses (
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
					cmd.CommandText = "INSERT INTO Sessions (startTime, appVersion, platform, osVersion, processorCount, dotnetRuntime, language)" +
						" VALUES (datetime(), ?, ?, ?, ?, ?, ?);";
					cmd.Parameters.Add(new SQLiteParameter { Value = RevisionClass.FullVersion });
					cmd.Parameters.Add(new SQLiteParameter { Value = Environment.OSVersion.Platform.ToString() });
					cmd.Parameters.Add(new SQLiteParameter { Value = Environment.OSVersion.Version.ToString() });
					cmd.Parameters.Add(new SQLiteParameter { Value = Environment.ProcessorCount });
					cmd.Parameters.Add(new SQLiteParameter { Value = Environment.Version.ToString() });
					cmd.Parameters.Add(new SQLiteParameter { Value = ICSharpCode.Core.ResourceService.Language });
					cmd.ExecuteNonQuery();
				}
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "SELECT last_insert_rowid();";
					sessionID = (long)cmd.ExecuteScalar();
				}
				transaction.Commit();
			}
		}
		
		void EndSession()
		{
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "UPDATE Sessions SET endTime = datetime() WHERE id = ?;";
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.ExecuteNonQuery();
				}
				transaction.Commit();
			}
		}
		
		public long AddFeatureUse(string featureName, string activationMethod)
		{
			long featureRowId;
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "INSERT INTO FeatureUses (session, time, feature, activationMethod)" +
						" VALUES (?, datetime(), ?, ?);";
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.Parameters.Add(new SQLiteParameter { Value = featureName });
					cmd.Parameters.Add(new SQLiteParameter { Value = activationMethod });
					cmd.ExecuteNonQuery();
				}
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "SELECT last_insert_rowid();";
					featureRowId = (long)cmd.ExecuteScalar();
				}
				transaction.Commit();
			}
			return featureRowId;
		}
		
		public void WriteEndTimeForFeature(long featureID)
		{
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "UPDATE FeatureUses SET endTime = datetime() WHERE ROWID = ?;";
					cmd.Parameters.Add(new SQLiteParameter { Value = featureID });
					cmd.ExecuteNonQuery();
				}
				transaction.Commit();
			}
		}
		
		public void AddException(string exceptionType, string stacktrace)
		{
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "INSERT INTO Exceptions (session, time, type, stackTrace)" +
						" VALUES (?, datetime(), ?, ?);";
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.Parameters.Add(new SQLiteParameter { Value = exceptionType });
					cmd.Parameters.Add(new SQLiteParameter { Value = stacktrace });
					cmd.ExecuteNonQuery();
				}
				transaction.Commit();
			}
		}
		
		public void Dispose()
		{
			EndSession();
			connection.Close();
		}
	}
	
	[Serializable]
	public class DatabaseTooNewException : Exception
	{
		public DatabaseTooNewException() {}
		protected DatabaseTooNewException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}