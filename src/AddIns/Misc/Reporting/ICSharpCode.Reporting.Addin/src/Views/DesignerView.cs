/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.02.2014
 * Time: 19:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using System.Collections;
using System.ComponentModel.Design;

using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.DesignableItems;
using ICSharpCode.Reporting.Addin.DesignerBinding;
using ICSharpCode.Reporting.Addin.Services;
using ICSharpCode.Reporting.Addin.Toolbox;
using ICSharpCode.Reporting.Addin.UndoRedo;

namespace ICSharpCode.Reporting.Addin.Views
{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class DesignerView : AbstractViewContent,IHasPropertyContainer, IToolsHost,IUndoHandler,IClipboardHandler
	{
		readonly IDesignerGenerator generator;
		ReportDesignerUndoEngine undoEngine;
		bool unloading;
		bool hasUnmergedChanges;
		bool shouldUpdateSelectableObjects;
		bool isFormsDesignerVisible;
		string reportFileContent;
		Panel panel;
		ReportDesignerLoader loader;
		DefaultServiceContainer defaultServiceContainer;
		DesignSurface designSurface;
		
		public DesignerView (OpenedFile openedFile,IDesignerGenerator generator) : base(openedFile){
			if (openedFile == null) {
				throw new ArgumentNullException("openedFile");
			}
			LoggingService.Info("DesignerView: Load from: " + openedFile.FileName);
			
			TabPageText = ResourceService.GetString("SharpReport.Design");
			
			this.generator = generator;
			this.generator.Attach(this);
			//Start Toolbox
			ToolboxProvider.AddViewContent(this);
		}

	
		void LoadDesigner (Stream stream) {
			LoggingService.Info("ReportDesigner LoadDesigner_Start");
			panel = CreatePanel();
			defaultServiceContainer = CreateAndInitServiceContainer();
			
			LoggingService.Info("Create DesignSurface and add event's");
			designSurface = CreateDesignSurface(defaultServiceContainer);
			SetDesignerEvents();
			
			var ambientProperties = new AmbientProperties();
			defaultServiceContainer.AddService(typeof(AmbientProperties), ambientProperties);
			
			defaultServiceContainer.AddService(typeof(ITypeResolutionService), new TypeResolutionService());
			defaultServiceContainer.AddService(typeof(ITypeDiscoveryService),new TypeDiscoveryService());
			                                   
			defaultServiceContainer.AddService(typeof(IMenuCommandService),
				new ICSharpCode.Reporting.Addin.Services.MenuCommandService(panel, designSurface));
			
			defaultServiceContainer.AddService(typeof(MemberRelationshipService),new DefaultMemberRelationshipService());
			defaultServiceContainer.AddService(typeof(OpenedFile),base.PrimaryFile);
			
			LoggingService.Info("Load DesignerOptionService");
			var designerOptionService = CreateDesignerOptions();
			defaultServiceContainer.AddService( typeof( DesignerOptionService ), designerOptionService );
			
			LoggingService.Info("Create ReportDesignerLoader"); 
			
			loader = new ReportDesignerLoader(generator, stream);
			designSurface.BeginLoad(this.loader);
			if (!designSurface.IsLoaded) {
				//				throw new FormsDesignerLoadException(FormatLoadErrors(designSurface));
				LoggingService.Error("designer not loaded");
			}
			//-------------
			
			defaultServiceContainer.AddService(typeof(INameCreationService),new NameCreationService());
			                                   
			
			var selectionService = (ISelectionService)this.designSurface.GetService(typeof(ISelectionService));
			selectionService.SelectionChanged  += SelectionChangedHandler;
			
			undoEngine = new ReportDesignerUndoEngine(Host);
		
			var componentChangeService = (IComponentChangeService)this.designSurface.GetService(typeof(IComponentChangeService));
			
			
			componentChangeService.ComponentChanged += OnComponentChanged;
			componentChangeService.ComponentAdded   += OnComponentListChanged;
			componentChangeService.ComponentRemoved += OnComponentListChanged;
			componentChangeService.ComponentRename  += OnComponentListChanged;
			
			this.Host.TransactionClosed += TransactionClose;
		
			UpdatePropertyPad();
	
			hasUnmergedChanges = false;
			
			LoggingService.Info("ReportDesigner LoadDesigner_End");
		}	

		
		static Panel CreatePanel ()
		{
			var ctl = new Panel();
			ctl.Dock = DockStyle.Fill;
			ctl.BackColor = System.Drawing.Color.LightBlue;
			return ctl;
		}

		
		static DefaultServiceContainer CreateAndInitServiceContainer()
		{
			LoggingService.Debug("ReportDesigner: CreateAndInitServiceContainer...");
			var serviceContainer = new DefaultServiceContainer();
			serviceContainer.AddService(typeof(IUIService), new UIService());
			serviceContainer.AddService(typeof(IToolboxService),new ICSharpCode.Reporting.Addin.Services.ToolboxService());
			serviceContainer.AddService(typeof(IHelpService), new HelpService());
			return serviceContainer;
		}
		
		
		void SetDesignerEvents()
		{
			LoggingService.Debug("ReportDesigner: SetDesignerEvents...");
			
			designSurface.Loading += DesignerLoading;
			designSurface.Loaded += DesignerLoaded;
			designSurface.Flushed += DesignerFlushed;
			designSurface.Unloading += DesingerUnloading;
		}
		
		
		static WindowsFormsDesignerOptionService CreateDesignerOptions()
		{
			LoggingService.Debug("ReportDesigner: CreateDesignerOptions...");
			
			var designerOptionService = new WindowsFormsDesignerOptionService();
			designerOptionService.Options.Properties.Find("UseSmartTags", true).SetValue(designerOptionService, true);
			designerOptionService.Options.Properties.Find("ShowGrid", true).SetValue(designerOptionService, false);
			designerOptionService.Options.Properties.Find("UseSnapLines", true).SetValue(designerOptionService, true);
			return designerOptionService;
		}
	
		#region ComponentChangeService
		
		void OnComponentChanged (object sender, ComponentChangedEventArgs e)
		{
//			BaseImageItem item = e.Component as BaseImageItem;
//			
//			if (item != null) {
//				item.ReportFileName = this.loader.ReportModel.ReportSettings.FileName;
//			}
			
			bool loading = this.loader != null && this.loader.Loading;
			LoggingService.Debug("ReportDesignerView: ComponentChanged: " + (e.Component == null ? "<null>" : e.Component.ToString()) + ", Member=" + (e.Member == null ? "<null>" : e.Member.Name) + ", OldValue=" + (e.OldValue == null ? "<null>" : e.OldValue.ToString()) + ", NewValue=" + (e.NewValue == null ? "<null>" : e.NewValue.ToString()) + "; Loading=" + loading + "; Unloading=" + this.unloading);
			if (!loading && !unloading) {
				this.MakeDirty();
			}
//			MergeFormChanges();
		}
		
		
		void OnComponentListChanged(object sender, EventArgs e)
		{
			bool loading = this.loader != null && this.loader.Loading;
			LoggingService.Debug("ReportDesigner: Component added/removed/renamed, Loading=" + loading + ", Unloading=" + this.unloading);
			if (!loading && !unloading) {
				shouldUpdateSelectableObjects = true;
				MakeDirty();
			}
		}
		
		void MakeDirty()
		{
			hasUnmergedChanges = true;
			PrimaryFile.MakeDirty();
		}
		
		#endregion
		
		#region SelectionService
		
		void SelectionChangedHandler(object sender, EventArgs args)
		{
			var selectionService = (ISelectionService)sender;
			var abstractItem = selectionService.PrimarySelection as AbstractItem;
			if (abstractItem != null) {
				if (String.IsNullOrEmpty(abstractItem.Site.Name)) {
					abstractItem.Site.Name = abstractItem.Name;
				}
			}
			UpdatePropertyPadSelection((ISelectionService)sender);
		}
		
		#endregion
		
		#region Transaction
		
		void TransactionClose(object sender, DesignerTransactionCloseEventArgs e)
		{
			if (shouldUpdateSelectableObjects) {
				// update the property pad after the transaction is *really* finished
				// (including updating the selection)
//				WorkbenchSingleton.SafeThreadAsyncCall(UpdatePropertyPad);
				shouldUpdateSelectableObjects = false;
			}
		}
		
		#endregion
		
		
		#region IToolsHost
		
		object IToolsHost.ToolsContent {
			get {
				return ToolboxProvider.ReportingSideBar;
			}
		}
		
		#endregion
		
		
		#region HasPropertyContainer implementation
		
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		
		void UpdatePropertyPad()
		{
			if (isFormsDesignerVisible && Host != null) {
				propertyContainer.Host = Host;
				propertyContainer.SelectableObjects = Host.Container.Components;
				var selectionService = (ISelectionService)this.designSurface.GetService(typeof(ISelectionService));
				if (selectionService != null) {
					UpdatePropertyPadSelection(selectionService);
				}
			}
		}
		
		
		void UpdatePropertyPadSelection(ISelectionService selectionService)
		{
			ICollection selection = selectionService.GetSelectedComponents();
			var selArray = new object[selection.Count];
			selection.CopyTo(selArray, 0);
			propertyContainer.SelectedObjects = selArray;
		}
		
		
		#endregion
	
		
		#region DesignerEvents
		
		void DesignerLoading(object sender, EventArgs e)
		{
			LoggingService.Debug("ReportDesigner: Event > DesignerLoader loading...");
			this.unloading = false;
		}
		
		
		void DesignerLoaded(object sender, LoadedEventArgs e)
		{
			LoggingService.Debug("ReportDesigner: Event > DesignerLoaded...");
			this.unloading = false;
			
			if (e.HasSucceeded) {

				SetupDesignSurface();
				isFormsDesignerVisible = true;
				generator.MergeFormChanges(null);
//				StartReportExplorer ();

				LoggingService.Debug("FormsDesigner loaded, setting ActiveDesignSurface to " + this.designSurface.ToString());
				designSurfaceManager.ActiveDesignSurface = this.designSurface;
				UpdatePropertyPad();
			}
		}

		void DesignerFlushed(object sender, EventArgs e)
		{
			LoggingService.Debug("ReportDesigner: Event > DesignerFlushed");
		}

		
		void DesingerUnloading(object sender, EventArgs e)
		{
			LoggingService.Debug("ReportDesigner: Event > DesignernUnloading...");
		}

		
		#endregion
		
		
		#region Design surface manager (static)
		
		static readonly DesignSurfaceManager designSurfaceManager = new DesignSurfaceManager();
		
		static DesignSurface CreateDesignSurface(IServiceProvider serviceProvider)
		{
			return designSurfaceManager.CreateDesignSurface(serviceProvider);
		}
		
		#endregion
		
		#region IDesignerHost implementation
		
		public IDesignerHost Host {
			get {
				return this.designSurface.GetService(typeof(IDesignerHost)) as IDesignerHost;
			}
		}
		
		#endregion
		
		
		#region IUndoHandler implementation
		
		public bool EnableUndo {
			get {
				if (undoEngine != null) {
					return undoEngine.EnableUndo;
				}
				return false;
			}
		}
		
		
		public bool EnableRedo {
			get {
				if (undoEngine != null) {
					return undoEngine.EnableRedo;
				}
				return false;
			}
		}
		
		
		public virtual void Undo()
		{
			if (undoEngine != null) {
				undoEngine.Undo();
			}
		}
		
		
		public virtual void Redo()
		{
			if (undoEngine != null) {
				undoEngine.Redo();
			}
		}
		
		#endregion		
		
		
		#region IClipboardHandler implementation
		
		
		public void Cut()
		{
			var menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Cut);
		}
		
