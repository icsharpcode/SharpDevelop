// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.FormsDesigner.UndoRedo;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Util;

namespace ICSharpCode.FormsDesigner
{
	public class FormsDesignerViewContent : AbstractViewContentHandlingLoadErrors, IClipboardHandler, IUndoHandler, IHasPropertyContainer, IContextHelpProvider, IToolsHost
	{
		Panel p = new Panel();
		DesignSurface designSurface;
		bool disposing;
		bool loadingDesigner;
		
		readonly IViewContent primaryViewContent;
		readonly IDesignerLoaderProvider loaderProvider;
		readonly IDesignerGenerator generator;
		readonly ResourceStore resourceStore;
		FormsDesignerUndoEngine undoEngine;
		
		Encoding primaryFileEncoding;
		readonly IDocument primaryFileDocument = new DocumentFactory().CreateDocument();
		Encoding designerCodeFileEncoding;
		OpenedFile designerCodeFile;
		IDocument designerCodeFileDocument;
		
		protected DesignSurface DesignSurface {
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
		
		public OpenedFile DesignerCodeFile {
			get { return this.designerCodeFile; }
		}
		
		public virtual IDocument PrimaryFileDocument {
			get { return this.primaryFileDocument; }
		}
		
		public string PrimaryFileContent {
			get { return this.PrimaryFileDocument.TextContent; }
			set { this.PrimaryFileDocument.TextContent = value; }
		}
		
		public virtual Encoding PrimaryFileEncoding {
			get { return this.primaryFileEncoding; }
		}
		
		public virtual IDocument DesignerCodeFileDocument {
			get { return this.designerCodeFileDocument; }
		}
		
		public string DesignerCodeFileContent {
			get { return this.DesignerCodeFileDocument.TextContent; }
			set { this.DesignerCodeFileDocument.TextContent = value; }
		}
		
		public virtual Encoding DesignerCodeFileEncoding {
			get { return this.designerCodeFileEncoding; }
		}
		
		public IViewContent PrimaryViewContent {
			get { return this.primaryViewContent; }
		}
		
		FormsDesignerViewContent(IViewContent primaryViewContent)
			: base()
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			
			if (!FormKeyHandler.inserted) {
				FormKeyHandler.Insert();
			}
			
			this.primaryViewContent = primaryViewContent;
			
			p.BackColor    = Color.White;
			p.RightToLeft = RightToLeft.No;
			// Make sure auto-scaling is based on the correct font.
			// This is required on Vista, I don't know why it works correctly in XP
			p.Font = Control.DefaultFont;
			
			this.UserControl = this.p;
			
			this.resourceStore = new ResourceStore(this);
			
			// null check is required to support running in unit test mode
			if (WorkbenchSingleton.Workbench != null) {
				this.IsActiveViewContentChanged += this.IsActiveViewContentChangedHandler;
			}
		}
		
		public FormsDesignerViewContent(IViewContent primaryViewContent, IDesignerLoaderProvider loaderProvider, IDesignerGenerator generator)
			: this(primaryViewContent)
		{
			if (loaderProvider == null)
				throw new ArgumentNullException("loaderProvider");
			if (generator == null)
				throw new ArgumentNullException("generator");
			
			this.loaderProvider    = loaderProvider;
			this.generator = generator;
			this.generator.Attach(this);
			
			this.Files.Add(this.primaryViewContent.PrimaryFile);
		}
		
		/// <summary>
		/// This constructor allows running in unit test mode with a mock file.
		/// </summary>
		public FormsDesignerViewContent(IViewContent primaryViewContent, OpenedFile mockFile)
			: this(primaryViewContent)
		{
			this.primaryFileDocument = new DocumentFactory().CreateDocument();
			this.designerCodeFileDocument = this.primaryFileDocument;
			this.designerCodeFileEncoding = System.Text.Encoding.UTF8;
			this.designerCodeFile = mockFile;
		}
		
