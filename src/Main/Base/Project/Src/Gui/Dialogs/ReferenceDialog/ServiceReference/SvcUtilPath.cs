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
