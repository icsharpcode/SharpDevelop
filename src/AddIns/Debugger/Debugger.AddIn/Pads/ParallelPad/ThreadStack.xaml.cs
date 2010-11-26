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

namespace Debugger.AddIn.Pads.ParallelPad
{
	public partial class ThreadStack : UserControl
	{
		public static SolidColorBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(84, 169, 255));
		
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ThreadStack),
			                            new FrameworkPropertyMetadata());
		
		private ObservableCollection<ExpandoObject> itemCollection = new ObservableCollection<ExpandoObject>();
		
		public ThreadStack()
		{
			InitializeComponent();
		}
		
		internal bool IsAdded { get; set; }
		
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
		
		private void SelectParent(bool isSelected)
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
			var thread = Process.Threads.Find(t => t.ID == threadId);
			if (thread == null)
				return;
			dynamic obj = selectedItem;
			Process.SelectedThread = thread;
			foreach(var frame in thread.Callstack)
			{
				if (frame.GetMethodName() == obj.MethodName)
				{
					Process.SelectedThread.SelectedStackFrame = frame;
					break;
				}
			}
			
			Process.OnPaused();
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
				m.Checked += delegate(object sender, RoutedEventArgs e) {
					var menuItem = e.OriginalSource as MenuItem;
					SelectFrame((uint)menuItem.Tag, item);
				};
				m.Tag = id;
				m.Header = id.ToString() + ":" + obj.MethodName;
				
				menu.Items.Add(m);
			}
			
			return menu;
		}
	}
}