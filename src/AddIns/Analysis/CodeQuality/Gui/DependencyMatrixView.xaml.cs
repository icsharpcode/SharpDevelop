// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSharpCode.CodeQuality;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeQuality.Gui
{
	/// <summary>
	/// Interaction logic for DependencyMatrixView.xaml
	/// </summary>
	public partial class DependencyMatrixView : UserControl
	{
		ScrollViewer topTreeScrollViewer, leftTreeScrollViewer;
		NodeDescriptionViewModel nodeDescriptionViewModel;
		ToolTip infoTooltip;
		
		public DependencyMatrixView()
		{
			InitializeComponent();
			Visibility = Visibility.Hidden;
			 popUp.IsOpen = true;

			nodeDescriptionViewModel = new NodeDescriptionViewModel();
			this.inform.DataContext = nodeDescriptionViewModel;
			topTree.Root = new ICSharpCode.TreeView.SharpTreeNode();
			leftTree.Root = new ICSharpCode.TreeView.SharpTreeNode();
			matrix.Colorizer = new DependencyColorizer();
			matrix.ScrollOwner = scrollViewer;
			
			infoTooltip = new ToolTip() { StaysOpen = false };
		}
		
		
		public void Update(IEnumerable<INode> nodes)
		{
			this.Visibility = Visibility.Visible;
		
			popUp.IsOpen = false;
			popUp.StaysOpen = true;

			Extensions.FillTree(topTree, nodes);
			Extensions.FillTree(leftTree, nodes);
			
			var leftCol = leftTree.Items.SourceCollection as INotifyCollectionChanged;
			leftCol.CollectionChanged += BuildLeftINodeList;
			
			var topCol = topTree.Items.SourceCollection as INotifyCollectionChanged;
			topCol.CollectionChanged += BuildTopINodeList;
			
			var matrix = new DependencyMatrix();
			if (nodes != null)
			AddChildrenToMatrix(matrix, nodes);
			this.matrix.Matrix = matrix;
			BuildLeftINodeList(null, null);
			BuildTopINodeList(null, null);
			
		}

		void AddChildrenToMatrix(DependencyMatrix matrix, IEnumerable<INode> nodes)
		{
			foreach (var node in nodes) {
				matrix.AddColumn(node);
				matrix.AddRow(node);
				if (node.Children != null)
				AddChildrenToMatrix(matrix, node.Children);
			}
		}
		
		void ViewScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			ScrollViewer sv = e.OriginalSource as ScrollViewer;
			topTreeScrollViewer = topTreeScrollViewer ?? topTree.GetScrollViewer();
			leftTreeScrollViewer = leftTreeScrollViewer ?? leftTree.GetScrollViewer();
			
			if (sv == this.scrollViewer) {
				topTreeScrollViewer.SynchronizeScroll(sv, ScrollSyncOption.HorizontalToVertical);
				leftTreeScrollViewer.SynchronizeScroll(sv, ScrollSyncOption.Vertical);
			}
		}
		
		#region Update MatrixControl
		bool rebuildLeftNodeListRequested;
		
		void BuildLeftINodeList(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (rebuildLeftNodeListRequested)
				return;
			rebuildLeftNodeListRequested = true;
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(SetVisibleItemsForRows));
		}

		void SetVisibleItemsForRows()
		{
			List<INode> leftNodes = new List<INode>();
			foreach (MatrixTreeNode node in leftTree.Items.OfType<MatrixTreeNode>()) {
				var n = node.Node;
				leftNodes.Add(n);
			}
			rebuildLeftNodeListRequested = false;
			matrix.SetVisibleItems(HeaderType.Rows, leftNodes);
		}
		
		bool rebuildTopNodeListRequested;
		
		void BuildTopINodeList(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (rebuildTopNodeListRequested)
				return;
			rebuildTopNodeListRequested = true;
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(SetVisibleItemsForColumns));
		}

		void SetVisibleItemsForColumns()
		{
			List<INode> topNodes = new List<INode>();
			foreach (MatrixTreeNode node in topTree.Items.OfType<MatrixTreeNode>()) {
				var n = node.Node;
				topNodes.Add(n);
			}
			rebuildTopNodeListRequested = false;
			matrix.SetVisibleItems(HeaderType.Columns, topNodes);
		}
		#endregion
		
		#region Tree MouseMove
		void LeftTreeMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			MatrixTreeNode n = ConvertNode(e.OriginalSource as DependencyObject);
			if (n != null) {
				nodeDescriptionViewModel.Node = n.Node;
				matrix.HighlightLine(HeaderType.Rows, n.Node);
				leftTree.SelectedItem = n;
			}
			infoTooltip.IsOpen = false;
		}
		
		void TopTreeMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			MatrixTreeNode n = ConvertNode(e.OriginalSource as DependencyObject);
			if (n != null) {
				nodeDescriptionViewModel.Node = n.Node;
				matrix.HighlightLine(HeaderType.Columns, n.Node);
				topTree.SelectedItem = n;
			}
			infoTooltip.IsOpen = false;
		}
		
		MatrixTreeNode ConvertNode(DependencyObject node)
		{
			var c = Extensions.GetParent<SharpTreeViewItem>(node);
			if (c != null)
				return c.Node as MatrixTreeNode;
			return null;
		}
		
		void MatrixHoveredCellChanged(object sender, HoveredCellEventArgs<Relationship> e)
		{
			// need to add 1 to index, because first item in treeview is invisible root node
			if (e.HoveredCell.RowIndex < leftTree.Items.Count) {
				leftTree.SelectedItem = leftTree.Items[e.HoveredCell.RowIndex + 1];
			}
			if (e.HoveredCell.ColumnIndex < topTree.Items.Count) {
				topTree.SelectedItem = topTree.Items[e.HoveredCell.ColumnIndex + 1];
			}
		}
		
		void MatrixMouseMove(object sender, MouseEventArgs e)
		{
			infoTooltip.Placement = PlacementMode.Relative;
			infoTooltip.VerticalOffset = 15;
			infoTooltip.PlacementTarget = this;
			infoTooltip.Content = GetTooltip(matrix.HoveredCell.Value);
			infoTooltip.IsOpen = true;
		}
		
		object GetTooltip(Relationship relationship)
		{
			string text = "is not related to";
			if (relationship.Relationships.Any(r => r == RelationshipType.Uses))
				text = "uses";
			else if (relationship.Relationships.Any(r => r == RelationshipType.UsedBy))
				text = "is used by";
			else if (relationship.Relationships.Any(r => r == RelationshipType.Same))
				text = "is the same as";
			return string.Format("{0} {1} {2}", relationship.From.Name, text, relationship.To.Name);
		}
		
		void MatrixMouseLeave(object sender, MouseEventArgs e)
		{
			infoTooltip.IsOpen = false;
		}
		#endregion
	}
}