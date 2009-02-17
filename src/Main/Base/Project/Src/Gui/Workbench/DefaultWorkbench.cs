// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the Workspace with a multiple document interface.
	/// </summary>
	sealed class DefaultWorkbench : Form, IWorkbench
	{
		readonly static string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";
		readonly static string viewContentPath = "/SharpDevelop/Workbench/Pads";
		
		List<PadDescriptor> padViewContentCollection = new List<PadDescriptor>();
		List<IViewContent> primaryViewContentCollection = new List<IViewContent>();
		
		bool isActiveWindow; // Gets whether SharpDevelop is the active application in Windows
		
		bool closeAll = false;
		
		bool            fullscreen;
		FormWindowState defaultWindowState = FormWindowState.Normal;
		Rectangle       normalBounds       = new Rectangle(0, 0, 640, 480);
		
		IWorkbenchLayout layout = null;
		
		#region FullScreen & View content properties
		public bool FullScreen {
			get {
				return fullscreen;
			}
			set {
				if (fullscreen == value)
					return;
				fullscreen = value;
				if (fullscreen) {
					defaultWindowState = WindowState;
					// - Hide window to prevet any further animations.
					// - It fixes .NET Framework bug where the bounds of
					//   visible window are set incorectly too.
					Visible            = false;
					FormBorderStyle    = FormBorderStyle.None;
					WindowState        = FormWindowState.Maximized;
					Visible            = true;
				} else {
					FormBorderStyle = FormBorderStyle.Sizable;
					Bounds          = normalBounds;
					WindowState     = defaultWindowState;
				}
				RedrawAllComponents();
			}
		}
		
		public string Title {
			get {
				return Text;
			}
			set {
				Text = value;
			}
		}
		
		/// <summary>
		/// Gets whether SharpDevelop is the active application in Windows.
		/// </summary>
		public bool IsActiveWindow {
			get {
				return isActiveWindow;
			}
		}
		
		public IWorkbenchLayout WorkbenchLayout {
			get {
				return layout;
			}
			set {
				if (layout != null) {
					layout.ActiveWorkbenchWindowChanged -= OnActiveWindowChanged;
					layout.Detach();
				}
				value.Attach(this);
				layout = value;
				layout.ActiveWorkbenchWindowChanged += OnActiveWindowChanged;
				OnActiveWindowChanged(null, null);
			}
		}
		
		public IList<PadDescriptor> PadContentCollection {
			get {
				Debug.Assert(padViewContentCollection != null);
				return padViewContentCollection;
			}
		}
		
		public IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get {
				return primaryViewContentCollection.Select(vc => vc.WorkbenchWindow)
					.Distinct().ToArray().AsReadOnly();
			}
		}
		
		public ICollection<IViewContent> PrimaryViewContents {
			get {
//				return Linq.ToArray(Linq.Concat(Linq.Select<IWorkbenchWindow, IEnumerable<IViewContent>>(
//					workbenchWindowCollection, delegate (IWorkbenchWindow w) { return w.ViewContents; }
//				)));
				return primaryViewContentCollection.AsReadOnly();
			}
		}
		
		public ICollection<IViewContent> ViewContentCollection {
			get {
				ICollection<IViewContent> primaryContents = PrimaryViewContents;
				List<IViewContent> contents = new List<IViewContent>(primaryContents);
				contents.AddRange(primaryContents.SelectMany(vc => vc.SecondaryViewContents));
				return contents.AsReadOnly();
			}
		}
		
		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (closeAll) return;
			
			if (layout == null) {
				this.ActiveWorkbenchWindow = null;
				this.ActiveContent = null;
			} else {
				this.ActiveWorkbenchWindow = layout.ActiveWorkbenchWindow;
				this.ActiveContent = layout.ActiveContent;
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
						activeWorkbenchWindow.ActiveViewContentChanged -= OnWorkbenchActiveViewContentChanged;
					}
					activeWorkbenchWindow = value;
					if (activeWorkbenchWindow != null) {
						activeWorkbenchWindow.ActiveViewContentChanged += OnWorkbenchActiveViewContentChanged;
					}
					
					if (ActiveWorkbenchWindowChanged != null) {
						ActiveWorkbenchWindowChanged(this, EventArgs.Empty);
					}
					
					OnWorkbenchActiveViewContentChanged(null, null);
				}
			}
		}
		
		void OnWorkbenchActiveViewContentChanged(object sender, EventArgs e)
		{
			// update ActiveViewContent
			IWorkbenchWindow window = this.ActiveWorkbenchWindow;
			if (window != null)
				this.ActiveViewContent = window.ActiveViewContent;
			else
				this.ActiveViewContent = null;
			
			this.ActiveContent = layout.ActiveContent;
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		
		IViewContent activeViewContent;
		
		/// <summary>
		/// The active view content inside the active workbench window.
		/// </summary>
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
		
		/// <summary>
		/// Is called, when the active view content has changed.
		/// </summary>
		public event EventHandler ActiveViewContentChanged;
		
		object activeContent;
		
		public object ActiveContent {
			get {
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
		
		/// <summary>
		/// Is called, when the active content has changed.
		/// </summary>
		public event EventHandler ActiveContentChanged;
		#endregion
		
		public DefaultWorkbench()
		{
			Text = ResourceService.GetString("MainWindow.DialogName");
			Icon = WinFormsResourceService.GetIcon("Icons.SharpDevelopIcon");
			
			StartPosition = FormStartPosition.Manual;
			AllowDrop     = true;
		}
		
		public Form MainForm {
			get { return this; }
		}
		
		protected override void WndProc(ref Message m)
		{
			if (!SingleInstanceHelper.PreFilterMessage(ref m)) {
				base.WndProc(ref m);
			}
		}
		
		System.Windows.Forms.Timer toolbarUpdateTimer;

		public void Initialize()
		{
			UpdateRenderer();
			
			MenuComplete                 += new EventHandler(SetStandardStatusBar);
			
			SetStandardStatusBar(null, null);
			
			ProjectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);

			FileService.FileRemoved += CheckRemovedOrReplacedFile;
			FileService.FileReplaced += CheckRemovedOrReplacedFile;
			FileService.FileRenamed += CheckRenamedFile;
			
			FileService.FileRemoved += FileService.RecentOpen.FileRemoved;
			FileService.FileRenamed += FileService.RecentOpen.FileRenamed;
			
			try {
				ArrayList contents = AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this);
				foreach (PadDescriptor content in contents) {
					if (content != null) {
						ShowPad(content);
					}
				}
			} catch (TreePathNotFoundException) {}

			CreateMainMenu();
			CreateToolBars();

			toolbarUpdateTimer =  new System.Windows.Forms.Timer();
			toolbarUpdateTimer.Tick += new EventHandler(UpdateMenu);
			
			toolbarUpdateTimer.Interval = 500;
			toolbarUpdateTimer.Start();
			
			RightToLeftConverter.Convert(this);
		}
		
		public void CloseContent(IViewContent content)
		{
			if (PropertyService.Get("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable) {
				StoreMemento(content);
			}
			if (primaryViewContentCollection.Contains(content)) {
				primaryViewContentCollection.Remove(content);
			}
			OnViewClosed(new ViewContentEventArgs(content));
			content.Dispose();
			content = null;
		}
		
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
		
		public void ShowView(IViewContent content)
		{
			this.ShowView(content, true);
		}
		
		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			if (content.WorkbenchWindow != null)
				throw new ArgumentException("Cannot show view content that is already visible in another workbench window");
			
			if (layout == null)
				throw new InvalidOperationException("No layout is attached.");
			
			primaryViewContentCollection.Add(content);
			if (PropertyService.Get("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable) {
				try {
					Properties memento = GetStoredMemento(content);
					if (memento != null) {
						((IMementoCapable)content).SetMemento(memento);
					}
				} catch (Exception e) {
					MessageService.ShowError(e, "Can't get/set memento");
				}
			}
			
			layout.ShowView(content, switchToOpenedView);
			if (switchToOpenedView) {
				content.WorkbenchWindow.SelectWindow();
			}
			OnViewOpened(new ViewContentEventArgs(content));
		}
		
		public void ShowPad(PadDescriptor content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			PadContentCollection.Add(content);
			
			if (layout != null) {
				layout.ShowPad(content);
			}
		}
		
		/// <summary>
		/// Closes and disposes a <see cref="IPadContent"/>.
		/// </summary>
		public void UnloadPad(PadDescriptor content)
		{
			PadContentCollection.Remove(content);
			
			if (layout != null) {
				layout.UnloadPad(content);
			}
			content.Dispose();
		}
		
		public void UpdateRenderer()
		{
			bool pro = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer", true);
			if (pro) {
				ToolStripManager.Renderer = new ToolStripProfessionalRenderer();
			} else {
				ProfessionalColorTable colorTable = new ProfessionalColorTable();
				colorTable.UseSystemColors        = true;
				ToolStripManager.Renderer         = new ToolStripProfessionalRenderer(colorTable);
			}
		}
		
		public void RedrawAllComponents()
		{
			RightToLeftConverter.ConvertRecursive(this);
			
			foreach (ToolStripItem item in TopMenu.Items) {
				if (item is IStatusUpdate)
					((IStatusUpdate)item).UpdateText();
			}
			
			foreach (IViewContent content in this.ViewContentCollection) {
				content.RedrawContent();
			}
			foreach (IWorkbenchWindow window in this.WorkbenchWindowCollection) {
				window.RedrawContent();
			}
			
			foreach (PadDescriptor content in padViewContentCollection) {
				content.RedrawContent();
			}
			
			if (layout != null) {
				layout.RedrawAllComponents();
			}
			
			StatusBarService.RedrawStatusbar();
		}
		
		#region Load/save view content mementos
		
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
			return String.Concat(viewContent.GetType().FullName.GetHashCode().ToString("x", CultureInfo.InvariantCulture), ":", FileUtility.NormalizePath(viewContent.PrimaryFileName).ToLowerInvariant());
		}
		
		void StoreMemento(IViewContent viewContent)
		{
			if (viewContent.PrimaryFileName == null)
				return;
			
			string key = GetMementoKeyName(viewContent);
			LoggingService.Debug("Saving memento of '" + viewContent.ToString() + "' to key '" + key + "'");
			
			Properties memento = ((IMementoCapable)viewContent).CreateMemento();
			Properties p = this.LoadOrCreateViewContentMementos();
			p.Set(key, memento);
			FileUtility.ObservedSave(new NamedFileOperationDelegate(p.Save), this.ViewContentMementosFileName, FileErrorPolicy.Inform);
		}
		
		Properties GetStoredMemento(IViewContent viewContent)
		{
			if (viewContent.PrimaryFileName == null)
				return null;
			
			string key = GetMementoKeyName(viewContent);
			LoggingService.Debug("Trying to restore memento of '" + viewContent.ToString() + "' from key '" + key + "'");
			
			return this.LoadOrCreateViewContentMementos().Get<Properties>(key, null);
		}
		
		#endregion
		
		// interface IMementoCapable
		public Properties CreateMemento()
		{
			Properties properties = new Properties();
			properties["bounds"] = normalBounds.X.ToString(NumberFormatInfo.InvariantInfo)
				+ "," + normalBounds.Y.ToString(NumberFormatInfo.InvariantInfo)
				+ "," + normalBounds.Width.ToString(NumberFormatInfo.InvariantInfo)
				+ "," + normalBounds.Height.ToString(NumberFormatInfo.InvariantInfo);
			
			if (FullScreen || WindowState == FormWindowState.Minimized)
				properties["windowstate"] = defaultWindowState.ToString();
			else
				properties["windowstate"] = WindowState.ToString();
			properties["defaultstate"] = defaultWindowState.ToString();
			
			return properties;
		}
		
		public void SetMemento(Properties properties)
		{
			if (properties != null && properties.Contains("bounds")) {
				string[] bounds = properties["bounds"].Split(',');
				if (bounds.Length == 4) {
					Bounds = normalBounds = new Rectangle(int.Parse(bounds[0], NumberFormatInfo.InvariantInfo),
					                                      int.Parse(bounds[1], NumberFormatInfo.InvariantInfo),
					                                      int.Parse(bounds[2], NumberFormatInfo.InvariantInfo),
					                                      int.Parse(bounds[3], NumberFormatInfo.InvariantInfo));
				}
				
				defaultWindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), properties["defaultstate"]);
				FullScreen         = properties.Get("fullscreen", false);
				WindowState        = (FormWindowState)Enum.Parse(typeof(FormWindowState), properties["windowstate"]);
			}
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (!FullScreen && WindowState != FormWindowState.Minimized) {
				defaultWindowState = WindowState;
				if (WindowState == FormWindowState.Normal) {
					normalBounds = Bounds;
				}
			}
		}
		
		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);
			if (WindowState == FormWindowState.Normal) {
				normalBounds = Bounds;
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
		}
		
		void CheckRenamedFile(object sender, FileRenameEventArgs e)
		{
			if (e.IsDirectory) {
				foreach (OpenedFile file in FileService.OpenedFiles) {
					if (file.FileName != null && FileUtility.IsBaseDirectory(e.SourceFile, file.FileName)) {
						file.FileName = FileUtility.RenameBaseDirectory(file.FileName, e.SourceFile, e.TargetFile);
					}
				}
			} else {
				foreach (OpenedFile file in FileService.OpenedFiles) {
					if (file.FileName != null &&
					    FileUtility.IsEqualFileName(file.FileName, e.SourceFile))
					{
						file.FileName  = e.TargetFile;
						return;
					}
				}
			}
		}
		
