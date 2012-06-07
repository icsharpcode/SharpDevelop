// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			dockingManager.PropertyChanged += dockingManager_PropertyChanged;
			dockingManager.Loaded += dockingManager_Loaded;
		}
		
		#if DEBUG
		internal void WriteState(TextWriter output)
		{
			output.WriteLine("AvalonDock: ActiveContent = " + WpfWorkbench.GetElementName(dockingManager.ActiveContent));
			output.WriteLine("AvalonDock: ActiveDocument = " + WpfWorkbench.GetElementName(dockingManager.ActiveDocument));
		}
		#endif
		
		void dockingManager_Loaded(object sender, RoutedEventArgs e)
		{
			// LoadConfiguration doesn't do anything until the docking manager is loaded,
			// so we have to load the configuration now
			LoggingService.Info("dockingManager_Loaded");
			LoadConfiguration();
			EnsureFloatingWindowsLocations();
		}
		
		void EnsureFloatingWindowsLocations()
		{
			foreach (var window in dockingManager.FloatingWindows) {
				var newLocation = FormLocationHelper.Validate(new Rect(window.Left, window.Top, window.Width, window.Height).TransformToDevice(window).ToSystemDrawing()).ToWpf().TransformFromDevice(window);
				window.Left = newLocation.Left;
				window.Top = newLocation.Top;
			}
		}
		
		void dockingManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActiveContent") {
				WpfWorkbench.FocusDebug("AvalonDock: ActiveContent changed to {0}", WpfWorkbench.GetElementName(dockingManager.ActiveContent));
				if (ActiveContentChanged != null)
					ActiveContentChanged(this, e);
				CommandManager.InvalidateRequerySuggested();
			} else if (e.PropertyName == "ActiveDocument") {
				WpfWorkbench.FocusDebug("AvalonDock: ActiveDocument changed to {0}", WpfWorkbench.GetElementName(dockingManager.ActiveDocument));
				
				if (ActiveWorkbenchWindowChanged != null)
					ActiveWorkbenchWindowChanged(this, e);
				CommandManager.InvalidateRequerySuggested();
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				return dockingManager.ActiveDocument as IWorkbenchWindow;
			}
		}
		
		public event EventHandler ActiveContentChanged;
		
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
			CommandManager.AddCanExecuteHandler(this.workbench, OnCanExecuteRoutedCommand);
			CommandManager.AddExecutedHandler(this.workbench, OnExecuteRoutedCommand);
			Busy = true;
			try {
				foreach (PadDescriptor pd in workbench.PadContentCollection) {
					ShowPad(pd);
				}
			} finally {
				Busy = false;
			}
			LoadConfiguration();
			EnsureFloatingWindowsLocations();
		}
		
		public void Detach()
		{
			StoreConfiguration();
			this.workbench.mainContent.Content = null;
			CommandManager.RemoveCanExecuteHandler(this.workbench, OnCanExecuteRoutedCommand);
			CommandManager.RemoveExecutedHandler(this.workbench, OnExecuteRoutedCommand);
		}

		bool isInNestedCanExecute;

		// Custom command routing:
		// if the command isn't handled on the current focus, try to execute it on the focus inside the active workbench window
		void OnCanExecuteRoutedCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			workbench.VerifyAccess();
			RoutedCommand routedCommand = e.Command as RoutedCommand;
			AvalonWorkbenchWindow workbenchWindow = ActiveWorkbenchWindow as AvalonWorkbenchWindow;
			if (!e.Handled && routedCommand != null && workbenchWindow != null && !isInNestedCanExecute) {
				IInputElement target = workbenchWindow.GetCommandTarget();
				if (target != null && target != e.OriginalSource) {
					isInNestedCanExecute = true;
					try {
						e.CanExecute = routedCommand.CanExecute(e.Parameter, target);
					} finally {
						isInNestedCanExecute = false;
					}
					e.Handled = true;
				}
			}
		}

		bool isInNestedExecute;
		
		void OnExecuteRoutedCommand(object sender, ExecutedRoutedEventArgs e)
		{
			workbench.VerifyAccess();
			RoutedCommand routedCommand = e.Command as RoutedCommand;
			AvalonWorkbenchWindow workbenchWindow = ActiveWorkbenchWindow as AvalonWorkbenchWindow;
			if (!e.Handled && routedCommand != null && workbenchWindow != null && !isInNestedExecute) {
				IInputElement target = workbenchWindow.GetCommandTarget();
				if (target != null && target != e.OriginalSource) {
					isInNestedExecute = true;
					try {
						routedCommand.Execute(e.Parameter, target);
					} finally {
						isInNestedExecute = false;
					}
					e.Handled = true;
				}
			}
		}
		
		Dictionary<PadDescriptor, AvalonPadContent> pads = new Dictionary<PadDescriptor, AvalonPadContent>();
		Dictionary<string, AvalonPadContent> padsByClass = new Dictionary<string, AvalonPadContent>();
		
		public void ShowPad(PadDescriptor padDescriptor)
		{
			AvalonPadContent pad;
			if (pads.TryGetValue(padDescriptor, out pad)) {
				pad.Show(dockingManager);
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
			AvalonPadContent p;
			if (pads.TryGetValue(padDescriptor, out p)) {
				if (!p.IsVisible)
					p.Show();
				p.Activate();
			} else {
				ShowPad(padDescriptor);
			}
		}
		
		public void HidePad(PadDescriptor padDescriptor)
		{
			AvalonPadContent p;
			if (pads.TryGetValue(padDescriptor, out p))
				p.Hide();
		}
		
		public void UnloadPad(PadDescriptor padDescriptor)
		{
			AvalonPadContent p = pads[padDescriptor];
			p.Hide();
			DockablePane pane = p.Parent as DockablePane;
			if (pane != null)
				pane.Items.Remove(p);
			p.Dispose();
		}
		
		public bool IsVisible(PadDescriptor padDescriptor)
		{
			AvalonPadContent p;
			if (pads.TryGetValue(padDescriptor, out p))
				return p.State != DockableContentState.Hidden;
			else
				return false;
		}
		
		public IWorkbenchWindow ShowView(IViewContent content, bool switchToOpenedView)
		{
			AvalonWorkbenchWindow window = new AvalonWorkbenchWindow(this);
			workbenchWindows.Add(window);
			window.ViewContents.Add(content);
			window.ViewContents.AddRange(content.SecondaryViewContents);
			window.Show(dockingManager);
			if (switchToOpenedView) {
				window.Activate();
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
			Busy = true;
			try {
				TryLoadConfiguration();
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
		
		void TryLoadConfiguration()
		{
			bool isPlainLayout = LayoutConfiguration.CurrentLayoutName == "Plain";
			if (File.Exists(LayoutConfiguration.CurrentLayoutFileName)) {
				try {
					LoadLayout(LayoutConfiguration.CurrentLayoutFileName, isPlainLayout);
					return;
				} catch (FileFormatException) {
					// error when version of AvalonDock has changed: ignore and load template instead
				}
			}
			if (File.Exists(LayoutConfiguration.CurrentLayoutTemplateFileName)) {
				LoadLayout(LayoutConfiguration.CurrentLayoutTemplateFileName, isPlainLayout);
			}
		}
		
		void LoadLayout(string fileName, bool hideAllLostPads)
		{
			LoggingService.Info("Loading layout file: " + fileName + ", hideAllLostPads=" + hideAllLostPads);
//			DockableContent[] oldContents = dockingManager.DockableContents;
			dockingManager.RestoreLayout(fileName);
//			DockableContent[] newContents = dockingManager.DockableContents;
			// Restoring a AvalonDock layout will remove pads that are not
			// stored in the layout file.
			// We'll re-add those lost pads.
//			foreach (DockableContent lostContent in oldContents.Except(newContents)) {
//				AvalonPadContent padContent = lostContent as AvalonPadContent;
//				LoggingService.Debug("Re-add lost pad: " + padContent);
//				if (padContent != null && !hideAllLostPads) {
//					padContent.ShowInDefaultPosition();
//				} else {
//					dockingManager.Hide(lostContent);
//				}
//			}
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
					// Save docking layout into memory stream first, then write the contents to file.
					// This prevents corruption when there is an exception saving the layout.
					var memoryStream = new MemoryStream();
					dockingManager.SaveLayout(memoryStream);
					memoryStream.Position = 0;
					try {
						using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
							memoryStream.CopyTo(stream);
					} catch (IOException ex) {
						// ignore IO errors (maybe switching layout in two SharpDevelop instances at once?)
						LoggingService.Warn(ex);
					}
				}
			} catch (Exception e) {
				MessageService.ShowException(e);
			}
		}
	}
}
