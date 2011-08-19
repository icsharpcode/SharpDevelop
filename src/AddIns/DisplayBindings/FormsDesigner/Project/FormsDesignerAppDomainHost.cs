// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of FormsDesignerAppDomainHost.
	/// </summary>
	public class FormsDesignerAppDomainHost : MarshalByRefObject, IServiceProvider
	{
		DesignSurface designSurface;
		ServiceContainer container;
		
		public string DesignSurfaceName {
			get {
				return (designSurface == null) ? null : designSurface.ToString();
			}
		}
		
		public bool HasDesignerHost {
			get {
				return Host != null;
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
		
		public static FormsDesignerAppDomainHost CreateFormsDesignerInAppDomain(ref AppDomain appDomain, string fileName, ITypeLocator typeLocator, IGacWrapper gacWrapper, ICommandProvider commands, IFormsDesigner formsDesignerProxy, IFormsDesignerLoggingService logger)
		{
			if (appDomain == null) {
				// Construct and initialize settings for a second AppDomain.
				AppDomainSetup formsDesignerAppDomainSetup = new AppDomainSetup();
				formsDesignerAppDomainSetup.ApplicationBase = Path.GetDirectoryName(typeof(FormsDesignerAppDomainHost).Assembly.Location);
				formsDesignerAppDomainSetup.DisallowBindingRedirects = false;
				formsDesignerAppDomainSetup.DisallowCodeDownload = true;
				formsDesignerAppDomainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

				// Create the second AppDomain.
				appDomain = AppDomain.CreateDomain("FormsDesigner AD", null, formsDesignerAppDomainSetup);
			}
			var host = (FormsDesignerAppDomainHost)appDomain.CreateInstanceAndUnwrap(typeof(FormsDesignerAppDomainHost).Assembly.FullName, typeof(FormsDesignerAppDomainHost).FullName);
			host.Initialize(fileName, typeLocator, gacWrapper, commands, formsDesignerProxy, logger);
			return host;
		}
		
		void Initialize(string fileName, ITypeLocator typeLocator, IGacWrapper gacWrapper, ICommandProvider commands, IFormsDesigner formsDesignerProxy, IFormsDesignerLoggingService logger)
		{
			this.container = new DefaultServiceContainer();
			container.AddService(typeof(IFormsDesignerLoggingService), logger);
			container.AddService(typeof(System.Drawing.Design.IPropertyValueUIService), new PropertyValueUIService());
			container.AddService(typeof(ITypeResolutionService), new TypeResolutionService(fileName, container, typeLocator));
			container.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService(gacWrapper, container));
			container.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService(container));
			
			this.designSurface = designSurfaceManager.CreateDesignSurface(container);
			
			container.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormsDesigner.Services.MenuCommandService(commands, designSurface).Proxy);
			Services.EventBindingService eventBindingService = new Services.EventBindingService(formsDesignerProxy, designSurface);
			container.AddService(typeof(System.ComponentModel.Design.IEventBindingService), eventBindingService);
			
			InitializeEvents();
		}
		
		#region Events
		public event EventHandler DesignSurfaceLoading;
		
		protected virtual void OnDesignSurfaceLoading(EventArgs e)
		{
			if (DesignSurfaceLoading != null) {
				DesignSurfaceLoading(this, e);
			}
		}
		
		public event LoadedEventHandler DesignSurfaceLoaded;
		
		protected virtual void OnDesignSurfaceLoaded(LoadedEventArgs e)
		{
			if (DesignSurfaceLoaded != null) {
				DesignSurfaceLoaded(this, e);
			}
		}
		
		public event EventHandler DesignSurfaceFlushed;
		
		protected virtual void OnDesignSurfaceFlushed(EventArgs e)
		{
			if (DesignSurfaceFlushed != null) {
				DesignSurfaceFlushed(this, e);
			}
		}

		public event EventHandler DesignSurfaceUnloading;
		
		protected virtual void OnDesignSurfaceUnloading(EventArgs e)
		{
			if (DesignSurfaceUnloading != null) {
				DesignSurfaceUnloading(this, e);
			}
		}
		
		public event ComponentChangedEventHandler ComponentChanged;
		
		protected virtual void OnComponentChanged(ComponentChangedEventArgs e)
		{
			if (ComponentChanged != null) {
				ComponentChanged(this, e);
			}
		}
		
		public event ComponentEventHandler ComponentAdded;
		
		protected virtual void OnComponentAdded(ComponentEventArgs e)
		{
			if (ComponentAdded != null) {
				ComponentAdded(this, e);
			}
		}
		
		public event ComponentEventHandler ComponentRemoved;
		
		protected virtual void OnComponentRemoved(ComponentEventArgs e)
		{
			if (ComponentRemoved != null) {
				ComponentRemoved(this, e);
			}
		}
		
		public event ComponentRenameEventHandler ComponentRename;
		
		protected virtual void OnComponentRename(ComponentRenameEventArgs e)
		{
			if (ComponentRename != null) {
				ComponentRename(this, e);
			}
		}
		
		public event DesignerTransactionCloseEventHandler HostTransactionClosed;
		
		protected virtual void OnHostTransactionClosed(DesignerTransactionCloseEventArgs e)
		{
			if (HostTransactionClosed != null) {
				HostTransactionClosed(this, e);
			}
		}
		
		public event EventHandler SelectionChanged;
		
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null) {
				SelectionChanged(this, e);
			}
		}
		
		void InitializeEvents()
		{
			designSurface.Loading += designSurface_Loading;
			designSurface.Loaded += designSurface_Loaded;
			designSurface.Flushed += designSurface_Flushed;
			designSurface.Unloading += designSurface_Unloading;
			
			IComponentChangeService componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentChanged += componentChangeService_ComponentChanged;
				componentChangeService.ComponentAdded   += componentChangeService_ComponentAdded;
				componentChangeService.ComponentRemoved += componentChangeService_ComponentRemoved;
				componentChangeService.ComponentRename  += componentChangeService_ComponentRename;
			}
			
			ISelectionService selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null) {
				selectionService.SelectionChanged += selectionService_SelectionChanged;
			}
			
			Host.TransactionClosed += Host_TransactionClosed;
		}

		void selectionService_SelectionChanged(object sender, EventArgs e)
		{
			OnSelectionChanged(e);
		}

		void Host_TransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
		{
			OnHostTransactionClosed(e);
		}

		void componentChangeService_ComponentRename(object sender, ComponentRenameEventArgs e)
		{
			OnComponentRename(e);
		}

		void componentChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			OnComponentRemoved(e);
		}

		void componentChangeService_ComponentAdded(object sender, ComponentEventArgs e)
		{
			OnComponentAdded(e);
		}

		void componentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			OnComponentChanged(e);
		}

		void designSurface_Unloading(object sender, EventArgs e)
		{
			OnDesignSurfaceUnloading(e);
		}

		void designSurface_Flushed(object sender, EventArgs e)
		{
			OnDesignSurfaceFlushed(e);
		}

		void designSurface_Loaded(object sender, LoadedEventArgs e)
		{
			OnDesignSurfaceLoaded(e);
		}

		void designSurface_Loading(object sender, EventArgs e)
		{
			OnDesignSurfaceLoading(e);
		}
		#endregion
		
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
		
		public object GetService(Type serviceType)
		{
			return designSurface.GetService(serviceType);
		}
		
		public void BeginDesignSurfaceLoad(DesignerLoader loader)
		{
			designSurface.BeginLoad(loader);
		}
		
		static string FormatLoadErrors(DesignSurface designSurface)
		{
			StringBuilder sb = new StringBuilder();
			foreach(Exception le in designSurface.LoadErrors) {
				sb.AppendLine(le.ToString());
				sb.AppendLine();
			}
			return sb.ToString();
		}
		
		public string LoadErrors {
			get { return FormatLoadErrors(designSurface); }
		}
		
		public bool IsDesignSurfaceLoaded {
			get { return designSurface.IsLoaded; }
		}
		
		public bool ReferencedAssemblyChanged {
			get { return ((TypeResolutionService)GetService(typeof(ITypeResolutionService))).ReferencedAssemblyChanged; }
		}
		
		public void FlushDesignSurface()
		{
			designSurface.Flush();
		}
		
		WindowsFormsHost host;
		
		public INativeHandleContract DesignSurfaceView {
			get {
				if (host == null) {
					host = new WindowsFormsHost();
					host.Child = (Control)designSurface.View;
				}
				
				return FrameworkElementAdapters.ViewToContractAdapter(host);
			}
		}
		
		public void DisposeDesignSurface()
		{
			designSurface.Dispose();
		}
		
		public INativeHandleContract CreatePropertyGrid()
		{
			var grid = new PropertyGrid() { Dock = DockStyle.Fill };
			var host = new WindowsFormsHost();
			host.Child = grid;
			
			return FrameworkElementAdapters.ViewToContractAdapter(host);
		}
		
		public IFormsDesignerLoggingService LoggingService {
			get {
				return GetService(typeof(IFormsDesignerLoggingService)) as IFormsDesignerLoggingService;
			}
		}
		
		public void AddService(Type type, object service)
		{
			Services.AddService(type, service);
		}
	}
}
