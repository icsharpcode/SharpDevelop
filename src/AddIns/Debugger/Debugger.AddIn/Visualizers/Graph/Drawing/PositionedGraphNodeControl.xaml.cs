// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
using Debugger.AddIn.Visualizers.Common;
using Debugger.AddIn.Visualizers.Graph.Layout;

namespace Debugger.AddIn.Visualizers.Graph.Drawing
{
	/// <summary>
	/// Interaction logic for PositionedGraphNodeControl.xaml
	/// </summary>
	public partial class PositionedGraphNodeControl : UserControl
	{
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
		private ObservableCollection<ContentNode> view = new ObservableCollection<ContentNode>();
		
		private ContentNode root;
		/// <summary>
		/// The tree to be displayed in this Control.
		/// </summary>
		public ContentNode Root
		{
			get { return this.root; }
			set
			{
				this.root = value;
				this.view = getInitialView(this.root);
				// data virtualization, ContentPropertyNode implements IEvaluate
				this.listView.ItemsSource = new VirtualizingObservableCollection<ContentNode>(this.view);
				
				/*int maxLen = this.view.MaxOrDefault(contentNode => { return contentNode.Name.Length; }, 0);
				int spaces = Math.Max((int)(maxLen * 1.8 - 3), 0);
				string addedSpaces = StringHelper.Repeat(' ', spaces);
				GridView gv = listView.View as GridView;
				// hack - autosize Name column
				gv.Columns[1].Header = "Name" + addedSpaces;*/
				
				//AutoSizeColumns();
				
				/*DispatcherTimer t = new DispatcherTimer();
				t.Interval = TimeSpan.FromMilliseconds(1000);
				t.Start();
				t.Tick += (s, ee) => { AutoSizeColumns(); t.Stop(); };*/
			}
		}
		
		public void AutoSizeColumns()
		{
			//listView.UpdateLayout();
			//listView.Measure(new Size(800, 800));
			//double sum = 0;
			GridView gv = listView.View as GridView;
			if (gv != null)
			{
				foreach (var c in gv.Columns)
				{
					//var header = c.Header as GridViewColumnHeader;
					// Code below was found in GridViewColumnHeader.OnGripperDoubleClicked() event handler (using Reflector)
					// i.e. it is the same code that is executed when the gripper is double clicked
					//var uw = c.GetType().GetMethod("UpdateActualWidth", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
					//uw.Invoke(c, new object[] { });
					//if (double.IsNaN(c.Width))
					{
						c.Width = c.ActualWidth;
					}
					c.Width = double.NaN;

					/*var dw = c.GetType().GetProperty("DesiredWidth", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
					double desired = (double)dw.GetValue(c, null);
					c.Width = c.ActualWidth;	// ActualWidth is not correct until GridViewRowPresenter gets measured
					c.Width = double.NaN;
					sum += c.Width;*/
				}
			}
		}
		
		public PositionedGraphNodeControl()
		{
			InitializeComponent();
			Init();
		}
		
		public void Init()
		{
			PropertyExpanded = null;
			PropertyCollapsed = null;
			ContentNodeExpanded = null;
			ContentNodeCollapsed = null;
			this.listView.ItemsSource = null;
		}
		
		void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			/*var clickedText = (TextBlock)e.Source;
			var clickedNode = (ContentNode)(clickedText).DataContext;
			var propNode = clickedNode as ContentPropertyNode;
			if (propNode != null && propNode.Property != null && propNode.Property.Edge != null && propNode.Property.Edge.Spline != null)
			{
				propNode.Property.Edge.Spline.StrokeThickness = propNode.Property.Edge.Spline.StrokeThickness + 1;
			}*/
		}
		
		private void PropertyExpandButton_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.Source;
			ContentPropertyNode clickedNode = null;
			try
			{
				clickedNode = (ContentPropertyNode)(clickedButton).DataContext;
			}
			catch(InvalidCastException)
			{
				throw new InvalidOperationException("Clicked property expand button, button shouln't be there - DataContext is not PropertyNodeViewModel.");
			}
			
			PositionedNodeProperty property = clickedNode.Property;
			//property.IsPropertyExpanded = !property.IsPropertyExpanded;  // done by databinding
			clickedButton.Content = property.IsPropertyExpanded ? "-" : "+";
			
			if (property.IsPropertyExpanded)
			{
				OnPropertyExpanded(property);
			}
			else
			{
				OnPropertyCollapsed(property);
			}
		}
		
		private void NestedExpandButton_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.Source;
			var clickedNode = (ContentNode)(clickedButton).DataContext;
			int clickedIndex = this.view.IndexOf(clickedNode);
			//clickedNode.IsExpanded = !clickedNode.IsExpanded;		// done by data binding
			clickedButton.Content = clickedNode.IsExpanded ? "-" : "+";	// could be done by a converter

			if (clickedNode.IsExpanded)
			{
				// insert children
				int i = 1;
				foreach (var childNode in clickedNode.Children)
				{
					this.view.Insert(clickedIndex + i, childNode);
					i++;
				}
				// insertChildren(clickedNode, this.view, clickedIndex); // TODO
				OnContentNodeExpanded(clickedNode);
			}
			else
			{
				// remove whole subtree
				int size = subtreeSize(clickedNode) - 1;
				for (int i = 0; i < size; i++)
				{
					this.view.RemoveAt(clickedIndex + 1);
				}
				OnContentNodeCollapsed(clickedNode);
			}
			
			//AutoSizeColumns();

			// set to Auto again to resize columns
			/*var colName = (this.listView.View as GridView).Columns[0];
			var colValue = (this.listView.View as GridView).Columns[1];
			colName.Width = 300;
			colName.Width = double.NaN;
			colValue.Width = 300;
			colValue.Width = double.NaN;
			//this.view.Insert(0, new ContentNode());
			//this.view.RemoveAt(0);
			this.listView.UpdateLayout();
			this.listView.Width = colName.ActualWidth + colValue.ActualWidth + 30;
			this.listView.Width = double.NaN;*/
		}
		
		private ObservableCollection<ContentNode> getInitialView(ContentNode root)
		{
			return new ObservableCollection<ContentNode>(root.FlattenChildrenExpanded());
		}
		
		private int subtreeSize(ContentNode node)
		{
			return 1 + node.Children.Sum(child => (child.IsExpanded ? subtreeSize(child) : 1));
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
		#endregion
	}
}
