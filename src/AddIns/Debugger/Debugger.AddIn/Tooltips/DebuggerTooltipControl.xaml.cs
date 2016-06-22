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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using Debugger.AddIn.TreeModel;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace Debugger.AddIn.Tooltips
{
	public partial class DebuggerTooltipControl : Popup, ITooltip
	{
		static Point ChildPopupOffset = new Point(16, 15);
		
		public DebuggerTooltipControl ChildTooltip { get; private set; }
		readonly IEnumerator<TreeNode> treeNodesGenerator;
		readonly ObservableCollection<TreeNode> treeNodes;
		
		public DebuggerTooltipControl(IEnumerable<TreeNode> treeNodesGenerator)
		{
			InitializeComponent();
			
			this.treeNodesGenerator = treeNodesGenerator.GetEnumerator();
			treeNodes = new ObservableCollection<TreeNode>();
			GetNextItems();
			this.dataGrid.ItemsSource = treeNodes;
			
			// Only the leaf of the tooltip has this set to false
			// Therefore it will automatically close if something else gets focus
			this.StaysOpen = false;
			this.Placement = PlacementMode.Absolute;
		}
		
		public DebuggerTooltipControl(params TreeNode[] treeNodes)
			: this((IEnumerable<TreeNode>)treeNodes)
		{
		}
		
		private void Expand_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.OriginalSource;
			var clickedNode = (TreeNode)clickedButton.DataContext;
			
			if (clickedButton.IsChecked == true && clickedNode.GetChildren != null) {
				Point popupPos = clickedButton.PointToScreen(ChildPopupOffset).TransformFromDevice(clickedButton);
				this.ChildTooltip = new DebuggerTooltipControl(clickedNode.GetChildren()) {
					// We can not use placement target otherwise we would get too deep logical tree
					Placement = PlacementMode.Absolute,
					HorizontalOffset = popupPos.X,
					VerticalOffset = popupPos.Y,
				};
				
				// The child is now tracking the focus
				this.StaysOpen = true;
				this.ChildTooltip.StaysOpen = false;
				
				this.ChildTooltip.Closed += delegate {
					// The null will have the effect of ignoring the next click
					clickedButton.IsChecked = clickedButton.IsMouseOver ? (bool?)null : false;
					// Either keep closing or make us the new leaf
					if (this.IsMouseOver) {
						this.StaysOpen = false;
					} else {
						this.IsOpen = false;
					}
				};
				this.ChildTooltip.IsOpen = true;
			}
		}
		
		bool ITooltip.CloseWhenMouseMovesAway {
			get {
				return this.ChildTooltip == null;
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			// Closing the popup does not normally cause LostFocus on the textbox so we have to update it manually
			TextBox textBox = FocusManager.GetFocusedElement(this) as TextBox;
			if (textBox != null) {
				BindingExpression be = textBox.GetBindingExpression(TextBox.TextProperty);
				be.UpdateSource();
			}

			base.OnClosed(e);

			if (this.ChildTooltip != null) {
				this.ChildTooltip.IsOpen = false;
			}
		}
		
		void CopyMenuItemClick(object sender, RoutedEventArgs e)
		{
			ValueNode node = ((MenuItem)sender).DataContext as ValueNode;
			if (node != null) {
				Clipboard.SetText(node.FullText);
			}
		}
		
		void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.ExtentHeight > e.ViewportHeight) {
				if (Math.Abs(e.ExtentHeight - e.VerticalOffset - e.ViewportHeight) < 2)
					GetNextItems();
			}
		}

		void GetNextItems(int max = 25)
		{
			int count = 0;
			while (treeNodesGenerator.MoveNext()) {
				treeNodes.Add(treeNodesGenerator.Current);
				count++;
				if (count >= max)
					return;
			}
		}
	}
}