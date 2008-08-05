// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Workbench implementation using WPF and AvalonDock.
	/// </summary>
	sealed class WpfWorkbench : Window, IWorkbench
	{
		const string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";
		const string viewContentPath = "/SharpDevelop/Workbench/Pads";
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		public event EventHandler ActiveViewContentChanged;
		public event EventHandler ActiveContentChanged;
		public event ViewContentEventHandler ViewOpened;
		public event ViewContentEventHandler ViewClosed;
		public event System.Windows.Forms.KeyEventHandler ProcessCommandKey;
		
		public System.Windows.Forms.IWin32Window MainWin32Window { get; private set; }
		public ISynchronizeInvoke SynchronizingObject { get; set; }
		public Window MainWindow { get { return this; } }
		
		DockPanel dockPanel;
		Menu mainMenu;
		ContentControl mainContent;
		
		public WpfWorkbench()
		{
			this.Title = "SharpDevelop (experimental WPF build)";
			this.SynchronizingObject = new WpfSynchronizeInvoke(this.Dispatcher);
			this.MainWin32Window = this.GetWin32Window();
			this.WindowStartupLocation = WindowStartupLocation.Manual;
		}
		
		public void Initialize()
		{
			this.Content = dockPanel = new DockPanel();
			mainMenu = new Menu() {
				ItemsSource = MenuService.CreateMenuItems(this, mainMenuPath)
			};
			DockPanel.SetDock(mainMenu, Dock.Top);
			dockPanel.Children.Add(mainMenu);
			
			mainContent = new ContentControl();
			dockPanel.Children.Add(mainContent);
		}
		
		public ICollection<IViewContent> ViewContentCollection {
			get {
				return new IViewContent[0];
			}
		}
		
		public IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get {
				return new IWorkbenchWindow[0];
			}
		}
		
		public IList<PadDescriptor> PadContentCollection {
			get {
				return new PadDescriptor[0];
			}
		}
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				return null;
			}
		}
		
		public IViewContent ActiveViewContent {
			get {
				return null;
			}
		}
		
		public object ActiveContent {
			get {
				return null;
			}
		}
		
		public IWorkbenchLayout WorkbenchLayout { get; set; }
		
		public bool IsActiveWindow {
			get {
				return IsActive;
			}
		}
		
		public void ShowView(IViewContent content)
		{
		}
		
		public void ShowPad(PadDescriptor content)
		{
		}
		
		public void UnloadPad(PadDescriptor content)
		{
		}
		
		public PadDescriptor GetPad(Type type)
		{
			return null;
		}
		
		public void CloseContent(IViewContent content)
		{
		}
		
		public void CloseAllViews()
		{
		}
		
		public void RedrawAllComponents()
		{
		}
		
		public void UpdateRenderer()
		{
		}
		
		public Properties CreateMemento()
		{
			Properties prop = new Properties();
			prop.Set("WindowState", this.WindowState);
			if (this.WindowState == System.Windows.WindowState.Normal) {
				prop.Set("Left", this.Left);
				prop.Set("Top", this.Top);
				prop.Set("Width", this.Width);
				prop.Set("Height", this.Height);
			}
			return prop;
		}
		
		public void SetMemento(Properties memento)
		{
			this.Left = memento.Get("Left", 10.0);
			this.Top = memento.Get("Top", 10.0);
			this.Width = memento.Get("Width", 600.0);
			this.Height = memento.Get("Height", 400.0);
			this.WindowState = memento.Get("WindowState", System.Windows.WindowState.Maximized);
		}
	}
}
