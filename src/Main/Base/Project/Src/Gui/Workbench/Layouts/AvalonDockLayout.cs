// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using AvalonDock;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Workbench layout using the AvalonDock library.
	/// </summary>
	sealed class AvalonDockLayout : IWorkbenchLayout
	{
		WpfWorkbench workbench;
		DockingManager dockingManager = new DockingManager();
		DocumentPane documentPane = new DocumentPane();
		List<IWorkbenchWindow> workbenchWindows = new List<IWorkbenchWindow>();
		internal bool Busy;
		
		public WpfWorkbench Workbench {
			get { return workbench; }
		}
		
		public DockingManager DockingManager {
			get { return dockingManager; }
		}
		
		public AvalonDockLayout()
		{
			dockingManager.Content = documentPane;
			dockingManager.PropertyChanged += dockingManager_PropertyChanged;
			dockingManager.Loaded += dockingManager_Loaded;
		}
		
		void dockingManager_Loaded(object sender, RoutedEventArgs e)
		{
			// LoadConfiguration doesn't do anything until the docking manager is loaded,
			// so we have to load the configuration now
			LoggingService.Info("dockingManager_Loaded");
			LoadConfiguration();
		}
		
		void dockingManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActiveContent") {
				if (ActiveContentChanged != null)
					ActiveContentChanged(this, e);
				CommandManager.InvalidateRequerySuggested();
			}
		}
		
		public event EventHandler ActiveContentChanged;
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				return (AvalonWorkbenchWindow)dockingManager.ActiveDocument;
			}
		}
		
		public object ActiveContent {
			get {
				object activeContent = dockingManager.ActiveContent;
				AvalonPadContent padContent = activeContent as AvalonPadContent;
				if (padContent != null)
					return padContent.PadContent;
				AvalonWorkbenchWindow window = activeContent as AvalonWorkbenchWindow;
				if (window != null)
					return window.ActiveViewContent;
				return null;
			}
		}
		
		public IList<IWorkbenchWindow> WorkbenchWindows {
			get {
				return workbenchWindows.AsReadOnly();
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			if (this.workbench != null)
				throw new InvalidOperationException("Can attach only once!");
			this.workbench = (WpfWorkbench)workbench;
			this.workbench.mainContent.Content = dockingManager;
			Busy = true;
			try {
				foreach (PadDescriptor pd in workbench.PadContentCollection) {
					ShowPad(pd);
				}
				LoadConfiguration();
			} finally {
				Busy = false;
			}
			foreach (AvalonPadContent p in pads.Values) {
				p.LoadPadContentIfRequired();
			}
		}
		
		public void Detach()
		{
			StoreConfiguration();
			this.workbench.mainContent.Content = null;
		}
		
		Dictionary<PadDescriptor, AvalonPadContent> pads = new Dictionary<PadDescriptor, AvalonPadContent>();
		Dictionary<string, AvalonPadContent> padsByClass = new Dictionary<string, AvalonPadContent>();
		
		public void ShowPad(PadDescriptor content)
		{
			AvalonPadContent pad;
			if (pads.TryGetValue(content, out pad)) {
				dockingManager.Show(pad);
			} else {
				pad = new AvalonPadContent(this, content);
				pads.Add(content, pad);
				padsByClass.Add(content.Class, pad);
				AnchorStyle style;
				if ((content.DefaultPosition & DefaultPadPositions.Top) != 0)
					style = AnchorStyle.Top;
				else if ((content.DefaultPosition & DefaultPadPositions.Left) != 0)
					style = AnchorStyle.Left;
				else if ((content.DefaultPosition & DefaultPadPositions.Bottom) != 0)
					style = AnchorStyle.Bottom;
				else
					style = AnchorStyle.Right;
				dockingManager.Show(pad, DockableContentState.Docked, style);
				SetPaneSizeWorkaround(pad.ContainerPane);
				if ((content.DefaultPosition & DefaultPadPositions.Hidden) != 0)
					dockingManager.Hide(pad);
			}
		}
		
		static void SetPaneSizeWorkaround(Pane pane)
		{
			ResizingPanel panel = pane.Parent as ResizingPanel;
			if (panel.Orientation == Orientation.Horizontal) {
				if (ResizingPanel.GetResizeWidth(pane) == 0)
					ResizingPanel.SetResizeWidth(pane, 200);
			} else if (panel.Orientation == Orientation.Vertical) {
				if (ResizingPanel.GetResizeHeight(pane) == 0)
					ResizingPanel.SetResizeHeight(pane, 150);
			}
		}
		
		public void ActivatePad(PadDescriptor content)
		{
			ShowPad(content);
		}
		
		public void HidePad(PadDescriptor content)
		{
			AvalonPadContent p;
			if (pads.TryGetValue(content, out p))
				dockingManager.Hide(p);
		}
		
		public void UnloadPad(PadDescriptor content)
		{
			AvalonPadContent p = pads[content];
			dockingManager.Hide(p);
			DockablePane pane = p.Parent as DockablePane;
			if (pane != null)
				pane.Items.Remove(p);
			p.Dispose();
		}
		
		public bool IsVisible(PadDescriptor padContent)
		{
			AvalonPadContent p;
			if (pads.TryGetValue(padContent, out p))
				return p.IsVisible;
			else
				return false;
		}
		
		public IWorkbenchWindow ShowView(IViewContent content, bool switchToOpenedView)
		{
			AvalonWorkbenchWindow window = new AvalonWorkbenchWindow(this);
			workbenchWindows.Add(window);
			window.ViewContents.Add(content);
			window.ViewContents.AddRange(content.SecondaryViewContents);
			documentPane.Items.Add(window);
			if (switchToOpenedView) {
				dockingManager.Show(window);
			}
			window.Closed += window_Closed;
			return window;
		}
		
		void window_Closed(object sender, EventArgs e)
		{
			workbenchWindows.Remove((IWorkbenchWindow)sender);
		}
		
		public void LoadConfiguration()
		{
			if (!dockingManager.IsLoaded)
				return;
			try {
				if (File.Exists(LayoutConfiguration.CurrentLayoutFileName)) {
					dockingManager.RestoreLayout(LayoutConfiguration.CurrentLayoutFileName);
				} else {
					LoadDefaultLayoutConfiguration();
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
				// ignore errors loading configuration
			}
		}
		
		void LoadDefaultLayoutConfiguration()
		{
			if (File.Exists(LayoutConfiguration.CurrentLayoutTemplateFileName)) {
				dockingManager.RestoreLayout(LayoutConfiguration.CurrentLayoutTemplateFileName);
			}
		}
		
		public void StoreConfiguration()
		{
			try {
				LayoutConfiguration current = LayoutConfiguration.CurrentLayout;
				if (current != null && !current.ReadOnly) {
					string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
					Directory.CreateDirectory(configPath);
					dockingManager.SaveLayout(Path.Combine(configPath, current.FileName));
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
	}
}
