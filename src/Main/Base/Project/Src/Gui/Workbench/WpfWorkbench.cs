// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Workbench implementation using WPF and AvalonDock.
	/// </summary>
	sealed partial class WpfWorkbench : Window, IWorkbench
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
		
		public System.Windows.Forms.IWin32Window MainWin32Window { get; private set; }
		public ISynchronizeInvoke SynchronizingObject { get; private set; }
		public Window MainWindow { get { return this; } }
		
		List<PadDescriptor> padViewContentCollection = new List<PadDescriptor>();
		
		ToolBar[] toolBars;
		
		public WpfWorkbench()
		{
			this.SynchronizingObject = new WpfSynchronizeInvoke(this.Dispatcher);
			InitializeComponent();
		}
		
		protected override void OnSourceInitialized(EventArgs e)
		{
			this.MainWin32Window = this.GetWin32Window();
			base.OnSourceInitialized(e);
		}
		
		public void Initialize()
		{
			foreach (PadDescriptor content in AddInTree.BuildItems<PadDescriptor>(viewContentPath, this, false)) {
				if (content != null) {
					ShowPad(content);
				}
			}
			
			mainMenu.ItemsSource = MenuService.CreateMenuItems(this, this, mainMenuPath);
			
			toolBars = ToolBarService.CreateToolBars(this, "/SharpDevelop/Workbench/ToolBar");
			foreach (ToolBar tb in toolBars) {
				DockPanel.SetDock(tb, Dock.Top);
				dockPanel.Children.Insert(1, tb);
			}
			DockPanel.SetDock(StatusBarService.Control, Dock.Bottom);
			dockPanel.Children.Insert(dockPanel.Children.Count - 2, StatusBarService.Control);
			
			UpdateMenu();
			
			AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigate));
			
			requerySuggestedEventHandler = new EventHandler(CommandManager_RequerySuggested);
			CommandManager.RequerySuggested += requerySuggestedEventHandler;
			
			StatusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}
		
		void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
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
		
		// keep a reference to the event handler to prevent it from being gargabe collected
		// (CommandManager.RequerySuggested only keeps weak references to the event handlers)
		EventHandler requerySuggestedEventHandler;

		void CommandManager_RequerySuggested(object sender, EventArgs e)
		{
			UpdateMenu();
		}
		
		void UpdateMenu()
		{
			MenuService.UpdateStatus(mainMenu.ItemsSource);
			foreach (ToolBar tb in toolBars) {
				ToolBarService.UpdateStatus(tb.ItemsSource);
			}
		}
		
		public ICollection<IViewContent> ViewContentCollection {
			get {
				return WorkbenchWindowCollection.SelectMany(w => w.ViewContents).ToList().AsReadOnly();
			}
		}
		
		public ICollection<IViewContent> PrimaryViewContents {
			get {
				return (from window in WorkbenchWindowCollection
				        where window.ViewContents.Count > 0
				        select window.ViewContents[0]
				       ).ToList().AsReadOnly();
			}
		}
		
		public IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get {
				if (workbenchLayout != null)
					return workbenchLayout.WorkbenchWindows;
				else
					return new IWorkbenchWindow[0];
			}
		}
		
		public IList<PadDescriptor> PadContentCollection {
			get {
				return padViewContentCollection.AsReadOnly();
			}
		}
		
		IWorkbenchWindow activeWorkbenchWindow;
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
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
			get { return activeViewContent; }
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
			get { return activeContent; }
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
			if (content == null)
				throw new ArgumentNullException("content");
			System.Diagnostics.Debug.Assert(WorkbenchLayout != null);
			
			LoadViewContentMemento(content);
			
			WorkbenchLayout.ShowView(content, switchToOpenedView);
			if (switchToOpenedView) {
				content.WorkbenchWindow.SelectWindow();
			}
		}
		
		public void ShowPad(PadDescriptor content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			
			padViewContentCollection.Add(content);
			
			if (WorkbenchLayout != null) {
				WorkbenchLayout.ShowPad(content);
			}
		}
		
		public void UnloadPad(PadDescriptor content)
		{
		}
		
		public PadDescriptor GetPad(Type type)
		{
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
		
		/// <summary>
		/// Stores the memento for the view content.
		/// Such mementos are automatically loaded in ShowView().
		/// </summary>
		public void StoreMemento(IViewContent viewContent)
		{
			IMementoCapable mementoCapable = viewContent as IMementoCapable;
			if (mementoCapable != null && PropertyService.Get("SharpDevelop.LoadDocumentProperties", true)) {
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
			if (mementoCapable != null && PropertyService.Get("SharpDevelop.LoadDocumentProperties", true)) {
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
					MessageService.ShowError(e, "Can't get/set memento");
				}
			}
		}
		#endregion
		
		public void RedrawAllComponents()
		{
		}
		
		public void UpdateRenderer()
		{
		}
		
		public Properties CreateMemento()
		{
			Properties prop = new Properties();
			prop.Set("WindowState", this.WindowState);
			if (this.WindowState == System.Windows.WindowState.Normal) {
				prop.Set("Left", this.Left);
				prop.Set("Top", this.Top);
				prop.Set("Width", this.Width);
				prop.Set("Height", this.Height);
			}
			return prop;
		}
		
		public void SetMemento(Properties memento)
		{
			this.Left = memento.Get("Left", 10.0);
			this.Top = memento.Get("Top", 10.0);
			this.Width = memento.Get("Width", 600.0);
			this.Height = memento.Get("Height", 400.0);
			this.WindowState = memento.Get("WindowState", System.Windows.WindowState.Maximized);
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!e.Cancel) {
				this.WorkbenchLayout = null;
			}
		}
	}
}
