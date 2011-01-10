// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.EnterpriseServices.Internal;
using ICSharpCode.Core;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum IISVersion
	{
		IIS5 = 5,
		IIS6,
		IIS7,
		IISExpress,
		IIS_Future
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
		const string DEFAULT_WEB_SITE = "Default Web Site";
		const string IIS_WEB_LOCATION = "IIS://localhost/W3SVC/1/Root";
		
		const string ASPNET_REG_PATH = @"SOFTWARE\MICROSOFT\ASP.NET";
		const string ASPNET_ROOT_VER = @"RootVer";
		
		const string FRAMEWORK_LOCATION = @"%systemroot%\Microsoft.NET\";
		const string FRAMEWORK32 = @"Framework\";
		const string FRAMEWORK64 = @"Framework64\";
		
		public const string IIS_EXPRESS_PROCESS_NAME = "iisexpress";
		public const string IIS_5_PROCESS_NAME = "aspnet_wp";
		public const string IIS_NEW_PROCESS_NAME = "w3wp";
		
		/// <summary>
		/// Gets the IIS worker process name.
		/// </summary>
		public static string WorkerProcessName {
			get {
				if (!IsIISInstalled)
					return ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.IISNotFound");
				
				try {
					string name;
					
					switch(IISVersion)
					{
						case IISVersion.IIS5:
							name = IIS_5_PROCESS_NAME;
							break;
						case IISVersion.IISExpress:
							name = IIS_EXPRESS_PROCESS_NAME;
							break;
						default:
							name = IIS_NEW_PROCESS_NAME;
							break;
					}
					
					return name;
				}
				catch (Exception ex) {
					return ex.Message;
				}
			}
		}
		
		public static string WorkerProcessLocation {
			get {
				if (!IsIISInstalled)
					return ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.IISNotFound");
				
				try {
					string location;
					
					switch(IISVersion)
					{
						case IISVersion.IIS5:
							location = FRAMEWORK_LOCATION + (Environment.Is64BitOperatingSystem ? FRAMEWORK64 : FRAMEWORK32);
							
							string frameworkString = "";
							
							RegistryService.GetRegistryValue<string>(
								RegistryHive.LocalMachine,
								ASPNET_REG_PATH,
								ASPNET_ROOT_VER,
								RegistryValueKind.String,
								out frameworkString);
							int ind = frameworkString.LastIndexOf('.');
							location += "v" + frameworkString.Substring(0, ind) + "\\";
							
							break;
							
						default:
							string regValue = "";
							
							RegistryService.GetRegistryValue<string>(
								RegistryHive.LocalMachine,
								IIS_LOCATION,
								IIS_INSTALL_PATH,
								RegistryValueKind.String,
								out regValue);
							location = regValue + "\\";
							break;
					}
					
					return location;
				}
				catch (Exception ex) {
					return ex.Message;
				}
			}
		}
		
		/// <summary>
		/// Gets a value representing whether IIS is installed.
		/// </summary>
		public static bool IsIISInstalled {
			get {
				return (int)IISVersion > 4;
			}
		}
		
		/// <summary>
		/// Gets a value representing IIS version.
		/// </summary>
		public static IISVersion IISVersion
		{
			get {
				int regValue = 0;
				
				RegistryService.GetRegistryValue<int>(
					RegistryHive.LocalMachine,
					IIS_LOCATION,
					IIS_MAJOR_VERSION,
					RegistryValueKind.DWord,
					out regValue);
				
				return (IISVersion)regValue;
			}
		}
		
		/// <summary>
		/// Creates a virtual directory in IIS.
		/// </summary>
		/// <param name="virtualDirectoryName">Virtual directory name.</param>
		/// <param name="virtualDirectoryPath">Physical path.</param>
		/// <returns></returns>
		public static string CreateVirtualDirectory(string virtualDirectoryName, string physicalDirectoryPath)
		{
			try {
				if (!IsIISInstalled)
					return ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.IISNotFound");
				
				string error;
				
				switch(IISVersion)
				{
					case IISVersion.IIS5:
					case IISVersion.IIS6:
						var vr = new IISVirtualRoot();
						vr.Create(IIS_WEB_LOCATION,
						          physicalDirectoryPath,
						          virtualDirectoryName,
						          out error);
						break;
						
					default:
						using (var manager = new Microsoft.Web.Administration.ServerManager())
						{
							if (manager.Sites[DEFAULT_WEB_SITE] != null) {
								string name = "/" + virtualDirectoryName;
								if (manager.Sites[DEFAULT_WEB_SITE].Applications[name] == null) {
									manager.Sites[DEFAULT_WEB_SITE].Applications.Add(name, physicalDirectoryPath);
									manager.CommitChanges();
									error = string.Empty;
								} else {
									error = ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.ApplicationExists");
								}
							}
							else
								error = ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.MultipleIISServers");
						}
						break;
				}
				
				return error;
			}
			catch (Exception ex) {
				return ex.Message;
			}
		}
	}
}
