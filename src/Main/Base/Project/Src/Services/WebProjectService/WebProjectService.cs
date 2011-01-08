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
		const string IIS_REG_KEY_NAME = "Software\\Microsoft\\InetStp";
		const string IIS_REG_KEY_VALUE = "MajorVersion";
		const string DEFAULT_WEB_SITE = "Default Web Site";
		const string IIS_LOCATION = "IIS://localhost/W3SVC/1/Root";
		
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
							name = "aspnet_wp";
							break;
							
						default:
							name = "w3wp";
							break;
					}
					
					return name;
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
					IIS_REG_KEY_NAME,
					IIS_REG_KEY_VALUE,
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
						vr.Create(IIS_LOCATION,
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
