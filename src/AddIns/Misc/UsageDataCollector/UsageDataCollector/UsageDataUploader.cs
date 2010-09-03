// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

using ICSharpCode.UsageDataCollector.Contracts;
using UdcProxy = ICSharpCode.UsageDataCollector.ServiceReference;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Allows uploading collected data.
	/// </summary>
	public class UsageDataUploader
	{
		string databaseFileName;
		string productName;
		
		/// <summary>
		/// Creates a new UsageDataUploader.
		/// </summary>
		public UsageDataUploader(string databaseFileName, string productName)
		{
			if (databaseFileName == null)
				throw new ArgumentNullException("databaseFileName");
			if (productName == null)
				throw new ArgumentNullException("productName");
			this.databaseFileName = databaseFileName;
			this.productName = productName;
		}
		
		/// <summary>
		/// Gets/Sets environment data that is sent with the dummy session on the first upload.
		/// </summary>
		public IEnumerable<UsageDataEnvironmentProperty> EnvironmentDataForDummySession { get; set; }
		
		SQLiteConnection OpenConnection()
		{
			SQLiteConnectionStringBuilder conn = new SQLiteConnectionStringBuilder();
			conn.Add("Data Source", databaseFileName);
			SQLiteConnection connection = new SQLiteConnection(conn.ConnectionString);
			connection.Open();
			return connection;
		}
		
		/// <summary>
		/// Retrieves all stored data as XML text.
		/// </summary>
		/// <exception cref="IncompatibleDatabaseException">The database version is not compatible with this
		/// version of the AnalyticsSessionWriter.</exception>
		public string GetTextForStoredData()
		{
			UsageDataMessage message;
			using (SQLiteConnection connection = OpenConnection()) {
				using (SQLiteTransaction transaction = connection.BeginTransaction()) {
					CheckDatabaseVersion(connection);
					message = FetchDataForUpload(connection, true);
				}
			}
			using (StringWriter w = new StringWriter(CultureInfo.InvariantCulture)) {
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
		/// <exception cref="IncompatibleDatabaseException">The database version is not compatible with this
		/// version of the AnalyticsSessionWriter.</exception>
		public void StartUpload(string uploadUrl)
		{
			EndpointAddress epa = new EndpointAddress(uploadUrl);
			BasicHttpBinding binding = new BasicHttpBinding();
			binding.Security.Mode = BasicHttpSecurityMode.None;
			binding.TransferMode = TransferMode.Streamed;
			binding.MessageEncoding = WSMessageEncoding.Mtom;
			StartUpload(binding, epa);
		}
		
		/// <summary>
		/// Starts the upload of the usage data.
		/// </summary>
		/// <exception cref="IncompatibleDatabaseException">The database version is not compatible with this
		/// version of the AnalyticsSessionWriter.</exception>
		public void StartUpload(Binding binding, EndpointAddress endpoint)
		{
			UsageDataMessage message;
			bool addDummySession = false;
			using (SQLiteConnection connection = OpenConnection()) {
				using (SQLiteTransaction transaction = connection.BeginTransaction()) {
					CheckDatabaseVersion(connection);
					HasUploadedTodayStatus status = HasAlreadyUploadedToday(connection);
					if (status == HasUploadedTodayStatus.Yes) {
						message = null;
					} else {
						message = FetchDataForUpload(connection, false);
						if (status == HasUploadedTodayStatus.NeverUploaded)
							addDummySession = true;
					}
					transaction.Commit();
				}
			}
			if (message != null) {
				string commaSeparatedSessionIDList = GetCommaSeparatedIDList(message.Sessions);
				
				if (addDummySession) {
					// A dummy session is used for the first upload to notify the server of the user's environment.
					// Without this, we couldn't tell the version of a user who tries SharpDevelop once but doesn't use it long
					// enough for an actual session to be uploaded.
					UsageDataSession dummySession = new UsageDataSession();
					dummySession.SessionID = 0;
					dummySession.StartTime = DateTime.UtcNow;
					dummySession.EnvironmentProperties.AddRange(UsageDataSessionWriter.GetDefaultEnvironmentData());
					if (this.EnvironmentDataForDummySession != null)
						dummySession.EnvironmentProperties.AddRange(this.EnvironmentDataForDummySession);
					message.Sessions.Add(dummySession);
				}
				
				DataContractSerializer serializer = new DataContractSerializer(typeof(UsageDataMessage));
				byte[] data;
				using (MemoryStream ms = new MemoryStream()) {
					using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress)) {
						serializer.WriteObject(zip, message);
					}
					data = ms.ToArray();
				}
				
				UdcProxy.UDCUploadServiceClient client = new UdcProxy.UDCUploadServiceClient(binding, endpoint);
				try {
					client.BeginUploadUsageData(productName, new MemoryStream(data),
					                            ar => UsageDataUploaded(ar, client, commaSeparatedSessionIDList), null);
				} catch (CommunicationException) {
					// ignore error (maybe currently not connected to network)
				} catch (TimeoutException) {
					// ignore error (network connection trouble?)
				}
			}
		}
		
		void UsageDataUploaded(IAsyncResult result, UdcProxy.UDCUploadServiceClient client, string commaSeparatedSessionIDList)
		{
			bool success = false;
			try {
				client.EndUploadUsageData(result);
				success = true;
			} catch (CommunicationException) {
				// ignore error (maybe currently not connected to network)
			} catch (TimeoutException) {
				// ignore error (network connection trouble?)
			}
			client.Close();
			
			if (success) {
				RemoveUploadedData(commaSeparatedSessionIDList);
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
		
		enum HasUploadedTodayStatus
		{
			No,
			Yes,
			NeverUploaded
		}
		
		static HasUploadedTodayStatus HasAlreadyUploadedToday(SQLiteConnection connection)
		{
			using (SQLiteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "SELECT value > datetime('now','-1 day') FROM Properties WHERE name='lastUpload';";
				object result = cmd.ExecuteScalar();
				if (result == null)
					return HasUploadedTodayStatus.NeverUploaded; // no lastUpload entry -> DB was never uploaded
				return (long)result > 0 ? HasUploadedTodayStatus.Yes : HasUploadedTodayStatus.No;
			}
		}
		
		#region FetchDataForUpload
		static UsageDataMessage FetchDataForUpload(SQLiteConnection connection, bool fetchIncompleteSessions)
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
						if (!reader.IsDBNull(4))
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
						if (!reader.IsDBNull(3))
							exception.StackTrace = stringInterning.Intern(reader.GetString(3));
						session.Exceptions.Add(exception);
					}
				}
			}
			
			return message;
		}
		#endregion
		
		static string GetCommaSeparatedIDList(IEnumerable<UsageDataSession> sessions)
		{
			return string.Join(
				",",
				sessions.Select(s => s.SessionID.ToString(CultureInfo.InvariantCulture)).ToArray());
		}
		
		#region RemoveUploadedData
		/// <summary>
		/// Removes the data that was successfully uploaded and sets the 'lastUpload' property.
		/// </summary>
		void RemoveUploadedData(string commaSeparatedSessionIDList)
		{
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