		public void Copy()
		{
			var menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Copy);
		}
		
		public void Paste()
		{
			var menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Paste);
		}
		
		public void Delete()
		{
			var menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Delete);
		}
		
		public void SelectAll()
		{
			throw new NotImplementedException();
		}
		
		public bool EnableCut {
			get {return IsMenuCommandEnabled(StandardCommands.Cut);}
		}
		
		public bool EnableCopy {
			get {return IsMenuCommandEnabled(StandardCommands.Copy);}
		}
		
		public bool EnablePaste {
			get {return IsMenuCommandEnabled(StandardCommands.Paste);}
		}
		
		
		public bool EnableDelete {
			get {return IsMenuCommandEnabled(StandardCommands.Delete);}
		}
		
		
		public bool EnableSelectAll {
			get {return designSurface != null;}
		}
		
		
		bool IsMenuCommandEnabled(CommandID commandID)
		{
			if (designSurface == null) {
				return false;
			}
			
			var menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			if (menuCommandService == null) {
				return false;
			}
			
			MenuCommand menuCommand = menuCommandService.FindCommand(commandID);
			if (menuCommand == null) {
				return false;
			}
			return menuCommand.Enabled;
		}
		#endregion		
		
		
		#region Commands
		
		public void ShowSourceCode()
		{
			WorkbenchWindow.SwitchView(1);
		}
		
		#endregion
		#region UI
		
		void SetupDesignSurface()
		{
			var ctrl = designSurface.View as Control;
			ctrl.Parent = panel;
			ctrl.Dock = DockStyle.Fill;
		}

		
		
		void MergeFormChanges()
		{
			LoggingService.Info("MergeFormChanges");
			this.designSurface.Flush();
			generator.MergeFormChanges(null);
			hasUnmergedChanges = false;
		}

		void SetupSecondaryView()
		{
			if (!SecondaryViewContents.OfType<XmlView>().Any())
			{
				var xmlView = new XmlView(generator, this);
				SecondaryViewContents.Add(xmlView);
			}
			if (!SecondaryViewContents.OfType<WpfPreview>().Any())
			{
				var preview = new WpfPreview(loader, this);
				SecondaryViewContents.Add(preview);
			}
		}
		
		public string ReportFileContent {
			get {
				if (IsDirty) {
					MergeFormChanges();
				}
				return reportFileContent; }
			set { reportFileContent = value; }
		}
		
		#endregion
		
		#region overrides
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		public override object Control {
			get {return panel;}
		}
		
		
		public override void Load(OpenedFile file, Stream stream)
		{
			LoggingService.Debug("ReportDesigner: Load from: " + file.FileName);
			base.Load(file, stream);
			LoadDesigner(stream);
			SetupSecondaryView();
		}
		
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (IsDirty) {
				if (hasUnmergedChanges) {
					MergeFormChanges();
				}
				using(var writer = new StreamWriter(stream)) {
					writer.Write(ReportFileContent);
				}
			}
		}
		
		#endregion
	}
	
}
