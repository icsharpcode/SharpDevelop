/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 03.10.2007
 * Zeit: 16:54
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.Linq;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class ReportDesignerView : AbstractViewContent, IHasPropertyContainer,
	IClipboardHandler,IUndoHandler, IToolsHost,IPrintable
	{
		
		private bool IsFormsDesignerVisible;
		private bool tabOrderMode;
		private bool hasUnmergedChanges;
		private bool unloading;
		
		string reportFileContent;
		
		private Panel panel = new Panel();
		
		private ReportDesignerLoader loader;
		
		private IDesignerGenerator generator;
		private DesignSurface designSurface;
		private DefaultServiceContainer defaultServiceContainer;
		private ReportDesignerUndoEngine undoEngine;
		
		private XmlView xmlView;
		private ReportPreview preview;
		private ReportViewerSecondaryView reportViewer;
//		private Encoding defaultEncoding = Encoding.UTF8;
		
		#region Constructor
		
		/// <summary>
		/// Creates a new ReportDesignerView object
		/// </summary>
		
		public ReportDesignerView(OpenedFile openedFile, IDesignerGenerator generator):base (openedFile)
		{
			if (openedFile == null) {
				throw new ArgumentNullException("opendFile");
			}
			if (generator == null) {
				throw new ArgumentNullException("generator");
			}
			this.generator = generator;
			this.generator.Attach(this);
			base.TabPageText = ResourceService.GetString("SharpReport.Design");
			this.panel.Dock = DockStyle.Fill;
			this.panel.BackColor = System.Drawing.Color.LightBlue;
			ReportingSideTabProvider.AddViewContent(this);
		}
		
		/// <summary>
		/// This constructor allows running in unit test mode with a mock file. Get it from Matt Ward
		/// </summary>
		public ReportDesignerView(IViewContent primaryViewContent, OpenedFile mockFile)
//			: this(primaryViewContent)
		{
//			this.sourceCodeStorage.AddFile(mockFile, Encoding.UTF8);
//			this.sourceCodeStorage.DesignerCodeFile = mockFile;
		}
		
		
		private void SetupSecondaryView ()
		{
			xmlView = new XmlView(generator,this);
			SecondaryViewContents.Add(xmlView);
			preview = new ReportPreview(loader,this);
			SecondaryViewContents.Add(preview);
			reportViewer = new ReportViewerSecondaryView(loader,this);
			SecondaryViewContents.Add(reportViewer);
		}
		
		#endregion
		
		
		#region setup designer
		
		private void LoadDesigner(Stream stream)
		{
			LoggingService.Info("Form Designer: BEGIN INITIALIZE");
			
			defaultServiceContainer = new DefaultServiceContainer();
			
			defaultServiceContainer.AddService(typeof(System.Windows.Forms.Design.IUIService),
			                                   new UIService());

			defaultServiceContainer.AddService(typeof(IToolboxService),new ToolboxService());
			defaultServiceContainer.AddService(typeof(IHelpService), new HelpService());
			
			this.designSurface = CreateDesignSurface(defaultServiceContainer);
			designSurface.Loading += this.DesignerLoading;
			designSurface.Loaded += this.DesignerLoaded;
			designSurface.Flushed += this.DesignerFlushed;
			designSurface.Unloading += this.DesingerUnloading;
			
			AmbientProperties ambientProperties = new AmbientProperties();
			defaultServiceContainer.AddService(typeof(AmbientProperties), ambientProperties);
			
			defaultServiceContainer.AddService(typeof(ITypeResolutionService), new TypeResolutionService());
			
			defaultServiceContainer.AddService(typeof(ITypeDiscoveryService),
			                                   new TypeDiscoveryService());

			defaultServiceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService),
			                                   new MenuCommandService(panel,this.designSurface ));
			
			defaultServiceContainer.AddService(typeof(MemberRelationshipService),
			                                   new DefaultMemberRelationshipService());
			
			//need this to resolve the filename and manipulate
			//ReportSettings in ReportDefinitionDeserializer.LoadObjectFromXmlDocument
			//if the filename in ReportSettings is different from load location
			
			defaultServiceContainer.AddService(typeof(OpenedFile),base.PrimaryFile);
			
			DesignerOptionService dos = new System.Windows.Forms.Design.WindowsFormsDesignerOptionService();
			dos.Options.Properties.Find( "UseSmartTags", true ).SetValue( dos, true );
			dos.Options.Properties.Find( "ShowGrid", true ).SetValue( dos, false );
			dos.Options.Properties.Find( "UseSnapLines", true ).SetValue( dos, true );
			defaultServiceContainer.AddService( typeof( DesignerOptionService ), dos );
			this.loader = new ReportDesignerLoader(generator,stream);
			this.designSurface.BeginLoad(this.loader);
			if (!designSurface.IsLoaded) {
				throw new FormsDesignerLoadException(FormatLoadErrors(designSurface));
			}
			defaultServiceContainer.AddService(typeof(System.ComponentModel.Design.Serialization.INameCreationService),
			                                   new NameCreationService());
			
			ISelectionService selectionService = (ISelectionService)this.designSurface.GetService(typeof(ISelectionService));
			selectionService.SelectionChanged  += SelectionChangedHandler;
			
			undoEngine = new ReportDesignerUndoEngine(Host);
			
			IComponentChangeService componentChangeService = (IComponentChangeService)this.designSurface.GetService(typeof(IComponentChangeService));
			
			componentChangeService.ComponentChanged += OnComponentChanged;
			componentChangeService.ComponentAdded   += OnComponentListChanged;
			componentChangeService.ComponentRemoved += OnComponentListChanged;
			componentChangeService.ComponentRename  += OnComponentListChanged;
			
			this.Host.TransactionClosed += TransactionClose;
			
			UpdatePropertyPad();
			hasUnmergedChanges = false;
			
			LoggingService.Info("Form Designer: END INITIALIZE");
		}
		
		
		void DesignerLoading(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: DesignerLoader loading...");
//			this.reloadPending = false;
			this.unloading = false;
//			this.UserContent = this.pleaseWaitLabel;
			Application.DoEvents();
		}
		
		
		void DesignerLoaded(object sender, LoadedEventArgs e)
		{
			// This method is called when the designer has loaded.
			LoggingService.Debug("Report designer: DesignerLoader loaded, HasSucceeded=" + e.HasSucceeded.ToString());
//			this.reloadPending = false;
			this.unloading = false;
			
			if (e.HasSucceeded) {
				Control c = null;
				c = this.designSurface.View as Control;
				c.Parent = this.panel;
				c.Dock = DockStyle.Fill;
				this.IsFormsDesignerVisible = true;
				// to set ReportFileContent initially, we need to manually call MergeFormChanges.
				// updates to ReportFileContent will be done using the normal Save support in OpenedFile
				// (that just doesn't work initially because the file isn't dirty)
				generator.MergeFormChanges(null);
				StartReportExplorer ();
				// Display the designer on the view content
//				bool savedIsDirty = this.DesignerCodeFile.IsDirty;
//				Control designView = (Control)this.designSurface.View;
				
//				designView.BackColor = Color.White;
//				designView.RightToLeft = RightToLeft.No;
				// Make sure auto-scaling is based on the correct font.
				// This is required on Vista, I don't know why it works correctly in XP
//				designView.Font = Control.DefaultFont;
				
//				this.UserContent = designView;
				LoggingService.Debug("FormsDesigner loaded, setting ActiveDesignSurface to " + this.designSurface.ToString());
				designSurfaceManager.ActiveDesignSurface = this.designSurface;
//				this.DesignerCodeFile.IsDirty = savedIsDirty;
				this.UpdatePropertyPad();
			} else {
				// This method can not only be called during initialization,
				// but also when the designer reloads itself because of
				// a language change.
				// When a load error occurs there, we are not somewhere
				// below the Load method which handles load errors.
				// That is why we create an error text box here anyway.
				
				
//				TextBox errorTextBox = new TextBox() { Multiline=true, ScrollBars=ScrollBars.Both, ReadOnly=true, BackColor=SystemColors.Window, Dock=DockStyle.Fill };
//				errorTextBox.Text = String.Concat(this.LoadErrorHeaderText, FormatLoadErrors(designSurface));
//				this.UserContent = errorTextBox;
			}
		}
		
		
		private void DesingerUnloading(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: DesignerLoader unloading...");
			this.unloading = true;
//			if (!this.disposing) {
//				this.UserContent = this.pleaseWaitLabel;
//				Application.DoEvents();
//			}
		}
		
		
		private void DesignerFlushed(object sender, EventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("View:DesignerFlushed");
			this.hasUnmergedChanges = false;
		}
		
		
		private static string FormatLoadErrors(DesignSurface designSurface)
		{
			StringBuilder sb = new StringBuilder();
			foreach(Exception le in designSurface.LoadErrors) {
				sb.AppendLine(le.ToString());
				sb.AppendLine();
			}
			return sb.ToString();
		}
		#endregion

		private void MergeFormChanges()
		{
			System.Diagnostics.Trace.WriteLine("View:MergeFormChanges()");
			this.designSurface.Flush();
			generator.MergeFormChanges(null);
			LoggingService.Info("Finished merging form changes");
			hasUnmergedChanges = false;
		}
		
		
		
		
		public string ReportFileContent {
			get {
				if (this.IsDirty) {
					this.MergeFormChanges();
				}
				return this.reportFileContent; }
			set { this.reportFileContent = value; }
		}
		
		
		#region ReportExplorer
		
		private void StartReportExplorer ()
		{
			ReportExplorerPad explorerPad = CheckReportExplorer();
			WorkbenchSingleton.Workbench.ShowPad(WorkbenchSingleton.Workbench.GetPad(typeof(ReportExplorerPad)));
			explorerPad.AddContent(this.loader.ReportModel);
			explorerPad.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportExplorer_PropertyChanged);
		}
		
		
		private void ReportExplorer_PropertyChanged (object sender,System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.MakeDirty();
			ReportExplorerPad explorerPad = CheckReportExplorer();
			IComponentChangeService change = Host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			change.OnComponentChanged(explorerPad.ReportModel.ReportSettings.SortColumnCollection, null, null, null);
		}
		
		
		private static ReportExplorerPad CheckReportExplorer ()
		{
			ReportExplorerPad p = ReportExplorerPad.Instance;
			if (p == null) {
				WorkbenchSingleton.Workbench.GetPad(typeof(ReportExplorerPad)).CreatePad();
			}
			return ReportExplorerPad.Instance;
		}
		
		#endregion
		
		
		#region SelectionService
		
		private void SelectionChangedHandler(object sender, EventArgs args)
		{
			ISelectionService ser = (ISelectionService)sender;
			AbstractItem it = ser.PrimarySelection as AbstractItem;
			if (it != null) {
				if (String.IsNullOrEmpty(it.Site.Name)) {
					it.Site.Name = it.Name;
				}
			}
			UpdatePropertyPadSelection((ISelectionService)sender);
		}
		
		#endregion
		
		#region Transaction
		
		bool shouldUpdateSelectableObjects;
		
		void TransactionClose(object sender, DesignerTransactionCloseEventArgs e)
		{
			if (shouldUpdateSelectableObjects) {
				// update the property pad after the transaction is *really* finished
				// (including updating the selection)
				WorkbenchSingleton.SafeThreadAsyncCall(UpdatePropertyPad);
				shouldUpdateSelectableObjects = false;
			}
		}
		
		
		#endregion
		
		
		#region ComponentChangeService
		
		
		private void OnComponentChanged (object sender, ComponentChangedEventArgs e)
		{
			// More customization of items can be found in 
			//ICSharpCode.Reports.Addin.ReportRootDesigner
			BaseImageItem item = e.Component as BaseImageItem;
			
			if (item != null) {
				item.ReportFileName = this.loader.ReportModel.ReportSettings.FileName;
			}
			
			bool loading = this.loader != null && this.loader.Loading;
			LoggingService.Debug("ReportDesignerView: ComponentChanged: " + (e.Component == null ? "<null>" : e.Component.ToString()) + ", Member=" + (e.Member == null ? "<null>" : e.Member.Name) + ", OldValue=" + (e.OldValue == null ? "<null>" : e.OldValue.ToString()) + ", NewValue=" + (e.NewValue == null ? "<null>" : e.NewValue.ToString()) + "; Loading=" + loading + "; Unloading=" + this.unloading);
			if (!loading && !unloading) {
				this.MakeDirty();
			}
		}
		
		
		void OnComponentListChanged(object sender, EventArgs e)
		{
			bool loading = this.loader != null && this.loader.Loading;
			LoggingService.Debug("ReportDesigner: Component added/removed/renamed, Loading=" + loading + ", Unloading=" + this.unloading);
			if (!loading && !unloading) {
				shouldUpdateSelectableObjects = true;
				this.MakeDirty();
			}
		}
		
		private void MakeDirty()
		{
			hasUnmergedChanges = true;
			this.PrimaryFile.MakeDirty();
		}
		
		
		#endregion
		
		
		#region unload Designer
		/*
		void UnloadDesigner()
		{
			LoggingService.Debug("FormsDesigner unloading, setting ActiveDesignSurface to null");
			designSurfaceManager.ActiveDesignSurface = null;
			
			bool savedIsDirty = (this.DesignerCodeFile == null) ? false : this.DesignerCodeFile.IsDirty;
			this.UserContent = this.pleaseWaitLabel;
			Application.DoEvents();
			if (this.DesignerCodeFile != null) {
				this.DesignerCodeFile.IsDirty = savedIsDirty;
			}
			
			// We cannot dispose the design surface now because of SD2-451:
			// When the switch to the source view was triggered by a double-click on an event
			// in the PropertyPad, "InvalidOperationException: The container cannot be disposed
			// at design time" is thrown.
			// This is solved by calling dispose after the double-click event has been processed.
			if (designSurface != null) {
				designSurface.Loading -= this.DesignerLoading;
				designSurface.Loaded -= this.DesignerLoaded;
				designSurface.Flushed -= this.DesignerFlushed;
				designSurface.Unloading -= this.DesingerUnloading;
				
				IComponentChangeService componentChangeService = designSurface.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				if (componentChangeService != null) {
					componentChangeService.ComponentChanged -= ComponentChanged;
					componentChangeService.ComponentAdded   -= ComponentListChanged;
					componentChangeService.ComponentRemoved -= ComponentListChanged;
					componentChangeService.ComponentRename  -= ComponentListChanged;
				}
				if (this.Host != null) {
					this.Host.TransactionClosed -= TransactionClose;
				}
				
				ISelectionService selectionService = designSurface.GetService(typeof(ISelectionService)) as ISelectionService;
				if (selectionService != null) {
					selectionService.SelectionChanged -= SelectionChangedHandler;
				}
				
				if (disposing) {
					designSurface.Dispose();
				} else {
					WorkbenchSingleton.SafeThreadAsyncCall(designSurface.Dispose);
				}
				designSurface = null;
			}
			
			this.typeResolutionService = null;
			this.loader = null;
			
			foreach (KeyValuePair<Type, TypeDescriptionProvider> entry in this.addedTypeDescriptionProviders) {
				TypeDescriptor.RemoveProvider(entry.Value, entry.Key);
			}
			this.addedTypeDescriptionProviders.Clear();
		}
		 */
		#endregion
		
		#region HasPropertyContainer implementation
		
		private void UpdatePropertyPad()
		{
			if (IsFormsDesignerVisible && Host != null) {
				propertyContainer.Host = Host;
				propertyContainer.SelectableObjects = Host.Container.Components;
				ISelectionService selectionService = (ISelectionService)this.designSurface.GetService(typeof(ISelectionService));
				if (selectionService != null) {
					UpdatePropertyPadSelection(selectionService);
				}
			}
		}
		
		
		private void UpdatePropertyPadSelection(ISelectionService selectionService)
		{
			ICollection selection = selectionService.GetSelectedComponents();
			object[] selArray = new object[selection.Count];
			selection.CopyTo(selArray, 0);
			propertyContainer.SelectedObjects = selArray;
		}
		
		
		#endregion
		
		#region IHasPropertyContainer impementation
		
		PropertyContainer propertyContainer = new PropertyContainer();
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		#endregion
		
		#region IUnDohandler
		
		#endregion
		
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
		
		#region IClipboardHandler implementation
		
		private bool IsMenuCommandEnabled(CommandID commandID)
		{
			if (designSurface == null) {
				return false;
			}
			
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			if (menuCommandService == null) {
				return false;
			}
			
			System.ComponentModel.Design.MenuCommand menuCommand = menuCommandService.FindCommand(commandID);
			if (menuCommand == null) {
				return false;
			}
			return menuCommand.Enabled;
		}
		
		
		public bool EnableCut {
			get {
				return IsMenuCommandEnabled(StandardCommands.Cut);
			}
		}
		
		
		public bool EnableCopy {
			get {
				return IsMenuCommandEnabled(StandardCommands.Copy);
			}
		}
		
		
		const string ComponentClipboardFormat = "CF_DESIGNERCOMPONENTS";
		public bool EnablePaste {
			get {
				return IsMenuCommandEnabled(StandardCommands.Paste);
			}
		}
		
		
		public bool EnableDelete {
			get {
				return IsMenuCommandEnabled(StandardCommands.Delete);
			}
		}
		
		public bool EnableSelectAll {
			get {
				return designSurface != null;
			}
		}
		
		
		public void Cut()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Cut);
		}
		
		
		public void Copy()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Copy);
		}
		
		public void Paste()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Paste);
		}
		
		public void Delete()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Delete);
		}
		
		public void SelectAll()
		{
			MessageService.ShowError(new NotImplementedException());
		}
		
		#endregion
		
		#region IToolsHost
		
		System.Windows.Forms.Control IToolsHost.ToolsControl {
			get {
				return ReportingSideTabProvider.ReportingSideBar;
			}
		}
		#endregion
		
		
		#region IPrintable
		
		public System.Drawing.Printing.PrintDocument PrintDocument {
			get {
				ICSharpCode.Reports.Core.ReportModel model = this.loader.CreateRenderableModel();
				StandartPreviewManager reportManager = new StandartPreviewManager();
				ICSharpCode.Reports.Core.AbstractRenderer r = reportManager.CreateRenderer (model);
				r.ReportDocument.PrintController = new ICSharpCode.Reports.Core.ExtendedPrintController(new System.Drawing.Printing.PreviewPrintController());
				return r.ReportDocument;
			}
		}
		
		#endregion
		
		
		#region IDesignerHost implementation
		
		public IDesignerHost Host {
			get {
				return this.designSurface.GetService(typeof(IDesignerHost)) as IDesignerHost;
			}
		}
		
		#endregion

		#region Commands
		
		public void ShowSourceCode()
		{
			WorkbenchWindow.SwitchView(1);
		}
		
		public void TogglePageMargin ()
		{
			IDesignerHost h = (IDesignerHost)this.designSurface.GetService(typeof(IDesignerHost));
			RootReportModel r = (RootReportModel)h.RootComponent;
			r.Toggle();

		}
		#endregion
		
		#region Tab Order Handling
		
		public virtual bool IsTabOrderMode {
			get {
				return tabOrderMode;
			}
		}
		
		public virtual void ShowTabOrder()
		{
			if (!IsTabOrderMode) {
				IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
				menuCommandService.GlobalInvoke(StandardCommands.TabOrder);
				tabOrderMode = true;
			}
		}
		
		public virtual void HideTabOrder()
		{
			if (IsTabOrderMode) {
				IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
				menuCommandService.GlobalInvoke(StandardCommands.TabOrder);
				tabOrderMode = false;
			}
		}
		#endregion
		
		#region Design surface manager (static)
		
		static readonly DesignSurfaceManager designSurfaceManager = new DesignSurfaceManager();
		
		public static DesignSurface CreateDesignSurface(IServiceProvider serviceProvider)
		{
			return designSurfaceManager.CreateDesignSurface(serviceProvider);
		}
		
		#endregion
		
		#region overrides
		
		public override Control Control {
			get {
				return panel;
			}
		}
		
		
		public override void Load(OpenedFile file, Stream stream)
		{
			LoggingService.Debug("ReportDesigner: Load from: " + file.FileName);
			base.Load(file, stream);
			this.LoadDesigner(stream);
			this.SetupSecondaryView();
		}
		
		
		public override void Save(ICSharpCode.SharpDevelop.OpenedFile file,Stream stream)
		{
			LoggingService.Debug("ReportDesigner: Save to: " + file.FileName);
			
			if (hasUnmergedChanges) {
				this.MergeFormChanges();
			}
			using(StreamWriter writer = new StreamWriter(stream)) {
				writer.Write(this.ReportFileContent);
			}
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public sealed override void Dispose()
		{
			try {
				
				IComponentChangeService componentChangeService = (IComponentChangeService)this.designSurface.GetService(typeof(IComponentChangeService));
				if (componentChangeService != null) {
					componentChangeService.ComponentChanged -= OnComponentChanged;
					componentChangeService.ComponentAdded   -= OnComponentListChanged;
					componentChangeService.ComponentRemoved -= OnComponentListChanged;
					componentChangeService.ComponentRename  -= OnComponentListChanged;
				}
				
				ISelectionService selectionService = (ISelectionService)this.designSurface.GetService(typeof(ISelectionService));
				if (selectionService != null) {
					selectionService.SelectionChanged  -= SelectionChangedHandler;
				}
				

				if (this.loader != null) {
					this.loader.Dispose();
				}
				
				
				if (this.defaultServiceContainer != null) {
					this.defaultServiceContainer.Dispose();
				}
				
				if (this.undoEngine != null) {
					this.undoEngine.Dispose();
				}
				if (this.xmlView != null) {
					this.xmlView.Dispose();
				}
				if (this.preview != null) {
					this.preview.Dispose();
				}
				if (this.reportViewer != null) {
					this.reportViewer.Dispose();
				}
				if (this.panel != null) {
					this.panel.Dispose();
				}
				
				if (this.Host != null) {
					this.Host.TransactionClosed -= TransactionClose;
				}
				
			} finally {
				base.Dispose();
				
			}
		}
		
		#endregion
	}
	
}
