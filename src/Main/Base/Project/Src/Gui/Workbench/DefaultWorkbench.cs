// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
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
		
		EventHandler windowChangeEventHandler;
		
		public IWorkbenchLayout WorkbenchLayout {
			get {
				return layout;
			}
			set {
				if (layout != null) {
					layout.ActiveWorkbenchWindowChanged -= windowChangeEventHandler;
					layout.Detach();
				}
				value.Attach(this);
				layout = value;
				layout.ActiveWorkbenchWindowChanged += windowChangeEventHandler;
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
			
			windowChangeEventHandler = new EventHandler(OnActiveWindowChanged);
			
			StartPosition = FormStartPosition.Manual;
			AllowDrop     = true;
		}
		
		System.Windows.Forms.Timer toolbarUpdateTimer;

		public void InitializeWorkspace()
		{
			MenuComplete                 += new EventHandler(SetStandardStatusBar);
			SetStandardStatusBar(null, null);
			
			ProjectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);

			FileService.FileRemoved += new FileEventHandler(CheckRemovedFile);
			FileService.FileRenamed += new FileRenameEventHandler(CheckRenamedFile);
			
			FileService.FileRemoved += new FileEventHandler(FileService.RecentOpen.FileRemoved);
			FileService.FileRenamed += new FileRenameEventHandler(FileService.RecentOpen.FileRenamed);
			
			CreateMainMenu();
			CreateToolBars();
			
			try {
				ArrayList contents = AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this);
				foreach (PadDescriptor content in contents) {
					ShowPad(content);
				}
			} catch (TreePathNotFoundException) {}
			
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
				OnActiveWindowChanged(null, null);
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
					Console.WriteLine("Can't get/set memento : " + e.ToString());
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
		
		public void RedrawAllComponents()
		{
			RightToLeft = RightToLeftConverter.RightToLeft;
			
			foreach (IViewContent content in workbenchContentCollection) {
				content.RedrawContent();
				if (content.WorkbenchWindow != null) {
					content.WorkbenchWindow.RedrawContent();
				}
			}
			foreach (PadDescriptor content in viewContentCollection) {
				content.RedrawContent();
			}
			layout.RedrawAllComponents();
			
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
			properties["bounds"]      = normalBounds.X + "," + normalBounds.Y + "," + normalBounds.Width + "," + normalBounds.Height;
			
			properties["windowstate"] = WindowState.ToString();
			properties["defaultstate"]= defaultWindowState.ToString();
			properties["fullscreen"]  = fullscreen.ToString();
			
			return properties;
		}
		
		public void SetMemento(Properties properties)
		{
			if (properties != null && properties.Contains("bounds")) {
				string[] bounds = properties["bounds"].Split(',');
				if (bounds.Length == 4) {
					Bounds = normalBounds = new Rectangle(Int32.Parse(bounds[0]), Int32.Parse(bounds[1]), Int32.Parse(bounds[2]), Int32.Parse(bounds[3]));
				}
				
				defaultWindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), properties["defaultstate"]);
				FullScreen         = properties.Get("fullscreen", false);
				WindowState        = (FormWindowState)Enum.Parse(typeof(FormWindowState), properties["windowstate"]);
			}
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (WindowState == FormWindowState.Normal) {
				normalBounds = Bounds;
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
			
			while (WorkbenchSingleton.Workbench.ViewContentCollection.Count > 0) {
				IViewContent content = WorkbenchSingleton.Workbench.ViewContentCollection[0];
				content.WorkbenchWindow.CloseWindow(false);
				if (WorkbenchSingleton.Workbench.ViewContentCollection.IndexOf(content) >= 0) {
					e.Cancel = true;
					return;
				}
			}
			ProjectService.CloseSolution();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			
			ProjectService.CloseSolution();

			closeAll = true;

			layout.Detach();
			foreach (PadDescriptor padDescriptor in PadContentCollection) {
				padDescriptor.Dispose();
			}
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
				foreach (ToolStrip ToolStrip in ToolBars) {
					bool doRefresh = false;
					foreach (ToolStripItem item in ToolStrip.Items) {
						bool wasVisible = item.Visible;
						if (item is ToolBarCommand) {
							ToolBarCommand toolBarCommand = (ToolBarCommand)item;
							doRefresh |= toolBarCommand.LastEnabledStatus != toolBarCommand.CurrentEnableStatus;
							toolBarCommand.UpdateStatus();
						} else {
							if (item is IStatusUpdate) {
								((IStatusUpdate)item).UpdateStatus();
							}
						}
						doRefresh |= wasVisible != item.Visible;
					}
					if (doRefresh) {
						ToolStrip.Refresh();
					}
				}
			}
		}
		
		// this method simply copies over the enabled state of the toolbar,
		// this assumes that no item is inserted or removed.
		// TODO : make this method more add-in tree like, currently with Windows.Forms
		//        toolbars this is not possible. (drawing fragments, slow etc.)
		void CreateToolBars()
		{
			if (ToolBars == null) {
				ToolBars = ToolbarService.CreateToolbars(this, "/SharpDevelop/Workbench/ToolBar");
				UpdateToolbars();
			}
		}
		
//		// Handle keyboard shortcuts
//		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
//		{
//			if (this.ToolStripManager.PreProcessMessage(ref msg)) {
//				return true;
//			}
//
//			return base.ProcessCmdKey(ref msg, keyData);
//		}
		
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
