// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using Debugger.AddIn.Visualizers.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Debugger.AddIn.Visualizers.Graph.Layout;

namespace Debugger.AddIn.Visualizers.Graph.Drawing
{
	/// <summary>
	/// Interaction logic for PositionedGraphNodeControl.xaml
	/// </summary>
	public partial class PositionedGraphNodeControl : UserControl
	{
		public static readonly bool IsShowMemberIcon = true;
		
		/// <summary>
		/// Occurs when <see cref="PositionedNodeProperty"/> is expanded.
		/// </summary>
		public event EventHandler<PositionedPropertyEventArgs> PropertyExpanded;
		/// <summary>
		/// Occurs when <see cref="PositionedNodeProperty"/> is collaped.
		/// </summary>
		public event EventHandler<PositionedPropertyEventArgs> PropertyCollapsed;
		
		/// <summary>
		/// Occurs when <see cref="NesteNodeViewModel"/> is expanded.
		/// </summary>
		public event EventHandler<ContentNodeEventArgs> ContentNodeExpanded;
		/// <summary>
		/// Occurs when <see cref="NesteNodeViewModel"/> is collaped.
		/// </summary>
		public event EventHandler<ContentNodeEventArgs> ContentNodeCollapsed;
		
		
		// shown in the ListView
		private ObservableCollection<ContentNode> items = new ObservableCollection<ContentNode>();
		
		/// <summary>
		/// The tree to be displayed in this Control.
		/// </summary>
		public ContentNode Root { get; set; }
		
		/// <summary>
		/// Sets the node to be displayed by this control.
		/// </summary>
		/// <param name="node"></param>
		public void SetDataContext(PositionedNode node)
		{
			if (node == null) {
				this.DataContext = null;
				this.listView.ItemsSource = null;
				return;
			}
			this.DataContext = node;
			this.Root = node.Content;
			this.items = GetInitialItems(this.Root);
			// data virtualization, ContentPropertyNode implements IEvaluate
			this.listView.ItemsSource = new VirtualizingObservableCollection<ContentNode>(this.items);
		}
		
		public PositionedGraphNodeControl()
		{
			InitializeComponent();
			PropertyExpanded = null;
			PropertyCollapsed = null;
			ContentNodeExpanded = null;
			ContentNodeCollapsed = null;
			this.listView.ItemsSource = null;
		}
		
		void PropertyExpandButton_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.Source;
			if (clickedButton.DataContext == null) return;
			ContentPropertyNode clickedNode = clickedButton.DataContext as ContentPropertyNode;
			clickedNode = clickedButton.DataContext as ContentPropertyNode;
			if (clickedNode == null) {
				throw new InvalidOperationException("Clicked property expand button, button shouln't be there - DataContext is not ContentPropertyNode.");
			}
			
			PositionedNodeProperty property = clickedNode.Property;
			clickedButton.Content = property.IsPropertyExpanded ? "-" : "+";	// could be done using a converter
			
			if (property.IsPropertyExpanded) {
				OnPropertyExpanded(property);
			} else {
				OnPropertyCollapsed(property);
			}
		}
		
		void NestedExpandButton_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.Source;
			var clickedNode = clickedButton.DataContext as ContentNode;
			if (clickedNode == null) return;
			int clickedIndex = this.items.IndexOf(clickedNode);
			clickedButton.Content = clickedNode.IsExpanded ? "-" : "+";	// could be done using a converter

			if (clickedNode.IsExpanded) {
				// insert children
				int i = 1;
				foreach (var childNode in clickedNode.Children) {
					this.items.Insert(clickedIndex + i, childNode);
					i++;
				}
				OnContentNodeExpanded(clickedNode);
			} else {
				// remove whole subtree
				int size = SubtreeSize(clickedNode) - 1;
				for (int i = 0; i < size; i++) {
					this.items.RemoveAt(clickedIndex + 1);
				}
				OnContentNodeCollapsed(clickedNode);
			}
			
			CalculateWidthHeight();
		}
		
		ObservableCollection<ContentNode> GetInitialItems(ContentNode root)
		{
			return new ObservableCollection<ContentNode>(root.FlattenChildrenExpanded());
		}
		
		int SubtreeSize(ContentNode node)
		{
			return 1 + node.Children.Sum(child => (child.IsExpanded ? SubtreeSize(child) : 1));
		}
		
		public void CalculateWidthHeight()
		{
			if (!IsShowMemberIcon) {
				columnMemberIcon.Width = 0;
			}
			
			int nameColumnMaxLen = this.items.MaxOrDefault(contentNode => contentNode.Name.Length, 0);
			GridView gv = listView.View as GridView;
			columnName.Width = Math.Min(20 + nameColumnMaxLen * 6, 260);
			columnText.Width = 80;
			listView.Width = columnExpander.Width + columnMemberIcon.Width + columnName.Width + columnText.Width + 10;
			
			int maxItems = 10;
			listView.Height = 4 + Math.Min(this.items.Count, maxItems) * 20;
			if (this.items.Count > maxItems) {
				listView.Width += 30;	// for scrollbar
			}
			
			this.Width = listView.Width + 2;
			this.Height = listView.Height + this.typeNameHeaderBorder.Height + 2;
		}
		
		#region event helpers
		protected virtual void OnPropertyExpanded(PositionedNodeProperty property)
		{
			if (this.PropertyExpanded != null)
				this.PropertyExpanded(this, new PositionedPropertyEventArgs(property));
		}

		protected virtual void OnPropertyCollapsed(PositionedNodeProperty property)
		{
			if (this.PropertyCollapsed != null)
				this.PropertyCollapsed(this, new PositionedPropertyEventArgs(property));
		}
		
		protected virtual void OnContentNodeExpanded(ContentNode node)
		{
			if (this.ContentNodeExpanded != null)
				this.ContentNodeExpanded(this, new ContentNodeEventArgs(node));
		}

		protected virtual void OnContentNodeCollapsed(ContentNode node)
		{
			if (this.ContentNodeCollapsed != null)
				this.ContentNodeCollapsed(this, new ContentNodeEventArgs(node));
		}
		
		void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
		{
			SetEdgeStrokeThickness((ListViewItem)e.Source, 2);
		}

		void ListViewItem_MouseLeave(object sender, MouseEventArgs e)
		{
			SetEdgeStrokeThickness((ListViewItem)e.Source, 1);
		}
		
		void SetEdgeStrokeThickness(ListViewItem listViewItem, int thickness)
		{
			var propNode = listViewItem.DataContext as ContentPropertyNode;
			if (propNode == null) {
				return;
			}
			if (propNode != null && propNode.Property != null && propNode.Property.Edge != null && propNode.Property.Edge.Spline != null) {
				propNode.Property.Edge.Spline.StrokeThickness = thickness;
			}
		}
		#endregion
	}
}
