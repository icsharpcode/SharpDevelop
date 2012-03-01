// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AspNet.Mvc
{
	public abstract class IISAdministrator
	{
		public const string DEFAULT_WEB_SITE = "Default Web Site";

		protected IISAdministrator(WebProjectProperties properties)
		{
			this.Properties = properties;
		}
		
		protected IISAdministrator()
		{
		}
		
		protected WebProjectProperties Properties { get; set; }
		
		public static IISAdministrator CreateAdministrator(WebProjectProperties properties)
		{
			if (properties.UseIISExpress) {
				return new IISExpressAdministrator(properties);
			}
			switch (WebProjectService.IISVersion) {
				case IISVersion.IIS5:
				case IISVersion.IIS6:
					return new IIS6Administrator(properties);
				case IISVersion.None:
					return new NullIISAdministrator();
				default:
					return new IIS7Administrator(properties);
			}
		}
		
		public abstract bool IsIISInstalled();
		
		public abstract void CreateVirtualDirectory(WebProject project);
		
		protected dynamic CreateServerManager()
		{
			Assembly webAdministrationAssembly = GetWebAdminstrationAssembly();
			return webAdministrationAssembly.CreateInstance("Microsoft.Web.Administration.ServerManager");
		}
		
		protected Assembly GetWebAdminstrationAssembly()
		{
			foreach(DomAssemblyName assembly in GacInterop.GetAssemblyList()) {
				if (assembly.FullName.Contains("Microsoft.Web.Administration")) {
					if (IsServerManagementVersionRequired(assembly)) {
						return Assembly.Load(assembly.FullName);
					}
				}
			}
			throw new ApplicationException(ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound"));
		}

		
		protected virtual bool IsServerManagementVersionRequired(DomAssemblyName assemblyName)
		{
			return true;
		}
		
		protected void ThrowApplicationExistsException()
		{
			throw new ApplicationException(ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.ApplicationExists"));
		}
	}
}
