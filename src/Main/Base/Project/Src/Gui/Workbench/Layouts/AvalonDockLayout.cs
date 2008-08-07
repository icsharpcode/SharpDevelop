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
		
		public DockingManager DockingManager {
			get { return dockingManager; }
		}
		
		public DocumentPane DocumentPane {
			get { return documentPane; }
		}
		
		public AvalonDockLayout()
		{
			dockingManager.Content = documentPane;
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				return null;
			}
		}
		
		public object ActiveContent {
			get {
				return null;
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			this.workbench = (WpfWorkbench)workbench;
			this.workbench.mainContent.Content = dockingManager;
		}
		
		public void Detach()
		{
			this.workbench.mainContent.Content = null;
		}
		
		public void ShowPad(PadDescriptor content)
		{
			AvalonPadContent pad = new AvalonPadContent(content);
			GetPane(Dock.Right).Items.Add(pad);
			dockingManager.Show(pad, DockableContentState.Docked);
		}
		
		DockablePane GetPane(Dock dockPosition)
		{
			List<DockablePane> allPanes = new List<DockablePane>();
			ListAllPanes(allPanes, dockingManager.Content);
			// try to find an existing pane
			DockablePane pane = allPanes.Find(p => DetectDock(p) == dockPosition);
			if (pane == null) {
				// none found: create a new pane
				pane = new DockablePane();
				UIElement content = (UIElement)dockingManager.Content;
				ResizingPanel rp = new ResizingPanel();
				dockingManager.Content = rp;
				if (dockPosition == Dock.Left || dockPosition == Dock.Right) {
					rp.Orientation = Orientation.Horizontal;
				} else {
					rp.Orientation = Orientation.Vertical;
				}
				if (dockPosition == Dock.Left || dockPosition == Dock.Top) {
					rp.Children.Add(pane);
					rp.Children.Add(content);
				} else {
					rp.Children.Add(content);
					rp.Children.Add(pane);
				}
			}
			return pane;
		}
		
		static Dock? DetectDock(DockablePane pane)
		{
			ResizingPanel rp = pane.Parent as ResizingPanel;
			if (rp != null) {
				if (rp.Children[0] == pane) {
					return rp.Orientation == Orientation.Vertical ? Dock.Top : Dock.Left;
				} else if (rp.Children[rp.Children.Count - 1] == pane) {
					return rp.Orientation == Orientation.Vertical ? Dock.Bottom : Dock.Right;
				}
			}
			return null;
		}
		
		static void ListAllPanes(List<DockablePane> list, object content)
		{
			if (content is DockablePane) {
				list.Add((DockablePane)content);
			} else if (content is ResizingPanel) {
				ResizingPanel rp = (ResizingPanel)content;
				foreach (object o in rp.Children)
					ListAllPanes(list, o);
			}
		}
		
		public void ActivatePad(PadDescriptor content)
		{
		}
		
		public void ActivatePad(string fullyQualifiedTypeName)
		{
		}
		
		public void HidePad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void UnloadPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public bool IsVisible(PadDescriptor padContent)
		{
			return false;
		}
		
		public void RedrawAllComponents()
		{
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			AvalonWorkbenchWindow window = new AvalonWorkbenchWindow(this);
			window.ViewContents.Add(content);
			documentPane.Items.Add(window);
			dockingManager.Show(window);
			return window;
		}
		
		public void LoadConfiguration()
		{
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
