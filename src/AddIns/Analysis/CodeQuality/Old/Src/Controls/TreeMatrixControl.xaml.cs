// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.CodeQualityAnalysis.Utility;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	/// <summary>
	/// Interaction logic for TreeMatrixControl.xaml
	/// </summary>
	public partial class TreeMatrixControl : System.Windows.Controls.UserControl
	{
		
		private ScrollViewer leftScrollViewer;
		private ScrollViewer topScrollViewer;
		
		public DependencyMatrix Matrix
		{
			get
			{
				return (DependencyMatrix) matrixControl.Matrix;
			}
			
			set
			{
				matrixControl.Matrix = value;
			}
		}
		

		public TreeMatrixControl()
		{
			InitializeComponent();
			matrixControl.Colorizer = new DependencyColorizer();
			matrixControl.HoveredCellChanged += OnHoverChanged;
		}
		
		
		public void DrawTree(AssemblyNode module)
		{
			var leftCol = leftTree.Items.SourceCollection as INotifyCollectionChanged;
			leftCol.CollectionChanged += BuildLeftINodeList;
			
			var topCol = topTree.Items.SourceCollection as INotifyCollectionChanged;
			topCol.CollectionChanged += BuildTopINodeList;
			
			leftTree.MouseMove += LeftTree_MouseMove;
			topTree.MouseMove += TopTree_MouseMove;
			
			Helper.FillTree(leftTree, module);
			Helper.FillTree(topTree, module);
		}
		
		
		void Trees_Loaded (object sender, EventArgs e)
		{
			leftTree.ApplyTemplate();
			topTree.ApplyTemplate();
			
			leftScrollViewer = Helper.FindVisualChild<ScrollViewer>(leftTree);
			topScrollViewer = Helper.FindVisualChild<ScrollViewer>(topTree);
		}
		
		#region Tree MouseMove
		
		 void  LeftTree_MouseMove (object sender,System.Windows.Input.MouseEventArgs  e)
		{
			DependecyTreeNode n = ConvertNode(e.OriginalSource);
			if (n != null) {
				matrixControl.HighlightLine(HeaderType.Rows,n.INode);
				leftTree.SelectedItem = n;
			}
		}
			
		void TopTree_MouseMove (object sender,System.Windows.Input.MouseEventArgs  e)
		{
			DependecyTreeNode n = ConvertNode(e.OriginalSource);
			if (n != null) {
				matrixControl.HighlightLine(HeaderType.Columns,n.INode);
				topTree.SelectedItem = n;
			}
		}
			
		
		DependecyTreeNode ConvertNode (object node)
		{
			var c = Helper.GetParent<SharpTreeViewItem>(node as DependencyObject);
			if (c != null) {
				return c.Node as DependecyTreeNode;
			}
			return null;
		}
		
		#endregion
		
		
		#region Update MatricControl
		
		bool rebuildLeftNodeListRequested;
		
		void BuildLeftINodeList(object sender,NotifyCollectionChangedEventArgs e)
		{
			if (rebuildLeftNodeListRequested)
				return;
			rebuildLeftNodeListRequested = true;
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(SetVisibleItemsForRows));
		}

		void SetVisibleItemsForRows()
		{
			List<INode> leftNodes = new List<INode>();
			foreach (DependecyTreeNode element in leftTree.Items) {
				var n = element.INode;
				leftNodes.Add(n);
			}
			rebuildLeftNodeListRequested = false;
			matrixControl.SetVisibleItems(HeaderType.Rows, leftNodes);
		}
		
		
		bool rebuildTopNodeListRequested;
		
		void BuildTopINodeList(object sender,NotifyCollectionChangedEventArgs e)
		{
			if (rebuildTopNodeListRequested)
				return;
			rebuildTopNodeListRequested = true;
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(SetVisibleItemsForColumns));
		}

		void SetVisibleItemsForColumns()
		{
			List<INode> topNodes = new List<INode>();
			foreach (DependecyTreeNode element in topTree.Items) {
				var n = element.INode;
				topNodes.Add(n);
			}
			rebuildTopNodeListRequested = false;
			matrixControl.SetVisibleItems(HeaderType.Columns, topNodes);
		}
		
		#endregion
		
		#region OnHoover
		
		void OnHoverChanged(object sender ,HoveredCellEventArgs <Relationship> e)
		{
			if (e.HoveredCell.RowIndex < leftTree.Items.Count) {
				var leftNode = leftTree.Items[e.HoveredCell.RowIndex] as DependecyTreeNode;
				leftTree.SelectedItem = leftNode;
			}
			if (e.HoveredCell.ColumnIndex < topTree.Items.Count )
			{
				var topNode = topTree.Items[e.HoveredCell.ColumnIndex] as DependecyTreeNode;
				topTree.SelectedItem = topNode;
			}
		}
		
		#endregion
		
		#region Tree Scroll
		bool scrollerWorking;
		
		void LeftTree_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			scrollerWorking = true;
			scrollViewer.ScrollToVerticalOffset(e.VerticalOffset * matrixControl.CellHeight);
			scrollerWorking = false;
		}
		
		void TopTree_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			scrollerWorking = true;
			scrollViewer.ScrollToHorizontalOffset(e.VerticalOffset * matrixControl.CellWidth);
			scrollerWorking = false;
		}
		
		#endregion
		
		#region ScrollViewer Scroll
		
		void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (scrollerWorking) {
				return;
			}
			leftScrollViewer.ScrollToVerticalOffset (e.VerticalOffset / matrixControl.CellHeight);
			topScrollViewer.ScrollToVerticalOffset (e.HorizontalOffset / matrixControl.CellWidth);
		}
		
		#endregion
		
	}
}
