// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;

using ICSharpCode.UsageDataCollector.Contracts;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Creates a usage data session.
	/// 
	/// This class is thread-safe.
	/// </summary>
	public sealed class UsageDataSessionWriter : IDisposable
	{
		readonly object lockObj = new object();
		SQLiteConnection connection;
		long sessionID;
		Timer timer;
		DateTime startTime;
		Stopwatch stopwatch;
		
		/// <summary>
		/// Opens/Creates the database and starts writing a new session to it.
		/// </summary>
		/// <exception cref="IncompatibleDatabaseException">The database version is not compatible with this
		/// version of the AnalyticsSessionWriter.</exception>
		public UsageDataSessionWriter(string databaseFileName, Func<Guid> guidProvider = null)
		{
			SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
			conn.DataSource = databaseFileName;
			
			connection = new SQLiteConnection(conn.ConnectionString);
			connection.Open();
			try {
				InitializeTables(guidProvider ?? Guid.NewGuid);
				
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
					try {
						FlushOutstandingChanges();
					} catch (SQLiteException ex) {
						// Ignore exception if the DB file is locked
						if (ex.ErrorCode != SQLiteErrorCode.Locked)
							throw;
					}
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
		
		/// <summary>
		/// Retrieves the user ID from the specified database.
		/// </summary>
		public static Guid? RetrieveUserId(string databaseFileName)
		{
			SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
			conn.DataSource = databaseFileName;
			conn.FailIfMissing = true;
			
			try {
				using (var connection = new SQLiteConnection(conn.ConnectionString)) {
					connection.Open();
					return RetrieveUserId(connection);
				}
			} catch (SQLiteException) {
				return null;
			}
		}
		
		static Guid? RetrieveUserId(SQLiteConnection connection)
		{
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "SELECT value FROM Properties WHERE name = 'userID';";
				string userID = (string)cmd.ExecuteScalar();
				if (userID != null) {
					try {
						return new Guid(userID);
					} catch (FormatException) {
					} catch (OverflowException) {
						// ignore incorrect GUIDs
					}
				}
				return null;
			}
		}
		
		static readonly Version expectedDBVersion = new Version(1, 0, 1);
		
		/// <summary>
		/// Creates or upgrades the database
		/// </summary>
		void InitializeTables(Func<Guid> userIdProvider)
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
				if (RetrieveUserId(this.connection) == null) {
					using (SQLiteCommand cmd = this.connection.CreateCommand()) {
						cmd.CommandText = @"INSERT OR IGNORE INTO Properties (name, value) VALUES ('userID', ?);";
						cmd.Parameters.Add(new SQLiteParameter { Value = userIdProvider().ToString() });
						cmd.ExecuteNonQuery();
					}
				}
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = @"
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
		
		/// <summary>
		/// Gets the default environment data.
		/// </summary>
		public static UsageDataEnvironmentProperty[] GetDefaultEnvironmentData()
		{
			return new [] {
				new UsageDataEnvironmentProperty { Name = "platform", Value = Environment.OSVersion.Platform.ToString() },
				new UsageDataEnvironmentProperty { Name = "osVersion", Value = Environment.OSVersion.Version.ToString() },
				new UsageDataEnvironmentProperty { Name = "processorCount", Value = Environment.ProcessorCount.ToString() },
				new UsageDataEnvironmentProperty { Name = "dotnetRuntime", Value = Environment.Version.ToString() }
			};
		}
		
		void StartSession()
		{
			startTime = DateTime.UtcNow;
			stopwatch = Stopwatch.StartNew();
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "INSERT INTO Sessions (startTime) VALUES (?);" +
						"SELECT last_insert_rowid();";
					cmd.Parameters.Add(new SQLiteParameter { Value = TimeToString(startTime) });
					sessionID = (long)cmd.ExecuteScalar();
				}
				AddEnvironmentData(GetDefaultEnvironmentData());
				transaction.Commit();
			}
		}
		
		DateTime GetNow()
		{
			return startTime + stopwatch.Elapsed;
		}
		
		void EndSession()
		{
			using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
				FlushOutstandingChanges();
				using (SQLiteCommand cmd = this.connection.CreateCommand()) {
					cmd.CommandText = "UPDATE Sessions SET endTime = ? WHERE id = ?;";
					cmd.Parameters.Add(new SQLiteParameter { Value = TimeToString(GetNow()) });
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.ExecuteNonQuery();
				}
				transaction.Commit();
			}
		}
		
		/// <summary>
		/// Adds environment data to the current session.
		/// </summary>
		public void AddEnvironmentData(IEnumerable<UsageDataEnvironmentProperty> properties)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");
			UsageDataEnvironmentProperty[] pArray = properties.ToArray();
			lock (lockObj) {
				if (isDisposed)
					throw new ObjectDisposedException(GetType().Name);
				using (SQLiteTransaction transaction = this.connection.BeginTransaction()) {
					foreach (UsageDataEnvironmentProperty p in pArray) {
						using (SQLiteCommand cmd = this.connection.CreateCommand()) {
							cmd.CommandText = "INSERT INTO Environment (session, name, value)" +
								" VALUES (?, ?, ?);";
							cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
							cmd.Parameters.Add(new SQLiteParameter { Value = p.Name });
							cmd.Parameters.Add(new SQLiteParameter { Value = p.Value });
							cmd.ExecuteNonQuery();
						}
					}
					transaction.Commit();
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
				FeatureUse featureUse = new FeatureUse(this, GetNow());
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
				featureUse.endTime = GetNow();
				delayedEnd.Enqueue(featureUse);
			}
		}
		
		static string TimeToString(DateTime time)
		{
			return time.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
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
							time.Value = TimeToString(use.startTime);
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
							endTime.Value = TimeToString(use.endTime);
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
			string stacktrace = GetStackTrace(exception);
			while (exception.InnerException != null) {
				exception = exception.InnerException;
				
				stacktrace = GetStackTrace(exception) + Environment.NewLine
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
				return type + " (" + Marshal.GetHRForException(exception).ToString("x8") + ")";
			else
				return type;
		}
		
		static string GetStackTrace(Exception exception)
		{
			// Output stacktrace in custom format (very similar to Exception.StackTrace property on English systems).
			// Include filenames where available, but no paths.
			StackTrace stackTrace = new StackTrace(exception, true);
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < stackTrace.FrameCount; i++) {
				StackFrame frame = stackTrace.GetFrame(i);
				MethodBase method = frame.GetMethod();
				if (method == null)
					continue;
				
				if (b.Length > 0)
					b.AppendLine();
				
				b.Append("   at ");
				Type declaringType = method.DeclaringType;
				if (declaringType != null) {
					b.Append(declaringType.FullName.Replace('+', '.'));
					b.Append('.');
				}
				b.Append(method.Name);
				// output type parameters, if any
				if ((method is MethodInfo) && ((MethodInfo) method).IsGenericMethod) {
					Type[] genericArguments = ((MethodInfo) method).GetGenericArguments();
					b.Append('[');
					for (int j = 0; j < genericArguments.Length; j++) {
						if (j > 0)
							b.Append(',');
						b.Append(genericArguments[j].Name);
					}
					b.Append(']');
				}
				
				// output parameters, if any
				b.Append('(');
				ParameterInfo[] parameters = method.GetParameters();
				for (int j = 0; j < parameters.Length; j++) {
					if (j > 0)
						b.Append(", ");
					if (parameters[j].ParameterType != null) {
						b.Append(parameters[j].ParameterType.Name);
					} else {
						b.Append('?');
					}
					if (!string.IsNullOrEmpty(parameters[j].Name)) {
						b.Append(' ');
						b.Append(parameters[j].Name);
					}
				}
				b.Append(')');
				
				// source location
				if (frame.GetILOffset() >= 0) {
					string filename = null;
					try {
						string fullpath = frame.GetFileName();
						if (fullpath != null)
							filename = Path.GetFileName(fullpath);
					} catch (SecurityException) {
						// StackFrame.GetFileName requires PathDiscovery permission
					} catch (ArgumentException) {
						// Path.GetFileName might throw on paths with invalid chars
					}
					b.Append(" in ");
					if (filename != null) {
						b.Append(filename);
						b.Append(":line ");
						b.Append(frame.GetFileLineNumber());
					} else {
						b.Append("offset ");
						b.Append(frame.GetILOffset());
					}
				}
			}
			return b.ToString();
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
						" VALUES (?, ?, ?, ?);";
					cmd.Parameters.Add(new SQLiteParameter { Value = sessionID });
					cmd.Parameters.Add(new SQLiteParameter { Value = TimeToString(GetNow()) });
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
		internal readonly DateTime startTime;
		internal DateTime endTime;
		internal string name, activation;
		internal long rowId;
		readonly UsageDataSessionWriter writer;
		
		internal FeatureUse(UsageDataSessionWriter writer, DateTime startTime)
		{
			this.startTime = startTime;
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
		public IncompatibleDatabaseException(string message) : base(message) {}
		
		/// <summary>
		/// Creates a new IncompatibleDatabaseException instance.
		/// </summary>
		public IncompatibleDatabaseException(string message, Exception innerException) : base(message, innerException) {}
		
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
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
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
