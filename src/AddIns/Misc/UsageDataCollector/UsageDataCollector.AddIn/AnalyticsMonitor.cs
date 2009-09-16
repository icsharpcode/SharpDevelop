// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop;
using System.Threading;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Main singleton class of the analytics. This class is thread-safe.
	/// </summary>
	public sealed class AnalyticsMonitor : IAnalyticsMonitor
	{
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
		
		/// <summary>
		/// Opens the database connection, updates the database if required.
		/// Will start an upload to the server, if required.
		/// </summary>
		public void OpenSession()
		{
			bool sessionOpened = false;
			lock (lockObj) {
				if (session == null) {
					try {
						session = new UsageDataSessionWriter(dbFileName);
						session.OnException = MessageService.ShowException;
						session.AddEnvironmentData("appVersion", RevisionClass.FullVersion);
						session.AddEnvironmentData("language", ResourceService.Language);
						sessionOpened = true;
					} catch (IncompatibleDatabaseException ex) {
						if (ex.ActualVersion < ex.ExpectedVersion) {
							LoggingService.Info("AnalyticsMonitor: " + ex.Message + ", removing old database");
							// upgrade database by deleting the old one
							TryDeleteDatabase();
							try {
								session = new UsageDataSessionWriter(dbFileName);
							} catch (IncompatibleDatabaseException ex2) {
								LoggingService.Warn("AnalyticsMonitor: Could upgrade database: " + ex2.Message);
							}
						} else {
							LoggingService.Warn("AnalyticsMonitor: " + ex.Message);
						}
					}
				}
			}
			if (sessionOpened) {
				UsageDataUploader uploader = new UsageDataUploader(dbFileName);
				ThreadPool.QueueUserWorkItem(delegate { uploader.StartUpload(); });
			}
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
			UsageDataUploader uploader = new UsageDataUploader(dbFileName);
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
					// TODO: recognize inner exceptions
					session.AddException(exception.GetType().FullName, exception.StackTrace);
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
			if (AnalyticsMonitor.Enabled)
				AnalyticsMonitor.Instance.OpenSession();
		}
	}
}
