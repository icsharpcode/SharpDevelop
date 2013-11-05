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
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace Debugger.AddIn.Pads.ParallelPad
{
	public class FrameSelectedEventArgs : EventArgs
	{
		public StackFrame Item {
			get;
			private set;
		}
		
		public Location Location {
			get;
			private set;
		}
		
		public FrameSelectedEventArgs(StackFrame item, Location location)
		{
			Item = item;
			Location = location;
		}
	}
	
	public partial class ThreadStack : UserControl
	{
		public static SolidColorBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(84, 169, 255));
		
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ThreadStack),
			                            new FrameworkPropertyMetadata());
		
		public event EventHandler StackSelected;
		
		public event EventHandler<FrameSelectedEventArgs> FrameSelected;
		
		ObservableCollection<ParallelStackFrameModel> itemCollection = new ObservableCollection<ParallelStackFrameModel>();
		
		ToolTip toolTip = new ToolTip();
		List<uint> threadIds = new List<uint>();
		
		public ThreadStack()
		{
			InitializeComponent();
			datagrid.ToolTip = toolTip;
			datagrid.ToolTipOpening += OnToolTipOpening;
			datagrid.PreviewMouseMove += new MouseEventHandler(datagrid_PreviewMouseMove);
			datagrid.MouseLeave += delegate { toolTip.IsOpen = false; };
		}
		
		#region Public Properties
		
		public Process Process { get; set; }
		
		public int Level { get; set; }
		
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
		
		public List<ThreadStack> ThreadStackParents { get; set; }
		
		public List<ThreadStack> ThreadStackChildren { get; set; }
		
		public List<uint> ThreadIds {
			get { return threadIds; }
		}
		
		public ObservableCollection<ParallelStackFrameModel> ItemCollection {
			get { return itemCollection; }
			set {
				itemCollection = value;
				this.datagrid.ItemsSource = itemCollection;
			}
		}
		
		#endregion
		
		#region Public Methods
		
		public void UpdateThreadIds(bool isTask, params uint[] threadIds)
		{
			var list = new List<uint>();
			foreach (uint id in threadIds) {
				if (!this.threadIds.Contains(id)) {
					list.Add(id);
				}
			}
			this.threadIds.AddRange(list);
			
			if (this.threadIds.Count > 1) {
				string suffix = isTask ? " Tasks" : " Threads";
				this.HeaderText.Text = this.threadIds.Count.ToString() + suffix;
			}
			else {
				this.HeaderText.Text = isTask ? "1 Task" : "1 Thread";
			}
		}
		
		public void ClearImages()
		{
			foreach (ParallelStackFrameModel item in itemCollection) {
				if (!item.IsRunningStackFrame)
					item.Image = null;
			}
		}
		
		#endregion
		
		#region Private Methods
		
		private void SelectParent(bool isSelected)
		{
			if (this.ThreadStackParents == null || this.ThreadStackParents.Count == 0)
				return;
			
			foreach (var ts in this.ThreadStackParents)
				if (ts != null)
					ts.IsSelected = isSelected;
		}
		
		void datagrid_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
			if (result != null)
			{
				var row = TryFindParent<DataGridRow>(result.VisualHit);
				if (row != null)
				{
					datagrid.SelectedItem = row.DataContext;
					e.Handled = true;
				}
			}
		}
		
		private void Datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (Process.IsRunning) return;
			
			ParallelStackFrameModel selectedItem = datagrid.SelectedItem as ParallelStackFrameModel;
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
		
		private void SelectFrame(uint threadId, ParallelStackFrameModel selectedItem)
		{
			if (selectedItem == null)
				return;
			
			var thread = Process.Threads.Find(t => t.ID == threadId);
			if (thread == null)
				return;

			if (StackSelected != null)
				StackSelected(this, EventArgs.Empty);
			
			this.IsSelected = true;
			
			foreach(var frame in thread.Callstack)
			{
				if (frame.GetMethodName() == selectedItem.MethodName)
				{
					if (!selectedItem.IsRunningStackFrame)
						selectedItem.Image = PresentationResourceService.GetImage("Icons.48x48.CurrentFrame").Source;
					
					SourcecodeSegment nextStatement = frame.NextStatement;
					if (nextStatement != null) {
						var location = new Location(nextStatement.StartColumn, nextStatement.StartLine);
						FileService.JumpToFilePosition(
							nextStatement.Filename, location.Line, location.Column);
						
						if (FrameSelected != null)
							FrameSelected(this, new FrameSelectedEventArgs(frame, location));
					}
					
					break;
				}
			}
		}
		
		private void Datagrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (Process.IsRunning) return;
			
			ParallelStackFrameModel selectedItem = datagrid.SelectedItem as ParallelStackFrameModel;
			if (selectedItem == null)
				return;
			
			datagrid.ContextMenu = CreateContextMenu(selectedItem);
			datagrid.ContextMenu.IsOpen = true;
		}
		
		ContextMenu CreateContextMenu(ParallelStackFrameModel item)
		{
			var menu = new ContextMenu();
			
			foreach (var id in ThreadIds) {
				MenuItem m = new MenuItem();
				m.IsCheckable = true;
				m.IsChecked = id == Process.SelectedThread.ID;
				m.Click += delegate(object sender, RoutedEventArgs e) {
					var menuItem = e.OriginalSource as MenuItem;
					SelectFrame((uint)menuItem.Tag, item);
				};
				m.Tag = id;
				m.Header = id.ToString() + ":" + item.MethodName;
				
				menu.Items.Add(m);
			}
			
			return menu;
		}
		
		private void OnToolTipOpening(object sender, ToolTipEventArgs e)
		{
			if (Process.IsRunning)
				return;
			
			StackPanel panel = new StackPanel();
			
			ParallelStackFrameModel selectedItem = datagrid.SelectedItem as ParallelStackFrameModel;
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
							TextBlock tb = new TextBlock();
							tb.Text = thread.ID + ": " + CallStackPadContent.GetFullName(frame);
							panel.Children.Add(tb);
						}
					}
				}
			}
			
			this.toolTip.Content = panel;
		}
		
		#endregion
		
		#region Static Methods
		
		private static T TryFindParent<T>(DependencyObject child) where T : DependencyObject
		{
			if (child is T) return child as T;

			DependencyObject parentObject = GetParentObject(child);
			if (parentObject == null) return null;

			var parent = parentObject as T;
			if (parent != null && parent is T)
			{
				return parent;
			}
			else
			{
				return TryFindParent<T>(parentObject);
			}
		}

		private static DependencyObject GetParentObject(DependencyObject child)
		{
			if (child == null) return null;

			ContentElement contentElement = child as ContentElement;
			if (contentElement != null)
			{
				DependencyObject parent = ContentOperations.GetParent(contentElement);
				if (parent != null) return parent;

				FrameworkContentElement fce = contentElement as FrameworkContentElement;
				return fce != null ? fce.Parent : null;
			}

			FrameworkElement frameworkElement = child as FrameworkElement;
			if (frameworkElement != null)
			{
				DependencyObject parent = frameworkElement.Parent;
				if (parent != null) return parent;
			}

			return VisualTreeHelper.GetParent(child);
		}
		
		#endregion
	}
}