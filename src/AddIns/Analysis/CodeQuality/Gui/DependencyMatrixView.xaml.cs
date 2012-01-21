// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeQuality.Gui
{
	/// <summary>
	/// Interaction logic for DependencyMatrixView.xaml
	/// </summary>
	public partial class DependencyMatrixView : UserControl
	{
		public DependencyMatrixView()
		{
			InitializeComponent();
			
			topTree.Root = new ICSharpCode.TreeView.SharpTreeNode();
			leftTree.Root = new ICSharpCode.TreeView.SharpTreeNode();
			matrix.Colorizer = new DependencyColorizer();
		}
		
		public void Update(IEnumerable<INode> nodes)
		{
			Extensions.FillTree(topTree, nodes);
			Extensions.FillTree(leftTree, nodes);
			
			var leftCol = leftTree.Items.SourceCollection as INotifyCollectionChanged;
			leftCol.CollectionChanged += BuildLeftINodeList;
			
			var topCol = topTree.Items.SourceCollection as INotifyCollectionChanged;
			topCol.CollectionChanged += BuildTopINodeList;
			
			var matrix = new DependencyMatrix();
			AddChildrenToMatrix(matrix, nodes);
			this.matrix.Matrix = matrix;
		}

		void AddChildrenToMatrix(DependencyMatrix matrix, IEnumerable<INode> nodes)
		{
			foreach (var node in nodes) {
				matrix.AddColumn(node);
				matrix.AddRow(node);
				AddChildrenToMatrix(matrix, node.Children);
			}
		}
		
		void ViewScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			ScrollViewer scrollViewer = e.OriginalSource as ScrollViewer;
			ScrollViewer topTree = this.topTree.GetScrollViewer();
			ScrollViewer leftTree = this.leftTree.GetScrollViewer();
			if (scrollViewer.Content == matrix) {
				scrollViewer.SynchronizeScroll(topTree, ScrollSyncOption.HorizontalToVertical);
				scrollViewer.SynchronizeScroll(leftTree, ScrollSyncOption.Vertical);
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
	}
}