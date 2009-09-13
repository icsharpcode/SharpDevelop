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
	/// Creates a usage data session.
	/// 
	/// Instance methods on this class are not thread-safe. If you are using it in a multi-threaded application,
	/// you should consider writing your own wrapper class that takes care of the thread-safety.
	/// In SharpDevelop, this is done by the AnalyticsMonitor class.
	/// </summary>
	public class UsageDataSessionWriter : IDisposable
	{
		SQLiteConnection connection;
		long sessionID;
		
		/// <summary>
		/// Opens/Creates the database and starts writing a new session to it.
		/// </summary>
		/// <exception cref="IncompatibleDatabaseException">The database version is not compatible with this
		/// version of the AnalyticsSessionWriter.</exception>
		public UsageDataSessionWriter(string databaseFileName)
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
		
		/// <summary>
		/// Adds environment data to the current session.
		/// </summary>
		/// <param name="name">Name of the data entry.</param>
		/// <param name="value">Value of the data entry.</param>
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
		
		/// <summary>
		/// Adds a feature use to the session.
		/// </summary>
		/// <param name="featureName">Name of the feature.</param>
		/// <param name="activationMethod">How the feature was activated (Menu, Toolbar, Shortcut, etc.)</param>
		/// <returns>ID that can be used for <see cref="WriteEndTimeForFeature"/></returns>
		public long AddFeatureUse(string featureName, string activationMethod)
		{
			if (featureName == null)
				throw new ArgumentNullException("featureName");
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
		
		/// <summary>
		/// Marks the end time for a feature use.
		/// </summary>
		/// <param name="featureUseID">A value returned from <see cref="AddFeatureUse"/>.</param>
		public void WriteEndTimeForFeature(long featureUseID)
		{
			using (SQLiteCommand cmd = this.connection.CreateCommand()) {
				cmd.CommandText = "UPDATE FeatureUses SET endTime = datetime('now') WHERE id = ?;";
				cmd.Parameters.Add(new SQLiteParameter { Value = featureUseID });
				cmd.ExecuteNonQuery();
			}
		}
		
		/// <summary>
		/// Adds an exception report to the session.
		/// </summary>
		/// <param name="exceptionType">Full .NET type name of the exception.</param>
		/// <param name="stacktrace">Stacktrace</param>
		public void AddException(string exceptionType, string stacktrace)
		{
			if (exceptionType == null)
				throw new ArgumentNullException("exceptionType");
			using (SQLiteCommand cmd = this.connection.CreateCommand()) {
				cmd.CommandText = "INSERT INTO Exceptions (session, time, type, stackTrace)" +
					" VALUES (?, datetime('now'), ?, ?);";
				cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
				cmd.Parameters.Add(new SQLiteParameter { Value = exceptionType });
				cmd.Parameters.Add(new SQLiteParameter { Value = stacktrace });
				cmd.ExecuteNonQuery();
			}
		}
		
		bool isDisposed;
		
		/// <summary>
		/// Closes the session.
		/// </summary>
		public void Dispose()
		{
			if (!isDisposed) {
				isDisposed = true;
				EndSession();
				connection.Dispose();
			}
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