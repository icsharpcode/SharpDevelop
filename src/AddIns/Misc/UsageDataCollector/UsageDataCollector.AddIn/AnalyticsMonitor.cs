// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UsageDataCollector.Contracts;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Main singleton class of the analytics. This class is thread-safe.
	/// </summary>
	public sealed partial class AnalyticsMonitor : IAnalyticsMonitor
	{
		const string UploadUrl = "http://usagedatacollector.sharpdevelop.net/upload/UploadUsageData.svc";
		const string ProductName = "sharpdevelop";
		public static readonly Uri PrivacyStatementUrl = new Uri("http://www.icsharpcode.net/OpenSource/SD/UsageDataCollector/");
		
		public static readonly AnalyticsMonitor Instance = new AnalyticsMonitor();
		
		public static bool EnabledIsUndecided {
			get {
				return string.IsNullOrEmpty(PropertyService.Get("ICSharpCode.UsageDataCollector.Enabled"));
			}
		}
		
		/// <summary>
		/// Allows to enable/disable the usage data monitoring.
		/// </summary>
		public static bool Enabled {
			get {
				return string.Equals(PropertyService.Get("ICSharpCode.UsageDataCollector.Enabled"), bool.TrueString, StringComparison.OrdinalIgnoreCase);
			}
			set {
				PropertyService.Set("ICSharpCode.UsageDataCollector.Enabled", value.ToString());
				// Initially opening the session takes some time; which is bad for the startpage
				// because the animation would start with a delay. We solve this by calling Open/CloseSession
				// on a background thread.
				ThreadPool.QueueUserWorkItem(delegate { AsyncEnableDisable(); } );
			}
		}
		
		static void AsyncEnableDisable()
		{
			if (System.Diagnostics.Debugger.IsAttached)
				return;
			if (Enabled) {
				Instance.OpenSession();
			} else {
				Instance.CloseSession();
				Instance.TryDeleteDatabase();
			}
		}
		
		readonly object lockObj = new object();
		string dbFileName;
		UsageDataSessionWriter session;
		
		private AnalyticsMonitor()
		{
			var container = ServiceManager.Instance.GetRequiredService<ThreadSafeServiceContainer>();
			container.TryAddService(typeof(IAnalyticsMonitor), this);
			dbFileName = Path.Combine(PropertyService.ConfigDirectory, "usageData.dat");
			
			SharpDevelop.Gui.WorkbenchSingleton.WorkbenchUnloaded += delegate { CloseSession(); };
		}
		
		static Guid FindUserId()
		{
			// Ensure we assign only 1 ID to each user; even when he has multiple UDC databases because there
			// are multiple SharpDevelop versions installed. We do this by reading out the userID GUID from
			// the existing databases in any neighbor config directory.
			string[] otherSharpDevelopVersions;
			try {
				otherSharpDevelopVersions = Directory.GetDirectories(Path.Combine(PropertyService.ConfigDirectory, ".."));
			} catch (IOException) {
				otherSharpDevelopVersions = new string[0];
			} catch (UnauthorizedAccessException) {
				otherSharpDevelopVersions = new string[0];
			}
			LoggingService.Debug("Looking for existing UDC database in " + otherSharpDevelopVersions.Length + " directories");
			foreach (string path in otherSharpDevelopVersions) {
				string dbFileName = Path.Combine(path, "usageData.dat");
				if (File.Exists(dbFileName)) {
					LoggingService.Info("Found existing UDC database: " + dbFileName);
					Guid? guid = UsageDataSessionWriter.RetrieveUserId(dbFileName);
					if (guid.HasValue) {
						LoggingService.Info("Found GUID in existing UDC database: " + guid.Value);
						return guid.Value;
					}
				}
			}
			LoggingService.Info("Did not find existing UDC database; creating new GUID.");
			return Guid.NewGuid();
		}
		
		/// <summary>
		/// Opens the database connection, updates the database if required.
		/// Will start an upload to the server, if required.
		/// </summary>
		public void OpenSession()
		{
			IEnumerable<UsageDataEnvironmentProperty> appEnvironmentProperties = GetAppProperties();
			bool sessionOpened = false;
			lock (lockObj) {
				if (session == null) {
					try {
						session = new UsageDataSessionWriter(dbFileName, FindUserId);
					} catch (IncompatibleDatabaseException ex) {
						if (ex.ActualVersion < ex.ExpectedVersion) {
							LoggingService.Info("AnalyticsMonitor: " + ex.Message + ", removing old database");
							Guid? oldUserId = UsageDataSessionWriter.RetrieveUserId(dbFileName);
							// upgrade database by deleting the old one
							TryDeleteDatabase();
							try {
								session = new UsageDataSessionWriter(dbFileName, () => (oldUserId ?? FindUserId()));
							} catch (IncompatibleDatabaseException ex2) {
								LoggingService.Warn("AnalyticsMonitor: Could not upgrade database: " + ex2.Message);
							}
						} else {
							LoggingService.Warn("AnalyticsMonitor: " + ex.Message);
						}
					}
					
					if (session != null) {
						session.OnException = MessageService.ShowException;
						session.AddEnvironmentData(appEnvironmentProperties);
						
						sessionOpened = true;
					}
				}
			}
			if (sessionOpened) {
				UsageDataUploader uploader = new UsageDataUploader(dbFileName, ProductName);
				uploader.EnvironmentDataForDummySession = appEnvironmentProperties;
				ThreadPool.QueueUserWorkItem(delegate { uploader.StartUpload(UploadUrl); });
			}
		}
		
		static IEnumerable<UsageDataEnvironmentProperty> GetAppProperties()
		{
			List<UsageDataEnvironmentProperty> properties = new List<UsageDataEnvironmentProperty> {
				new UsageDataEnvironmentProperty { Name = "appVersion", Value = RevisionClass.Major + "." + RevisionClass.Minor + "." + RevisionClass.Build + "." + RevisionClass.Revision },
				new UsageDataEnvironmentProperty { Name = "language", Value = ResourceService.Language },
				new UsageDataEnvironmentProperty { Name = "culture", Value = CultureInfo.CurrentCulture.Name },
				new UsageDataEnvironmentProperty { Name = "userAddInCount", Value = AddInTree.AddIns.Count(a => !a.IsPreinstalled).ToString() },
				new UsageDataEnvironmentProperty { Name = "branch", Value = BranchName },
				new UsageDataEnvironmentProperty { Name = "commit", Value = CommitHash },
				new UsageDataEnvironmentProperty { Name = "renderingTier", Value = (RenderCapability.Tier >> 16).ToString() }
			};
			string PROCESSOR_ARCHITECTURE = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
			if (string.IsNullOrEmpty(PROCESSOR_ARCHITECTURE)) {
				PROCESSOR_ARCHITECTURE = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
			}
			if (!string.IsNullOrEmpty(PROCESSOR_ARCHITECTURE)) {
				properties.Add(new UsageDataEnvironmentProperty { Name = "architecture", Value = PROCESSOR_ARCHITECTURE });
			}
			#if DEBUG
			properties.Add(new UsageDataEnvironmentProperty { Name = "debug", Value = "true" });
			#endif
			return properties;
		}
		
		/// <summary>
		/// Retrieves all stored data as XML text.
		/// </summary>
		/// <exception cref="IncompatibleDatabaseException">The database version is not compatible with this
		/// version of the AnalyticsSessionWriter.</exception>
		public string GetTextForStoredData()
		{
			lock (lockObj) {
				if (session != null)
					session.Flush();
			}
			UsageDataUploader uploader = new UsageDataUploader(dbFileName, ProductName);
			return uploader.GetTextForStoredData();
		}
		
		void TryDeleteDatabase()
		{
			lock (lockObj) {
				CloseSession();
				try {
					File.Delete(dbFileName);
				} catch (IOException ex) {
					LoggingService.Warn("AnalyticsMonitor: Could delete database: " + ex.Message);
				} catch (AccessViolationException ex) {
					LoggingService.Warn("AnalyticsMonitor: Could delete database: " + ex.Message);
				}
			}
		}
		
		public void CloseSession()
		{
			lock (lockObj) {
				if (session != null) {
					session.Dispose();
					session = null;
				}
			}
		}
		
		public void TrackException(Exception exception)
		{
			lock (lockObj) {
				if (session != null) {
					session.AddException(exception);
				}
			}
		}
		
		public IAnalyticsMonitorTrackedFeature TrackFeature(string featureName, string activationMethod)
		{
			TrackedFeature feature = new TrackedFeature();
			lock (lockObj) {
				if (session != null) {
					feature.Feature = session.AddFeatureUse(featureName, activationMethod);
				}
			}
			return feature;
		}
		
		sealed class TrackedFeature : IAnalyticsMonitorTrackedFeature
		{
			internal FeatureUse Feature;
			
			public void EndTracking()
			{
				if (Feature != null)
					Feature.TrackEndTime();
			}
		}
	}
	
	public class AutoStartCommand : AbstractCommand
	{
		public override void Run()
		{
			if (System.Diagnostics.Debugger.IsAttached)
				return;
			if (AnalyticsMonitor.Enabled)
				AnalyticsMonitor.Instance.OpenSession();
		}
	}
}
