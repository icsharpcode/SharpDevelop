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
			//only for testing
			topTree.FontSize = 18;
			leftTree.FontSize = 18;
			
			nodeDescriptionViewModel = new NodeDescriptionViewModel();
			this.inform.DataContext = nodeDescriptionViewModel;
			topTree.Root = new ICSharpCode.TreeView.SharpTreeNode();
			leftTree.Root = new ICSharpCode.TreeView.SharpTreeNode();
			matrix.Colorizer = new DependencyColorizer();
			matrix.ScrollOwner = scrollViewer;
		}
		
		
		public void Update(IEnumerable<NodeBase> nodes)
		{
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
				SetTooltip (e.OriginalSource as DependencyObject);
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
				SetTooltip (e.OriginalSource as DependencyObject);
				nodeDescriptionViewModel.Node = n.Node;
				matrix.HighlightLine(HeaderType.Columns, n.Node);
				topTree.SelectedItem = n;
				UpdateInfoText();
			}
		}
		
		static void SetTooltip(DependencyObject node)
		{
			var c = Extensions.GetParent<SharpTreeViewItem>(node);
			var n = c.Node as MatrixTreeNode;
			if (n != null) {
				c.ToolTip = string.Format("Name : {0} has Children : {1}", n.Node.Name, n.Node.Children.Count);
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
			{
				nodeDescriptionViewModel.InfoText = left.Node.GetInfoText(top.Node);
				matrix.ToolTip = nodeDescriptionViewModel.InfoText;
			}
		}
		
	
		#endregion
	}
}
