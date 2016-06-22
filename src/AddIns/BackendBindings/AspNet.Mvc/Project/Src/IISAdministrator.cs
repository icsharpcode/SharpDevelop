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
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;

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
			foreach (DomAssemblyName assembly in SD.GlobalAssemblyCache.Assemblies) {
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