//		protected void OnTopMenuSelected(MenuCommand mc)
//		{
//
//
//			StatusBarService.SetMessage(mc.Description);
//		}
//
//		protected void OnTopMenuDeselected(MenuCommand mc)
//		{
//			SetStandardStatusBar(null, null);
//		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			
			if (ProjectService.IsBuilding) {
				MessageService.ShowMessage(StringParser.Parse("${res:MainWindow.CannotCloseWithBuildInProgressMessage}"));
				e.Cancel = true;
				return;
			}
			
			ProjectService.SaveSolutionPreferences();
			
			while (WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count > 0) {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.WorkbenchWindowCollection[0];
				if (!window.CloseWindow(false)) {
					e.Cancel = true;
					return;
				}
			}
			
			closeAll = true;
			
			ParserService.StopParserThread();
			
			layout.Detach();
			foreach (PadDescriptor padDescriptor in PadContentCollection) {
				padDescriptor.Dispose();
			}
			
			ProjectService.CloseSolution();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}
		
		void SetProjectTitle(object sender,  ProjectEventArgs e)
		{
			if (e.Project != null) {
				Title = e.Project.Name + " - " + ResourceService.GetString("MainWindow.DialogName");
			} else {
				Title = ResourceService.GetString("MainWindow.DialogName");
			}
		}
		
		void SetStandardStatusBar(object sender, EventArgs e)
		{
			StatusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}
		
