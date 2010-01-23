// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Creates a usage data session.
	/// 
	/// This class is thread-safe.
	/// </summary>
	public class UsageDataSessionWriter : IDisposable
	{
		readonly object lockObj = new object();
		SQLiteConnection connection;
		long sessionID;
		Timer timer;
		
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
			
			timer = new Timer(OnTimer, 0, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
		}
		
		/// <summary>
		/// This delegate is called when an exception occurs on the background timer.
		/// </summary>
		public Action<Exception> OnException { get; set; }
		
		void OnTimer(object state)
		{
			lock (lockObj) {
				if (isDisposed)
					return;
				try {
					FlushOutstandingChanges();
				} catch (Exception ex) {
					Action<Exception> onException = this.OnException;
					if (onException != null)
						onException(ex);
					else
						throw;
				}
			}
		}
		
		/// <summary>
		/// Flushes changes that are not written yet.
		/// </summary>
		public void Flush()
		{
			lock (lockObj) {
				if (isDisposed)
					throw new ObjectDisposedException(GetType().Name);
				FlushOutstandingChanges();
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
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				FlushOutstandingChanges();
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "UPDATE Sessions SET endTime = datetime('now') WHERE id = ?;";
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.ExecuteNonQuery();
				}
				transaction.Commit();
			}
		}
		
		/// <summary>
		/// Adds environment data to the current session.
		/// </summary>
		/// <param name="name">Name of the data entry.</param>
		/// <param name="value">Value of the data entry.</param>
		public void AddEnvironmentData(string name, string value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			lock (lockObj) {
				if (isDisposed)
					throw new ObjectDisposedException(GetType().Name);
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "INSERT INTO Environment (session, name, value)" +
						" VALUES (?, ?, ?);";
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.Parameters.Add(new SQLiteParameter { Value = name });
					cmd.Parameters.Add(new SQLiteParameter { Value = value });
					cmd.ExecuteNonQuery();
				}
			}
		}
		
		Queue<FeatureUse> delayedStart = new Queue<FeatureUse>();
		Queue<FeatureUse> delayedEnd = new Queue<FeatureUse>();
		
		/// <summary>
		/// Adds a feature use to the session.
		/// </summary>
		/// <param name="featureName">Name of the feature.</param>
		/// <param name="activationMethod">How the feature was activated (Menu, Toolbar, Shortcut, etc.)</param>
		public FeatureUse AddFeatureUse(string featureName, string activationMethod)
		{
			if (featureName == null)
				throw new ArgumentNullException("featureName");
			
			lock (lockObj) {
				if (isDisposed)
					throw new ObjectDisposedException(GetType().Name);
				FeatureUse featureUse = new FeatureUse(this);
				featureUse.name = featureName;
				featureUse.activation = activationMethod;
				delayedStart.Enqueue(featureUse);
				return featureUse;
			}
		}
		
		internal void WriteEndTimeForFeature(FeatureUse featureUse)
		{
			lock (lockObj) {
				if (isDisposed)
					return;
				featureUse.endTime = DateTime.UtcNow;
				delayedEnd.Enqueue(featureUse);
			}
		}
		
		void FlushOutstandingChanges()
		{
			//Console.WriteLine("Flushing {0} starts and {1} ends", delayedStart.Count, delayedEnd.Count);
			if (delayedStart.Count == 0 && delayedEnd.Count == 0)
				return;
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				if (delayedStart.Count > 0) {
					using (SQLiteCommand cmd = this.connection.CreateCommand()) {
						cmd.CommandText = "INSERT INTO FeatureUses (session, time, feature, activationMethod)" +
							"    VALUES (?, ?, ?, ?);" +
							"SELECT last_insert_rowid();";
						SQLiteParameter time, feature, activationMethod;
						cmd.Parameters.Add(new SQLiteParameter() { Value = sessionID });
						cmd.Parameters.Add(time = new SQLiteParameter());
						cmd.Parameters.Add(feature = new SQLiteParameter());
						cmd.Parameters.Add(activationMethod = new SQLiteParameter());
						while (delayedStart.Count > 0) {
							FeatureUse use = delayedStart.Dequeue();
							time.Value = use.startTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
							feature.Value = use.name;
							activationMethod.Value = use.activation;
							
							use.rowId = (long)cmd.ExecuteScalar();
						}
					}
				}
				if (delayedEnd.Count > 0) {
					using (SQLiteCommand cmd = this.connection.CreateCommand()) {
						cmd.CommandText = "UPDATE FeatureUses SET endTime = ? WHERE id = ?;";
						SQLiteParameter endTime, id;
						cmd.Parameters.Add(endTime = new SQLiteParameter());
						cmd.Parameters.Add(id = new SQLiteParameter());
						while (delayedEnd.Count > 0) {
							FeatureUse use = delayedEnd.Dequeue();
							endTime.Value = use.endTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
							id.Value = use.rowId;
							cmd.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}
		
		/// <summary>
		/// Adds an exception report to the session.
		/// </summary>
		/// <param name="exception">The exception instance.</param>
		public void AddException(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");
			string exceptionType = GetTypeName(exception);
			string stacktrace = exception.StackTrace;
			while (exception.InnerException != null) {
				exception = exception.InnerException;
				
				stacktrace = exception.StackTrace + Environment.NewLine
					+ "-- continuing with outer exception (" + exceptionType + ") --" + Environment.NewLine
					+ stacktrace;
				exceptionType = GetTypeName(exception);
			}
			AddException(exceptionType, stacktrace);
		}
		
		static string GetTypeName(Exception exception)
		{
			string type = exception.GetType().FullName;
			if (exception is ExternalException || exception is System.IO.IOException)
				return type + " (" + Marshal.GetHRForException(ee).ToString("x8") + ")";
			else
				return type;
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
			lock (lockObj) {
				if (isDisposed)
					throw new ObjectDisposedException(GetType().Name);
				// first, insert the exception (it's most important to have)
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "INSERT INTO Exceptions (session, time, type, stackTrace)" +
						" VALUES (?, datetime('now'), ?, ?);";
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.Parameters.Add(new SQLiteParameter { Value = exceptionType });
					cmd.Parameters.Add(new SQLiteParameter { Value = stacktrace });
					cmd.ExecuteNonQuery();
				}
				// then, flush outstanding changes (SharpDevelop will likely exit soon)
				FlushOutstandingChanges();
			}
		}
		
		bool isDisposed;
		
		/// <summary>
		/// Closes the session.
		/// </summary>
		public void Dispose()
		{
			lock (lockObj) {
				if (!isDisposed) {
					isDisposed = true;
					EndSession();
					timer.Dispose();
					connection.Dispose();
				}
			}
		}
	}
	
	/// <summary>
	/// Represents a feature use that is currently being written.
	/// </summary>
	public sealed class FeatureUse
	{
		internal readonly DateTime startTime = DateTime.UtcNow;
		internal DateTime endTime;
		internal string name, activation;
		internal long rowId;
		UsageDataSessionWriter writer;
		
		internal FeatureUse(UsageDataSessionWriter writer)
		{
			this.writer = writer;
		}
		
		/// <summary>
		/// Marks the end time for a feature use.
		/// </summary>
		public void TrackEndTime()
		{
			writer.WriteEndTimeForFeature(this);
		}
	}

	/// <summary>
	/// Thrown when the database used to internally store the usage data is incompatible with the DB version
	/// expected by the UsageDataCollector library.
	/// </summary>
	[Serializable]
	public class IncompatibleDatabaseException : Exception
	{
		/// <summary>
		/// Expected database version.
		/// </summary>
		public Version ExpectedVersion { get; set; }
		
		/// <summary>
		/// Actual database version.
		/// </summary>
		public Version ActualVersion { get; set; }
		
		/// <summary>
		/// Creates a new IncompatibleDatabaseException instance.
		/// </summary>
		public IncompatibleDatabaseException() {}
		
		/// <summary>
		/// Creates a new IncompatibleDatabaseException instance.
		/// </summary>
		public IncompatibleDatabaseException(Version expectedVersion, Version actualVersion)
			: base("Expected DB version " + expectedVersion + " but found " + actualVersion)
		{
			this.ExpectedVersion = expectedVersion;
			this.ActualVersion = actualVersion;
		}
		
		/// <summary>
		/// Deserializes an IncompatibleDatabaseException instance.
		/// </summary>
		protected IncompatibleDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null) {
				this.ExpectedVersion = (Version)info.GetValue("ExpectedVersion", typeof(Version));
				this.ActualVersion = (Version)info.GetValue("ActualVersion", typeof(Version));
			}
		}
		
		/// <inheritdoc/>
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