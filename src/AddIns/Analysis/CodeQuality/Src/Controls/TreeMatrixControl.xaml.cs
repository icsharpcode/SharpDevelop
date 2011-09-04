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
		
		public void DrawTree(Module module)
		{
			var leftCol = leftTree.Items.SourceCollection as INotifyCollectionChanged;
			leftCol.CollectionChanged += BuildLeftINodeList;
			Helper.FillTree(leftTree, module);
			leftTree.MouseMove += (s,e)=>
			{
				var c = Helper.GetParent<SharpTreeViewItem>(e.OriginalSource as DependencyObject);
				if (c != null){
				
				
			};
			var topCol = topTree.Items.SourceCollection as INotifyCollectionChanged;
			topCol.CollectionChanged += BuildTopINodeList;
			Helper.FillTree(topTree, module);
		}
		
		
		void Trees_Loaded (object sender, EventArgs e)
		{
			leftTree.ApplyTemplate();
			topTree.ApplyTemplate();
			
			leftScrollViewer = Helper.FindVisualChild<ScrollViewer>(leftTree);
			topScrollViewer = Helper.FindVisualChild<ScrollViewer>(topTree);
		}
		
		#region Update MatricControl
		
		bool rebuildLeftNodeListRequested;
		
		void BuildLeftINodeList(object sender,NotifyCollectionChangedEventArgs e)
		{
			if (rebuildLeftNodeListRequested)
				return;
			rebuildLeftNodeListRequested = true;
			Dispatcher.BeginInvoke(
				DispatcherPriority.DataBind,
				new Action(SetVisibleItemsForRows));
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
			Dispatcher.BeginInvoke(
				DispatcherPriority.DataBind,
				new Action(SetVisibleItemsForColumns));
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
		
		
		void OnHoverChanged(object sender ,HoveredCellEventArgs <Relationship> e)
		{
			if (e.HoveredCell.RowIndex < leftTree.Items.Count) {
				var leftNode = leftTree.Items[e.HoveredCell.RowIndex] as DependecyTreeNode;
				leftTree.SelectedItem = leftNode;
				leftTree.FocusNode(leftNode);
			}
			if (e.HoveredCell.ColumnIndex < topTree.Items.Count )
			{
				var topNode = topTree.Items[e.HoveredCell.ColumnIndex] as DependecyTreeNode;
				topTree.SelectedItem = topNode;
				topTree.FocusNode(topNode);
			}
		}
		
		
		void LeftTree_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			Console.WriteLine("Left TreeScroll {0} - {1}",e.VerticalOffset * matrixControl.CellHeight,leftTree.Items.Count);
			scrollViewer.ScrollToVerticalOffset(e.VerticalOffset * matrixControl.CellHeight);
			Console.WriteLine("--");
		}
		
		void TopTree_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
//			Console.WriteLine("Top TreeScroll ");
//			scrollViewer.ScrollToHorizontalOffset(e.VerticalChange * matrixControl.CellHeight);
//			Console.WriteLine("--");
		}
		
		
		void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
//			Console.WriteLine("ScrollViewer_ScrollChanged {0} _ {1}",e.VerticalChange,scrollViewer != null);
			//leftScrollViewer.ScrollToVerticalOffset (e.VerticalChange * matrixControl.CellHeight);
		}
	}
}
