// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using ICSharpCode.Core;
using TreeNode = Debugger.AddIn.TreeModel.TreeNode;

namespace Debugger.AddIn.TreeModel
{
	public sealed class ItemIcon: NodeIcon {
		protected override Image GetIcon(TreeNodeAdv node)
		{
			return ((TreeViewVarNode)node).Content.Image;
		}
	}
	
	public sealed class ItemName: NodeTextBox {
		protected override bool CanEdit(TreeNodeAdv node)
		{
			return false;
		}
		public override object GetValue(TreeNodeAdv node)
		{
			if (node is TreeViewVarNode) {
				return ((TreeViewVarNode)node).Content.Name;
			} else {
				// Happens during incremental search
				return base.GetValue(node);
			}
		}
	}
	
	public sealed class ItemText: NodeTextBox {
		public ItemText()
		{
			this.EditEnabled = true;
			this.EditOnClick = true;
		}
		protected override bool CanEdit(TreeNodeAdv node)
		{
			TreeNode content = ((TreeViewVarNode)node).Content;
			return (content is ISetText) && ((ISetText)content).CanSetText;
		}
		public override object GetValue(TreeNodeAdv node)
		{
			if (node is TreeViewVarNode) {
				return ((TreeViewVarNode)node).Content.Text;
			} else {
				// Happens during incremental search
				return base.GetValue(node);
			}
		}
		public override void SetValue(TreeNodeAdv node, object value)
		{
			ISetText content = (ISetText)((TreeViewVarNode)node).Content;
			if (content.CanSetText) {
				content.SetText(value.ToString());
			}
		}
		protected override void OnDrawText(DrawEventArgs args)
		{
			TreeNode content = ((TreeViewVarNode)args.Node).Content;
			if (content is ExpressionNode && ((ExpressionNode)content).Error != null) {
				args.TextColor = Color.Red;
			} else if (((TreeViewVarNode)args.Node).TextChanged) {
				args.TextColor = Color.Blue;
			}
			base.OnDrawText(args);
		}
		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			TreeNode content = ((TreeViewVarNode)args.Node).Content;
			if (content is IContextMenu && args.Button == MouseButtons.Right) {
				ContextMenuStrip menu = ((IContextMenu)content).GetContextMenu();
				if (menu != null) {
					menu.Show(args.Node.Tree, args.Location);
					args.Handled = true;
				}
			} else {
				base.MouseDown(args);
			}
		}
	}
	
	public sealed class ItemType: NodeTextBox {
		protected override bool CanEdit(TreeNodeAdv node)
		{
			return false;
		}
		public override object GetValue(TreeNodeAdv node)
		{
			if (node is TreeViewVarNode) {
				return ((TreeViewVarNode)node).Content.Type;
			} else {
				// Happens during incremental search
				return base.GetValue(node);
			}
		}
	}
	
	/// <summary>
	/// A child class of TreeNodeAdv that displays exceptions.
	/// </summary>
	public class TreeViewVarNode: TreeNodeAdv
	{
		static Dictionary<string, bool> expandedNodes = new Dictionary<string, bool>();
		
		TreeViewAdv localVarList;
		Process process;
		TreeNode content;
		
		bool childsLoaded;
		bool textChanged;
		
		public TreeNode Content {
			get { return content; }
		}
		
		public bool TextChanged {
			get { return textChanged; }
		}
		
		string FullName {
			get {
				if (this.Parent != null && this.Parent is TreeViewVarNode) {
					return ((TreeViewVarNode)this.Parent).FullName + "." + Content.Name;
				} else {
					return Content.Name;
				}
			}
		}
		
		public TreeViewVarNode(Process process, TreeViewAdv localVarList, TreeNode content): base(localVarList, new object())
		{
			this.process = process;
			this.localVarList = localVarList;
			SetContentRecursive(content);
		}
		
		static TimeSpan workTime = TimeSpan.FromMilliseconds(40);
		static Stopwatch timeSinceLastRepaintEnd;
		
		#region SetContentRecursive
		
		/// <summary>
		/// A simple form of SetContentRecursive that changes the current ChildViewNode to
		/// display the data provided by <c>content</c>. If the node had any children and is expanded,
		/// it will recureively set those as well.
		/// </summary>
		/// <param name="content">Contains the name value and type of the variable stored in this particular TreeViewNode.</param>
		private void SetContentRecursive(TreeNode content)
		{
			this.textChanged =
				this.content != null &&
				this.content.Name == content.Name &&
				this.content.Text != content.Text;
			this.content = content;
			this.IsLeaf = (content.ChildNodes == null);
			childsLoaded = false;
			this.IsExpandedOnce = false;	
			if (!IsLeaf && expandedNodes.ContainsKey(this.FullName) && expandedNodes[this.FullName]) {
				LoadChildren();
				this.Expand();
			} else {
				this.Children.Clear();
				this.Collapse();
			}
			// Process user commands
			Utils.DoEvents(process);
			// Repaint
			if (timeSinceLastRepaintEnd == null || timeSinceLastRepaintEnd.Elapsed > workTime) {
				using(new PrintTime("Repainting Local Variables Pad")) {
					try {
						this.Tree.EndUpdate();   // Enable painting
						Utils.DoEvents(process); // Paint
					} finally {
						this.Tree.BeginUpdate(); // Disable painting
						timeSinceLastRepaintEnd = new Stopwatch();
						timeSinceLastRepaintEnd.Start();
					}
				}
			}
		}
		
		/// <summary>
		/// Private form of SetContentRecursive. This form contains an extra parameter used by LoadChildren.
		/// This adds the childNodes parameter, which can be set to the children of a particular child element.
		/// </summary>
		/// <param name="process"></param>
		/// <param name="localVarList"></param>
		/// <param name="childNodes"></param>
		/// <param name="contentEnum"></param>
		private static void SetContentRecursive(Process process, TreeViewAdv localVarList, IList<TreeNodeAdv> childNodes, IEnumerable<TreeNode> contentEnum)
		{
			contentEnum = contentEnum ?? new TreeNode[0];
			
			int index = 0;
			foreach(TreeNode content in contentEnum) {
				// Add or overwrite existing items
				if (index < childNodes.Count) {
					// Overwrite
					((TreeViewVarNode)childNodes[index]).SetContentRecursive(content);
				} else {
					// Add
					childNodes.Add(new TreeViewVarNode(process, localVarList, content));
				}
				index++;
			}
			int count = index;
			// Delete other nodes
			while(childNodes.Count > count) {
				childNodes.RemoveAt(count);
			}
		}
		
		/// <summary>
		/// Function for setting the root treenode of a TreeViewAdv ment to display debugger variables.
		/// </summary>
		/// <param name="process">The process that contains the stackframe with the given variables.</param>
		/// <param name="localVarList">A list of local variables.</param>
		/// <param name="contentEnum">A list of local variables.</param>
		public static void SetContentRecursive(Process process, TreeViewAdv localVarList, IEnumerable<TreeNode> contentEnum) {
			IList<TreeNodeAdv> childNodes = localVarList.Root.Children;
			SetContentRecursive(process, localVarList, childNodes, contentEnum);
		}
		
		#endregion SetContentRecursive
		
		protected override void OnExpanding()
		{
			base.OnExpanding();
		}
		
		
		/// <summary>
		/// This displays all the immediate children of a TreeViewNode in its containing TreeViewAdv.
		/// </summary>
		void LoadChildren()
		{
			if (!childsLoaded) {
				childsLoaded = true;
				this.IsExpandedOnce = true;
				SetContentRecursive(process, this.localVarList, this.Children, this.Content.ChildNodes);
			}
		}
		
		
		/// <summary>
		/// Expands the current treenode and displays all its immediate children.
		/// </summary>
		protected override void OnExpanded()
		{
			base.OnExpanded();
			if (process == null)
				return;
			expandedNodes[FullName] = true;
			if (process.IsRunning) {
				MessageService.ShowMessage(
					"${res:MainWindow.Windows.Debug.LocalVariables.CannotExploreVariablesWhileRunning}",
					"${res:MainWindow.Windows.Debug.LocalVariables}"
				);
				return;
			}
			try {
				this.Tree.BeginUpdate();
				LoadChildren();
			} catch (AbortedBecauseDebuggeeResumedException) {
			} finally {
				this.Tree.EndUpdate();
			}
		}
		
		protected override void OnCollapsing()
		{
			base.OnCollapsing();
		}
		
		protected override void OnCollapsed()
		{
			base.OnCollapsed();
			expandedNodes[FullName] = false;
		}
	}
}
