// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace MSHelpSystem.Core
{
	public sealed class HelpLibraryManager
	{
		HelpLibraryManager()
		{
		}

		public static bool IsRunning
		{
			get
			{
				Process[] managers = Process.GetProcessesByName("HelpLibManager");
				LoggingService.Debug(string.Format("Help 3.0: {0} {1} of HelpLibraryManager.exe found", managers.Length, (managers.Length == 1)?"process":"processes"));
				return managers.Length > 0;				
			}
		}

		public static string Manager
		{
			get
			{
				if (string.IsNullOrEmpty(Help3Environment.AppRoot)) return string.Empty;
				string manager = Path.Combine(Help3Environment.AppRoot, "HelpLibManager.exe");
				LoggingService.Debug(string.Format("Help 3.0: Help library manager is \"{0}\"", manager));
				return (File.Exists(manager)) ? manager : string.Empty;
			}
		}

		#region InitializeLocaleStore

		public static void InitializeLocaleStore(string productCode, string productVersion)
		{
			InitializeLocaleStore(productCode, productVersion, CultureInfo.CurrentCulture.Name.ToUpper(), string.Empty);
		}

		public static void InitializeLocaleStore(string productCode, string productVersion, string locale)
		{
			InitializeLocaleStore(productCode, productVersion, locale, string.Empty);
		}

		public static void InitializeLocaleStore(string productCode, string productVersion, string locale, string brandingPackage)
		{
			if (Help3Environment.IsLocalStoreInitialized) { return; }
			if (string.IsNullOrEmpty(productCode)) { throw new ArgumentNullException("productCode"); }
			if (string.IsNullOrEmpty(productVersion)) { throw new ArgumentNullException("productVersion"); }
			if (string.IsNullOrEmpty(locale)) { throw new ArgumentNullException("locale"); }
			if (!Regex.IsMatch(productVersion, @"^\d{3}$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)) { throw new ArgumentOutOfRangeException("productVersion"); }
			if (!Regex.IsMatch(locale, @"^\w{2}-\w{2}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)) { throw new ArgumentOutOfRangeException("locale"); }

			string brandingSwitch = (!string.IsNullOrEmpty(brandingPackage)) ? string.Format("/brandingPackage \"{0}\"", brandingPackage):"";
			string arguments = string.Format("/product {0} /version {1} /locale {2} /content \"{3}\" {4}", productCode, productVersion, locale, Help3Environment.BuildLocalStoreFolder, brandingSwitch);

			LoggingService.Debug(string.Format("Help 3.0: Initializing local store with \"{0}\"", arguments));
			HelpLibManagerProcessRunner(arguments);
		}

		#endregion

		#region InstallHelpDocumentsFromLocalSource

		public static void InstallHelpDocumentsFromLocalSource(string productCode, string productVersion, string locale, string sourceMedia)
		{
			InstallHelpDocumentsFromLocalSource(productCode, productVersion, locale, sourceMedia, string.Empty);
		}

		public static void InstallHelpDocumentsFromLocalSource(string productCode, string productVersion, string locale, string sourceMedia, string brandingPackage)
		{
			if (string.IsNullOrEmpty(productCode)) { throw new ArgumentNullException("productCode"); }
			if (string.IsNullOrEmpty(productVersion)) { throw new ArgumentNullException("productVersion"); }
			if (string.IsNullOrEmpty(locale)) { throw new ArgumentNullException("locale"); }
			if (string.IsNullOrEmpty(sourceMedia)) { throw new ArgumentNullException("sourceMedia"); }
			if (!File.Exists(sourceMedia)) { throw new FileNotFoundException(); }
			if (!Regex.IsMatch(productVersion, @"^\d{3}$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)) { throw new ArgumentOutOfRangeException("productVersion"); }
			if (!Regex.IsMatch(locale, @"^\w{2}-\w{2}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)) { throw new ArgumentOutOfRangeException("locale"); }		

			string initLS = (!Help3Environment.IsLocalStoreInitialized) ? string.Format("/content \"{0}\"", Help3Environment.BuildLocalStoreFolder):"";
			string brandingSwitch = (!string.IsNullOrEmpty(brandingPackage)) ? string.Format("/brandingPackage \"{0}\"", brandingPackage):"";
			string arguments = string.Format("/product {0} /version {1} /locale {2} /sourceMedia \"{3}\" {4} {5}", productCode, productVersion, locale, sourceMedia, initLS, brandingSwitch);

			LoggingService.Debug(string.Format("Help 3.0: Installing local help documents with \"{0}\"", arguments));
			HelpLibManagerProcessRunner(arguments);
		}

		#endregion

		#region InstallHelpDocumentsFromWebSource

		public static void InstallHelpDocumentsFromWebSource(string productCode, string productVersion, string locale, string sourceWeb)
		{
			InstallHelpDocumentsFromWebSource(productCode, productVersion, locale, sourceWeb, string.Empty);
		}

		public static void InstallHelpDocumentsFromWebSource(string productCode, string productVersion, string locale, string sourceWeb, string brandingPackage)
		{
			if (string.IsNullOrEmpty(productCode)) { throw new ArgumentNullException("productCode"); }
			if (string.IsNullOrEmpty(productVersion)) { throw new ArgumentNullException("productVersion"); }
			if (string.IsNullOrEmpty(locale)) { throw new ArgumentNullException("locale"); }
			if (string.IsNullOrEmpty(sourceWeb)) { throw new ArgumentNullException("sourceWeb"); }
			if (!Regex.IsMatch(productVersion, @"^\d{3}$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)) { throw new ArgumentOutOfRangeException("productVersion"); }
			if (!Regex.IsMatch(locale, @"^\w{2}-\w{2}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)) { throw new ArgumentOutOfRangeException("locale"); }		
		
			string initLS = (!Help3Environment.IsLocalStoreInitialized) ? string.Format("/content \"{0}\"", Help3Environment.BuildLocalStoreFolder):"";
			string brandingSwitch = (!string.IsNullOrEmpty(brandingPackage)) ? string.Format("/brandingPackage \"{0}\"", brandingPackage):"";
			string arguments = string.Format("/product {0} /version {1} /locale {2} /sourceWeb \"{3}\" {4} {5}", productCode, productVersion, locale, sourceWeb, initLS, brandingSwitch);

			LoggingService.Debug(string.Format("Help 3.0: Installing help documents from web with \"{0}\"", arguments));
			HelpLibManagerProcessRunner(arguments);
		}

		#endregion

		#region UninstallHelpDocuments

		public static void UninstallHelpDocuments(string productCode, string productVersion, string locale, string vendor, string productName, string mediaBookList)
		{
			if (!Help3Environment.IsLocalStoreInitialized) { return; }
			if (string.IsNullOrEmpty(productCode)) { throw new ArgumentNullException("productCode"); }
			if (string.IsNullOrEmpty(productVersion)) { throw new ArgumentNullException("productVersion"); }
			if (string.IsNullOrEmpty(locale)) { throw new ArgumentNullException("locale"); }
			if (string.IsNullOrEmpty(vendor)) { throw new ArgumentNullException("vendor"); }
			if (string.IsNullOrEmpty(productName)) { throw new ArgumentNullException("productName"); }
			if (string.IsNullOrEmpty(mediaBookList)) { throw new ArgumentNullException("mediaBookList"); }
			if (!Regex.IsMatch(productVersion, @"^\d{3}$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)) { throw new ArgumentOutOfRangeException("productVersion"); }
			if (!Regex.IsMatch(locale, @"^\w{2}-\w{2}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)) { throw new ArgumentOutOfRangeException("locale"); }					

			string arguments = string.Format("/product {0} /version {1} /locale {2} /vendor \"{3}\" /productName \"{4}\" /mediaBookList {5} /uninstall", productCode, productVersion, locale, vendor, productName, mediaBookList);

			LoggingService.Debug(string.Format("Help 3.0: Uninstalling help documents with \"{0}\"", arguments));
			HelpLibManagerProcessRunner(arguments);
		}
		                                         
		#endregion

		static int HelpLibManagerProcessRunner(string arguments)
		{
			return HelpLibManagerProcessRunner(arguments, false);
		}

		static int HelpLibManagerProcessRunner(string arguments, bool silent)
		{
			if (string.IsNullOrEmpty(Manager) || string.IsNullOrEmpty(Help3Environment.AppRoot)) return -10;
			Stop();
			
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = Manager;
			psi.WorkingDirectory = Help3Environment.AppRoot;
			psi.Arguments = string.Format("{0} {1}", arguments, (silent)?"/silent":string.Empty);
			psi.UseShellExecute = true;
			psi.Verb = "runas";
			psi.WindowStyle = ProcessWindowStyle.Normal;

			try {
				Process p = Process.Start(psi);
				p.WaitForExit();
				return p.ExitCode;
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
			}
			return -1;
		}


		public static bool Start()
		{
			if (IsRunning) return true;
			if (string.IsNullOrEmpty(Manager)) {
				throw new ArgumentNullException("Manager");
			}
			if (Help3Service.ActiveCatalog == null) {
				throw new ArgumentNullException("Help3Service.ActiveCatalog");
			}
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = Manager;
			psi.WorkingDirectory = Help3Environment.AppRoot;
			psi.Arguments = Help3Service.ActiveCatalog.AsCmdLineParam;
			psi.UseShellExecute = true;
			psi.Verb = "runas";
			psi.WindowStyle = ProcessWindowStyle.Normal;
			try {
				Process p = Process.Start(psi);
				p.WaitForInputIdle();
				LoggingService.Info("Help 3.0: Help library manager started");
				return IsRunning;
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
			}
			return false;
		}

		public static bool Stop()
		{
			return Stop(true);
		}

		public static bool Stop(bool waitForExit)
		{
			try {
				Process[] managers = Process.GetProcessesByName("HelpLibManager");

				foreach (Process manager in managers) {
					manager.Kill();
					if (waitForExit) manager.WaitForExit();
				}
				LoggingService.Debug(string.Format("Help 3.0: {0} {1} of HelpLibraryManager.exe stopped", managers.Length, (managers.Length == 1)?"process":"processes"));
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
			}			
			return true;
		}
	}
}
