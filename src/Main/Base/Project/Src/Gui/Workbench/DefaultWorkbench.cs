// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a multiple document interface.
	/// </summary>
	public class DefaultWorkbench : Form, IWorkbench
	{
		readonly static string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";
		readonly static string viewContentPath = "/SharpDevelop/Workbench/Pads";
		
		List<PadDescriptor>  viewContentCollection    = new List<PadDescriptor>();
		List<IViewContent> workbenchContentCollection = new List<IViewContent>();
		
		bool closeAll = false;
		
		bool            fullscreen;
		FormWindowState defaultWindowState = FormWindowState.Normal;
		Rectangle       normalBounds       = new Rectangle(0, 0, 640, 480);
		
		IWorkbenchLayout layout = null;
		
		public bool FullScreen {
			get {
				return fullscreen;
			}
			set {
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
			}
		}
		
		public List<PadDescriptor> PadContentCollection {
			get {
				System.Diagnostics.Debug.Assert(viewContentCollection != null);
				return viewContentCollection;
			}
		}
		
		public List<IViewContent> ViewContentCollection {
			get {
				System.Diagnostics.Debug.Assert(workbenchContentCollection != null);
				return workbenchContentCollection;
			}
		}
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				if (layout == null) {
					return null;
				}
				return layout.ActiveWorkbenchwindow;
			}
		}
		
		public object ActiveContent {
			get {
				if (layout == null) {
					return null;
				}
				return layout.ActiveContent;
			}
		}
		
		
		public DefaultWorkbench()
		{
			RightToLeft = RightToLeftConverter.RightToLeft;
			Text = ResourceService.GetString("MainWindow.DialogName");
			Icon = ResourceService.GetIcon("Icons.SharpDevelopIcon");
			
			StartPosition = FormStartPosition.Manual;
			AllowDrop     = true;
		}
		
		System.Windows.Forms.Timer toolbarUpdateTimer;

		public void InitializeWorkspace()
		{
			UpdateRenderer();
			
			MenuComplete                 += new EventHandler(SetStandardStatusBar);
			
			SetStandardStatusBar(null, null);
			
			ProjectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);

			FileService.FileRemoved += CheckRemovedFile;
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
		}
		
		public void CloseContent(IViewContent content)
		{
			if (PropertyService.Get("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable) {
				StoreMemento(content);
			}
			if (ViewContentCollection.Contains(content)) {
				ViewContentCollection.Remove(content);
			}
			OnViewClosed(new ViewContentEventArgs(content));
			content.Dispose();
			content = null;
		}
		
		public void CloseAllViews()
		{
			try {
				closeAll = true;
				List<IViewContent> fullList = new List<IViewContent>(workbenchContentCollection);
				foreach (IViewContent content in fullList) {
					IWorkbenchWindow window = content.WorkbenchWindow;
					window.CloseWindow(false);
				}
			} finally {
				closeAll = false;
				OnActiveWindowChanged(this, EventArgs.Empty);
			}
		}
		
		public virtual void ShowView(IViewContent content)
		{
			System.Diagnostics.Debug.Assert(layout != null);
			ViewContentCollection.Add(content);
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
			
			layout.ShowView(content);
			content.WorkbenchWindow.SelectWindow();
			OnViewOpened(new ViewContentEventArgs(content));
		}
		
		public virtual void ShowPad(PadDescriptor content)
		{
			PadContentCollection.Add(content);
			
			if (layout != null) {
				layout.ShowPad(content);
			}
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
			RightToLeft = RightToLeftConverter.RightToLeft;
			
			foreach (ToolStripItem item in TopMenu.Items) {
				if (item is IStatusUpdate)
					((IStatusUpdate)item).UpdateText();
			}
			
			foreach (IViewContent content in workbenchContentCollection) {
				content.RedrawContent();
				if (content.WorkbenchWindow != null) {
					content.WorkbenchWindow.RedrawContent();
				}
			}
			
			foreach (PadDescriptor content in viewContentCollection) {
				content.RedrawContent();
			}
			
			if (layout != null) {
				layout.RedrawAllComponents();
			}
			
			StatusBarService.RedrawStatusbar();
		}
		
		public Properties GetStoredMemento(IViewContent content)
		{
			if (content != null && content.FileName != null) {
				string directory = Path.Combine(PropertyService.ConfigDirectory, "temp");
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}
				string fileName = content.FileName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
				string fullFileName = Path.Combine(directory, fileName);
				// check the file name length because it could be more than the maximum length of a file name
				
				if (FileUtility.IsValidFileName(fullFileName) && File.Exists(fullFileName)) {
					return Properties.Load(fullFileName);
				}
			}
			return null;
		}
		
		public void StoreMemento(IViewContent content)
		{
			if (content.FileName == null) {
				return;
			}
			
			string directory = PropertyService.ConfigDirectory + "temp";
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			
			Properties memento = ((IMementoCapable)content).CreateMemento();
			string fileName = content.FileName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
			string fullFileName = Path.Combine(directory, fileName);
			
			if (FileUtility.IsValidFileName(fullFileName)) {
				FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
			}
		}
		
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
		
		void CheckRemovedFile(object sender, FileEventArgs e)
		{
			for (int i = 0; i < ViewContentCollection.Count;) {
				if (FileUtility.IsBaseDirectory(e.FileName, ViewContentCollection[i].FileName)) {
					ViewContentCollection[i].WorkbenchWindow.CloseWindow(true);
				} else {
					++i;
				}
			}
		}
		
		void CheckRenamedFile(object sender, FileRenameEventArgs e)
		{
			if (e.IsDirectory) {
				foreach (IViewContent content in ViewContentCollection) {
					if (content.FileName.StartsWith(e.SourceFile)) {
						content.FileName = e.TargetFile + content.FileName.Substring(e.SourceFile.Length);
					}
				}
			} else {
				foreach (IViewContent content in ViewContentCollection) {
					if (content.FileName != null &&
					    FileUtility.IsEqualFileName(content.FileName, e.SourceFile)) {
						content.FileName  = e.TargetFile;
						content.TitleName = Path.GetFileName(e.TargetFile);
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
			
			ProjectService.SaveSolutionPreferences();
			
			while (WorkbenchSingleton.Workbench.ViewContentCollection.Count > 0) {
				IViewContent content = WorkbenchSingleton.Workbench.ViewContentCollection[0];
				if (content.WorkbenchWindow == null) {
					LoggingService.Warn("Content with empty WorkbenchWindow found");
					WorkbenchSingleton.Workbench.ViewContentCollection.RemoveAt(0);
				} else {
					content.WorkbenchWindow.CloseWindow(false);
					if (WorkbenchSingleton.Workbench.ViewContentCollection.IndexOf(content) >= 0) {
						e.Cancel = true;
						return;
					}
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
		
		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (!closeAll && ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
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
				ToolStripItem[] items = (ToolStripItem[])(AddInTree.GetTreeNode(mainMenuPath).BuildChildItems(this)).ToArray(typeof(ToolStripItem));
				TopMenu.Items.AddRange(items);
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
		
		const int VK_RMENU = 0xA5; // right alt key
		
		[System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling=true)]
		static extern short GetAsyncKeyState(int vKey);
		
		public bool IsAltGRPressed {
			get {
				return GetAsyncKeyState(VK_RMENU) < 0 && (Control.ModifierKeys & Keys.Control) == Keys.Control;
			}
		}
		
		// Handle keyboard shortcuts
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (IsAltGRPressed)
				return false;
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		protected override void OnDragEnter(DragEventArgs e)
		{
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
		}
		
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				
				foreach (string file in files) {
					if (File.Exists(file)) {
						FileService.OpenFile(file);
					}
				}
			}
		}
		
		protected virtual void OnViewOpened(ViewContentEventArgs e)
		{
			if (ViewOpened != null) {
				ViewOpened(this, e);
			}
		}
		
		protected virtual void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}
		
		public event ViewContentEventHandler ViewOpened;
		public event ViewContentEventHandler ViewClosed;
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
