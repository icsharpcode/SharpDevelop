// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.FormsDesigner.UndoRedo;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of FormsDesignerAppDomainHost.
	/// </summary>
	public class FormsDesignerAppDomainHost : MarshalByRefObject, IServiceProvider
	{
		DesignSurface designSurface;
		ServiceContainer container;
		DesignerLoader loader;
		IFormsDesignerLoggingService logger;
		
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
		
		public bool IsLoaderLoading {
			get {
				return this.loader != null && this.loader.Loading;
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
		
		public static FormsDesignerAppDomainHost CreateFormsDesignerInAppDomain(ref AppDomain appDomain, FormsDesignerAppDomainCreationProperties properties)
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
			host.Initialize(properties);
			return host;
		}
		
		void Initialize(FormsDesignerAppDomainCreationProperties properties)
		{
			this.container = new DefaultServiceContainer();
			container.AddService(typeof(FormsDesignerAppDomainHost), this);
			container.AddService(typeof(IFormsDesignerLoggingService), logger = new FormsDesignerLoggingServiceProxy(properties.Logger));
			container.AddService(typeof(System.Drawing.Design.IPropertyValueUIService), new PropertyValueUIService());
			container.AddService(typeof(ITypeResolutionService), new TypeResolutionService(properties.FileName, container, properties.TypeLocator));
			container.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService(properties.GacWrapper, container));
			container.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService(container));
			container.AddService(typeof(AmbientProperties), new AmbientProperties());
			container.AddService(typeof(DesignerOptionService), new SharpDevelopDesignerOptionService(properties.Options));
			
			this.designSurface = designSurfaceManager.CreateDesignSurface(container);
			
			container.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormsDesigner.Services.MenuCommandService(properties.Commands, designSurface).Proxy);
			Services.EventBindingService eventBindingService = new Services.EventBindingService(properties.FormsDesignerProxy, designSurface);
			container.AddService(typeof(System.ComponentModel.Design.IEventBindingService), eventBindingService);
			container.AddService(typeof(IToolboxService), new SharpDevelopToolboxService(this));
//			container.AddService(typeof(System.ComponentModel.Design.IResourceService), new DesignerResourceService(properties.ResourceStore, this));
			
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
		
		public event EventHandler<LoadedEventArgsProxy> DesignSurfaceLoaded;
		
		protected virtual void OnDesignSurfaceLoaded(LoadedEventArgs e)
		{
			if (DesignSurfaceLoaded != null) {
				DesignSurfaceLoaded(this, new LoadedEventArgsProxy { HasSucceeded = e.HasSucceeded });
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
		
		public void BeginDesignSurfaceLoad(IDesignerGenerator generator, IDesignerLoaderProvider loaderProvider)
		{
			var loader = new SharpDevelopDesignerLoader(this, generator, loaderProvider.CreateLoader(generator));
			var provider = loader.GetCodeDomProviderInstance();
			if (provider != null) {
				AddService(typeof(System.CodeDom.Compiler.CodeDomProvider), provider);
			}
			this.loader = loader;
			
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
			get {
				var typeResolutionService = (TypeResolutionService)GetService(typeof(ITypeResolutionService));
				return typeResolutionService != null && typeResolutionService.ReferencedAssemblyChanged;
			}
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
		
		public IMessageService MessageService {
			get {
				return GetService(typeof(IMessageService)) as IMessageService;
			}
		}
		
		public void AddService(Type type, object service)
		{
			Services.AddService(type, service);
		}
		
		public Bitmap LoadComponentIcon(ToolComponent component)
		{
			Assembly asm = component.LoadAssembly();
			Type type = asm.GetType(component.FullName);
			Bitmap b = null;
			if (type != null) {
				object[] attributes = type.GetCustomAttributes(false);
				foreach (object attr in attributes) {
					if (attr is ToolboxBitmapAttribute) {
						ToolboxBitmapAttribute toolboxBitmapAttribute = (ToolboxBitmapAttribute)attr;
						b = new Bitmap(toolboxBitmapAttribute.GetImage(type));
						b.MakeTransparent();
						break;
					}
				}
			}
			if (b == null) {
				try {
					Stream imageStream = asm.GetManifestResourceStream(component.FullName + ".bmp");
					if (imageStream != null) {
						b = new Bitmap(Image.FromStream(imageStream));
						b.MakeTransparent();
					}
				} catch (Exception e) {
					logger.Warn("ComponentLibraryLoader.GetIcon: " + e.Message);
				}
			}
			
			// TODO: Maybe default icon needed ??!?!
			return b;
		}
		
		public AssemblyName GetAssemblyName(ToolComponent component)
		{
			return component.LoadAssembly().GetName();
		}
		
		public void InitializeUndoEngine()
		{
			var undoEngine = new FormsDesignerUndoEngine(this);
			container.AddService(typeof(UndoEngine), undoEngine);
			container.AddService(typeof(IFormsDesignerUndoEngine), new FormsDesignerUndoEngineProxy(undoEngine));
		}
	}
	
	public class FormsDesignerAppDomainCreationProperties : MarshalByRefObject
	{
		public string FileName { get; set; }
		public ITypeLocator TypeLocator { get; set; }
		public IGacWrapper GacWrapper { get; set; }
		public ICommandProvider Commands { get; set; }
		public IFormsDesigner FormsDesignerProxy { get; set; }
		public IFormsDesignerLoggingService Logger { get; set; }
		public SharpDevelopDesignerOptions Options { get; set; }
		public IResourceStore ResourceStore { get; set; }
	}
}
