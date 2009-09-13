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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Allows uploading collected data.
	/// </summary>
	public class UsageDataUploader
	{
		string databaseFileName;
		
		public UsageDataUploader(string databaseFileName)
		{
			this.databaseFileName = databaseFileName;
		}
		
		SQLiteConnection OpenConnection()
		{
			SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
			conn.Add("Data Source", databaseFileName);
			SQLiteConnection connection = new SQLiteConnection(conn.ConnectionString);
			connection.Open();
			return connection;
		}
		
		public string GetTextForStoredData()
		{
			UsageDataMessage message;
			using (SQLiteConnection connection = OpenConnection()) {
				using (SQLiteTransaction transaction = connection.BeginTransaction()) {
					CheckDatabaseVersion(connection);
					message = FetchDataForUpload(connection, true);
				}
			}
			using (StringWriter w = new StringWriter()) {
				using (XmlTextWriter xmlWriter = new XmlTextWriter(w)) {
					xmlWriter.Formatting = Formatting.Indented;
					DataContractSerializer serializer = new DataContractSerializer(typeof(UsageDataMessage));
					serializer.WriteObject(xmlWriter, message);
				}
				return w.ToString();
			}
		}
		
		/// <summary>
		/// Starts the upload of the usage data.
		/// </summary>
		public void StartUpload()
		{
			UsageDataMessage message;
			using (SQLiteConnection connection = OpenConnection()) {
				using (SQLiteTransaction transaction = connection.BeginTransaction()) {
					CheckDatabaseVersion(connection);
					if (HasAlreadyUploadedToday(connection)) {
						message = null;
					} else {
						message = FetchDataForUpload(connection, false);
					}
					transaction.Commit();
				}
			}
			if (message != null) {
				DataContractSerializer serializer = new DataContractSerializer(typeof(UsageDataMessage));
				using (FileStream fs = new FileStream(Path.Combine(Path.GetTempPath(), "SharpDevelopUsageData.xml.gz"), FileMode.Create, FileAccess.Write)) {
					using (GZipStream zip = new GZipStream(fs, CompressionMode.Compress)) {
						serializer.WriteObject(zip, message);
					}
				}
			}
		}
		
		Version expectedDBVersion = new Version(1, 0, 1);
		
		void CheckDatabaseVersion(SQLiteConnection connection)
		{
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "SELECT value FROM Properties WHERE name = 'dbVersion';";
				string version = (string)cmd.ExecuteScalar();
				if (version == null)
					throw new InvalidOperationException("Error retrieving database version");
				Version actualDBVersion = new Version(version);
				if (actualDBVersion != expectedDBVersion) {
					throw new IncompatibleDatabaseException(expectedDBVersion, actualDBVersion);
				}
			}
		}
		
		bool HasAlreadyUploadedToday(SQLiteConnection connection)
		{
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "SELECT value > datetime('now','-1 day') FROM Properties WHERE name='lastUpload';";
				object result = cmd.ExecuteScalar();
				if (result == null)
					return false; // no lastUpload entry -> DB was never uploaded
				return (long)result > 0;
			}
		}
		
		#region FetchDataForUpload
		UsageDataMessage FetchDataForUpload(SQLiteConnection connection, bool fetchIncompleteSessions)
		{
			UsageDataMessage message = new UsageDataMessage();
			// Retrieve the User ID
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "SELECT value FROM Properties WHERE name = 'userID';";
				string userID = (string)cmd.ExecuteScalar();
				message.UserID = new Guid(userID);
			}
			
			// Retrieve the list of sessions
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				if (fetchIncompleteSessions) {
					cmd.CommandText = @"SELECT id, startTime, endTime FROM Sessions;";
				} else {
					// Fetch all sessions which are either closed or inactive for more than 24 hours
					cmd.CommandText = @"SELECT id, startTime, endTime FROM Sessions
						WHERE (endTime IS NOT NULL)
							OR (ifnull((SELECT max(time) FROM FeatureUses WHERE FeatureUses.session = Sessions.id), Sessions.startTime)
								< datetime('now','-1 day'));";
				}
				using (SQLiteDataReader reader = cmd.ExecuteReader()) {
					while (reader.Read()) {
						UsageDataSession session = new UsageDataSession();
						session.SessionID = reader.GetInt64(0);
						session.StartTime = reader.GetDateTime(1);
						if (!reader.IsDBNull(2))
							session.EndTime = reader.GetDateTime(2);
						message.Sessions.Add(session);
					}
				}
			}
			string commaSeparatedSessionIDList = GetCommaSeparatedIDList(message.Sessions);
			
			StringInterner stringInterning = new StringInterner();
			// Retrieve the environment
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "SELECT session, name, value FROM Environment WHERE session IN (" + commaSeparatedSessionIDList + ");";
				using (SQLiteDataReader reader = cmd.ExecuteReader()) {
					while (reader.Read()) {
						long sessionID = reader.GetInt64(0);
						UsageDataSession session = message.FindSession(sessionID);
						session.EnvironmentProperties.Add(
							new UsageDataEnvironmentProperty {
								Name = stringInterning.Intern(reader.GetString(1)),
								Value = stringInterning.Intern(reader.GetString(2))
							});
					}
				}
			}
			
			// Retrieve the feature uses
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "SELECT session, time, endTime, feature, activationMethod FROM FeatureUses WHERE session IN (" + commaSeparatedSessionIDList + ");";
				using (SQLiteDataReader reader = cmd.ExecuteReader()) {
					while (reader.Read()) {
						long sessionID = reader.GetInt64(0);
						UsageDataSession session = message.FindSession(sessionID);
						UsageDataFeatureUse featureUse = new UsageDataFeatureUse();
						featureUse.Time = reader.GetDateTime(1);
						if (!reader.IsDBNull(2))
							featureUse.EndTime = reader.GetDateTime(2);
						featureUse.FeatureName = stringInterning.Intern(reader.GetString(3));
						featureUse.ActivationMethod = stringInterning.Intern(reader.GetString(4));
						session.FeatureUses.Add(featureUse);
					}
				}
			}
			
			// Retrieve the exceptions
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "SELECT session, time, type, stackTrace FROM Exceptions WHERE session IN (" + commaSeparatedSessionIDList + ");";
				using (SQLiteDataReader reader = cmd.ExecuteReader()) {
					while (reader.Read()) {
						long sessionID = reader.GetInt64(0);
						UsageDataSession session = message.FindSession(sessionID);
						UsageDataException exception = new UsageDataException();
						exception.Time = reader.GetDateTime(1);
						exception.ExceptionType = stringInterning.Intern(reader.GetString(2));
						exception.StackTrace = stringInterning.Intern(reader.GetString(3));
						session.Exceptions.Add(exception);
					}
				}
			}
			
			return message;
		}
		#endregion
		
		string GetCommaSeparatedIDList(IEnumerable<UsageDataSession> sessions)
		{
			return string.Join(
				",",
				sessions.Select(s => s.SessionID.ToString(CultureInfo.InvariantCulture)).ToArray());
		}
		
		#region RemoveUploadedData
		/// <summary>
		/// Removes the data that was successfully uploaded and sets the 'lastUpload' property.
		/// </summary>
		void RemoveUploadedData(IEnumerable<UsageDataSession> sessions)
		{
			string commaSeparatedSessionIDList = GetCommaSeparatedIDList(sessions);
			using (SQLiteConnection connection = OpenConnection()) {
				using (SQLiteTransaction transaction = connection.BeginTransaction()) {
					using (SQLiteCommand cmd = connection.CreateCommand()) {
						cmd.CommandText = @"DELETE FROM Sessions WHERE id IN (" + commaSeparatedSessionIDList + @");
							DELETE FROM Environment WHERE session IN (" + commaSeparatedSessionIDList + @");
							DELETE FROM FeatureUses WHERE session IN (" + commaSeparatedSessionIDList + @");
							DELETE FROM Exceptions WHERE session IN (" + commaSeparatedSessionIDList + @");
							INSERT OR REPLACE INTO Properties (name, value) VALUES ('lastUpload', datetime('now'));
						";
						cmd.ExecuteNonQuery();
					}
					transaction.Commit();
				}
			}
		}
		#endregion
		
		/// <summary>
		/// Helps keep the memory usage during data preparation down (there are lots of duplicate strings, and we don't
		/// want to keep them in RAM repeatedly).
		/// </summary>
		sealed class StringInterner
		{
			Dictionary<string, string> cache = new Dictionary<string, string>();
			
			public string Intern(string input)
			{
				if (input != null) {
					string result;
					if (cache.TryGetValue(input, out result))
						return result;
					cache.Add(input, input);
				}
				return input;
			}
		}
	}
}
