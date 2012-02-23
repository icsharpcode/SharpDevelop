// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Workbench implementation using WPF and AvalonDock.
	/// </summary>
	sealed partial class WpfWorkbench : FullScreenEnabledWindow, IWorkbench, System.Windows.Forms.IWin32Window
	{
		const string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";
		const string viewContentPath = "/SharpDevelop/Workbench/Pads";
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		public event EventHandler ActiveViewContentChanged;
		public event EventHandler ActiveContentChanged;
		public event ViewContentEventHandler ViewOpened;
		
		internal void OnViewOpened(ViewContentEventArgs e)
		{
			if (ViewOpened != null) {
				ViewOpened(this, e);
			}
		}
		
		public event ViewContentEventHandler ViewClosed;
		
		internal void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}
		
		public System.Windows.Forms.IWin32Window MainWin32Window { get { return this; } }
		public ISynchronizeInvoke SynchronizingObject { get; private set; }
		public Window MainWindow { get { return this; } }
		public IStatusBarService StatusBar { get; private set; }
		
		IntPtr System.Windows.Forms.IWin32Window.Handle {
			get {
				var wnd = System.Windows.PresentationSource.FromVisual(this) as System.Windows.Interop.IWin32Window;
				if (wnd != null)
					return wnd.Handle;
				else
					return IntPtr.Zero;
			}
		}
		
		List<PadDescriptor> padDescriptorCollection = new List<PadDescriptor>();
		SdStatusBar statusBar = new SdStatusBar();
		ToolBar[] toolBars;
		
		public WpfWorkbench()
		{
			this.SynchronizingObject = new WpfSynchronizeInvoke(this.Dispatcher);
			this.StatusBar = new SdStatusBarService(statusBar);
			InitializeComponent();
			InitFocusTrackingEvents();
		}
		
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource.FromHwnd(this.MainWin32Window.Handle).AddHook(SingleInstanceHelper.WndProc);
			// validate after PresentationSource is initialized
			Rect bounds = new Rect(Left, Top, Width, Height);
			bounds = FormLocationHelper.Validate(bounds.TransformToDevice(this).ToSystemDrawing()).ToWpf().TransformFromDevice(this);
			SetBounds(bounds);
			// Set WindowState after PresentationSource is initialized, because now bounds and location are properly set.
			this.WindowState = lastNonMinimizedWindowState;
		}

		void SetBounds(Rect bounds)
		{
			this.Left = bounds.Left;
			this.Top = bounds.Top;
			this.Width = bounds.Width;
			this.Height = bounds.Height;
		}
		
		public void Initialize()
		{
			UpdateFlowDirection();
			
			foreach (PadDescriptor content in AddInTree.BuildItems<PadDescriptor>(viewContentPath, this, false)) {
				if (content != null) {
					ShowPad(content);
				}
			}
			
			mainMenu.ItemsSource = MenuService.CreateMenuItems(this, this, mainMenuPath, activationMethod: "MainMenu", immediatelyExpandMenuBuildersForShortcuts: true);
			
			toolBars = ToolBarService.CreateToolBars(this, this, "/SharpDevelop/Workbench/ToolBar");
			foreach (ToolBar tb in toolBars) {
				DockPanel.SetDock(tb, Dock.Top);
				dockPanel.Children.Insert(1, tb);
			}
			DockPanel.SetDock(statusBar, Dock.Bottom);
			dockPanel.Children.Insert(dockPanel.Children.Count - 2, statusBar);
			
			UpdateMenu();
			
			AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigate));
			Project.ProjectService.CurrentProjectChanged += SetProjectTitle;
			
			FileService.FileRemoved += CheckRemovedOrReplacedFile;
			FileService.FileReplaced += CheckRemovedOrReplacedFile;
			FileService.FileRenamed += CheckRenamedFile;
			
			FileService.FileRemoved += FileService.RecentOpen.FileRemoved;
			FileService.FileRenamed += FileService.RecentOpen.FileRenamed;
			
			requerySuggestedEventHandler = new EventHandler(CommandManager_RequerySuggested);
			CommandManager.RequerySuggested += requerySuggestedEventHandler;
			ResourceService.LanguageChanged += OnLanguageChanged;
			
			this.StatusBar.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}
		
		// keep a reference to the event handler to prevent it from being garbage collected
		// (CommandManager.RequerySuggested only keeps weak references to the event handlers)
		EventHandler requerySuggestedEventHandler;

		void CommandManager_RequerySuggested(object sender, EventArgs e)
		{
			UpdateMenu();
		}
		
		void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			e.Handled = true;
			if (e.Uri.Scheme == "mailto") {
				try {
					Process.Start(e.Uri.ToString());
				} catch {
					// catch exceptions - e.g. incorrectly installed mail client
				}
			} else {
				FileService.OpenFile(e.Uri.ToString());
			}
		}
		
		void SetProjectTitle(object sender, Project.ProjectEventArgs e)
		{
			if (e.Project != null) {
				Title = e.Project.Name + " - " + ResourceService.GetString("MainWindow.DialogName");
			} else {
				Title = ResourceService.GetString("MainWindow.DialogName");
			}
		}
		
		void CheckRemovedOrReplacedFile(object sender, FileEventArgs e)
		{
			foreach (OpenedFile file in FileService.OpenedFiles) {
				if (FileUtility.IsBaseDirectory(e.FileName, file.FileName)) {
					foreach (IViewContent content in file.RegisteredViewContents.ToArray()) {
						// content.WorkbenchWindow can be null if multiple view contents
						// were in the same WorkbenchWindow and both should be closed
						// (e.g. Windows Forms Designer, Subversion History View)
						if (content.WorkbenchWindow != null) {
							content.WorkbenchWindow.CloseWindow(true);
						}
					}
				}
			}
			Editor.PermanentAnchorService.FileDeleted(e);
		}
		
		void CheckRenamedFile(object sender, FileRenameEventArgs e)
		{
			if (e.IsDirectory) {
				foreach (OpenedFile file in FileService.OpenedFiles) {
					if (file.FileName != null && FileUtility.IsBaseDirectory(e.SourceFile, file.FileName)) {
						file.FileName = new FileName(FileUtility.RenameBaseDirectory(file.FileName, e.SourceFile, e.TargetFile));
					}
				}
			} else {
				OpenedFile file = FileService.GetOpenedFile(e.SourceFile);
				if (file != null) {
					file.FileName = new FileName(e.TargetFile);
				}
			}
			Editor.PermanentAnchorService.FileRenamed(e);
		}
		
		void UpdateMenu()
		{
			MenuService.UpdateStatus(mainMenu.ItemsSource);
			foreach (ToolBar tb in toolBars) {
				ToolBarService.UpdateStatus(tb.ItemsSource);
			}
		}
		
		void OnLanguageChanged(object sender, EventArgs e)
		{
			MenuService.UpdateText(mainMenu.ItemsSource);
			UpdateFlowDirection();
		}
		
		void UpdateFlowDirection()
		{
			Language language = LanguageService.GetLanguage(ResourceService.Language);
			Core.WinForms.RightToLeftConverter.IsRightToLeft = language.IsRightToLeft;
			this.FlowDirection = language.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
			App.Current.Resources[GlobalStyles.FlowDirectionKey] = this.FlowDirection;
		}
		
		public ICollection<IViewContent> ViewContentCollection {
			get {
				WorkbenchSingleton.AssertMainThread();
				return WorkbenchWindowCollection.SelectMany(w => w.ViewContents).ToList().AsReadOnly();
			}
		}
		
		public ICollection<IViewContent> PrimaryViewContents {
			get {
				WorkbenchSingleton.AssertMainThread();
				return (from window in WorkbenchWindowCollection
				        where window.ViewContents.Count > 0
				        select window.ViewContents[0]
				       ).ToList().AsReadOnly();
			}
		}
		
		public IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get {
				WorkbenchSingleton.AssertMainThread();
				if (workbenchLayout != null)
					return workbenchLayout.WorkbenchWindows;
				else
					return new IWorkbenchWindow[0];
			}
		}
		
		public IList<PadDescriptor> PadContentCollection {
			get {
				WorkbenchSingleton.AssertMainThread();
				return padDescriptorCollection.AsReadOnly();
			}
		}
		
		IWorkbenchWindow activeWorkbenchWindow;
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				WorkbenchSingleton.AssertMainThread();
				return activeWorkbenchWindow;
			}
			private set {
				if (activeWorkbenchWindow != value) {
					if (activeWorkbenchWindow != null) {
						activeWorkbenchWindow.ActiveViewContentChanged -= WorkbenchWindowActiveViewContentChanged;
					}
					
					activeWorkbenchWindow = value;
					
					if (value != null) {
						value.ActiveViewContentChanged += WorkbenchWindowActiveViewContentChanged;
					}
					
					if (ActiveWorkbenchWindowChanged != null) {
						ActiveWorkbenchWindowChanged(this, EventArgs.Empty);
					}
					WorkbenchWindowActiveViewContentChanged(null, null);
				}
			}
		}

		void WorkbenchWindowActiveViewContentChanged(object sender, EventArgs e)
		{
			if (workbenchLayout != null) {
				// update ActiveViewContent
				IWorkbenchWindow window = this.ActiveWorkbenchWindow;
				if (window != null)
					this.ActiveViewContent = window.ActiveViewContent;
				else
					this.ActiveViewContent = null;
				
				// update ActiveContent
				this.ActiveContent = workbenchLayout.ActiveContent;
			}
		}
		
		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (closeAll)
				return;
			
			if (workbenchLayout != null) {
				this.ActiveContent = workbenchLayout.ActiveContent;
				this.ActiveWorkbenchWindow = workbenchLayout.ActiveWorkbenchWindow;
			} else {
				this.ActiveContent = null;
				this.ActiveWorkbenchWindow = null;
			}
		}
		
		IViewContent activeViewContent;
		
		public IViewContent ActiveViewContent {
			get {
				WorkbenchSingleton.AssertMainThread();
				return activeViewContent;
			}
			private set {
				if (activeViewContent != value) {
					activeViewContent = value;
					
					if (ActiveViewContentChanged != null) {
						ActiveViewContentChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		object activeContent;
		
		public object ActiveContent {
			get {
				WorkbenchSingleton.AssertMainThread();
				return activeContent;
			}
			private set {
				if (activeContent != value) {
					activeContent = value;
					
					if (ActiveContentChanged != null) {
						ActiveContentChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		IWorkbenchLayout workbenchLayout;
		
		public IWorkbenchLayout WorkbenchLayout {
			get {
				return workbenchLayout;
			}
			set {
				WorkbenchSingleton.AssertMainThread();
				
				if (workbenchLayout != null) {
					workbenchLayout.ActiveContentChanged -= OnActiveWindowChanged;
					workbenchLayout.Detach();
				}
				if (value != null) {
					value.Attach(this);
					value.ActiveContentChanged += OnActiveWindowChanged;
				}
				workbenchLayout = value;
				OnActiveWindowChanged(null, null);
			}
		}
		
		public bool IsActiveWindow {
			get {
				return IsActive;
			}
		}
		
		public void ShowView(IViewContent content)
		{
			ShowView(content, true);
		}
		
		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			WorkbenchSingleton.AssertMainThread();
			if (content == null)
				throw new ArgumentNullException("content");
			if (ViewContentCollection.Contains(content))
				throw new ArgumentException("ViewContent was already shown");
			System.Diagnostics.Debug.Assert(WorkbenchLayout != null);
			
			LoadViewContentMemento(content);
			
			WorkbenchLayout.ShowView(content, switchToOpenedView);
		}
		
		public void ShowPad(PadDescriptor content)
		{
			WorkbenchSingleton.AssertMainThread();
			if (content == null)
				throw new ArgumentNullException("content");
			if (padDescriptorCollection.Contains(content))
				throw new ArgumentException("Pad is already loaded");
			
			padDescriptorCollection.Add(content);
			
			if (WorkbenchLayout != null) {
				WorkbenchLayout.ShowPad(content);
			}
		}
		
		public void UnloadPad(PadDescriptor content)
		{
		}
		
		public PadDescriptor GetPad(Type type)
		{
			WorkbenchSingleton.AssertMainThread();
			if (type == null)
				throw new ArgumentNullException("type");
			foreach (PadDescriptor pad in PadContentCollection) {
				if (pad.Class == type.FullName) {
					return pad;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Flag used to prevent repeated ActiveWindowChanged events during CloseAllViews().
		/// </summary>
		bool closeAll;
		
		public void CloseAllViews()
		{
			WorkbenchSingleton.AssertMainThread();
			try {
				closeAll = true;
				foreach (IWorkbenchWindow window in this.WorkbenchWindowCollection.ToArray()) {
					window.CloseWindow(false);
				}
			} finally {
				closeAll = false;
				OnActiveWindowChanged(this, EventArgs.Empty);
			}
		}
		
		public bool CloseAllSolutionViews()
		{
			bool result = true;
			
			WorkbenchSingleton.AssertMainThread();
			try {
				closeAll = true;
				foreach (IWorkbenchWindow window in this.WorkbenchWindowCollection.ToArray()) {
					if (window.ActiveViewContent != null && window.ActiveViewContent.CloseWithSolution)
						result &= window.CloseWindow(false);
				}
			} finally {
				closeAll = false;
				OnActiveWindowChanged(this, EventArgs.Empty);
			}
			
			return result;
		}
		
		#region ViewContent Memento Handling
		string viewContentMementosFileName;
		
		string ViewContentMementosFileName {
			get {
				if (viewContentMementosFileName == null) {
					viewContentMementosFileName = Path.Combine(PropertyService.ConfigDirectory, "LastViewStates.xml");
				}
				return viewContentMementosFileName;
			}
		}
		
		Properties LoadOrCreateViewContentMementos()
		{
			try {
				return Properties.Load(this.ViewContentMementosFileName) ?? new Properties();
			} catch (Exception ex) {
				LoggingService.Warn("Error while loading the view content memento file. Discarding any saved view states.", ex);
				return new Properties();
			}
		}
		
		static string GetMementoKeyName(IViewContent viewContent)
		{
			return String.Concat(viewContent.GetType().FullName.GetHashCode().ToString("x", CultureInfo.InvariantCulture), ":", FileUtility.NormalizePath(viewContent.PrimaryFileName).ToUpperInvariant());
		}
		
		public static bool LoadDocumentProperties {
			get { return PropertyService.Get("SharpDevelop.LoadDocumentProperties", true); }
			set { PropertyService.Set("SharpDevelop.LoadDocumentProperties", value); }
		}
		
		/// <summary>
		/// Stores the memento for the view content.
		/// Such mementos are automatically loaded in ShowView().
		/// </summary>
		public void StoreMemento(IViewContent viewContent)
		{
			IMementoCapable mementoCapable = viewContent as IMementoCapable;
			if (mementoCapable != null && LoadDocumentProperties) {
				if (viewContent.PrimaryFileName == null)
					return;
				
				string key = GetMementoKeyName(viewContent);
				LoggingService.Debug("Saving memento of '" + viewContent.ToString() + "' to key '" + key + "'");
				
				Properties memento = mementoCapable.CreateMemento();
				Properties p = this.LoadOrCreateViewContentMementos();
				p.Set(key, memento);
				FileUtility.ObservedSave(new NamedFileOperationDelegate(p.Save), this.ViewContentMementosFileName, FileErrorPolicy.Inform);
			}
		}
		
		void LoadViewContentMemento(IViewContent viewContent)
		{
			IMementoCapable mementoCapable = viewContent as IMementoCapable;
			if (mementoCapable != null && LoadDocumentProperties) {
				if (viewContent.PrimaryFileName == null)
					return;
				
				try {
					string key = GetMementoKeyName(viewContent);
					LoggingService.Debug("Trying to restore memento of '" + viewContent.ToString() + "' from key '" + key + "'");
					
					Properties memento = this.LoadOrCreateViewContentMementos().Get<Properties>(key, null);
					if (memento != null) {
						mementoCapable.SetMemento(memento);
					}
				} catch (Exception e) {
					MessageService.ShowException(e, "Can't get/set memento");
				}
			}
		}
		#endregion
		
		System.Windows.WindowState lastNonMinimizedWindowState = System.Windows.WindowState.Normal;
		Rect restoreBoundsBeforeClosing;
		
		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
			if (this.WindowState != System.Windows.WindowState.Minimized)
				lastNonMinimizedWindowState = this.WindowState;
		}
		
		public Properties CreateMemento()
		{
			Properties prop = new Properties();
			prop.Set("WindowState", lastNonMinimizedWindowState);
			var bounds = this.RestoreBounds;
			if (bounds.IsEmpty) bounds = restoreBoundsBeforeClosing;
			if (!bounds.IsEmpty) {
				prop.Set("Bounds", bounds);
			}
			return prop;
		}
		
		public void SetMemento(Properties memento)
		{
			Rect bounds = memento.Get("Bounds", new Rect(10, 10, 750, 550));
			// bounds are validated after PresentationSource is initialized (see OnSourceInitialized)
			lastNonMinimizedWindowState = memento.Get("WindowState", System.Windows.WindowState.Maximized);
			SetBounds(bounds);
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!e.Cancel) {
				if (Project.ProjectService.IsBuilding) {
					MessageService.ShowMessage(StringParser.Parse("${res:MainWindow.CannotCloseWithBuildInProgressMessage}"));
					e.Cancel = true;
					return;
				}
				
				if (!Project.ProjectService.IsClosingCanceled()) {
					// save preferences
					Project.ProjectService.SaveSolutionPreferences();
					
					while (WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count > 0) {
						IWorkbenchWindow window = WorkbenchSingleton.Workbench.WorkbenchWindowCollection[0];
						if (!window.CloseWindow(false)) {
							e.Cancel = true;
							return;
						}
					}
					
					Project.ProjectService.CloseSolution();
					ParserService.StopParserThread();
					
					restoreBoundsBeforeClosing = this.RestoreBounds;
					
					this.WorkbenchLayout = null;
					
					foreach (PadDescriptor padDescriptor in this.PadContentCollection) {
						padDescriptor.Dispose();
					}
				} else {
					e.Cancel = true;
				}
			}
		}
		
		protected override void OnDragEnter(DragEventArgs e)
		{
			try {
				base.OnDragEnter(e);
				if (!e.Handled) {
					e.Effects = GetEffect(e.Data);
					e.Handled = true;
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		protected override void OnDragOver(DragEventArgs e)
		{
			try {
				base.OnDragOver(e);
				if (!e.Handled) {
					e.Effects = GetEffect(e.Data);
					e.Handled = true;
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		DragDropEffects GetEffect(IDataObject data)
		{
			try {
				if (data != null && data.GetDataPresent(DataFormats.FileDrop)) {
					string[] files = (string[])data.GetData(DataFormats.FileDrop);
					if (files != null) {
						foreach (string file in files) {
							if (File.Exists(file)) {
								return DragDropEffects.Link;
							}
						}
					}
				}
			} catch (COMException) {
				// Ignore errors getting the data (e.g. happens when dragging attachments out of Thunderbird)
			}
			return DragDropEffects.None;
		}
		
		protected override void OnDrop(DragEventArgs e)
		{
			try {
				base.OnDrop(e);
				if (!e.Handled && e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
					e.Handled = true;
					string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
					if (files == null)
						return;
					foreach (string file in files) {
						if (File.Exists(file)) {
							Project.IProjectLoader loader = Project.ProjectService.GetProjectLoader(file);
							if (loader != null) {
								FileUtility.ObservedLoad(new NamedFileOperationDelegate(loader.Load), file);
							} else {
								FileService.OpenFile(file);
							}
						}
					}
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		void InitFocusTrackingEvents()
		{
			#if DEBUG
			this.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(WpfWorkbench_PreviewLostKeyboardFocus);
			this.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(WpfWorkbench_PreviewGotKeyboardFocus);
			#endif
		}
		
		[Conditional("DEBUG")]
		internal static void FocusDebug(string format, params object[] args)
		{
			#if DEBUG
			if (enableFocusDebugOutput)
				LoggingService.DebugFormatted(format, args);
			#endif
		}
		
		#if DEBUG
		static bool enableFocusDebugOutput;
		
		void WpfWorkbench_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			FocusDebug("GotKeyboardFocus: oldFocus={0}, newFocus={1}", e.OldFocus, e.NewFocus);
		}
		
		void WpfWorkbench_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			FocusDebug("LostKeyboardFocus: oldFocus={0}, newFocus={1}", e.OldFocus, e.NewFocus);
		}
		
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (!e.Handled && e.Key == Key.D && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) {
				enableFocusDebugOutput = !enableFocusDebugOutput;
				
				StringWriter output = new StringWriter();
				output.WriteLine("Keyboard.FocusedElement = " + GetElementName(Keyboard.FocusedElement));
				output.WriteLine("ActiveContent = " + GetElementName(this.ActiveContent));
				output.WriteLine("ActiveViewContent = " + GetElementName(this.ActiveViewContent));
				output.WriteLine("ActiveWorkbenchWindow = " + GetElementName(this.ActiveWorkbenchWindow));
				((AvalonDockLayout)workbenchLayout).WriteState(output);
				LoggingService.Debug(output.ToString());
				e.Handled = true;
			}
			if (!e.Handled && e.Key == Key.F && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) {
				if (TextOptions.GetTextFormattingMode(this) == TextFormattingMode.Display)
					TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);
				else
					TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
				this.StatusBar.SetMessage("TextFormattingMode=" + TextOptions.GetTextFormattingMode(this));
			}
			if (!e.Handled && e.Key == Key.R && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) {
				switch (TextOptions.GetTextRenderingMode(this)) {
					case TextRenderingMode.Auto:
					case TextRenderingMode.ClearType:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.Grayscale);
						break;
					case TextRenderingMode.Grayscale:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.Aliased);
						break;
					default:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
						break;
				}
				this.StatusBar.SetMessage("TextRenderingMode=" + TextOptions.GetTextRenderingMode(this));
			}
			if (!e.Handled && e.Key == Key.G && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) {
				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
				this.StatusBar.SetMessage("Total memory = " + (GC.GetTotalMemory(true) / 1024 / 1024f).ToString("f1") + " MB");
			}
		}
		#endif
		
		internal static string GetElementName(object element)
		{
			if (element == null)
				return "<null>";
			else
				return element.GetType().FullName + ": " + element.ToString();
		}
	}
}
