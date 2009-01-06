// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
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
	public class FormsDesignerViewContent : AbstractViewContentHandlingLoadErrors, IClipboardHandler, IUndoHandler, IHasPropertyContainer, IContextHelpProvider, IToolsHost, IFileDocumentProvider
	{
		readonly Control pleaseWaitLabel = new Label() {Text=StringParser.Parse("${res:Global.PleaseWait}"), TextAlign=ContentAlignment.MiddleCenter};
		DesignSurface designSurface;
		bool disposing;
		
		readonly IViewContent primaryViewContent;
		readonly IDesignerLoaderProvider loaderProvider;
		DesignerLoader loader;
		readonly IDesignerGenerator generator;
		readonly ResourceStore resourceStore;
		FormsDesignerUndoEngine undoEngine;
		TypeResolutionService typeResolutionService;
		
		Encoding primaryFileEncoding;
		readonly IDocument primaryFileDocument = new DocumentFactory().CreateDocument();
		Encoding designerCodeFileEncoding;
		OpenedFile designerCodeFile;
		IDocument designerCodeFileDocument;
		
		readonly Dictionary<Type, TypeDescriptionProvider> addedTypeDescriptionProviders = new Dictionary<Type, TypeDescriptionProvider>();
		
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
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			if (file == this.DesignerCodeFile) {
				return this.DesignerCodeFileDocument;
			} else if (file == this.PrimaryFile) {
				return this.PrimaryFileDocument;
			} else {
				return null;
			}
		}
		
		public IViewContent PrimaryViewContent {
			get { return this.primaryViewContent; }
		}
		
		protected override string LoadErrorHeaderText {
			get { return StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.LoadErrorCheckSourceCodeForErrors}") + Environment.NewLine + Environment.NewLine; }
		}
		
		FormsDesignerViewContent(IViewContent primaryViewContent)
			: base()
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			
			if (!FormKeyHandler.inserted) {
				FormKeyHandler.Insert();
			}
			
			this.primaryViewContent = primaryViewContent;
			
			this.Control.BackColor = Color.White;
			this.Control.RightToLeft = RightToLeft.No;
			// Make sure auto-scaling is based on the correct font.
			// This is required on Vista, I don't know why it works correctly in XP
			this.Control.Font = Control.DefaultFont;
			
			this.UserControl = this.pleaseWaitLabel;
			
			this.resourceStore = new ResourceStore(this);
			
			// null check is required to support running in unit test mode
			if (WorkbenchSingleton.Workbench != null) {
				this.IsActiveViewContentChanged += this.IsActiveViewContentChangedHandler;
			}
			
			FileService.FileRemoving += this.FileServiceFileRemoving;
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
				} else if (this.HasLoadError || this.designSurface == null || !this.designSurface.IsLoaded) {
					LoggingService.Debug("Forms designer: Having a load error. Reloading designer.");
					this.ReloadDesignerFromMemory();
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
				if (this.loader != null && !this.loader.Loading) {
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
			this.typeResolutionService = new TypeResolutionService(this.PrimaryFileName);
			serviceContainer.AddService(typeof(ITypeResolutionService), this.typeResolutionService);
			serviceContainer.AddService(typeof(DesignerOptionService), new SharpDevelopDesignerOptionService());
			serviceContainer.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService());
			serviceContainer.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService());
			serviceContainer.AddService(typeof(ProjectResourceService), new ProjectResourceService(ParserService.GetParseInformation(this.DesignerCodeFile.FileName).MostRecentCompilationUnit.ProjectContent));
			
			// Provide the ImageResourceEditor for all Image and Icon properties
			this.addedTypeDescriptionProviders.Add(typeof(Image), TypeDescriptor.AddAttributes(typeof(Image), new EditorAttribute(typeof(ImageResourceEditor), typeof(System.Drawing.Design.UITypeEditor))));
			this.addedTypeDescriptionProviders.Add(typeof(Icon), TypeDescriptor.AddAttributes(typeof(Icon), new EditorAttribute(typeof(ImageResourceEditor), typeof(System.Drawing.Design.UITypeEditor))));
			
			if (generator.CodeDomProvider != null) {
				serviceContainer.AddService(typeof(System.CodeDom.Compiler.CodeDomProvider), generator.CodeDomProvider);
			}
			
			designSurface = CreateDesignSurface(serviceContainer);
			designSurface.Loading += this.DesignerLoading;
			designSurface.Loaded += this.DesignerLoaded;
			designSurface.Flushed += this.DesignerFlushed;
			designSurface.Unloading += this.DesingerUnloading;
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormsDesigner.Services.MenuCommandService(this.Control, designSurface));
			ICSharpCode.FormsDesigner.Services.EventBindingService eventBindingService = new ICSharpCode.FormsDesigner.Services.EventBindingService(this, designSurface);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IEventBindingService), eventBindingService);
			
			this.loader = loaderProvider.CreateLoader(generator);
			designSurface.BeginLoad(this.loader);
			
			if (!designSurface.IsLoaded) {
				throw new FormsDesignerLoadException(FormatLoadErrors(designSurface));
			}
			
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
				this.Control.BeginInvoke(new MethodInvoker(UpdatePropertyPad));
				shouldUpdateSelectableObjects = false;
			}
		}
		
		void ComponentListChanged(object sender, EventArgs e)
		{
			if (this.loader == null || !this.loader.Loading) {
				shouldUpdateSelectableObjects = true;
			}
		}
		
		void UnloadDesigner()
		{
			LoggingService.Debug("FormsDesigner unloading, setting ActiveDesignSurface to null");
			designSurfaceManager.ActiveDesignSurface = null;
			
			bool savedIsDirty = (this.DesignerCodeFile == null) ? false : this.DesignerCodeFile.IsDirty;
			this.UserControl = this.pleaseWaitLabel;
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
					componentChangeService.ComponentChanged -= MakeDirty;
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
					this.Control.BeginInvoke(new MethodInvoker(designSurface.Dispose));
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
				
				LoadDesigner();
				
			} catch (Exception e) {
				
				if (e.InnerException is FormsDesignerLoadException) {
					throw new FormsDesignerLoadException(e.InnerException.Message, e);
				} else if (e is FormsDesignerLoadException) {
					throw;
				} else if (designSurface != null && !designSurface.IsLoaded && designSurface.LoadErrors != null) {
					throw new FormsDesignerLoadException(FormatLoadErrors(designSurface), e);
				} else {
					throw;
				}
				
			}
		}
		
		void DesignerLoading(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: DesignerLoader loading...");
			this.UserControl = this.pleaseWaitLabel;
			Application.DoEvents();
		}
		
		void DesingerUnloading(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: DesignerLoader unloading...");
			if (!this.disposing) {
				this.UserControl = this.pleaseWaitLabel;
				Application.DoEvents();
			}
		}
		
		void DesignerLoaded(object sender, LoadedEventArgs e)
		{
			// This method is called when the designer has loaded.
			LoggingService.Debug("Forms designer: DesignerLoader loaded, HasSucceeded=" + e.HasSucceeded.ToString());
			
			if (e.HasSucceeded) {
				// Display the designer on the view content
				bool savedIsDirty = this.DesignerCodeFile.IsDirty;
				this.UserControl = (Control)this.designSurface.View;
				LoggingService.Debug("FormsDesigner loaded, setting ActiveDesignSurface to " + this.designSurface.ToString());
				designSurfaceManager.ActiveDesignSurface = this.designSurface;
				this.DesignerCodeFile.IsDirty = savedIsDirty;
				this.UpdatePropertyPad();
			} else {
				// This method can not only be called during initialization,
				// but also when the designer reloads itself because of
				// a language change.
				// When a load error occurs there, we are not somewhere
				// below the Load method which handles load errors.
				// That is why we create an error text box here anyway.
				TextBox errorTextBox = new TextBox() { Multiline=true, ScrollBars=ScrollBars.Both, ReadOnly=true, BackColor=SystemColors.Window, Dock=DockStyle.Fill };
				errorTextBox.Text = String.Concat(this.LoadErrorHeaderText, FormatLoadErrors(designSurface));
				this.UserControl = errorTextBox;
			}
		}
		
		void DesignerFlushed(object sender, EventArgs e)
		{
			this.resourceStore.CommitAllResourceChanges();
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
				
				if (this.DesignSurface != null && this.Host != null) {
					// Reload designer when a referenced assembly has changed
					// (the default Load/Save logic using OpenedFile cannot catch this case)
					if (this.typeResolutionService.ReferencedAssemblyChanged) {
						IDesignerLoaderService loaderService = this.DesignSurface.GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;
						if (loaderService != null) {
							if (!this.Host.Loading) {
								LoggingService.Info("Forms designer reloading due to change in referenced assembly");
								if (!loaderService.Reload()) {
									MessageService.ShowMessage("The designer has detected that a referenced assembly has been changed, but the designer loader did not accept the reload command. Please reload the designer manually by closing and reopening this file.");
								}
							} else {
								LoggingService.Debug("Forms designer detected change in referenced assembly, but is in load operation");
							}
						} else {
							MessageService.ShowMessage("The designer has detected that a referenced assembly has been changed, but it cannot reload itself because IDesignerLoaderService is unavailable. Please reload the designer manually by closing and reopening this file.");
						}
					}
				}
				
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
				
				FileService.FileRemoving -= this.FileServiceFileRemoving;
				
				this.UnloadDesigner();
				
				// null check is required to support running in unit test mode
				if (WorkbenchSingleton.Workbench != null) {
					this.IsActiveViewContentChanged -= this.IsActiveViewContentChangedHandler;
				}
				
				if (this.generator != null) {
					this.generator.Detach();
				}
				
				this.resourceStore.Dispose();
				
				this.UserControl = null;
				this.pleaseWaitLabel.Dispose();
				
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
		
		void FileServiceFileRemoving(object sender, FileCancelEventArgs e)
		{
			if (!e.Cancel && this.DesignerCodeFile != null) {
				if (WorkbenchSingleton.InvokeRequired) {
					WorkbenchSingleton.SafeThreadAsyncCall(this.CheckForDesignerCodeFileDeletion, e);
				} else {
					this.CheckForDesignerCodeFileDeletion(e);
				}
			}
		}
		
		void CheckForDesignerCodeFileDeletion(FileCancelEventArgs e)
		{
			if (this.DesignerCodeFile == null || String.IsNullOrEmpty(this.DesignerCodeFile.FileName)) {
				return;
			}
			
			if ((!e.IsDirectory && FileUtility.IsEqualFileName(this.DesignerCodeFile.FileName, e.FileName)) ||
			    (e.IsDirectory && FileUtility.IsBaseDirectory(e.FileName, this.DesignerCodeFile.FileName))) {
				
				LoggingService.Info("Forms designer: Handling deletion of open designer code file '" + this.DesignerCodeFile.FileName + "'");
				
				// When our designer code file is deleted,
				// unload the designer and remove the file from the
				// file list so that the primary view is not closed
				// because of this event.
				
				this.UnloadDesigner();
				this.Files.Remove(this.DesignerCodeFile);
				this.designerCodeFile = null;
				this.designerCodeFileDocument = null;
				this.designerCodeFileEncoding = null;
				
			}
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
