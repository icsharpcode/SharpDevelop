// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of DesignerAppDomainHost.
	/// </summary>
	public class DesignerAppDomainHost
	{
		static DesignerAppDomainHost instance;
		DesignerAppDomainManager manager;
		AppDomain appDomain;
		
		public AppDomain DesignerAppDomain {
			get { return appDomain; }
		}
		
		public static DesignerAppDomainHost Instance {
			get { Initialize(); return instance; }
		}
		
		public DesignerAppDomainHost(DesignerAppDomainManager manager, AppDomain appDomain)
		{
			this.manager = manager;
			this.appDomain = appDomain;
		}
		
		static void Initialize()
		{
			if (instance != null && instance.appDomain != null)
				return;
			
			// Construct and initialize settings for a second AppDomain.
			AppDomainSetup formsDesignerAppDomainSetup = new AppDomainSetup();
			formsDesignerAppDomainSetup.ApplicationBase = Path.GetDirectoryName(typeof(DesignerAppDomainManager).Assembly.Location);
			formsDesignerAppDomainSetup.DisallowBindingRedirects = false;
			formsDesignerAppDomainSetup.DisallowCodeDownload = true;
			formsDesignerAppDomainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

			// Create the second AppDomain.
			AppDomain appDomain = AppDomain.CreateDomain("FormsDesigner AD", null, formsDesignerAppDomainSetup);
			
			var manager = (DesignerAppDomainManager)appDomain.CreateInstanceAndUnwrap(typeof(DesignerAppDomainManager).Assembly.FullName, typeof(DesignerAppDomainManager).FullName);
			
			instance = new DesignerAppDomainHost(manager, appDomain);
		}
		
		public FormsDesignerManager CreateFormsDesignerInAppDomain(FormsDesignerCreationProperties creationProperties)
		{
			return manager.CreateFormsDesignerInAppDomain(creationProperties);
		}
	}
}