//		public ToolStripManager ToolStripManager = new ToolStripManager();
		public MenuStrip   TopMenu  = null;
		public ToolStrip[] ToolBars = null;
		
		public PadDescriptor GetPad(Type type)
		{
			foreach (PadDescriptor pad in PadContentCollection) {
				if (pad.Class == type.FullName) {
					return pad;
				}
			}
			return null;
		}
		void CreateMainMenu()
		{
			TopMenu = new MenuStrip();
			TopMenu.Items.Clear();
			try {
				MenuService.AddItemsToMenu(TopMenu.Items, this, mainMenuPath);
				UpdateMenus();
			} catch (TreePathNotFoundException) {}
		}
		
		void UpdateMenu(object sender, EventArgs e)
		{
			UpdateMenus();
			UpdateToolbars();
		}
		
		void UpdateMenus()
		{
			// update menu
			foreach (object o in TopMenu.Items) {
				if (o is IStatusUpdate) {
					((IStatusUpdate)o).UpdateStatus();
				}
			}
		}
		
		void UpdateToolbars()
		{
			if (ToolBars != null) {
				foreach (ToolStrip toolStrip in ToolBars) {
					ToolbarService.UpdateToolbar(toolStrip);
				}
			}
		}
		
		void CreateToolBars()
		{
			if (ToolBars == null) {
				ToolBars = ToolbarService.CreateToolbars(this, "/SharpDevelop/Workbench/ToolBar");
			}
		}
		
		public static bool IsAltGRPressed {
			get {
				return NativeMethods.IsKeyPressed(Keys.RMenu) && (Control.ModifierKeys & Keys.Control) == Keys.Control;
			}
		}
		
		public event KeyEventHandler ProcessCommandKey;
		
		// Handle keyboard shortcuts
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (ProcessCommandKey != null) {
				KeyEventArgs e = new KeyEventArgs(keyData);
				ProcessCommandKey(this, e);
				if (e.Handled || e.SuppressKeyPress)
					return true;
			}
			if (IsAltGRPressed)
				return false;
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		protected override void OnDragEnter(DragEventArgs e)
		{
			try {
				base.OnDragEnter(e);
				if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
					string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
					foreach (string file in files) {
						if (File.Exists(file)) {
							e.Effect = DragDropEffects.Copy;
							return;
						}
					}
				}
				e.Effect = DragDropEffects.None;
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		protected override void OnDragDrop(DragEventArgs e)
		{
			try {
				base.OnDragDrop(e);
				if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
					string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
					
					foreach (string file in files) {
						if (File.Exists(file)) {
							IProjectLoader loader = ProjectService.GetProjectLoader(file);
							if (loader != null) {
								FileUtility.ObservedLoad(new NamedFileOperationDelegate(loader.Load), file);
							} else {
								FileService.OpenFile(file);
							}
						}
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void OnViewOpened(ViewContentEventArgs e)
		{
			if (ViewOpened != null) {
				ViewOpened(this, e);
			}
		}
		
		void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}
		
		public event ViewContentEventHandler ViewOpened;
		public event ViewContentEventHandler ViewClosed;
		
		protected override void OnActivated(EventArgs e)
		{
			isActiveWindow = true;
			base.OnActivated(e);
		}
		
		protected override void OnDeactivate(EventArgs e)
		{
			isActiveWindow = false;
			base.OnDeactivate(e);
		}
	}
}
