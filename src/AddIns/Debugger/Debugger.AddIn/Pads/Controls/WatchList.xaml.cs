// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.TreeView;

namespace Debugger.AddIn.Pads.Controls
{
	public enum WatchListType
	{
		LocalVar,
		Watch
	}
	
	public partial class WatchList : UserControl
	{
		public WatchList(WatchListType type)
		{
			InitializeComponent();
			WatchType = type;
			if (type == WatchListType.Watch)
				myList.Root = new WatchRootNode();
			else
				myList.Root = new SharpTreeNode();
		}
		
		public WatchListType WatchType { get; private set; }
		
		public SharpTreeNodeCollection WatchItems {
			get { return myList.Root.Children; }
		}
		
		public TreeNodeWrapper SelectedNode {
			get { return myList.SelectedItem as TreeNodeWrapper; }
		}
		
		void OnValueTextBoxKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Escape) {
				e.Handled = true;
				return;
			}
			
			if (e.Key == Key.Enter) {
				if(SelectedNode.Node is ExpressionNode) {
					var node = (ExpressionNode)SelectedNode.Node;
					node.SetText(((TextBox)sender).Text);
				}
			}
			if (e.Key == Key.Enter || e.Key == Key.Escape) {
				myList.UnselectAll();
				if (LocalVarPad.Instance != null)
					LocalVarPad.Instance.InvalidatePad();
				if (WatchPad.Instance != null)
					WatchPad.Instance.InvalidatePad();
			}
		}
		
		void WatchListAutoCompleteCellCommandEntered(object sender, EventArgs e)
		{
			var selectedNode = SelectedNode;
			if (selectedNode == null) return;
			if (WatchType != WatchListType.Watch) return;
			
			var cell = ((WatchListAutoCompleteCell)sender);
			
			selectedNode.Node.Name = cell.CommandText;
			myList.UnselectAll();
			if (WatchType == WatchListType.Watch && WatchPad.Instance != null) {
				WatchPad.Instance.InvalidatePad();
			}
			selectedNode.IsEditing = false;
		}
		
		void MyListPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (SelectedNode == null) return;
			if (WatchType != WatchListType.Watch)
				return;
			SelectedNode.IsEditing = true;
		}
	}
}