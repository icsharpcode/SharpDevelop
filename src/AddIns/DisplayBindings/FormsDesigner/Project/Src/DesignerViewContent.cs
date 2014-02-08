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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Designer;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.FormsDesigner.UndoRedo;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.FormsDesigner
{
	public class FormsDesignerViewContent : AbstractViewContentHandlingLoadErrors, IClipboardHandler, IUndoHandler, IHasPropertyContainer, IContextHelpProvider, IToolsHost, IFileDocumentProvider
	{
		readonly Control pleaseWaitLabel = new Label() { Text = StringParser.Parse("${res:Global.PleaseWait}"), TextAlign=ContentAlignment.MiddleCenter };
		DesignSurface designSurface;
		bool disposing;
		
		readonly IViewContent primaryViewContent;
		readonly IDesignerLoaderProvider loaderProvider;
		DesignerLoader loader;
		readonly ResourceStore resourceStore;
		FormsDesignerUndoEngine undoEngine;
		TypeResolutionService typeResolutionService;
		
		readonly DesignerSourceCodeStorage sourceCodeStorage;
		
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
			get { return this.sourceCodeStorage.DesignerCodeFile; }
		}
		
		public IDocument PrimaryFileDocument {
			get { return this.sourceCodeStorage[this.PrimaryFile]; }
		}
		
		public ITextSource PrimaryFileContent {
			get { return this.PrimaryFileDocument.CreateSnapshot(); }
		}
		
		public IDocument DesignerCodeFileDocument {
			get {
				if (this.sourceCodeStorage.DesignerCodeFile == null) {
					return null;
				} else {
					return this.sourceCodeStorage[this.sourceCodeStorage.DesignerCodeFile];
				}
			}
		}
		
		public string DesignerCodeFileContent {
			get { return this.DesignerCodeFileDocument.Text; }
			set { this.DesignerCodeFileDocument.Text = value; }
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			return this.sourceCodeStorage[file];
		}
		
		public IEnumerable<KeyValuePair<OpenedFile, IDocument>> SourceFiles {
			get { return this.sourceCodeStorage; }
		}
		
		protected DesignerSourceCodeStorage SourceCodeStorage {
			get { return this.sourceCodeStorage; }
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
			
			this.UserContent = this.pleaseWaitLabel;
			
			this.sourceCodeStorage = new DesignerSourceCodeStorage();
			this.resourceStore = new ResourceStore(this);
			
			this.IsActiveViewContentChanged += this.IsActiveViewContentChangedHandler;
			
			FileService.FileRemoving += this.FileServiceFileRemoving;
			SD.Debugger.DebugStarting += this.DebugStarting;
		}

		public FormsDesignerViewContent(IViewContent primaryViewContent, IDesignerLoaderProvider loaderProvider)
			: this(primaryViewContent)
		{
			if (loaderProvider == null)
				throw new ArgumentNullException("loaderProvider");
			
			this.loaderProvider = loaderProvider;
			
			this.Files.Add(this.primaryViewContent.PrimaryFile);
		}
		
		/// <summary>
		/// This constructor allows running in unit test mode with a mock file.
		/// </summary>
		public FormsDesignerViewContent(IViewContent primaryViewContent, OpenedFile mockFile)
			: this(primaryViewContent)
		{
			this.sourceCodeStorage.AddFile(mockFile, Encoding.UTF8);
			this.sourceCodeStorage.DesignerCodeFile = mockFile;
			this.Files.Add(primaryViewContent.PrimaryFile);
		}
		
		bool inMasterLoadOperation;
		
		protected override void LoadInternal(OpenedFile file, System.IO.Stream stream)
		{
			LoggingService.Debug("Forms designer: Load " + file.FileName + "; inMasterLoadOperation=" + this.inMasterLoadOperation);
			
			if (this.typeResolutionService != null)
				this.typeResolutionService.ClearCaches();
			
			if (inMasterLoadOperation) {
				
				if (this.sourceCodeStorage.ContainsFile(file)) {
					LoggingService.Debug("Forms designer: Loading " + file.FileName + " in source code storage");
					this.sourceCodeStorage.LoadFile(file, stream);
				} else {
					LoggingService.Debug("Forms designer: Loading " + file.FileName + " in resource store");
					this.resourceStore.Load(file, stream);
				}
				
			} else if (file == this.PrimaryFile || this.sourceCodeStorage.ContainsFile(file)) {
				
				if (this.loader != null && this.loader.Loading) {
					throw new InvalidOperationException("Designer loading a source code file while DesignerLoader is loading and the view is not in a master load operation. This must not happen.");
				}
				
				if (this.designSurface != null) {
					this.UnloadDesigner();
				}
				
				this.inMasterLoadOperation = true;
				
				try {
					
					this.sourceCodeStorage.LoadFile(file, stream);
					
					LoggingService.Debug("Forms designer: Determining designer source files for " + file.FileName);
					OpenedFile newDesignerCodeFile;
					IReadOnlyList<OpenedFile> sourceFiles = loaderProvider.GetSourceFiles(this, out newDesignerCodeFile);
					if (sourceFiles == null || newDesignerCodeFile == null) {
						throw new FormsDesignerLoadException("The designer source files could not be determined.");
					}
					
					// Unload all source files from the view which are no longer in the returned collection
					foreach (OpenedFile f in this.Files.Except(sourceFiles).ToArray()) {
						// Ensure that we only unload source files, but not resource files.
						if (this.sourceCodeStorage.ContainsFile(f)) {
							LoggingService.Debug("Forms designer: Unloading file '" + f.FileName + "' because it no longer belongs to the designed form");
							this.Files.Remove(f);
							this.sourceCodeStorage.RemoveFile(f);
						}
					}
					
					// Load all files which are new in the returned collection
					foreach (OpenedFile f in sourceFiles.Except(this.Files).ToArray()) {
						this.sourceCodeStorage.AddFile(f);
						this.Files.Add(f);
					}
					
					this.sourceCodeStorage.DesignerCodeFile = newDesignerCodeFile;
					
					this.LoadAndDisplayDesigner();
					
				} finally {
					this.inMasterLoadOperation = false;
				}
				
			} else {
				
				// Loading a resource file
				
				bool mustReload;
				if (this.loader != null && !this.loader.Loading) {
					LoggingService.Debug("Forms designer: Reloading designer because of LoadInternal on resource file");
					this.UnloadDesigner();
					mustReload = true;
					this.inMasterLoadOperation = true;
				} else {
					mustReload = false;
				}
				
				try {
					LoggingService.Debug("Forms designer: Loading " + file.FileName + " in resource store");
					this.resourceStore.Load(file, stream);
					if (mustReload) {
						this.LoadAndDisplayDesigner();
					}
				} finally {
					this.inMasterLoadOperation = false;
				}
				
			}
		}
		
		protected override void SaveInternal(OpenedFile file, System.IO.Stream stream)
		{
			LoggingService.Debug("Forms designer: Save " + file.FileName);
			if (hasUnmergedChanges) {
				this.MergeFormChanges();
			}
			if (this.sourceCodeStorage.ContainsFile(file)) {
				this.sourceCodeStorage.SaveFile(file, stream);
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
			serviceContainer.AddService(typeof(ProjectResourceService), CreateProjectResourceService());
			
			// Provide the ImageResourceEditor for all Image and Icon properties
			this.addedTypeDescriptionProviders.Add(typeof(Image), TypeDescriptor.AddAttributes(typeof(Image), new EditorAttribute(typeof(ImageResourceEditor), typeof(System.Drawing.Design.UITypeEditor))));
			this.addedTypeDescriptionProviders.Add(typeof(Icon), TypeDescriptor.AddAttributes(typeof(Icon), new EditorAttribute(typeof(ImageResourceEditor), typeof(System.Drawing.Design.UITypeEditor))));
			
//			if (generator.CodeDomProvider != null) {
//				serviceContainer.AddService(typeof(System.CodeDom.Compiler.CodeDomProvider), generator.CodeDomProvider);
//			}
			
			designSurface = CreateDesignSurface(serviceContainer);
			designSurface.Loading += this.DesignerLoading;
			designSurface.Loaded += this.DesignerLoaded;
			designSurface.Flushed += this.DesignerFlushed;
			designSurface.Unloading += this.DesignerUnloading;
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormsDesigner.Services.MenuCommandService(this, designSurface));
			
			this.loader = loaderProvider.CreateLoader(this);
			designSurface.BeginLoad(this.loader);
			
			if (!designSurface.IsLoaded) {
				throw new FormsDesignerLoadException(FormatLoadErrors(designSurface));
			}
			
			undoEngine = new FormsDesignerUndoEngine(Host);
			serviceContainer.AddService(typeof(UndoEngine), undoEngine);
			
			IComponentChangeService componentChangeService = (IComponentChangeService)designSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged += ComponentChanged;
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
		
		ProjectResourceService CreateProjectResourceService()
		{
			var project = GetProjectForFile();
			return new ProjectResourceService(project);
		}
		
		IProject GetProjectForFile()
		{
			return SD.ProjectService.FindProjectContainingFile(this.DesignerCodeFile.FileName);
		}
		
		bool hasUnmergedChanges;
		
		void MakeDirty()
		{
			hasUnmergedChanges = true;
			this.DesignerCodeFile.MakeDirty();
			this.resourceStore.MarkResourceFilesAsDirty();
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}
		
		bool shouldUpdateSelectableObjects = false;
		
		void TransactionClose(object sender, DesignerTransactionCloseEventArgs e)
		{
			if (shouldUpdateSelectableObjects) {
				// update the property pad after the transaction is *really* finished
				// (including updating the selection)
				SD.MainThread.InvokeAsyncAndForget(UpdatePropertyPad);
				shouldUpdateSelectableObjects = false;
			}
		}
		
		void ComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			bool loading = loader != null && loader.Loading;
			LoggingService.Debug("Forms designer: ComponentChanged: " + (e.Component == null ? "<null>" : e.Component.ToString()) + ", Member=" + (e.Member == null ? "<null>" : e.Member.Name) + ", OldValue=" + (e.OldValue == null ? "<null>" : e.OldValue.ToString()) + ", NewValue=" + (e.NewValue == null ? "<null>" : e.NewValue.ToString()) + "; Loading=" + loading + "; Unloading=" + unloading);
			if (!loading && !unloading) {
				MakeDirty();
			}
		}

		void ComponentListChanged(object sender, EventArgs e)
		{
			bool loading = this.loader != null && this.loader.Loading;
			LoggingService.Debug("Forms designer: Component added/removed/renamed, Loading=" + loading + ", Unloading=" + this.unloading);
			if (!loading && !unloading) {
				shouldUpdateSelectableObjects = true;
				this.MakeDirty();
			}
		}

		void UnloadDesigner()
		{
			LoggingService.Debug("FormsDesigner unloading, setting ActiveDesignSurface to null");
			designSurfaceManager.ActiveDesignSurface = null;
			
			bool savedIsDirty = (this.DesignerCodeFile == null) ? false : this.DesignerCodeFile.IsDirty;
			this.UserContent = this.pleaseWaitLabel;
			if (this.DesignerCodeFile != null) {
				this.DesignerCodeFile.IsDirty = savedIsDirty;
			}
			
			if (designSurface != null) {
				designSurface.Loading -= this.DesignerLoading;
				designSurface.Loaded -= this.DesignerLoaded;
				designSurface.Flushed -= this.DesignerFlushed;
				designSurface.Unloading -= this.DesignerUnloading;
				
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
				
				designSurface.Unloaded += delegate {
					ServiceContainer serviceContainer = designSurface.GetService(typeof(ServiceContainer)) as ServiceContainer;
					if (serviceContainer != null) {
						// Workaround for .NET bug: .NET unregisters the designer host only if no component throws an exception,
						// but then in a finally block assumes that the designer host is already unloaded.
						// Thus we would get the confusing "InvalidOperationException: The container cannot be disposed at design time"
						// when any component throws an exception.
						
						// See http://community.sharpdevelop.net/forums/p/10928/35288.aspx
						// Reproducible with a custom control that has a designer that crashes on unloading
						// e.g. http://www.codeproject.com/KB/toolbars/WinFormsRibbon.aspx
						
						// We work around this problem by unregistering the designer host manually.
						try {
							var services = (Dictionary<Type, object>)typeof(ServiceContainer).InvokeMember(
								"Services",
								BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic,
								null, serviceContainer, null);
							foreach (var pair in services.ToArray()) {
								if (pair.Value is IDesignerHost) {
									serviceContainer.GetType().InvokeMember(
										"RemoveFixedService",
										BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
										null, serviceContainer, new object[] { pair.Key });
								}
							}
						} catch (Exception ex) {
							LoggingService.Error(ex);
						}
					}
				};
				try {
					designSurface.Dispose();
				} catch (ExceptionCollection exceptions) {
					foreach (Exception ex in exceptions.Exceptions) {
						LoggingService.Error(ex);
					}
				} finally {
					designSurface = null;
				}
			}
			
			this.typeResolutionService = null;
			this.loader = null;
			UpdatePropertyPad();
			
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

		internal new Control UserContent {
			get {
				CustomWindowsFormsHost host = base.UserContent as CustomWindowsFormsHost;
				return host != null ? host.Child : null;
			}
			set {
				CustomWindowsFormsHost host = base.UserContent as CustomWindowsFormsHost;
				if (value == null) {
					base.UserContent = null;
					if (host != null)
						host.Dispose();
					return;
				}
				if (host != null && host.Child == value) {
					return;
				}
				if (host == null) {
					host = SD.WinForms.CreateWindowsFormsHost(this, true);
				}
				host.Child = value;
				base.UserContent = host;
			}
		}

		void DesignerLoading(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: DesignerLoader loading...");
			this.reloadPending = false;
			this.unloading = false;
			this.UserContent = this.pleaseWaitLabel;
		}

		void DesignerUnloading(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: DesignerLoader unloading...");
			this.unloading = true;
			if (!this.disposing) {
				this.UserContent = this.pleaseWaitLabel;
			}
		}

		bool reloadPending;
		bool unloading;

		void DesignerLoaded(object sender, LoadedEventArgs e)
		{
			// This method is called when the designer has loaded.
			LoggingService.Debug("Forms designer: DesignerLoader loaded, HasSucceeded=" + e.HasSucceeded.ToString());
			this.reloadPending = false;
			this.unloading = false;
			
			if (e.HasSucceeded) {
				// Display the designer on the view content
				bool savedIsDirty = this.DesignerCodeFile.IsDirty;
				Control designView = (Control)this.designSurface.View;
				
				designView.BackColor = Color.White;
				designView.RightToLeft = RightToLeft.No;
				// Make sure auto-scaling is based on the correct font.
				// This is required on Vista, I don't know why it works correctly in XP
				designView.Font = System.Windows.Forms.Control.DefaultFont;
				
				this.UserContent = designView;
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
				this.UserContent = errorTextBox;
			}
		}

		void DesignerFlushed(object sender, EventArgs e)
		{
			this.resourceStore.CommitAllResourceChanges();
			this.hasUnmergedChanges = false;
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
			if (this.HasLoadError || this.designSurface == null) {
				LoggingService.Debug("Forms designer: Cannot merge form changes because the designer is not loaded successfully or not loaded at all");
				return;
			} else if (this.DesignerCodeFile == null) {
				throw new InvalidOperationException("Cannot merge form changes without a designer code file.");
			}
			bool isDirty = this.DesignerCodeFile.IsDirty;
			LoggingService.Info("Merging form changes...");
			designSurface.Flush();
			this.resourceStore.CommitAllResourceChanges();
			LoggingService.Info("Finished merging form changes");
			hasUnmergedChanges = false;
			this.DesignerCodeFile.IsDirty = isDirty;
		}

		public void ShowSourceCode(int lineNumber = 0)
		{
			this.WorkbenchWindow.ActiveViewContent = this.PrimaryViewContent;
			ITextEditor editor = this.primaryViewContent.GetService<ITextEditor>();
			if (editor != null) {
				editor.JumpTo(lineNumber, 1);
			}
		}

		/*
		public void ShowSourceCode(IComponent component, EventDescriptor edesc, string eventMethodName)
		{
			int position;
			string file;
			bool eventCreated = generator.InsertComponentEvent(component, edesc, eventMethodName, "", out file, out position);
			if (eventCreated) {
				if (FileUtility.IsEqualFileName(file, this.primaryViewContent.PrimaryFileName)) {
					ShowSourceCode(position);
				} else {
					FileService.JumpToFilePosition(file, position, 0);
				}
			}
		}

		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			return generator.GetCompatibleMethods(edesc);
		}
		*/

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
								this.reloadPending = true;
								if (!loaderService.Reload()) {
									this.reloadPending = false;
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
				SD.Debugger.DebugStarting -= this.DebugStarting;
				FileService.FileRemoving -= this.FileServiceFileRemoving;
				
				this.UnloadDesigner();
				
				this.IsActiveViewContentChanged -= this.IsActiveViewContentChangedHandler;
				
				this.resourceStore.Dispose();
				
				this.UserContent = null;
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
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
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
			} else {
				propertyContainer.Host = null;
				propertyContainer.SelectableObjects = null;
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
			using(MemoryStream ms = new MemoryStream(this.sourceCodeStorage.GetFileEncoding(this.DesignerCodeFile).GetBytes(this.DesignerCodeFileContent), false)) {
				this.Load(this.DesignerCodeFile, ms);
			}
			
			UpdatePropertyPad();
		}

		public virtual object ToolsContent {
			get { return ToolboxProvider.FormsDesignerSideBar; }
		}

		void FileServiceFileRemoving(object sender, FileCancelEventArgs e)
		{
			if (!e.Cancel) {
				this.CheckForDesignerCodeFileDeletion(e);
			}
		}

		void CheckForDesignerCodeFileDeletion(FileCancelEventArgs e)
		{
			OpenedFile file;
			
			if (e.IsDirectory) {
				file = this.Files.SingleOrDefault(
					f => FileUtility.IsBaseDirectory(e.FileName, f.FileName)
				);
			} else {
				file = this.Files.SingleOrDefault(
					f => FileUtility.IsEqualFileName(f.FileName, e.FileName)
				);
			}
			
			if (file == null || file == this.PrimaryFile)
				return;
			
			LoggingService.Info("Forms designer: Handling deletion of open designer code file '" + file.FileName + "'");
			
			if (file == this.sourceCodeStorage.DesignerCodeFile) {
				this.UnloadDesigner();
				this.sourceCodeStorage.DesignerCodeFile = null;
			}
			
			// When any of our designer code files is deleted,
			// remove the file from the file list so that
			// the primary view is not closed because of this event.
			this.Files.Remove(file);
			this.sourceCodeStorage.RemoveFile(file);
		}

		#region Design surface manager (static)

		static readonly DesignSurfaceManager designSurfaceManager = new DesignSurfaceManager();

		public static DesignSurface CreateDesignSurface(IServiceProvider serviceProvider)
		{
			return designSurfaceManager.CreateDesignSurface(serviceProvider);
		}

		#endregion

		#region Debugger event handling (to prevent designer reload while debugger is starting)

		void DebugStarting(object sender, EventArgs e)
		{
			if (designSurfaceManager.ActiveDesignSurface != this.DesignSurface ||
			    !this.reloadPending)
				return;
			
			// The designer loader does not reload immediately,
			// but only when the Application.Idle event is raised.
			// When the IsActiveViewContentChangedHandler has been called because of the
			// layout change prior to starting the debugger, and it has
			// initiated a reload because of a changed referenced assembly,
			// the reload can interrupt the starting of the debugger.
			// To prevent this, we explicitly raise the Idle event here.
			LoggingService.Debug("Forms designer: DebugStarting raises the Idle event to force pending reload now");
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try {
				Application.RaiseIdle(EventArgs.Empty);
			} finally {
				Cursor.Current = oldCursor;
			}
		}

		#endregion
	}
}
