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

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	/// <summary>
	/// Interaction logic for TreeMatrixControl.xaml
	/// </summary>
	public partial class TreeMatrixControl : System.Windows.Controls.UserControl
	{
		
		private ScrollViewer leftScrollViewer;
		private ScrollViewer topScrollViewer;
		
		public Matrix<INode, Relationship> Matrix
		{
			get
			{
				return matrixControl.Matrix;
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
			Helper.FillTree(leftTree,module);
			Helper.FillTree(topTree,module);
			var leftCol = leftTree.Items.SourceCollection as INotifyCollectionChanged;
			leftCol.CollectionChanged += BuildLeftINodeList;
			
			var rCol = leftTree.Items.SourceCollection as INotifyCollectionChanged;
			rCol.CollectionChanged += BuildRightINodeList;
		}
		
		
		void Trees_Loaded (object sender, EventArgs e)
		{
			leftTree.ApplyTemplate();
			topTree.ApplyTemplate();
			
			leftScrollViewer = Helper.FindVisualChild<ScrollViewer>(leftTree);
			topScrollViewer = Helper.FindVisualChild<ScrollViewer>(topTree);
		}
		
		bool rebuildLeftNodeListRequested;
		
		void BuildLeftINodeList (object sender,NotifyCollectionChangedEventArgs e)
		{
			if (rebuildLeftNodeListRequested)
				return;
			rebuildLeftNodeListRequested = true;
			Dispatcher.BeginInvoke(
				DispatcherPriority.DataBind,
				new Action(
					delegate {
						List <INode> leftNodes = new List<INode>();
						foreach (DependecyTreeNode element in leftTree.Items) {
							var n = element.INode;
							leftNodes.Add(n);
						}
						
						rebuildLeftNodeListRequested = false;
						Console.WriteLine("List {0}",leftNodes.Count);
						matrixControl.SetVisibleItems(HeaderType.Rows,leftNodes);
					}
				));
		}
		
		
		bool rebuildRightNodeListRequested;
		
		void BuildRightINodeList (object sender,NotifyCollectionChangedEventArgs e)
		{
			if (rebuildRightNodeListRequested)
				return;
			rebuildRightNodeListRequested = true;
			Dispatcher.BeginInvoke(
				DispatcherPriority.DataBind,
				new Action(
					delegate {
						List <INode> rNodes = new List<INode>();
						foreach (DependecyTreeNode element in topTree.Items) {
							var n = element.INode;
							rNodes.Add(n);
						}
						
						rebuildRightNodeListRequested = false;
						
						matrixControl.SetVisibleItems(HeaderType.Columns,rNodes);
					}
				));
		}
		
		void OnHoverChanged (object sender ,HoveredCellEventArgs <Relationship> e)
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
//			Console.WriteLine("Left TreeScroll");
//			scrollViewer.ScrollToVerticalOffset(e.VerticalOffset * matrixControl.CellHeight);
			Console.WriteLine("--");
		}
		
		void TopTree_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
//			Console.WriteLine("Top TreeScroll ");
//			scrollViewer.ScrollToHorizontalOffset(e.VerticalChange * matrixControl.CellHeight);
			Console.WriteLine("--");
		}
		
		
		void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
//			Console.WriteLine("ScrollViewer_ScrollChanged {0} _ {1}",e.VerticalChange,scrollViewer != null);
			//leftScrollViewer.ScrollToVerticalOffset (e.VerticalChange * matrixControl.CellHeight);
		}
	}
}
