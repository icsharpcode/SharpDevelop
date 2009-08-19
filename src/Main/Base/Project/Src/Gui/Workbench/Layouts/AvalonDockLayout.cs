// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		
		#if DEBUG
		internal void WriteState(TextWriter output)
		{
			
		}
		#endif
		
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
				UpdateActiveWorkbenchWindow();
				if (ActiveContentChanged != null)
					ActiveContentChanged(this, e);
				CommandManager.InvalidateRequerySuggested();
			} else if (e.PropertyName == "ActiveDocument") {
				UpdateActiveWorkbenchWindow();
				CommandManager.InvalidateRequerySuggested();
			}
		}
		
		public event EventHandler ActiveContentChanged;
		
		public IWorkbenchWindow ActiveWorkbenchWindow { get; private set; }
		
		void UpdateActiveWorkbenchWindow()
		{
			IWorkbenchWindow window = dockingManager.ActiveDocument as IWorkbenchWindow;
			if (window != null)
				this.ActiveWorkbenchWindow = window;
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
			} finally {
				Busy = false;
			}
			LoadConfiguration();
		}
		
		public void Detach()
		{
			StoreConfiguration();
			this.workbench.mainContent.Content = null;
		}
		
		Dictionary<PadDescriptor, AvalonPadContent> pads = new Dictionary<PadDescriptor, AvalonPadContent>();
		Dictionary<string, AvalonPadContent> padsByClass = new Dictionary<string, AvalonPadContent>();
		
		public void ShowPad(PadDescriptor padDescriptor)
		{
			AvalonPadContent pad;
			if (pads.TryGetValue(padDescriptor, out pad)) {
				dockingManager.Show(pad);
			} else {
				LoggingService.Debug("Add pad " + padDescriptor.Class + " at " + padDescriptor.DefaultPosition);
				
				pad = new AvalonPadContent(this, padDescriptor);
				pads.Add(padDescriptor, pad);
				padsByClass.Add(padDescriptor.Class, pad);
				pad.ShowInDefaultPosition();
			}
		}
		
		public void ActivatePad(PadDescriptor padDescriptor)
		{
			ShowPad(padDescriptor);
		}
		
		public void HidePad(PadDescriptor padDescriptor)
		{
			AvalonPadContent p;
			if (pads.TryGetValue(padDescriptor, out p))
				dockingManager.Hide(p);
		}
		
		public void UnloadPad(PadDescriptor padDescriptor)
		{
			AvalonPadContent p = pads[padDescriptor];
			dockingManager.Hide(p);
			DockablePane pane = p.Parent as DockablePane;
			if (pane != null)
				pane.Items.Remove(p);
			p.Dispose();
		}
		
		public bool IsVisible(PadDescriptor padDescriptor)
		{
			AvalonPadContent p;
			if (pads.TryGetValue(padDescriptor, out p))
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
			if (this.ActiveWorkbenchWindow == sender) {
				this.ActiveWorkbenchWindow = null;
				UpdateActiveWorkbenchWindow();
			}
		}
		
		public void LoadConfiguration()
		{
			if (!dockingManager.IsLoaded)
				return;
			Busy = true;
			try {
				bool isPlainLayout = LayoutConfiguration.CurrentLayoutName == "Plain";
				if (File.Exists(LayoutConfiguration.CurrentLayoutFileName)) {
					LoadLayout(LayoutConfiguration.CurrentLayoutFileName, isPlainLayout);
				} else if (File.Exists(LayoutConfiguration.CurrentLayoutTemplateFileName)) {
					LoadLayout(LayoutConfiguration.CurrentLayoutTemplateFileName, isPlainLayout);
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
				// ignore errors loading configuration
			} finally {
				Busy = false;
			}
			foreach (AvalonPadContent p in pads.Values) {
				p.LoadPadContentIfRequired();
			}
		}
		
		void LoadLayout(string fileName, bool hideAllLostPads)
		{
			LoggingService.Info("Loading layout file: " + fileName + ", hideAllLostPads=" + hideAllLostPads);
			DockableContent[] oldContents = dockingManager.DockableContents;
			dockingManager.RestoreLayout(fileName);
			DockableContent[] newContents = dockingManager.DockableContents;
			// Restoring a AvalonDock layout will remove pads that are not
			// stored in the layout file.
			// We'll re-add those lost pads.
			foreach (DockableContent lostContent in oldContents.Except(newContents)) {
				AvalonPadContent padContent = lostContent as AvalonPadContent;
				LoggingService.Debug("Re-add lost pad: " + padContent);
				if (padContent != null && !hideAllLostPads) {
					padContent.ShowInDefaultPosition();
				} else {
					dockingManager.Hide(lostContent);
				}
			}
		}
		
		public void StoreConfiguration()
		{
			try {
				LayoutConfiguration current = LayoutConfiguration.CurrentLayout;
				if (current != null && !current.ReadOnly) {
					string configPath = LayoutConfiguration.ConfigLayoutPath;
					Directory.CreateDirectory(configPath);
					string fileName = Path.Combine(configPath, current.FileName);
					LoggingService.Info("Saving layout file: " + fileName);
					dockingManager.SaveLayout(fileName);
				}
			} catch (Exception e) {
				MessageService.ShowException(e);
			}
		}
	}
}
