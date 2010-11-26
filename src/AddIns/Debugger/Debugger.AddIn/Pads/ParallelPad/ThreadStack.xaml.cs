// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Pads.ParallelPad
{
	public partial class ThreadStack : UserControl
	{
		public static SolidColorBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(84, 169, 255));
		
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ThreadStack),
			                            new FrameworkPropertyMetadata());
		
		
		public event EventHandler FrameSelected;
		
		private ObservableCollection<ExpandoObject> itemCollection = new ObservableCollection<ExpandoObject>();
		
		private ToolTip toolTip = new ToolTip();
		
		public ThreadStack()
		{
			InitializeComponent();
			ToolTip = toolTip;
			ToolTipOpening += new ToolTipEventHandler(OnToolTipOpening);
		}
		
		public Process Process { get; set; }
		
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set {
				if (value) {
					BorderParent.BorderBrush = SelectedBrush;
					BorderParent.BorderThickness = new Thickness(5);
				}
				else {
					BorderParent.BorderBrush = Brushes.Black;
					BorderParent.BorderThickness = new Thickness(3);
				}
				
				SetValue(IsSelectedProperty, value);
				
				SelectParent(value);
			}
		}
		
		public ThreadStack ThreadStackParent { get; set; }
		
		public ThreadStack[] ThreadStackChildren { get; set; }
		
		public List<uint> ThreadIds { get; set; }
		
		public ObservableCollection<ExpandoObject> ItemCollection {
			get {
				return itemCollection;
			}
			
			set {
				itemCollection = value;
				this.datagrid.ItemsSource = itemCollection;
				
				if (ThreadIds.Count > 1)
					this.HeaderText.Text = ThreadIds.Count.ToString() + " Threads";
				else
					this.HeaderText.Text = "1 Thread";
			}
		}
		
		public void ClearImages()
		{
			foreach(dynamic item in itemCollection) {
				item.Image = null;
			}
		}
		
		void SelectParent(bool isSelected)
		{
			var ts = this.ThreadStackParent;
			while(ts != null) {
				ts.IsSelected = isSelected;
				ts = ts.ThreadStackParent;
			}
		}
		
		void Datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (Process.IsRunning) return;
			
			dynamic selectedItem = datagrid.SelectedItem;
			if (selectedItem != null) {
				if (ThreadIds.Count > 1) {
					datagrid.ContextMenu = CreateContextMenu(selectedItem);
					datagrid.ContextMenu.IsOpen = true;
				}
				else
				{
					SelectFrame(ThreadIds[0], selectedItem);
				}
			}
		}
		
		void SelectFrame(uint threadId, ExpandoObject selectedItem)
		{
			if (selectedItem == null)
				return;
			
			var thread = Process.Threads.Find(t => t.ID == threadId);
			if (thread == null)
				return;

			if (FrameSelected != null)
				FrameSelected(this, EventArgs.Empty);
			
			this.IsSelected = true;
			
			dynamic obj = selectedItem;
			Process.SelectedThread = thread;
			foreach(var frame in thread.Callstack)
			{
				if (frame.GetMethodName() == obj.MethodName)
				{
					Process.SelectedThread.SelectedStackFrame = frame;
					obj.Image = PresentationResourceService.GetImage("Bookmarks.CurrentLine").Source;
					((WindowsDebugger)DebuggerService.CurrentDebugger).JumpToCurrentLine();
					break;
				}
			}
		}
		
		void Datagrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (Process.IsRunning) return;
			
			dynamic selectedItem = datagrid.SelectedItem;
			if (selectedItem == null)
				return;
			
			datagrid.ContextMenu = CreateContextMenu(selectedItem);
			datagrid.ContextMenu.IsOpen = true;
		}
		
		ContextMenu CreateContextMenu(ExpandoObject item)
		{
			dynamic obj = item;
			
			var menu = new ContextMenu();
			foreach (var id in ThreadIds)
			{
				MenuItem m = new MenuItem();
				m.IsCheckable = true;
				m.IsChecked = id == Process.SelectedThread.ID;
				m.Click += delegate(object sender, RoutedEventArgs e) {
					var menuItem = e.OriginalSource as MenuItem;
					SelectFrame((uint)menuItem.Tag, item);
				};
				m.Tag = id;
				m.Header = id.ToString() + ":" + obj.MethodName;
				
				menu.Items.Add(m);
			}
			
			return menu;
		}
		
		void OnToolTipOpening(object sender, ToolTipEventArgs e)
		{
			StackPanel panel = new StackPanel();
			
			dynamic selectedItem = datagrid.SelectedItem;
			if (selectedItem == null) {
				panel.Children.Add(new TextBlock { Text = "No item selected" });
				this.toolTip.Content = panel;
				return;
			}
			
			foreach(var thread in Process.Threads)
			{
				if (ThreadIds.Contains(thread.ID))
				{
					foreach (var frame in thread.Callstack)
					{
						if (selectedItem.MethodName == frame.GetMethodName())
						{
							// TODO : get method parameter values
							TextBlock tb = new TextBlock();
							tb.Text = thread.ID + ": " + CallStackPadContent.GetFullName(frame);
							panel.Children.Add(tb);
						}
					}
				}
			}
			
			this.toolTip.Content = panel;
		}
	}
}