		protected override void LoadInternal(OpenedFile file, System.IO.Stream stream)
		{
			LoggingService.Debug("Forms designer: Load " + file.FileName);
			
			if (file == this.PrimaryFile) {
				
				this.primaryFileEncoding = ParserService.DefaultFileEncoding;
				this.primaryFileDocument.TextContent = FileReader.ReadFileContent(stream, ref this.primaryFileEncoding);
				
				LoggingService.Debug("Forms designer: Determining designer code file for " + file.FileName);
				OpenedFile newDesignerCodeFile = this.generator.DetermineDesignerCodeFile();
				if (newDesignerCodeFile == null) {
					throw new InvalidOperationException("The designer code file could not be determined.");
				}
				
				if (this.designerCodeFile != null && newDesignerCodeFile != this.designerCodeFile) {
					this.Files.Remove(this.designerCodeFile);
				}
				this.designerCodeFile = newDesignerCodeFile;
				
				if (this.designerCodeFile == this.PrimaryFile) {
					
					LoggingService.Debug("Forms designer: Designer code file is equal to primary file. Reloading designer.");
					
					this.UnloadDesigner();
					this.designerCodeFileEncoding = this.PrimaryFileEncoding;
					this.designerCodeFileDocument = this.PrimaryFileDocument;
					this.LoadAndDisplayDesigner();
					
				} else if (!this.Files.Contains(this.designerCodeFile)) {
					LoggingService.Debug("Forms designer: Adding designer code file " + this.designerCodeFile.FileName);
					this.Files.Insert(1, this.designerCodeFile);
				}
				
			} else if (file == this.DesignerCodeFile) {
				
				LoggingService.Debug("Forms designer: Reloading designer because of LoadInternal on DesignerCodeFile");
				
				this.UnloadDesigner();
				this.designerCodeFileEncoding = ParserService.DefaultFileEncoding;
				this.designerCodeFileDocument = new DocumentFactory().CreateDocument();
				this.designerCodeFileDocument.TextContent = FileReader.ReadFileContent(stream, ref this.designerCodeFileEncoding);
				this.LoadAndDisplayDesigner();
				
			} else {
				
				// Loading a resource file
				
				bool mustReload;
				if (this.designSurface != null && !this.loadingDesigner) {
					LoggingService.Debug("Forms designer: Reloading designer because of LoadInternal on resource file");
					this.UnloadDesigner();
					mustReload = true;
				} else {
					mustReload = false;
				}
				LoggingService.Debug("Forms designer: Loading " + file.FileName + " in resource store");
				this.resourceStore.Load(file, stream);
				if (mustReload) {
					this.LoadAndDisplayDesigner();
				}
				
			}
		}
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			LoggingService.Debug("Forms designer: Save " + file.FileName);
			if (hasUnmergedChanges) {
				this.MergeFormChanges();
			}
			if (file == this.DesignerCodeFile) {
				using(StreamWriter writer = new StreamWriter(stream, this.DesignerCodeFileEncoding)) {
					writer.Write(this.DesignerCodeFileContent);
				}
			} else if (file == this.PrimaryFile) {
				using(StreamWriter writer = new StreamWriter(stream, this.PrimaryFileEncoding)) {
					writer.Write(this.PrimaryFileContent);
				}
			} else {
				this.resourceStore.Save(file, stream);
			}
		}
		
		internal void AddResourceFile(OpenedFile file)
		{
			this.Files.Add(file);
		}
		
