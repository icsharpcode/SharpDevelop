// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets.AutoHide;
using WeifenLuo.WinFormsUI;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		DefaultWorkbench wbForm;
		
		DockPanel dockPanel;
		Dictionary<string, PadContentWrapper> contentHash = new Dictionary<string, PadContentWrapper>();
		ToolStripContainer toolStripContainer;
		AutoHideMenuStripContainer mainMenuContainer;
		AutoHideStatusStripContainer statusStripContainer;
		#if DEBUG
		static bool firstTimeError = true; // TODO: Debug statement only, remove me
		#endif
		
		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (dockPanel == null)  {
					return null;
				}
				
				// TODO: Debug statements only, remove me
				#if DEBUG
				if (dockPanel.ActiveDocument != null && !(dockPanel.ActiveDocument is IWorkbenchWindow)) {
					if (firstTimeError) {
						MessageBox.Show("ActiveDocument was " + dockPanel.ActiveDocument.GetType().FullName);
						firstTimeError = false;
					}
				}
				#endif
				
				IWorkbenchWindow window = dockPanel.ActiveDocument as IWorkbenchWindow;
				if (window == null || window.IsDisposed) {
					return null;
				}
				return window;
			}
		}
		
		// prevent setting ActiveContent to null when application loses focus (e.g. because of context menu popup)
		IDockContent lastActiveContent;
		
		public object ActiveContent {
			get {
				IDockContent activeContent;
				if (dockPanel == null)  {
					activeContent = lastActiveContent;
				} else {
					activeContent = dockPanel.ActiveContent ?? lastActiveContent;
				}
				if (activeContent != null && activeContent.IsDisposed)
					activeContent = null;
				
				lastActiveContent = activeContent;
				
				if (activeContent is IWorkbenchWindow)
					return ((IWorkbenchWindow)activeContent).ActiveViewContent;
				if (activeContent is PadContentWrapper)
					return ((PadContentWrapper)activeContent).PadContent;
				
				return activeContent;
			}
		}
		
		
		public void Attach(IWorkbench workbench)
		{
			wbForm = (DefaultWorkbench)workbench;
			wbForm.SuspendLayout();
			wbForm.Controls.Clear();
			toolStripContainer = new ToolStripContainer();
			toolStripContainer.SuspendLayout();
			toolStripContainer.Dock = DockStyle.Fill;
			
			mainMenuContainer = new AutoHideMenuStripContainer(((DefaultWorkbench)wbForm).TopMenu);
			mainMenuContainer.Dock = DockStyle.Top;
			
			statusStripContainer = new AutoHideStatusStripContainer((StatusStrip)StatusBarService.Control);
			statusStripContainer.Dock = DockStyle.Bottom;
			
			dockPanel = new WeifenLuo.WinFormsUI.DockPanel();
			dockPanel.DocumentStyle = DocumentStyles.DockingWindow;
			this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			
			Panel helperPanel = new Panel();
			helperPanel.Dock = DockStyle.Fill;
			helperPanel.Controls.Add(dockPanel);
			toolStripContainer.ContentPanel.Controls.Add(helperPanel);
			
			toolStripContainer.ContentPanel.Controls.Add(mainMenuContainer);
			toolStripContainer.ContentPanel.Controls.Add(statusStripContainer);
			
			wbForm.Controls.Add(toolStripContainer);
			// dock panel has to be added to the form before LoadLayoutConfiguration is called to fix SD2-463
			
			LoadLayoutConfiguration();
			ShowPads();
			
			ShowViewContents();
			
			RedrawAllComponents();
			
			dockPanel.ActiveDocumentChanged += new EventHandler(ActiveMdiChanged);
			dockPanel.ActiveContentChanged += new EventHandler(ActiveContentChanged);
			ActiveMdiChanged(this, EventArgs.Empty);
			
			toolStripContainer.ResumeLayout(false);
			wbForm.ResumeLayout(false);
			
			Properties fullscreenProperties = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.FullscreenOptions", new Properties());
			fullscreenProperties.PropertyChanged += TrackFullscreenPropertyChanges;
		}
		
		void TrackFullscreenPropertyChanges(object sender, PropertyChangedEventArgs e)
		{
			if (!Boolean.Equals(e.OldValue, e.NewValue) && wbForm.FullScreen) {
				switch (e.Key) {
					case "HideMainMenu":
					case "ShowMainMenuOnMouseMove":
						RedrawMainMenu();
						break;
					case "HideToolbars":
						RedrawToolbars();
						break;
						//case "HideTabs":
						//case "HideVerticalScrollbar":
						//case "HideHorizontalScrollbar":
					case "HideStatusBar":
					case "ShowStatusBarOnMouseMove":
						RedrawStatusBar();
						break;
						//case "HideWindowsTaskbar":
				}
			}
		}
		
		void ShowPads()
		{
			foreach (PadDescriptor content in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (!contentHash.ContainsKey(content.Class)) {
					ShowPad(content);
				}
			}
			// ShowPads could create new pads if new addins have been installed, so we
			// need to call AllowInitialize here instead of in LoadLayoutConfiguration
			foreach (PadContentWrapper content in contentHash.Values) {
				content.AllowInitialize();
			}
		}
		void ShowViewContents()
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				ShowView(content);
			}
		}
		
		void LoadLayoutConfiguration()
		{
			try {
				if (File.Exists(LayoutConfiguration.CurrentLayoutFileName)) {
					LoadDockPanelLayout(LayoutConfiguration.CurrentLayoutFileName);
				} else {
					LoadDefaultLayoutConfiguration();
				}
			} catch {
				// ignore errors loading configuration
			}
		}
		
		void LoadDefaultLayoutConfiguration()
		{
			if (File.Exists(LayoutConfiguration.CurrentLayoutTemplateFileName)) {
				LoadDockPanelLayout(LayoutConfiguration.CurrentLayoutTemplateFileName);
			}
		}
		
		void LoadDockPanelLayout(string fileName)
		{
			// LoadFromXml(fileName, ...) locks the file against simultanous read access
			// -> we would loose the layout when starting two SharpDevelop instances
			//    at the same time => open stream with shared read access.
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
				dockPanel.LoadFromXml(fs, new DeserializeDockContent(GetContent));
			}
		}
		
		void ShowToolBars()
		{
			if (wbForm.ToolBars != null) {
				ArrayList oldControls = new ArrayList();
				foreach (Control ctl in toolStripContainer.ContentPanel.Controls) {
					oldControls.Add(ctl);
				}
				toolStripContainer.ContentPanel.Controls.Clear();
				toolStripContainer.ContentPanel.Controls.Add(oldControls[0] as Control);
				foreach (ToolStrip toolBar in wbForm.ToolBars) {
					if (!toolStripContainer.ContentPanel.Controls.Contains(toolBar)) {
						toolStripContainer.ContentPanel.Controls.Add(toolBar);
					}
				}
				for (int i = 1; i < oldControls.Count; i++) {
					toolStripContainer.ContentPanel.Controls.Add(oldControls[i] as Control);
				}
			}
		}
		
		void HideToolBars()
		{
			if (wbForm.ToolBars != null) {
				foreach (ToolStrip toolBar in wbForm.ToolBars) {
					if (toolStripContainer.ContentPanel.Controls.Contains(toolBar)) {
						toolStripContainer.ContentPanel.Controls.Remove(toolBar);
					}
				}
			}
		}
		
		DockContent GetContent(string padTypeName)
		{
			foreach (PadDescriptor content in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (content.Class == padTypeName) {
					return CreateContent(content);
				}
			}
			return null;
		}
		
		public void LoadConfiguration()
		{
			if (dockPanel != null) {
				NativeMethods.SetWindowRedraw(wbForm.Handle, false);
				try {
					IViewContent activeView = GetActiveView();
					dockPanel.ActiveDocumentChanged -= new EventHandler(ActiveMdiChanged);
					
					DetachPadContents(false);
					DetachViewContents(false);
					dockPanel.ActiveDocumentChanged += new EventHandler(ActiveMdiChanged);
					
					LoadLayoutConfiguration();
					ShowPads();
					ShowViewContents();
					if (activeView != null && activeView.WorkbenchWindow != null) {
						activeView.WorkbenchWindow.SelectWindow();
					}
				} finally {
					NativeMethods.SetWindowRedraw(wbForm.Handle, true);
				}
			}
		}
		
		public void StoreConfiguration()
		{
			try {
				if (dockPanel != null) {
					LayoutConfiguration current = LayoutConfiguration.CurrentLayout;
					if (current != null && !current.ReadOnly) {
						
						string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
						if (!Directory.Exists(configPath))
							Directory.CreateDirectory(configPath);
						dockPanel.SaveAsXml(Path.Combine(configPath, current.FileName), System.Text.Encoding.UTF8);
					}
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		void DetachPadContents(bool dispose)
		{
			foreach (PadContentWrapper padContentWrapper in contentHash.Values) {
				padContentWrapper.allowInitialize = false;
			}
			foreach (PadDescriptor content in ((DefaultWorkbench)wbForm).PadContentCollection) {
				try {
					PadContentWrapper padContentWrapper = contentHash[content.Class];
					padContentWrapper.DockPanel = null;
					if (dispose) {
						padContentWrapper.DetachContent();
						padContentWrapper.Dispose();
					}
				} catch (Exception e) { MessageService.ShowError(e); }
			}
			if (dispose) {
				contentHash.Clear();
			}
		}
		
		void DetachViewContents(bool dispose)
		{
			foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
				try {
					SdiWorkspaceWindow f = (SdiWorkspaceWindow)viewContent.WorkbenchWindow;
					f.DockPanel = null;
					if (dispose) {
						viewContent.WorkbenchWindow = null;
						f.CloseEvent -= new EventHandler(CloseWindowEvent);
						f.DetachContent();
						f.Dispose();
					}
				} catch (Exception e) { MessageService.ShowError(e); }
			}
		}
		public void Detach()
		{
			StoreConfiguration();
			
			dockPanel.ActiveDocumentChanged -= new EventHandler(ActiveMdiChanged);
			
			DetachPadContents(true);
			DetachViewContents(true);
			
			try {
				if (dockPanel != null) {
					dockPanel.Dispose();
					dockPanel = null;
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			if (contentHash != null) {
				contentHash.Clear();
			}
			
			wbForm.Controls.Clear();
		}
		
		class PadContentWrapper : DockContent
		{
			PadDescriptor padDescriptor;
			bool isInitialized = false;
			internal bool allowInitialize = false;
			
			public IPadContent PadContent {
				get {
					return padDescriptor.PadContent;
				}
			}
			
			public PadContentWrapper(PadDescriptor padDescriptor)
			{
				if (padDescriptor == null)
					throw new ArgumentNullException("padDescriptor");
				this.padDescriptor = padDescriptor;
				this.DockableAreas = ((((WeifenLuo.WinFormsUI.DockAreas.Float | WeifenLuo.WinFormsUI.DockAreas.DockLeft) |
				                        WeifenLuo.WinFormsUI.DockAreas.DockRight) |
				                       WeifenLuo.WinFormsUI.DockAreas.DockTop) |
				                      WeifenLuo.WinFormsUI.DockAreas.DockBottom);
				HideOnClose = true;
			}
			
			public void DetachContent()
			{
				Controls.Clear();
				padDescriptor = null;
			}
			
			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);
				if (Visible && Width > 0)
					ActivateContent();
			}
			
			protected override void OnSizeChanged(EventArgs e)
			{
				base.OnSizeChanged(e);
				if (Visible && Width > 0)
					ActivateContent();
			}
			
			/// <summary>
			/// Enables initializing the content. This is used to prevent initializing all view
			/// contents when the layout configuration is changed.
			/// </summary>
			public void AllowInitialize()
			{
				allowInitialize = true;
				if (Visible && Width > 0)
					ActivateContent();
			}
			
			void ActivateContent()
			{
				if (!allowInitialize)
					return;
				if (!isInitialized) {
					isInitialized = true;
					IPadContent content = padDescriptor.PadContent;
					if (content == null)
						return;
					Control control = content.Control;
					control.Dock = DockStyle.Fill;
					Controls.Add(control);
				}
			}
			
			protected override string GetPersistString()
			{
				return padDescriptor.Class;
			}
			
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (disposing) {
					if (padDescriptor != null) {
						padDescriptor.Dispose();
						padDescriptor = null;
					}
				}
			}
		}
		
		PadContentWrapper CreateContent(PadDescriptor content)
		{
			if (contentHash.ContainsKey(content.Class)) {
				return contentHash[content.Class];
			}
			Properties properties = (Properties)PropertyService.Get("Workspace.ViewMementos", new Properties());
			
			PadContentWrapper newContent = new PadContentWrapper(content);
			if (!string.IsNullOrEmpty(content.Icon)) {
				newContent.Icon = IconService.GetIcon(content.Icon);
			}
			newContent.Text = StringParser.Parse(content.Title);
			contentHash[content.Class] = newContent;
			return newContent;
		}
		
		public void ShowPad(PadDescriptor content)
		{
			if (content == null) {
				return;
			}
			if (!contentHash.ContainsKey(content.Class)) {
				DockContent newContent = CreateContent(content);
				// TODO: read the default dock state from the PadDescriptor
				// we'll also need to allow for default-hidden (HideOnClose) contents
				// which seems to be not possible using any Show overload.
				newContent.Show(dockPanel);
			} else {
				contentHash[content.Class].Show();
			}
		}
		
		public bool IsVisible(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				return !contentHash[padContent.Class].IsHidden;
			}
			return false;
		}
		
		public void HidePad(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				contentHash[padContent.Class].Hide();
			}
		}
		
		public void UnloadPad(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				contentHash[padContent.Class].Close();
				contentHash[padContent.Class].Dispose();
				contentHash.Remove(padContent.Class);
			}
		}
		
		public void ActivatePad(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				//contentHash[padContent.Class].ActivateContent();
				contentHash[padContent.Class].Show();
			}
		}
		public void ActivatePad(string fullyQualifiedTypeName)
		{
			//contentHash[fullyQualifiedTypeName].ActivateContent();
			contentHash[fullyQualifiedTypeName].Show();
		}
		
		
		public void RedrawAllComponents()
		{
			// redraw correct pad content names (language changed).
			foreach (PadDescriptor padDescriptor in ((IWorkbench)wbForm).PadContentCollection) {
				DockContent c = contentHash[padDescriptor.Class];
				if (c != null) {
					c.Text = StringParser.Parse(padDescriptor.Title);
				}
			}
			
			RedrawMainMenu();
			RedrawToolbars();
			RedrawStatusBar();
		}
		
		void RedrawMainMenu()
		{
			Properties fullscreenProperties = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.FullscreenOptions", new Properties());
			bool hideInFullscreen = fullscreenProperties.Get("HideMainMenu", false);
			bool showOnMouseMove = fullscreenProperties.Get("ShowMainMenuOnMouseMove", true);
			
			mainMenuContainer.AutoHide = wbForm.FullScreen && hideInFullscreen;
			mainMenuContainer.ShowOnMouseDown = true;
			mainMenuContainer.ShowOnMouseMove = showOnMouseMove;
		}
		
		void RedrawToolbars()
		{
			Properties fullscreenProperties = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.FullscreenOptions", new Properties());
			bool hideInFullscreen = fullscreenProperties.Get("HideToolbars", true);
			bool toolBarVisible = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", true);
			
			if (toolBarVisible) {
				if (wbForm.FullScreen && hideInFullscreen) {
					HideToolBars();
				} else {
					ShowToolBars();
				}
			} else {
				HideToolBars();
			}
		}
		
		void RedrawStatusBar()
		{
			Properties fullscreenProperties = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.FullscreenOptions", new Properties());
			bool hideInFullscreen = fullscreenProperties.Get("HideStatusBar", true);
			bool showOnMouseMove = fullscreenProperties.Get("ShowStatusBarOnMouseMove", true);
			bool statusBarVisible = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
			
			statusStripContainer.AutoHide = wbForm.FullScreen && hideInFullscreen;
			statusStripContainer.ShowOnMouseDown = true;
			statusStripContainer.ShowOnMouseMove = showOnMouseMove;
			statusStripContainer.Visible = statusBarVisible;
		}
		
		public void CloseWindowEvent(object sender, EventArgs e)
		{
			SdiWorkspaceWindow f = (SdiWorkspaceWindow)sender;
			f.CloseEvent -= CloseWindowEvent;
			if (f.ViewContent != null) {
				((IWorkbench)wbForm).CloseContent(f.ViewContent);
				if (f == oldSelectedWindow) {
					oldSelectedWindow = null;
				}
				ActiveMdiChanged(this, null);
			}
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			if (content.WorkbenchWindow is SdiWorkspaceWindow) {
				SdiWorkspaceWindow oldSdiWindow = (SdiWorkspaceWindow)content.WorkbenchWindow;
				if (!oldSdiWindow.IsDisposed) {
					oldSdiWindow.Show(dockPanel);
					return oldSdiWindow;
				}
			}
			if (!content.Control.Visible) {
				content.Control.Visible = true;
			}
			content.Control.Dock = DockStyle.Fill;
			SdiWorkspaceWindow sdiWorkspaceWindow = new SdiWorkspaceWindow(content);
			sdiWorkspaceWindow.CloseEvent        += new EventHandler(CloseWindowEvent);
			if (dockPanel != null) {
				sdiWorkspaceWindow.Show(dockPanel);
			}
			
			return sdiWorkspaceWindow;
		}
		
		void ActiveMdiChanged(object sender, EventArgs e)
		{
			OnActiveWorkbenchWindowChanged(e);
		}
		
		void ActiveContentChanged(object sender, EventArgs e)
		{
			OnActiveWorkbenchWindowChanged(e);
		}
		
		static IViewContent GetActiveView()
		{
			IWorkbenchWindow activeWindow = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (activeWindow != null) {
				return activeWindow.ViewContent;
			}
			return null;
		}
		
		IWorkbenchWindow oldSelectedWindow = null;
		public virtual void OnActiveWorkbenchWindowChanged(EventArgs e)
		{
			IWorkbenchWindow newWindow = this.ActiveWorkbenchwindow;
			if (newWindow == null || newWindow.ViewContent != null) {
				if (ActiveWorkbenchWindowChanged != null) {
					ActiveWorkbenchWindowChanged(this, e);
				}
				//if (newWindow == null)
				//	LoggingService.Debug("window change to null");
				//else
				//	LoggingService.Debug("window change to " + newWindow);
			} else {
				//LoggingService.Debug("ignore window change to disposed window");
			}
			if (oldSelectedWindow != null) {
				oldSelectedWindow.OnWindowDeselected(EventArgs.Empty);
			}
			oldSelectedWindow = newWindow;
			if (oldSelectedWindow != null && oldSelectedWindow.ActiveViewContent != null && oldSelectedWindow.ActiveViewContent.Control != null) {
				oldSelectedWindow.OnWindowSelected(EventArgs.Empty);
				oldSelectedWindow.ActiveViewContent.SwitchedTo();
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
