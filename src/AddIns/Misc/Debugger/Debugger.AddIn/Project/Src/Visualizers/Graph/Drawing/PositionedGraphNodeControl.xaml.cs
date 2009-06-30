// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Debugger.AddIn.Visualizers.Graph.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.Visualizers.Common;

namespace Debugger.AddIn.Visualizers.Graph.Drawing
{
	/// <summary>
	/// Interaction logic for PositionedGraphNodeControl.xaml
	/// </summary>
	public partial class PositionedGraphNodeControl : UserControl
	{
		/// <summary>
		/// Occurs when a <see cref="PositionedNodeProperty"/> is expanded.
		/// </summary>
		public event EventHandler<PositionedPropertyEventArgs> PropertyExpanded;
		/// <summary>
		/// Occurs when a <see cref="PositionedNodeProperty"/> is collaped.
		/// </summary>
		public event EventHandler<PositionedPropertyEventArgs> PropertyCollapsed;
		
		
		// shown in the ListView
		private ObservableCollection<NestedNodeViewModel> view = new ObservableCollection<NestedNodeViewModel>();
		
		private NestedNodeViewModel root;
		/// <summary>
		/// The tree to be displayed in this Control.
		/// </summary>
		public NestedNodeViewModel Root
		{
			get { return this.root; }
			set
			{
				this.root = value;
				this.view = initializeView(this.root);
				// data virtualization, PropertyNodeViewModel implements IEvaluate
				this.listView.ItemsSource = new VirtualizingObservableCollection<NestedNodeViewModel>(this.view);
				//listView.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(listView_ScrollChanged));
			}
		}
		
		public PositionedGraphNodeControl()
		{
			InitializeComponent();
		}
		
		private void PropertyExpandButton_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.Source;
			PropertyNodeViewModel clickedNode = null;
			try
			{
				clickedNode = (PropertyNodeViewModel)(clickedButton).DataContext;
			}
			catch(InvalidCastException)
			{
				throw new InvalidOperationException("Clicked property expand button, button shouln't be there - DataContext is not PropertyNodeViewModel.");
			}
			
			PositionedNodeProperty property = clickedNode.Property;
			property.IsExpanded = !property.IsExpanded;
			clickedButton.Content = property.IsExpanded ? "-p" : "+p";
			if (property.IsExpanded)
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
			var clickedNode = (NestedNodeViewModel)(clickedButton).DataContext;
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
			}
			else
			{
				// remove whole subtree
				int size = subtreeSize(clickedNode) - 1;
				for (int i = 0; i < size; i++)
				{
					this.view.RemoveAt(clickedIndex + 1);
				}
			}

			var a = new ListViewItem();

			// set to Auto again to resize columns
			var colName = (this.listView.View as GridView).Columns[0];
			var colValue = (this.listView.View as GridView).Columns[1];
			colName.Width = 300;
			colName.Width = double.NaN;
			colValue.Width = 300;
			colValue.Width = double.NaN;
			this.view.Insert(0, new NestedNodeViewModel());
			this.view.RemoveAt(0);
			this.listView.UpdateLayout();
			this.listView.Width = colName.ActualWidth + colValue.ActualWidth + 30;
			this.listView.Width = double.NaN;
		}
		
		private ObservableCollection<NestedNodeViewModel> initializeView(NestedNodeViewModel root)
		{
			var view = new ObservableCollection<NestedNodeViewModel>();
			foreach (var topLevelNode in root.Children)
			{
				view.Add(topLevelNode);
			}
			return view;
		}
		
		private int subtreeSize(NestedNodeViewModel node)
		{
			return 1 + node.Children.Sum(child => (child.IsExpanded ? subtreeSize(child) : 1));
		}
		
		#region event helpers
		protected virtual void OnPropertyExpanded(PositionedNodeProperty property)
		{
			if (this.PropertyExpanded != null)
			{
				this.PropertyExpanded(this, new PositionedPropertyEventArgs(property));
			}
		}

		protected virtual void OnPropertyCollapsed(PositionedNodeProperty property)
		{
			if (this.PropertyCollapsed != null)
			{
				this.PropertyCollapsed(this, new PositionedPropertyEventArgs(property));
			}
		}
		#endregion
	}
}