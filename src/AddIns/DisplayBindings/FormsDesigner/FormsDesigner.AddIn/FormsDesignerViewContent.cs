// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.AddIn.Pipeline;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.FormsDesigner.Gui.OptionPanels;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.FormsDesigner.UndoRedo;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.FormsDesigner
{
	public class FormsDesignerViewContent : AbstractViewContentHandlingLoadErrors, IClipboardHandler, IUndoHandler, IHasPropertyContainer, IContextHelpProvider, IToolsHost, IFileDocumentProvider, IFormsDesigner
	{
		#region Fields
		readonly Control pleaseWaitLabel = new Label() { Text = StringParser.Parse("${res:Global.PleaseWait}"), TextAlign=ContentAlignment.MiddleCenter };
		readonly IViewContent primaryViewContent;
		readonly IDesignerLoaderProviderWithViewContent loaderProvider;
		readonly IDesignerGenerator generator;
		readonly IDesignerSourceProvider sourceProvider;
		readonly ResourceStore resourceStore;
		readonly DesignerSourceCodeStorage sourceCodeStorage;
		readonly PropertyContainer propertyContainer = new PropertyContainer();
		
		bool disposing;
		IFormsDesignerUndoEngine undoEngine;
		FormsDesignerManager appDomainManager;
		ToolboxProvider toolbox;
		bool inMasterLoadOperation;
		bool hasUnmergedChanges;
		bool reloadPending;
		CustomWindowsFormsHost designView;
		SharpDevelopDesignerOptions options;
		FormKeyHandler keyHandler;
		#endregion
		
		#region Properties
		public FormsDesignerManager AppDomainManager {
			get { return appDomainManager; }
		}
		
		public OpenedFile DesignerCodeFile {
			get { return this.sourceCodeStorage.DesignerCodeFile; }
		}
		
		public IDocument PrimaryFileDocument {
			get { return this.sourceCodeStorage[this.PrimaryFile]; }
		}
		
		public ITextBuffer PrimaryFileContent {
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
		
		public virtual object ToolsContent {
			get {
				LoadAppDomainManager();
				return toolbox.FormsDesignerSideBar;
			}
		}
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		public IDesignerGenerator Generator {
			get {
				return generator;
			}
		}
		
		public Control DesignerContent {
			get {
				if (designView == null)
					return null;
				return designView.Child;
			}
		}
		
		public SharpDevelopDesignerOptions DesignerOptions {
			get {
				return options;
			}
		}
		#endregion
		
		#region Constructors
		FormsDesignerViewContent(IViewContent primaryViewContent)
			: base()
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			
			this.primaryViewContent = primaryViewContent;
			
			this.UserContent = this.pleaseWaitLabel;
			
			this.sourceCodeStorage = new DesignerSourceCodeStorage();
			this.resourceStore = new ResourceStore(this);
			
			this.IsActiveViewContentChanged += this.IsActiveViewContentChangedHandler;
			
			FileService.FileRemoving += this.FileServiceFileRemoving;
			ICSharpCode.SharpDevelop.Debugging.DebuggerService.DebugStarting += this.DebugStarting;
		}
		
		public FormsDesignerViewContent(IViewContent primaryViewContent, IDesignerLoaderProviderWithViewContent loaderProvider, IDesignerGenerator generator, IDesignerSourceProvider sourceProvider)
			: this(primaryViewContent)
		{
			if (loaderProvider == null)
				throw new ArgumentNullException("loaderProvider");
			if (generator == null)
				throw new ArgumentNullException("generator");
			
			this.loaderProvider = loaderProvider;
			this.loaderProvider.ViewContent = this;
			this.generator = generator;
			this.sourceProvider = sourceProvider;
			this.sourceProvider.Attach(this);
			
			this.Files.Add(this.primaryViewContent.PrimaryFile);
			
			LoadAppDomainManager();
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
			
			LoadAppDomainManager();
		}
		#endregion
		
		#region Proxies
		class ViewContentIFormsDesignerProxy : MarshalByRefObject, IFormsDesigner
		{
			FormsDesignerViewContent vc;
			
			public ViewContentIFormsDesignerProxy(FormsDesignerViewContent vc)
			{
				this.vc = vc;
			}
			
			public IDesignerGenerator Generator {
				get {
					return vc.Generator;
				}
			}
			
			public SharpDevelopDesignerOptions DesignerOptions {
				get {
					return vc.DesignerOptions;
				}
			}
			
			public IntPtr GetDialogOwnerWindowHandle()
			{
				return vc.GetDialogOwnerWindowHandle();
			}
			
			public void ShowSourceCode()
			{
				vc.ShowSourceCode();
			}
			
			public void ShowSourceCode(int lineNumber)
			{
				vc.ShowSourceCode(lineNumber);
			}
			
			public void ShowSourceCode(IComponent component, EventDescriptorProxy edesc, string methodName)
			{
				vc.ShowSourceCode(component, edesc, methodName);
			}
			
			public void MakeDirty()
			{
				vc.MakeDirty();
			}
			
			public void InvalidateRequerySuggested()
			{
				((IFormsDesigner)vc).InvalidateRequerySuggested();
			}
			
			public bool IsTabOrderMode {
				get {
					return vc.IsTabOrderMode;
				}
			}
			
			public bool EnableDelete {
				get {
					return vc.EnableDelete;
				}
			}
			
			public void HideTabOrder()
			{
				vc.HideTabOrder();
			}
			
			public Control DesignerContent {
				get {
					return vc.DesignerContent;
				}
			}
		}
		#endregion
		
		public ICSharpCode.SharpDevelop.Editor.IDocument GetDocumentForFile(OpenedFile file)
		{
			return this.sourceCodeStorage[file];
		}
		
		void LoadAppDomainManager()
		{
			if (appDomainManager != null)
				return;
			
			options = LoadOptions();
			
			var creationProperties = new FormsDesignerCreationProperties {
				FileName = PrimaryFileName,
				TypeLocator = new DomTypeLocator(PrimaryFileName),
				GacWrapper = new DomGacWrapper(),
				Commands = new SharpDevelopCommandProvider(this),
				FormsDesignerProxy = new ViewContentIFormsDesignerProxy(this),
				Logger = new FormsDesignerLoggingServiceImpl(),
				Options = options,
				ResourceStore = resourceStore
			};
			
			appDomainManager = DesignerAppDomainHost.Instance.CreateFormsDesignerInAppDomain(creationProperties);
			toolbox = new ToolboxProvider(appDomainManager);
			appDomainManager.UseSDAssembly(typeof(CustomWindowsFormsHost).Assembly.FullName, typeof(CustomWindowsFormsHost).Assembly.Location);
			
			keyHandler = FormKeyHandler.Insert(this);
			
			Application.Idle += ApplicationIdle;
		}
		
		void UnloadAppDomainManager()
		{
			if (appDomainManager == null)
				return;
			Application.Idle -= ApplicationIdle;
			keyHandler.Remove();
			appDomainManager.Dispose();
			appDomainManager = null;
		}
		
		protected override void LoadInternal(OpenedFile file, System.IO.Stream stream)
		{
			LoggingService.Debug("Forms designer: Load " + file.FileName + "; inMasterLoadOperation=" + this.inMasterLoadOperation);
			
			if (inMasterLoadOperation) {
				
				if (this.sourceCodeStorage.ContainsFile(file)) {
					LoggingService.Debug("Forms designer: Loading " + file.FileName + " in source code storage");
					this.sourceCodeStorage.LoadFile(file, stream);
				} else {
					LoggingService.Debug("Forms designer: Loading " + file.FileName + " in resource store");
					this.resourceStore.Load(file, stream);
				}
				
			} else if (file == this.PrimaryFile || this.sourceCodeStorage.ContainsFile(file)) {
				
				if (appDomainManager != null && appDomainManager.IsLoaderLoading) {
					throw new InvalidOperationException("Designer loading a source code file while DesignerLoader is loading and the view is not in a master load operation. This must not happen.");
				}
				
				if (appDomainManager != null) {
					UnloadDesigner();
					UnloadAppDomainManager();
					LoadAppDomainManager();
				}
				
				this.inMasterLoadOperation = true;
				
				try {
					
					this.sourceCodeStorage.LoadFile(file, stream);
					
					LoggingService.Debug("Forms designer: Determining designer source files for " + file.FileName);
					OpenedFile newDesignerCodeFile;
					IEnumerable<OpenedFile> sourceFiles = this.sourceProvider.GetSourceFiles(out newDesignerCodeFile);
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
				if (appDomainManager.IsLoaderLoading) {
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
		
		CustomWindowsFormsHost WrapInCustomHost(Control control, bool enableFontInheritance = true)
		{
			var host = new SDWindowsFormsHost(DesignerAppDomainHost.Instance.DesignerAppDomain, true);
			host.DisposeChild = false;
			host.ServiceObject = this;
			host.EnableFontInheritance = enableFontInheritance;
			host.Child = control;
			return host;
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
			if (!this.Files.Contains(file))
				this.Files.Add(file);
		}
		
		void LoadDesigner()
		{
			LoggingService.Info("Form Designer: BEGIN INITIALIZE");
			
			appDomainManager.ResetServiceContainer();
			
			appDomainManager.AddService(typeof(ISharpDevelopIDEService), new FormsMessageService());
			appDomainManager.AddService(typeof(System.Windows.Forms.Design.IUIService), new UIService(this, appDomainManager));
			
			appDomainManager.AddService(typeof(IHelpService), new HelpService());
			
			appDomainManager.AddService(typeof(IProjectResourceService), CreateProjectResourceService());
			appDomainManager.AddService(typeof(IImageResourceEditorDialogWrapper), new ImageResourceEditorDialogWrapper(ParserService.GetParseInformation(this.DesignerCodeFile.FileName).CompilationUnit.ProjectContent.Project as IProject));
			
			appDomainManager.DesignSurfaceLoading += new EventHandlerProxy(DesignerLoading);
			appDomainManager.DesignSurfaceLoaded += new LoadedEventHandlerProxy(DesignerLoaded);
			appDomainManager.DesignSurfaceFlushed += new EventHandlerProxy(DesignerFlushed);
			appDomainManager.DesignSurfaceUnloading += new EventHandlerProxy(DesignerUnloading);
			
			appDomainManager.BeginDesignSurfaceLoad(generator, loaderProvider);

			if (!appDomainManager.IsDesignSurfaceLoaded) {
				throw new FormsDesignerLoadException(appDomainManager.LoadErrors);
			}
			
			appDomainManager.InitializeRemainingServices();
			
			undoEngine = (IFormsDesignerUndoEngine)appDomainManager.GetService(typeof(IFormsDesignerUndoEngine));
			
			if (IsTabOrderMode) { // fixes SD2-1015
				tabOrderMode = false; // let ShowTabOrder call the designer command again
				ShowTabOrder();
			}
			
			propertyContainer.PropertyGridReplacementContent = WrapInCustomHost(appDomainManager.CreatePropertyPad());
			appDomainManager.UpdatePropertyPad();
			
			hasUnmergedChanges = false;
			
			LoggingService.Info("Form Designer: END INITIALIZE");
		}
		
		void ApplicationIdle(object sender, EventArgs e)
		{
			appDomainManager.RaiseApplicationIdle();
		}
		
		ProjectResourceService CreateProjectResourceService()
		{
			IProjectContent projectContent = GetProjectContentForFile();
			return new ProjectResourceService(appDomainManager, projectContent);
		}
		
		IProjectContent GetProjectContentForFile()
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(this.DesignerCodeFile.FileName);
			if (parseInfo != null) {
				return parseInfo.CompilationUnit.ProjectContent;
			}
			return DefaultProjectContent.DummyProjectContent;
		}
		
		SharpDevelopDesignerOptions LoadOptions()
		{
			int w = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeWidth",  8);
			int h = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeHeight", 8);
			
			SharpDevelopDesignerOptions options = new SharpDevelopDesignerOptions();
			
			options.GridSize = new Size(w, h);
			
			options.ShowGrid   = PropertyService.Get("FormsDesigner.DesignerOptions.ShowGrid", true);
			options.SnapToGrid = PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGrid", true);
			
			options.UseSmartTags = GeneralOptionsPanel.UseSmartTags;
			options.UseSnapLines = PropertyService.Get("FormsDesigner.DesignerOptions.UseSnapLines", true);

			options.EnableInSituEditing          = PropertyService.Get("FormsDesigner.DesignerOptions.EnableInSituEditing", true);
			options.ObjectBoundSmartTagAutoShow  = GeneralOptionsPanel.SmartTagAutoShow;
			options.UseOptimizedCodeGeneration   = PropertyService.Get("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", true);
			options.EventHandlerNameFormat       = "{0}{1}";
			options.PropertyGridSortAlphabetical = PropertyService.Get("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false);
			
			return options;
		}
		
		void MakeDirty()
		{
			hasUnmergedChanges = true;
			this.DesignerCodeFile.MakeDirty();
			this.resourceStore.MarkResourceFilesAsDirty();
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}
		
		void IFormsDesigner.InvalidateRequerySuggested()
		{
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}
		
		void IFormsDesigner.MakeDirty()
		{
			MakeDirty();
		}
		
		void UnloadDesigner()
		{
			LoggingService.Debug("FormsDesigner unloading, setting ActiveDesignSurface to null");
			FormsDesignerManager.DeactivateDesignSurface();
			
			bool savedIsDirty = (this.DesignerCodeFile == null) ? false : this.DesignerCodeFile.IsDirty;
			this.UserContent = this.pleaseWaitLabel;
			if (this.DesignerCodeFile != null) {
				this.DesignerCodeFile.IsDirty = savedIsDirty;
			}
			
			if (appDomainManager != null && appDomainManager.DesignSurfaceName != null) {
				appDomainManager.DesignSurfaceLoading -= new EventHandlerProxy(DesignerLoading);
				appDomainManager.DesignSurfaceLoaded -= new LoadedEventHandlerProxy(DesignerLoaded);
				appDomainManager.DesignSurfaceFlushed -= new EventHandlerProxy(DesignerFlushed);
				appDomainManager.DesignSurfaceUnloading -= new EventHandlerProxy(DesignerUnloading);
				
				appDomainManager.DesignSurfaceUnloaded += new EventHandlerProxy(
					delegate {
						ServiceContainer serviceContainer = appDomainManager.GetService(typeof(ServiceContainer)) as ServiceContainer;
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
					});
				try {
					appDomainManager.DisposeDesignSurface();
				} catch (ExceptionCollection exceptions) {
					foreach (Exception ex in exceptions.Exceptions) {
						LoggingService.Error(ex);
					}
				}
				
				appDomainManager.UnregisterTypeProviders();
			}
		}
		
		public void ShowHelp()
		{
			if (appDomainManager == null) {
				return;
			}
			
			ISelectionService selectionService = (ISelectionService)appDomainManager.GetService(typeof(ISelectionService));
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
				LoadAppDomainManager();
				LoadDesigner();
			} catch (Exception e) {
				if (e.InnerException is FormsDesignerLoadException) {
					throw new FormsDesignerLoadException(e.InnerException.Message, e);
				} else if (e is FormsDesignerLoadException) {
					throw;
				} else if (appDomainManager != null && appDomainManager.DesignSurfaceName != null && !appDomainManager.IsDesignSurfaceLoaded && appDomainManager.LoadErrors != null) {
					throw new FormsDesignerLoadException(appDomainManager.LoadErrors, e);
				} else {
					throw;
				}
			}
		}
		
		void DesignerLoading(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: DesignerLoader loading...");
			this.reloadPending = false;
			this.UserContent = this.pleaseWaitLabel;
		}
		
		void DesignerUnloading(object sender, EventArgs e)
		{
			LoggingService.Debug("Forms designer: DesignerLoader unloading...");
			if (!this.disposing) {
				this.UserContent = this.pleaseWaitLabel;
			}
		}
		
		void DesignerLoaded(object sender, LoadedEventArgsProxy e)
		{
			// This method is called when the designer has loaded.
			LoggingService.Debug("Forms designer: DesignerLoader loaded, HasSucceeded=" + e.HasSucceeded.ToString());
			this.reloadPending = false;
			
			if (e.HasSucceeded) {
				// Display the designer on the view content
				bool savedIsDirty = this.DesignerCodeFile.IsDirty;
				
				// enableFontInheritance: Make sure auto-scaling is based on the correct font.
				// This is required on Vista, I don't know why it works correctly in XP
				designView = WrapInCustomHost(appDomainManager.DesignSurfaceView, enableFontInheritance: false);
				propertyContainer.PropertyGridReplacementContent = WrapInCustomHost(appDomainManager.CreatePropertyPad());
				
				this.UserContent = designView;
				LoggingService.Debug("FormsDesigner loaded, setting ActiveDesignSurface to " + appDomainManager.DesignSurfaceName);
				appDomainManager.ActivateDesignSurface();
				this.DesignerCodeFile.IsDirty = savedIsDirty;
				appDomainManager.UpdatePropertyPad();
			} else {
				// This method can not only be called during initialization,
				// but also when the designer reloads itself because of
				// a language change.
				// When a load error occurs there, we are not somewhere
				// below the Load method which handles load errors.
				// That is why we create an error text box here anyway.
				ShowError(new Exception(appDomainManager.LoadErrors));
			}
		}
		
		void DesignerFlushed(object sender, EventArgs e)
		{
			this.resourceStore.CommitAllResourceChanges();
			this.hasUnmergedChanges = false;
		}
		
		public virtual void MergeFormChanges()
		{
			if (this.HasLoadError || appDomainManager.DesignSurfaceName == null) {
				LoggingService.Debug("Forms designer: Cannot merge form changes because the designer is not loaded successfully or not loaded at all");
				return;
			} else if (this.DesignerCodeFile == null) {
				throw new InvalidOperationException("Cannot merge form changes without a designer code file.");
			}
			bool isDirty = this.DesignerCodeFile.IsDirty;
			LoggingService.Info("Merging form changes...");
			appDomainManager.FlushDesignSurface();
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
			ITextEditorProvider tecp = this.primaryViewContent as ITextEditorProvider;
			if (tecp != null) {
				tecp.TextEditor.JumpTo(lineNumber, 1);
			}
		}
		
		public void ShowSourceCode(IComponent component, EventDescriptor edesc, string eventMethodName)
		{
			ShowSourceCode(component, new EventDescriptorProxy(edesc), eventMethodName);
		}
		
		public void ShowSourceCode(IComponent component, EventDescriptorProxy edesc, string eventMethodName)
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
		
		void IsActiveViewContentChangedHandler(object sender, EventArgs e)
		{
			if (this.IsActiveViewContent) {
				LoggingService.Debug("FormsDesigner view content activated, setting ActiveDesignSurface to " + appDomainManager.DesignSurfaceName);
				appDomainManager.ActivateDesignSurface();
				
				if (appDomainManager.DesignSurfaceName != null) {
					// Reload designer when a referenced assembly has changed
					// (the default Load/Save logic using OpenedFile cannot catch this case)
					if (appDomainManager.ReferencedAssemblyChanged) {
						IDesignerLoaderService loaderService = appDomainManager.GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;
						if (loaderService != null) {
							if (!appDomainManager.Host.Loading) {
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
				FormsDesignerManager.DeactivateDesignSurface();
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
				
				ICSharpCode.SharpDevelop.Debugging.DebuggerService.DebugStarting -= this.DebugStarting;
				FileService.FileRemoving -= this.FileServiceFileRemoving;
				
				this.UnloadDesigner();

				Application.Idle -= ApplicationIdle;
				
				// null check is required to support running in unit test mode
				if (WorkbenchSingleton.Workbench != null) {
					this.IsActiveViewContentChanged -= this.IsActiveViewContentChangedHandler;
				}
				
				if (this.generator != null) {
					this.sourceProvider.Detach();
				}
				
				this.resourceStore.Dispose();
				
				this.UserContent = null;
				this.pleaseWaitLabel.Dispose();
				
				
			}
		}
		
		void SelectionChangedHandler(object sender, EventArgs args)
		{
			appDomainManager.UpdatePropertyPadSelection((ISelectionService)sender);
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
		bool IsMenuCommandEnabled(CommandIDEnum commandID)
		{
			if (appDomainManager == null || appDomainManager.DesignSurfaceName == null) {
				return false;
			}
			
			IMenuCommandServiceProxy menuCommandService = (IMenuCommandServiceProxy)appDomainManager.MenuCommandService;
			if (menuCommandService == null) {
				return false;
			}
			
			return menuCommandService.IsCommandEnabled(commandID);
		}
		
		public bool EnableCut {
			get {
				return IsMenuCommandEnabled(CommandIDEnum.Cut);
			}
		}
		
		public bool EnableCopy {
			get {
				return IsMenuCommandEnabled(CommandIDEnum.Copy);
			}
		}
		
		const string ComponentClipboardFormat = "CF_DESIGNERCOMPONENTS";
		public bool EnablePaste {
			get {
				return IsMenuCommandEnabled(CommandIDEnum.Paste);
			}
		}
		
		public bool EnableDelete {
			get {
				return IsMenuCommandEnabled(CommandIDEnum.Delete);
			}
		}
		
		public bool EnableSelectAll {
			get {
				return appDomainManager.DesignSurfaceName != null;
			}
		}
		
		public void Cut()
		{
			IMenuCommandServiceProxy menuCommandService = (IMenuCommandServiceProxy)appDomainManager.MenuCommandService;
			menuCommandService.GlobalInvoke(CommandIDEnum.Cut);
		}
		
		public void Copy()
		{
			IMenuCommandServiceProxy menuCommandService = (IMenuCommandServiceProxy)appDomainManager.MenuCommandService;
			menuCommandService.GlobalInvoke(CommandIDEnum.Copy);
		}
		
		public void Paste()
		{
			IMenuCommandServiceProxy menuCommandService = (IMenuCommandServiceProxy)appDomainManager.MenuCommandService;
			menuCommandService.GlobalInvoke(CommandIDEnum.Paste);
		}
		
		public void Delete()
		{
			IMenuCommandServiceProxy menuCommandService = (IMenuCommandServiceProxy)appDomainManager.MenuCommandService;
			menuCommandService.GlobalInvoke(CommandIDEnum.Delete);
		}
		
		public void SelectAll()
		{
			IMenuCommandServiceProxy menuCommandService = (IMenuCommandServiceProxy)appDomainManager.MenuCommandService;
			menuCommandService.GlobalInvoke(CommandIDEnum.SelectAll);
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
				IMenuCommandServiceProxy menuCommandService = (IMenuCommandServiceProxy)appDomainManager.MenuCommandService;
				menuCommandService.GlobalInvoke(CommandIDEnum.TabOrder);
				tabOrderMode = true;
			}
		}
		
		public virtual void HideTabOrder()
		{
			if (IsTabOrderMode) {
				IMenuCommandServiceProxy menuCommandService = (IMenuCommandServiceProxy)appDomainManager.MenuCommandService;
				menuCommandService.GlobalInvoke(CommandIDEnum.TabOrder);
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
			
			appDomainManager.UpdatePropertyPad();
		}
		
		void FileServiceFileRemoving(object sender, FileCancelEventArgs e)
		{
			if (!e.Cancel) {
				if (WorkbenchSingleton.InvokeRequired) {
					WorkbenchSingleton.SafeThreadAsyncCall(this.CheckForDesignerCodeFileDeletion, e);
				} else {
					this.CheckForDesignerCodeFileDeletion(e);
				}
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
		
		#region Debugger event handling (to prevent designer reload while debugger is starting)
		
		void DebugStarting(object sender, EventArgs e)
		{
			if (appDomainManager == null || appDomainManager.IsActiveDesignSurface ||
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
		
		public IntPtr GetDialogOwnerWindowHandle()
		{
			return WorkbenchSingleton.MainWin32Window.Handle;
		}
	}
}