		void LoadDesigner()
		{
			LoggingService.Info("Form Designer: BEGIN INITIALIZE");
			
			DefaultServiceContainer serviceContainer = new DefaultServiceContainer();
			serviceContainer.AddService(typeof(System.Windows.Forms.Design.IUIService), new UIService());
			serviceContainer.AddService(typeof(System.Drawing.Design.IToolboxService), ToolboxProvider.ToolboxService);
			
			serviceContainer.AddService(typeof(IHelpService), new HelpService());
			serviceContainer.AddService(typeof(System.Drawing.Design.IPropertyValueUIService), new PropertyValueUIService());
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IResourceService), new DesignerResourceService(this.resourceStore));
			AmbientProperties ambientProperties = new AmbientProperties();
			serviceContainer.AddService(typeof(AmbientProperties), ambientProperties);
			serviceContainer.AddService(typeof(ITypeResolutionService), new TypeResolutionService(this.PrimaryFileName));
			serviceContainer.AddService(typeof(DesignerOptionService), new SharpDevelopDesignerOptionService());
			serviceContainer.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService());
			serviceContainer.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService());
			
			if (generator.CodeDomProvider != null) {
				serviceContainer.AddService(typeof(System.CodeDom.Compiler.CodeDomProvider), generator.CodeDomProvider);
			}
			
			designSurface = CreateDesignSurface(serviceContainer);
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormsDesigner.Services.MenuCommandService(p, designSurface));
			ICSharpCode.FormsDesigner.Services.EventBindingService eventBindingService = new ICSharpCode.FormsDesigner.Services.EventBindingService(this, designSurface);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IEventBindingService), eventBindingService);
			
			DesignerLoader designerLoader = loaderProvider.CreateLoader(generator);
			designSurface.BeginLoad(designerLoader);
			
			undoEngine = new FormsDesignerUndoEngine(Host);
			serviceContainer.AddService(typeof(UndoEngine), undoEngine);
			
			IComponentChangeService componentChangeService = (IComponentChangeService)designSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged += MakeDirty;
			componentChangeService.ComponentAdded   += ComponentListChanged;
			componentChangeService.ComponentRemoved += ComponentListChanged;
			componentChangeService.ComponentRename  += ComponentListChanged;
			this.Host.TransactionClosed += TransactionClose;
			
			ISelectionService selectionService = (ISelectionService)designSurface.GetService(typeof(ISelectionService));
			selectionService.SelectionChanged  += SelectionChangedHandler;
			
			if (IsTabOrderMode) { // fixes SD2-1015
				tabOrderMode = false; // let ShowTabOrder call the designer command again
				ShowTabOrder();
			}
			
			UpdatePropertyPad();
			PropertyPad.PropertyValueChanged += PropertyValueChanged;
			
			hasUnmergedChanges = false;
			
			LoggingService.Info("Form Designer: END INITIALIZE");
		}
		
		bool hasUnmergedChanges;
		
		void MakeDirty(object sender, ComponentChangedEventArgs args)
		{
			hasUnmergedChanges = true;
			this.DesignerCodeFile.MakeDirty();
			this.resourceStore.MarkResourceFilesAsDirty();
		}
		
		bool shouldUpdateSelectableObjects = false;
		
		void TransactionClose(object sender, DesignerTransactionCloseEventArgs e)
		{
			if (shouldUpdateSelectableObjects) {
				// update the property pad after the transaction is *really* finished
				// (including updating the selection)
				p.BeginInvoke(new MethodInvoker(UpdatePropertyPad));
				shouldUpdateSelectableObjects = false;
			}
		}
		
		void ComponentListChanged(object sender, EventArgs e)
		{
			shouldUpdateSelectableObjects = true;
		}
		
		void UnloadDesigner()
		{
			LoggingService.Debug("FormsDesigner unloading, setting ActiveDesignSurface to null");
			designSurfaceManager.ActiveDesignSurface = null;
			PropertyPad.PropertyValueChanged -= PropertyValueChanged;
			bool savedIsDirty = (this.DesignerCodeFile == null) ? false : this.DesignerCodeFile.IsDirty;
			p.Controls.Clear();
			if (this.DesignerCodeFile != null) {
				this.DesignerCodeFile.IsDirty = savedIsDirty;
			}
			
			// We cannot dispose the design surface now because of SD2-451:
			// When the switch to the source view was triggered by a double-click on an event
			// in the PropertyPad, "InvalidOperationException: The container cannot be disposed
			// at design time" is thrown.
			// This is solved by calling dispose after the double-click event has been processed.
			if (designSurface != null) {
				if (disposing) {
					designSurface.Dispose();
				} else {
					p.BeginInvoke(new MethodInvoker(designSurface.Dispose));
				}
				designSurface = null;
			}
		}
		
		readonly PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		public void ShowHelp()
		{
			if (Host == null) {
				return;
			}
			
			ISelectionService selectionService = (ISelectionService)Host.GetService(typeof(ISelectionService));
			if (selectionService != null) {
				Control ctl = selectionService.PrimarySelection as Control;
				if (ctl != null) {
					ICSharpCode.SharpDevelop.HelpProvider.ShowHelp(ctl.GetType().FullName);
				}
			}
		}
		
		void LoadAndDisplayDesigner()
		{
			try {
				
				this.loadingDesigner = true;
				
				LoadDesigner();
				
				bool savedIsDirty = this.DesignerCodeFile.IsDirty;
				if (designSurface != null && p.Controls.Count == 0) {
					Control designer = designSurface.View as Control;
					designer.Dock = DockStyle.Fill;
					p.Controls.Add(designer);
					LoggingService.Debug("FormsDesigner loaded, setting ActiveDesignSurface to " + this.designSurface.ToString());
					designSurfaceManager.ActiveDesignSurface = this.designSurface;
				}
				this.DesignerCodeFile.IsDirty = savedIsDirty;
				
			} catch (Exception e) {
				
				string mainErrorMessage;
				if (e.InnerException is FormsDesignerLoadException) {
					mainErrorMessage = e.InnerException.Message;
				} else if (e is FormsDesignerLoadException) {
					mainErrorMessage = e.Message;
				} else if (designSurface != null && !designSurface.IsLoaded && designSurface.LoadErrors != null) {
					mainErrorMessage = StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.ErrorLoadingDesigner}\r\n\r\n");
					foreach(Exception le in designSurface.LoadErrors) {
						mainErrorMessage += le.ToString();
						mainErrorMessage += "\r\n";
					}
				} else {
					mainErrorMessage = e.ToString();
				}
				
				throw new FormsDesignerLoadException(StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.LoadErrorCheckSourceCodeForErrors}") + Environment.NewLine + mainErrorMessage + Environment.NewLine, e);
				
			} finally {
				this.loadingDesigner = false;
			}
		}
		
		public virtual void MergeFormChanges()
		{
			if (this.HasLoadError) {
				return;
			}
			bool isDirty = this.DesignerCodeFile.IsDirty;
			LoggingService.Info("Merging form changes...");
			designSurface.Flush();
			this.resourceStore.CommitAllResourceChanges();
			LoggingService.Info("Finished merging form changes");
			hasUnmergedChanges = false;
			this.DesignerCodeFile.IsDirty = isDirty;
		}
		
		public void ShowSourceCode()
		{
			this.WorkbenchWindow.ActiveViewContent = this.PrimaryViewContent;
		}
		
		public void ShowSourceCode(int lineNumber)
		{
			ShowSourceCode();
			ITextEditorControlProvider tecp = this.primaryViewContent as ITextEditorControlProvider;
			if (tecp != null) {
				tecp.TextEditorControl.ActiveTextAreaControl.JumpTo(lineNumber - 1);
			}
		}
		
		public void ShowSourceCode(IComponent component, EventDescriptor edesc, string eventMethodName)
		{
			int position;
			string file;
			bool eventCreated = generator.InsertComponentEvent(component, edesc, eventMethodName, "", out file, out position);
			if (eventCreated) {
				if (FileUtility.IsEqualFileName(file, this.primaryViewContent.PrimaryFileName)) {
					ShowSourceCode(position);
				} else {
					FileService.JumpToFilePosition(file, position - 1, 0);
				}
			}
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			return generator.GetCompatibleMethods(edesc);
		}
		
		void IsActiveViewContentChangedHandler(object sender, EventArgs e)
		{
			if (this.IsActiveViewContent) {
				LoggingService.Debug("FormsDesigner view content activated, setting ActiveDesignSurface to " + ((this.DesignSurface == null) ? "null" : this.DesignSurface.ToString()));
				designSurfaceManager.ActiveDesignSurface = this.DesignSurface;
			} else {
				LoggingService.Debug("FormsDesigner view content deactivated, setting ActiveDesignSurface to null");
				designSurfaceManager.ActiveDesignSurface = null;
			}
		}
		
		public override void Dispose()
		{
			disposing = true;
			try {
				// base.Dispose() is called first because it may trigger a call
				// to SaveInternal which requires the designer to be loaded.
				base.Dispose();
			} finally {
				
				this.UnloadDesigner();
				
				// null check is required to support running in unit test mode
				if (WorkbenchSingleton.Workbench != null) {
					this.IsActiveViewContentChanged -= this.IsActiveViewContentChangedHandler;
				}
				
				if (this.generator != null) {
					this.generator.Detach();
				}
				
				this.resourceStore.Dispose();
				
				p.Dispose();
				p = null;
				
			}
		}
		
		void SelectionChangedHandler(object sender, EventArgs args)
		{
			UpdatePropertyPadSelection((ISelectionService)sender);
		}
		
		void UpdatePropertyPadSelection(ISelectionService selectionService)
		{
			ICollection selection = selectionService.GetSelectedComponents();
			object[] selArray = new object[selection.Count];
			selection.CopyTo(selArray, 0);
			propertyContainer.SelectedObjects = selArray;
		}
		
		protected void UpdatePropertyPad()
		{
			if (Host != null) {
				propertyContainer.Host = Host;
				propertyContainer.SelectableObjects = Host.Container.Components;
				ISelectionService selectionService = (ISelectionService)Host.GetService(typeof(ISelectionService));
				if (selectionService != null) {
					UpdatePropertyPadSelection(selectionService);
				}
			}
		}
		
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
		bool IsMenuCommandEnabled(CommandID commandID)
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
			
			//int status = menuCommand.OleStatus;
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
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.SelectAll);
		}
		#endregion
		
		#region Tab Order Handling
		bool tabOrderMode = false;
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
		
		/// <summary>
		/// Reloads the form designer if the language property has changed.
		/// </summary>
		void PropertyValueChanged(object source, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem == null || e.OldValue == null)
				return;
			if (!propertyContainer.IsActivePropertyContainer)
				return;
			if (e.ChangedItem.GridItemType == GridItemType.Property) {
				if (e.ChangedItem.PropertyDescriptor.Name == "Language") {
					if (!e.OldValue.Equals(e.ChangedItem.Value)) {
						LoggingService.Debug("Reloading designer due to language change.");
						this.MergeAndUnloadDesigner();
						this.ReloadDesignerFromMemory();
					}
				}
			}
		}
		
		protected void MergeAndUnloadDesigner()
		{
			propertyContainer.Clear();
			if (!this.HasLoadError) {
				MergeFormChanges();
			}
			UnloadDesigner();
		}
		
		protected void ReloadDesignerFromMemory()
		{
			using(MemoryStream ms = new MemoryStream(this.DesignerCodeFileEncoding.GetBytes(this.DesignerCodeFileContent), false)) {
				this.Load(this.DesignerCodeFile, ms);
			}
			
			UpdatePropertyPad();
		}
		
		public virtual Control ToolsControl {
			get { return ToolboxProvider.FormsDesignerSideBar; }
		}
		
		#region Design surface manager (static)
		
		static readonly DesignSurfaceManager designSurfaceManager = new DesignSurfaceManager();
		
		public static DesignSurface CreateDesignSurface(IServiceProvider serviceProvider)
		{
			return designSurfaceManager.CreateDesignSurface(serviceProvider);
		}
		
		#endregion
	}
}
