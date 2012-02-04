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
		}
		
		
		public void Update(IEnumerable<NodeBase> nodes)
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

		void AddChildrenToMatrix(DependencyMatrix matrix, IEnumerable<NodeBase> nodes)
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
			List<NodeBase> leftNodes = new List<NodeBase>();
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
			List<NodeBase> topNodes = new List<NodeBase>();
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
				UpdateInfoText();
			}
		}
		
		void TopTreeMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			MatrixTreeNode n = ConvertNode(e.OriginalSource as DependencyObject);
			if (n != null) {
				nodeDescriptionViewModel.Node = n.Node;
				matrix.HighlightLine(HeaderType.Columns, n.Node);
				topTree.SelectedItem = n;
				UpdateInfoText();
			}
		}
		
		static MatrixTreeNode ConvertNode(DependencyObject node)
		{
			var c = Extensions.GetParent<SharpTreeViewItem>(node);
			if (c != null)
				return c.Node as MatrixTreeNode;
			return null;
		}
		
		void MatrixHoveredCellChanged(object sender, HoveredCellEventArgs<Tuple<int, int>> e)
		{
			// need to add 1 to index, because first item in treeview is invisible root node
			if (e.HoveredCell.RowIndex < leftTree.Items.Count) {
				leftTree.SelectedItem = leftTree.Items[e.HoveredCell.RowIndex + 1];
			}
			if (e.HoveredCell.ColumnIndex < topTree.Items.Count) {
				topTree.SelectedItem = topTree.Items[e.HoveredCell.ColumnIndex + 1];
			}
			UpdateInfoText();
		}

		void UpdateInfoText()
		{
			var left = leftTree.SelectedItem as MatrixTreeNode;
			var top = topTree.SelectedItem as MatrixTreeNode;
			if (left != null && top != null)
				nodeDescriptionViewModel.InfoText = left.Node.GetInfoText(top.Node);
		}
		#endregion
	}
}