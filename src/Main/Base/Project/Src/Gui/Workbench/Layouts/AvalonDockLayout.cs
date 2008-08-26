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
		
		public DockingManager DockingManager {
			get { return dockingManager; }
		}
		
		public AvalonDockLayout()
		{
			dockingManager.Content = documentPane;
			dockingManager.PropertyChanged += dockingManager_PropertyChanged;
		}
		
		public void OnWorkbenchWindowClosed(AvalonWorkbenchWindow window)
		{
			workbenchWindows.Remove(window);
			documentPane.Items.Remove(window);
		}
		
		void dockingManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActiveContent") {
				CommandManager.InvalidateRequerySuggested();
				if (ActiveContentChanged != null)
					ActiveContentChanged(this, e);
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
				dockingManager.Show(pad, DockableContentState.Docked, AnchorStyle.Right);
			}
		}
		
		public void ActivatePad(PadDescriptor content)
		{
			pads[content].BringIntoView();
		}
		
		public void ActivatePad(string fullyQualifiedTypeName)
		{
			padsByClass[fullyQualifiedTypeName].BringIntoView();
		}
		
		public void HidePad(PadDescriptor content)
		{
			dockingManager.Hide(pads[content]);
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
			return pads[padContent].IsVisible;
		}
		
		public void RedrawAllComponents()
		{
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			AvalonWorkbenchWindow window = new AvalonWorkbenchWindow(this);
			workbenchWindows.Add(window);
			window.ViewContents.Add(content);
			window.ViewContents.AddRange(content.SecondaryViewContents);
			documentPane.Items.Add(window);
			dockingManager.Show(window);
			return window;
		}
		
		public void LoadConfiguration()
		{
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
