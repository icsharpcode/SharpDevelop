// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Windows.Forms;

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
		IDesignerGenerator generator;
		bool unloading;
		FormsDesignerAppDomainCreationProperties properties;
		
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
			this.properties = properties;
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
		
		public event EventHandler DesignSurfaceUnloaded;
		
		protected virtual void OnDesignSurfaceUnloaded(EventArgs e)
		{
			if (DesignSurfaceUnloaded != null) {
				DesignSurfaceUnloaded(this, e);
			}
		}
		
		public event EventHandler<ComponentEventArgsProxy> ComponentAdded;
		
		protected virtual void OnComponentAdded(ComponentEventArgsProxy e)
		{
			if (ComponentAdded != null) {
				ComponentAdded(this, e);
			}
		}
		
		public event EventHandler<ComponentEventArgsProxy> ComponentRemoved;
		
		protected virtual void OnComponentRemoved(ComponentEventArgsProxy e)
		{
			if (ComponentRemoved != null) {
				ComponentRemoved(this, e);
			}
		}
		
		public event EventHandler<ComponentRenameEventArgsProxy> ComponentRename;
		
		protected virtual void OnComponentRename(ComponentRenameEventArgsProxy e)
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
		
		void ComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			bool loading = IsLoaderLoading;
			LoggingService.Debug("Forms designer: ComponentChanged: " + (e.Component == null ? "<null>" : e.Component.ToString()) + ", Member=" + (e.Member == null ? "<null>" : e.Member.Name) + ", OldValue=" + (e.OldValue == null ? "<null>" : e.OldValue.ToString()) + ", NewValue=" + (e.NewValue == null ? "<null>" : e.NewValue.ToString()) + "; Loading=" + loading + "; Unloading=" + this.unloading);
			if (!loading && !unloading) {
				try {
					properties.FormsDesignerProxy.MakeDirty();
					if (e.Component != null && e.Member != null && e.Member.Name == "Name" &&
					    e.NewValue is string && !object.Equals(e.OldValue, e.NewValue)) {
						// changing the name of the component
						generator.NotifyComponentRenamed(e.Component, (string)e.NewValue, (string)e.OldValue);
					}
				} catch (Exception ex) {
					MessageService.ShowException(ex, "");
				}
			}
		}
		
		bool shouldUpdateSelectableObjects = false;
		
		void TransactionClose(object sender, DesignerTransactionCloseEventArgs e)
		{
			if (shouldUpdateSelectableObjects) {
				// update the property pad after the transaction is *really* finished
				// (including updating the selection)
				DesignSurfaceView.BeginInvoke((Action)UpdatePropertyPad);
				shouldUpdateSelectableObjects = false;
			}
		}
		
		void ComponentListChanged(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: Component added/removed/renamed, Loading=" + IsLoaderLoading + ", Unloading=" + this.unloading);
			if (!IsLoaderLoading && !unloading) {
				shouldUpdateSelectableObjects = true;
				properties.FormsDesignerProxy.MakeDirty();
			}
		}
		
		void InitializeEvents()
		{
			designSurface.Loading += designSurface_Loading;
			designSurface.Loaded += designSurface_Loaded;
			designSurface.Flushed += designSurface_Flushed;
			designSurface.Unloading += designSurface_Unloading;
			designSurface.Unloaded += designSurface_Unloading;
			
			IComponentChangeService componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentChanged += ComponentChanged;
				componentChangeService.ComponentAdded   += ComponentListChanged;
				componentChangeService.ComponentRemoved += ComponentListChanged;
				componentChangeService.ComponentRename  += ComponentListChanged;
			}
			
			ISelectionService selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null) {
				selectionService.SelectionChanged += selectionService_SelectionChanged;
			}
			
			Host.TransactionClosed += TransactionClose;
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
			OnComponentRename(new ComponentRenameEventArgsProxy { Component = e.Component, NewName = e.NewName, OldName = e.OldName });
		}

		void componentChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			OnComponentRemoved(new ComponentEventArgsProxy { Component = e.Component });
		}

		void componentChangeService_ComponentAdded(object sender, ComponentEventArgs e)
		{
			OnComponentAdded(new ComponentEventArgsProxy { Component = e.Component });
		}
		
		void designSurface_Unloaded(object sender, EventArgs e)
		{
			OnDesignSurfaceUnloaded(e);
		}

		void designSurface_Unloading(object sender, EventArgs e)
		{
			unloading = true;
			OnDesignSurfaceUnloading(e);
		}

		void designSurface_Flushed(object sender, EventArgs e)
		{
			OnDesignSurfaceFlushed(e);
		}

		void designSurface_Loaded(object sender, LoadedEventArgs e)
		{
			unloading = false;
			OnDesignSurfaceLoaded(e);
		}

		void designSurface_Loading(object sender, EventArgs e)
		{
			unloading = false;
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
			this.generator = generator;
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
		
		public Control DesignSurfaceView {
			get {
				return (Control)designSurface.View;
			}
		}
		
		public void DisposeDesignSurface()
		{
			designSurface.Dispose();
		}
		
		public PropertyGrid CreatePropertyGrid()
		{
			var grid = new PropertyGrid() { Dock = DockStyle.Fill };
			
			return grid;
		}
		
		public void UpdatePropertyPad()
		{
			if (HasDesignerHost) {
//				propertyContainer.Host = appDomainHost.Host;
//				propertyContainer.SelectableObjects = appDomainHost.Host.Container.Components;
//				ISelectionService selectionService = (ISelectionService)appDomainHost.GetService(typeof(ISelectionService));
//				if (selectionService != null) {
//					UpdatePropertyPadSelection(selectionService);
//				}
			}
		}
		
		public void UpdatePropertyPadSelection(ISelectionService selectionService)
		{
//			ICollection selection = selectionService.GetSelectedComponents();
//			object[] selArray = new object[selection.Count];
//			selection.CopyTo(selArray, 0);
//			propertyContainer.SelectedObjects = selArray;
			properties.FormsDesignerProxy.InvalidateRequerySuggested();
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
		
		public void UseSDAssembly(string name, string location)
		{
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args) {
				LoggingService.DebugFormatted("Looking for: {0} in {1} {2}", args.Name, name, location);
				if (args.Name == name)
					return Assembly.LoadFile(location);
				return null;
			};
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
