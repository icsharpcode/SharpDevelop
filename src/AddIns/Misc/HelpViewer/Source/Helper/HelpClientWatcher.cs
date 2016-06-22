// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Configuration;
using System.Threading;
using ICSharpCode.Core;
using MSHelpSystem.Core;

namespace MSHelpSystem.Helper
{
	internal sealed class HelpClientWatcher
	{
		HelpClientWatcher()
		{
		}

		static HelpClientWatcher()
		{
			if (!Directory.Exists(clientPath)) {
				Directory.CreateDirectory(clientPath);
			}
			clientFileChanged = new FileSystemWatcher(clientPath);
			clientFileChanged.NotifyFilter = NotifyFilters.LastWrite;
			clientFileChanged.Filter = "HelpClient.cfg";
			clientFileChanged.Changed += new FileSystemEventHandler(OnFileChanged);
			clientFileChanged.Created += new FileSystemEventHandler(OnFileChanged);
			clientFileChanged.EnableRaisingEvents = true;

			helpMode = GetHelpMode();
		}

		static string clientPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\HelpLibrary");
		static FileSystemWatcher clientFileChanged = null;
		static string helpMode = string.Empty;

		static void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			helpMode = GetHelpMode();
			Help3Service.Config.OfflineMode = IsLocalHelp;
		}

		public static bool IsLocalHelp
		{
			get { return helpMode.Equals("offline", StringComparison.InvariantCultureIgnoreCase); }
		}

		public static void EnableLocalHelp()
		{
			helpMode = "offline";
			SetHelpMode();
		}

		public static void EnableOnlineHelp()
		{
			helpMode = "online";
			SetHelpMode();
		}

		static string GetHelpMode()
		{
			Configuration config = null;
			try {
				string clientFile = Path.Combine(clientPath, "HelpClient.cfg");
				if (File.Exists(clientFile)) {
					ExeConfigurationFileMap fm = new ExeConfigurationFileMap();
					fm.ExeConfigFilename = clientFile;
					try {
						config = ConfigurationManager.OpenMappedExeConfiguration(fm, ConfigurationUserLevel.None);
					}
					catch (ConfigurationErrorsException) {
						Thread.Sleep(0x3e8);
						config = ConfigurationManager.OpenMappedExeConfiguration(fm, ConfigurationUserLevel.None);
					}
					if (config != null) {
						AppSettingsSection section = (AppSettingsSection)config.GetSection("appSettings");
						string tmp = section.Settings["helpMode"].Value;
						return (string.IsNullOrEmpty(tmp)) ? "offline":tmp;
					}
				}
				else {
					ExeConfigurationFileMap fm = new ExeConfigurationFileMap();
					fm.ExeConfigFilename = Path.Combine(Help3Environment.AppRoot, "HelpClient.cfg");
					try {
						config = ConfigurationManager.OpenMappedExeConfiguration(fm, ConfigurationUserLevel.None);
					}
					catch (ConfigurationErrorsException) {
						Thread.Sleep(0x3e8);
						config = ConfigurationManager.OpenMappedExeConfiguration(fm, ConfigurationUserLevel.None);
					}
					if (config != null) {
						AppSettingsSection section = (AppSettingsSection)config.GetSection("appSettings");
						string tmp = section.Settings["helpMode"].Value;
						return (string.IsNullOrEmpty(tmp)) ? "offline":tmp;
					}
				}
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("HelpViewer: {0}", ex.ToString()));
			}
			return "offline";
		}

		static void SetHelpMode()
		{
			clientFileChanged.EnableRaisingEvents = false;
			LoggingService.Info(string.Format("HelpViewer: Trying to set help mode to \"{0}\"", helpMode));

			Configuration config = null;
			try {
				string clientFile = Path.Combine(clientPath, "HelpClient.cfg");
				if (!File.Exists(clientFile)) {
					string sourceFile = Path.Combine(Help3Environment.AppRoot, "HelpClient.cfg");
					if (!Directory.Exists(clientPath)) {
						Directory.CreateDirectory(clientPath);
					}
					File.Copy(sourceFile, clientFile);
					Thread.Sleep(0x3e8);
				}
				if (File.Exists(clientFile)) {
					ExeConfigurationFileMap fm = new ExeConfigurationFileMap();
					fm.ExeConfigFilename = clientFile;
					try {
						config = ConfigurationManager.OpenMappedExeConfiguration(fm, ConfigurationUserLevel.None);
					}
					catch (ConfigurationErrorsException) {
						Thread.Sleep(0x3e8);
						config = ConfigurationManager.OpenMappedExeConfiguration(fm, ConfigurationUserLevel.None);
					}
					if (config != null) {
						AppSettingsSection section = (AppSettingsSection)config.GetSection("appSettings");
						section.Settings["helpMode"].Value = helpMode;
						config.Save();
					}
				}
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("HelpViewer: {0}", ex.ToString()));
			}
			clientFileChanged.EnableRaisingEvents = true;
		}
	}
}
