// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of FormsDesignerAppDomainHost.
	/// </summary>
	public class FormsDesignerAppDomainHost : MarshalByRefObject
	{
		DesignSurface designSurface;
		ServiceContainer container;
		string fileName;
		
		public DesignSurface DesignSurface {
			get {
				return designSurface;
			}
		}
		
		public IDesignerHost Host {
			get {
				if (designSurface == null)
					return null;
				return (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
			}
		}
		
		public ServiceContainer Services {
			get { return container; }
		}
		
		static readonly DesignSurfaceManager designSurfaceManager = new DesignSurfaceManager();
		
		public static FormsDesignerAppDomainHost CreateFormsDesignerInAppDomain(ref AppDomain appDomain, ITypeLocator typeLocator, IGacWrapper gacWrapper)
		{
			if (appDomain == null) {
				// Construct and initialize settings for a second AppDomain.
				AppDomainSetup formsDesignerAppDomainSetup = new AppDomainSetup();
				// bamlDecompilerAppDomainSetup.ApplicationBase = "file:///" + Path.GetDirectoryName(assemblyFileName);
				formsDesignerAppDomainSetup.DisallowBindingRedirects = false;
				formsDesignerAppDomainSetup.DisallowCodeDownload = true;
				formsDesignerAppDomainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

				// Create the second AppDomain.
				appDomain = AppDomain.CreateDomain("FormsDesigner AD", null, formsDesignerAppDomainSetup);
			}
			var host = (FormsDesignerAppDomainHost)appDomain.CreateInstanceAndUnwrap(typeof(FormsDesignerAppDomainHost).Assembly.FullName, typeof(FormsDesignerAppDomainHost).FullName);
			
			ServiceContainer container = host.InitServices(typeLocator, gacWrapper);
			host.designSurface = designSurfaceManager.CreateDesignSurface(container);
			
			return host;
		}
		
		ServiceContainer InitServices(ITypeLocator typeLocator, IGacWrapper gacWrapper)
		{
			this.container = new DefaultServiceContainer();
			
			container.AddService(typeof(System.Drawing.Design.IPropertyValueUIService), new PropertyValueUIService());
			container.AddService(typeof(ITypeResolutionService), new TypeResolutionService(fileName, container, typeLocator));
			container.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService(gacWrapper));
			container.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService());
			
			return container;
		}
		
		public void ActivateDesignSurface()
		{
			designSurfaceManager.ActiveDesignSurface = this.designSurface;
		}
		
		public static void DeactivateDesignSurface()
		{
			designSurfaceManager.ActiveDesignSurface = null;
		}
		
		public bool IsActiveDesignSurface {
			get { return designSurfaceManager.ActiveDesignSurface == this.designSurface; }
		}
	}
}
