// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class SvcUtilPath
	{
		string svcUtilFileName = "svcutil.exe";
		bool exists = true;
		
		public SvcUtilPath()
		{
			FindSvcUtil();
		}
		
		public bool Exists {
			get { return exists; }
		}
		
		void FindSvcUtil()
		{
			if (ServiceReferenceOptions.HasSvcUtilPath) {
				SetSvcUtilPathIfFileExists(ServiceReferenceOptions.SvcUtilPath);
			} else {
				FindSvcUtilFromSdk();
			}
		}
		
		void SetSvcUtilPathIfFileExists(string fileName)
		{
			exists = File.Exists(fileName);
			if (exists) {
				svcUtilFileName = fileName;
			}
		}
		
		void FindSvcUtilFromSdk()
		{
			string sdkPath = TryGetCurrentUserSdkInstallPath();
			if (sdkPath == null) {
				sdkPath = TryGetLocalMachineSdkInstallPath();
			}
			if (sdkPath != null) {
				string fullPath = Path.Combine(sdkPath, @"bin\NETFX 4.0 Tools\svcutil.exe");
				SetSvcUtilPathIfFileExists(fullPath);
			}
		}
		
		string TryGetCurrentUserSdkInstallPath()
		{
			return TryGetSdkInstallPath("HKEY_CURRENT_USER");
		}
		
		string TryGetLocalMachineSdkInstallPath()
		{
			return TryGetSdkInstallPath("HKEY_LOCAL_MACHINE");
		}
		
		string TryGetSdkInstallPath(string root)
		{
			try {
				string keyName = root + @"\SOFTWARE\Microsoft\Microsoft SDKs\Windows";
				return (string)Registry.GetValue(keyName, "CurrentInstallFolder", null);
			} catch (Exception) {
			}
			return null;
		}
		
		public string FileName {
			get { return svcUtilFileName; }
		}
	}
}
