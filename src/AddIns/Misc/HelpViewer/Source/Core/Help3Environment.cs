// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using Microsoft.Win32;
using ICSharpCode.Core;
using MSHelpSystem.Core.Native;
using MSHelpSystem.Helper;

namespace MSHelpSystem.Core
{
	public sealed class Help3Environment
	{
		Help3Environment()
		{
		}
		
		public static bool IsHelp3ProtocolRegistered
		{
			get
			{
				try {
					RegistryKey hkcr = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(@"MS-XHelp\shell\open\command", false);
					string helpLibAgent = (string)hkcr.GetValue("", string.Empty);
					hkcr.Close();
					return (!string.IsNullOrEmpty(helpLibAgent));
				}
				catch (Exception ex) {
					LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
				}
				return false;
			}
		}

		public static bool IsLocalStoreInitialized
		{
			get
			{
				string localStore = LocalStore;
				return (!string.IsNullOrEmpty(localStore) && Directory.Exists(localStore));
			}
		}

		public static string LocalStore
		{
			get
			{
				try {
					RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Help\v1.0", false);
					string localStore = (string)hklm.GetValue("LocalStore", string.Empty);
					hklm.Close();
					return localStore;
				}
				catch (Exception ex) {
					LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
				}
				return string.Empty;
			}
		}

		public static string BuildLocalStoreFolder
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); }
		}

		public static string AppRoot
		{
			get
			{
				try {
					RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Help\v1.0", false);
					string appRoot = (string)hklm.GetValue("AppRoot", string.Empty);
					hklm.Close();
					return appRoot;
				}
				catch (Exception ex) {
					LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
				}
				return string.Empty;
			}
		}

		public static string ManifestFolder
		{
			get
			{
				string manifestFolder = LocalStore;
				if (string.IsNullOrEmpty(manifestFolder)) return string.Empty;
				manifestFolder = System.IO.Path.Combine(manifestFolder, "manifest");
				if (Directory.Exists(manifestFolder)) return manifestFolder;
					else return string.Empty;
			}
		}

		public static bool IsLocalHelp
		{
			get { 	return HelpClientWatcher.IsLocalHelp; }
		}
		
		public static string GetHttpFromMsXHelp(string helpUrl)
		{
			if (!HelpLibraryAgent.Start()) { return helpUrl; }
			if (!helpUrl.StartsWith("ms-xhelp://?")) { return helpUrl; }
			return string.Format(
				@"http://127.0.0.1:{0}/help/{1}-{2}/ms.help?{3}",
				HelpLibraryAgent.PortNumber, NativeMethods.GetSessionId(), HelpLibraryAgent.ProcessId, helpUrl.Replace("ms-xhelp://?", "")
			);
		}
	}
}
