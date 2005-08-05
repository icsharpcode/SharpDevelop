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
using System.Reflection;

using ICSharpCode.Core;
using WeifenLuo.WinFormsUI;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		Form wbForm;
		
		DockPanel dockPanel;
		Dictionary<string, PadContentWrapper> contentHash = new Dictionary<string, PadContentWrapper>();
		ToolStripContainer toolStripContainer;
		
		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (dockPanel == null || dockPanel.ActiveDocument == null || dockPanel.ActiveDocument.IsDisposed)  {
					return null;
				}
				return dockPanel.ActiveDocument as IWorkbenchWindow;
			}
		}
		public object ActiveContent {
			get {
				if (dockPanel == null || dockPanel.ActiveContent == null)  {
					if (this.ActiveWorkbenchwindow == null) {
						return null;
					}
					return this.ActiveWorkbenchwindow.ActiveViewContent;
				}
				
				if (dockPanel.ActiveContent.IsDisposed) {
					return null;
				}
				if (dockPanel.ActiveContent is IWorkbenchWindow) {
					return ((IWorkbenchWindow)dockPanel.ActiveContent).ActiveViewContent;
				}
				
				if (dockPanel.ActiveContent is PadContentWrapper) {
					return ((PadContentWrapper)dockPanel.ActiveContent).PadContent;
				}
				
				return dockPanel.ActiveContent;
			}
		}
		
		
		public void Attach(IWorkbench workbench)
		{
			wbForm = (Form)workbench;
			wbForm.SuspendLayout();
			wbForm.Controls.Clear();
			toolStripContainer = new ToolStripContainer();
			toolStripContainer.Dock = DockStyle.Fill;
			
//			RaftingContainer topRaftingContainer = new System.Windows.Forms.RaftingContainer();
//			RaftingContainer leftRaftingContainer = new System.Windows.Forms.RaftingContainer();
//			RaftingContainer rightRaftingContainer = new System.Windows.Forms.RaftingContainer();
//			RaftingContainer bottomRaftingContainer = new System.Windows.Forms.RaftingContainer();
//
//			((System.ComponentModel.ISupportInitialize)(leftRaftingContainer)).BeginInit();
//			((System.ComponentModel.ISupportInitialize)(rightRaftingContainer)).BeginInit();
//			((System.ComponentModel.ISupportInitialize)(topRaftingContainer)).BeginInit();
//			topRaftingContainer.SuspendLayout();
//			((System.ComponentModel.ISupportInitialize)(bottomRaftingContainer)).BeginInit();
//			bottomRaftingContainer.SuspendLayout();
//			leftRaftingContainer.Dock = System.Windows.Forms.DockStyle.Left;
//			rightRaftingContainer.Dock = System.Windows.Forms.DockStyle.Right;
			
//			topRaftingContainer.Dock = System.Windows.Forms.DockStyle.Top;
//			bottomRaftingContainer.Controls.Add(StatusBarService.Control);
//			bottomRaftingContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
			
			dockPanel = new WeifenLuo.WinFormsUI.DockPanel();
			toolStripContainer.ContentPanel.Controls.Add(this.dockPanel);
			
			this.dockPanel.ActiveAutoHideContent = null;
			this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockPanel.ActiveContentChanged += new EventHandler(DockPanelActiveContentChanged);
			
			LoadLayoutConfiguration();
			ShowPads();
			ShowViewContents();
			
			RedrawAllComponents();
			
			dockPanel.ActiveDocumentChanged += new EventHandler(ActiveMdiChanged);
			dockPanel.ActiveContentChanged += new EventHandler(ActiveContentChanged);
			ActiveMdiChanged(this, EventArgs.Empty);
			
//			wbForm.Controls.Add(this.dockPanel);
			
			toolStripContainer.ContentPanel.Controls.Add(((DefaultWorkbench)workbench).TopMenu);
			StatusBarService.Control.Dock = DockStyle.Bottom;
			toolStripContainer.ContentPanel.Controls.Add(StatusBarService.Control);
			
			wbForm.Controls.Add(toolStripContainer);
			
//			((System.ComponentModel.ISupportInitialize)(leftRaftingContainer)).EndInit();
//			((System.ComponentModel.ISupportInitialize)(rightRaftingContainer)).EndInit();
//			((System.ComponentModel.ISupportInitialize)(topRaftingContainer)).EndInit();
//			topRaftingContainer.ResumeLayout(false);
//			topRaftingContainer.PerformLayout();
//			((System.ComponentModel.ISupportInitialize)(bottomRaftingContainer)).EndInit();
//			bottomRaftingContainer.ResumeLayout(false);
//			bottomRaftingContainer.PerformLayout();
			
			wbForm.ResumeLayout(false);
		}
		
		void ShowPads()
		{
			foreach (PadDescriptor content in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (!contentHash.ContainsKey(content.Class)) {
					ShowPad(content);
				}
			}
			foreach (DockPane pane in dockPanel.Panes) {
				PadContentWrapper pad = pane.ActiveContent as PadContentWrapper;
				if (pad != null) {
					pad.ActivateContent();
				}
			}
		}
		void ShowViewContents()
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				ShowView(content);
			}
		}
		
		void DockPanelActiveContentChanged(object sender, EventArgs e)
		{
			PadContentWrapper padContent = dockPanel.ActiveContent as PadContentWrapper;
			if (padContent != null) {
				padContent.ActivateContent();
			}
		}
		public void ActivateVisiblePads()
		{
			foreach (PadContentWrapper content in contentHash.Values) {
				if (content.Visible) {
					content.ActivateContent();
				}
			}
		}
		void LoadLayoutConfiguration()
		{
			try {
				if (File.Exists(LayoutConfiguration.CurrentLayoutFileName)) {
					dockPanel.LoadFromXml(LayoutConfiguration.CurrentLayoutFileName, new DeserializeDockContent(GetContent));
				} else {
					LoadDefaultLayoutConfiguration();
				}
			} catch {
				// ignore errors loading configuration
			}
			ActivateVisiblePads();
		}
		
		void LoadDefaultLayoutConfiguration()
		{
			if (File.Exists(LayoutConfiguration.CurrentLayoutTemplateFileName)) {
				dockPanel.LoadFromXml(LayoutConfiguration.CurrentLayoutTemplateFileName, new DeserializeDockContent(GetContent));
			}
		}
		
		void ShowToolBars()
		{
			DefaultWorkbench wb = (DefaultWorkbench)wbForm;
			if (wb.ToolBars != null) {
				foreach (ToolStrip toolBar in wb.ToolBars) {
					if (!toolStripContainer.ContentPanel.Controls.Contains(toolBar)) {
						toolStripContainer.ContentPanel.Controls.Add(toolBar);
					}
				}
			}
		}
		
		void HideToolBars()
		{
			// TODO: Implement HIDE TOOLBARS.
//			DefaultWorkbench wb = (DefaultWorkbench)wbForm;
//			if (wb.ToolStripManager.ToolStrips.Count != 1) {
//				wb.ToolStripManager.ToolStrips.Clear();
//				wb.ToolStripManager.ToolStrips.Add(wb.TopMenu);
//			}
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
		
		[System.Runtime.InteropServices.DllImport("User32.dll")]
		public static extern bool LockWindowUpdate(IntPtr hWnd);
		
		public void LoadConfiguration()
		{
			if (dockPanel != null) {
				LockWindowUpdate(wbForm.Handle);
				try {
					dockPanel.ActiveDocumentChanged -= new EventHandler(ActiveMdiChanged);
					
					DetachPadContents(true);
					DetachViewContents(true);
					dockPanel.ActiveDocumentChanged += new EventHandler(ActiveMdiChanged);
					
					LoadLayoutConfiguration();
					ShowPads();
					ShowViewContents();
				} finally {
					LockWindowUpdate(IntPtr.Zero);
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
						dockPanel.SaveAsXml(Path.Combine(configPath, current.FileName));
					}
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		void DetachPadContents(bool dispose)
		{
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
			bool        isInitialized = false;
			
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
			
			public void ActivateContent()
			{
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
			if (content.Icon != null) {
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
		
		public void ActivatePad(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				contentHash[padContent.Class].ActivateContent();
				contentHash[padContent.Class].Show();
			}
		}
		public void ActivatePad(string fullyQualifiedTypeName)
		{
			contentHash[fullyQualifiedTypeName].ActivateContent();
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
			
			if (PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", true)) {
				ShowToolBars();
			} else {
				HideToolBars();
			}
		}
		
		public void CloseWindowEvent(object sender, EventArgs e)
		{
			SdiWorkspaceWindow f = (SdiWorkspaceWindow)sender;
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
		
		IWorkbenchWindow oldSelectedWindow = null;
		public virtual void OnActiveWorkbenchWindowChanged(EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
			if (oldSelectedWindow != null) {
				oldSelectedWindow.OnWindowDeselected(EventArgs.Empty);
			}
			oldSelectedWindow = ActiveWorkbenchwindow;
			if (oldSelectedWindow != null && oldSelectedWindow.ActiveViewContent != null && oldSelectedWindow.ActiveViewContent.Control != null) {
				oldSelectedWindow.OnWindowSelected(EventArgs.Empty);
				oldSelectedWindow.ActiveViewContent.SwitchedTo();
				oldSelectedWindow.ActiveViewContent.Control.Select();

			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
