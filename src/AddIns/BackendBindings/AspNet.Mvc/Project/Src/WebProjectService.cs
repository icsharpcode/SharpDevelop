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
using System.EnterpriseServices.Internal;
using System.IO;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using Microsoft.Win32;

namespace ICSharpCode.AspNet.Mvc
{
	public enum IISVersion
	{
		None = 0,
		IIS5 = 5,
		IIS6,
		IIS7,
		IIS8,
		IIS_Future = 100
	}
	
	public enum WebServer
	{
		None,
		IISExpress,
		IIS
	}
	
	/// <summary>
	/// Exposes common operations used in Web Projects
	/// </summary>
	public static class WebProjectService
	{
		const string IIS_LOCATION = "Software\\Microsoft\\InetStp";
		const string IIS_MAJOR_VERSION = "MajorVersion";
		const string IIS_INSTALL_PATH = "InstallPath";
		
		const string ASPNET_REG_PATH = @"SOFTWARE\MICROSOFT\ASP.NET";
		const string ASPNET_ROOT_VER = @"RootVer";
		
		const string FRAMEWORK_LOCATION = @"%systemroot%\Microsoft.NET\";
		const string FRAMEWORK32 = @"Framework\";
		const string FRAMEWORK64 = @"Framework64\";
		
		/// <summary>
		/// Gets &quot;iisexpress&quot; string.
		/// </summary>
		public const string IIS_EXPRESS_PROCESS_NAME = "iisexpress";
		
		/// <summary>
		/// Gets &quot;aspnet_wp&quot; string.
		/// </summary>
		public const string IIS_5_PROCESS_NAME = "aspnet_wp";
		
		/// <summary>
		/// Gets &quot;w3wp&quot; string.
		/// </summary>
		public const string IIS_NEW_PROCESS_NAME = "w3wp";
		
		/// <summary>
		/// Gets IIS Express process location.
		/// </summary>
		public static string IISExpressProcessLocation {
			get {
				return Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles) +
					@"\IIS Express\iisexpress.exe";
			}
		}
		
		/// <summary>
		/// Gets the IIS worker process name.
		/// </summary>
		public static string GetWorkerProcessName(WebServer webServer)
		{
			if (webServer == WebServer.IISExpress) {
				return GetIISExpressWorkerProcessName();
			}
			return GetIISWorkerProcessName();
		}
		
		public static string GetWorkerProcessName(WebProjectProperties properties)
		{
			if (properties.UseIISExpress) {
				return GetIISExpressWorkerProcessName();
			}
			return GetIISWorkerProcessName();
		}
		
		public static string GetIISExpressWorkerProcessName()
		{
			if (!IsIISExpressInstalled)
				return ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound");
			
			return IIS_EXPRESS_PROCESS_NAME;
		}
		
		public static string GetIISWorkerProcessName()
		{
			if (!IsIISInstalled)
				return ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound");
			
			try {
				switch (IISVersion) {
					case IISVersion.IIS5:
						return IIS_5_PROCESS_NAME;
					default:
						return IIS_NEW_PROCESS_NAME;
				}
			}
			catch (Exception ex) {
				return ex.Message;
			}
		}
		
		public static string GetWorkerProcessLocation(WebServer webServer)
		{
			if (webServer == WebServer.IISExpress) {
				return GetIISExpressWorkerProcessLocation();
			}
			return GetIISWorkerProcessLocation();
		}
		
		public static string GetIISExpressWorkerProcessLocation()
		{
			if (!IsIISExpressInstalled)
				return ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound");
					
			return GetDefaultIISWorkerProcessLocation();
		}
		
		public static string GetIISWorkerProcessLocation()
		{
			if (!IsIISInstalled)
				return ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound");
			
			try {
				if (IISVersion == IISVersion.IIS5) {
					return GetIIS5WorkerProcessLocation();
				}
				return GetDefaultIISWorkerProcessLocation();
			}
			catch (Exception ex) {
				return ex.Message;
			}
		}
		
		public static string GetIIS5WorkerProcessLocation()
		{
			string location = FRAMEWORK_LOCATION + (Environment.Is64BitOperatingSystem ? FRAMEWORK64 : FRAMEWORK32);
			
			string frameworkString = GetRegistryStringValue(ASPNET_REG_PATH, ASPNET_ROOT_VER);
			int ind = frameworkString.LastIndexOf('.');
			location += "v" + frameworkString.Substring(0, ind) + "\\";
			return location;
		}
		
		static string GetRegistryStringValue(string keyName, string valueName)
		{
			object value = GetRegistryValue(keyName, valueName);
			if (value != null) {
				return value.ToString();
			}
			return String.Empty;
		}
		
		static object GetRegistryValue(string keyName, string valueName)
		{
			try {
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyName)) {
					if (registryKey != null) {
						return registryKey.GetValue(valueName);
					}
				}
			} catch {
				// Do nothing.
			}
			return null;
		}
		
		static int GetRegistryIntegerValue(string keyName, string valueName)
		{
			object value = GetRegistryValue(keyName, valueName);
			if (value != null) {
				try {
					return Convert.ToInt32(value);
				} catch {
					// Do nothing.
				}
			}
			return 0;
		}
		
		public static string GetDefaultIISWorkerProcessLocation()
		{
			return GetRegistryValue(IIS_LOCATION, IIS_INSTALL_PATH) + @"\";
		}
		
		/// <summary>
		/// Gets a value representing whether IIS is installed.
		/// </summary>
		public static bool IsIISInstalled {
			get { return (int)IISVersion >= 4; }
		}
		
		public static bool IsIISExpressInstalled {
			get { return File.Exists(IISExpressProcessLocation); }
		}
		
		public static bool IsIISOrIISExpressInstalled {
			get { return IsIISInstalled || IsIISExpressInstalled; }
		}
		
		/// <summary>
		/// Gets a value representing IIS version.
		/// </summary>
		public static IISVersion IISVersion
		{
			get {
				int regValue = GetRegistryIntegerValue(IIS_LOCATION, IIS_MAJOR_VERSION);
				if (regValue > 4)
					return (IISVersion)regValue;
				
				return IISVersion.None;
			}
		}
	}
}
