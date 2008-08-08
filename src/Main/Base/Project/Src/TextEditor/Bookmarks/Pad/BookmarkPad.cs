// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public sealed class BookmarkPad : BookmarkPadBase
	{
		static BookmarkPad instance;
		
		public static BookmarkPad Instance {
			get {
				if (instance == null) {
					WorkbenchSingleton.Workbench.GetPad(typeof(BookmarkPad)).CreatePad();
				}
				return instance;
			}
		}
		
		public BookmarkPad()
		{
			instance = this;
		}
	}
	
	public abstract class BookmarkPadBase : AbstractPadContent
	{
		Panel       myPanel          = new Panel();
		ExtTreeView bookmarkTreeView = new ExtTreeView();
		
		Dictionary<string, BookmarkFolderNode> fileNodes = new Dictionary<string, BookmarkFolderNode>();
		
		public override Control Control {
			get {
				return myPanel;
			}
		}
		
		public TreeNode CurrentNode {
			get {
				return bookmarkTreeView.SelectedNode as TreeNode;
			}
		}
		
		protected virtual ToolStrip CreateToolStrip()
		{
			ToolStrip toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/BookmarkPad/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			return toolStrip;
		}
		
		protected BookmarkPadBase()
		{
			bookmarkTreeView.Dock = DockStyle.Fill;
			bookmarkTreeView.CheckBoxes = true;
			bookmarkTreeView.HideSelection = false;
			bookmarkTreeView.Font = ExtTreeNode.RegularBigFont;
			bookmarkTreeView.IsSorted = false;
			
			myPanel.Controls.AddRange(new Control[] { bookmarkTreeView, CreateToolStrip()} );
			BookmarkManager.Added   += new BookmarkEventHandler(BookmarkManagerAdded);
			BookmarkManager.Removed += new BookmarkEventHandler(BookmarkManagerRemoved);
			
			foreach (SDBookmark mark in BookmarkManager.Bookmarks) {
				AddMark(mark);
			}
		}
		
		public IEnumerable<TreeNode> AllNodes {
			get {
				Stack<TreeNode> treeNodes = new Stack<TreeNode>();
				foreach (TreeNode node in bookmarkTreeView.Nodes) {
					treeNodes.Push(node);
				}
				
				while (treeNodes.Count > 0) {
					TreeNode node = treeNodes.Pop();
					foreach (TreeNode childNode in node.Nodes) {
						treeNodes.Push(childNode);
					}
					yield return node;
				}
			}
		}
		
		public void EnableDisableAll()
		{
			bool isOneChecked = false;
			foreach (TreeNode node in AllNodes) {
				if (node is BookmarkNode) {
					if (((BookmarkNode)node).Checked) {
						isOneChecked = true;
						break;
					}
				}
			}
			foreach (TreeNode node in AllNodes) {
				if (node is BookmarkNode) {
					((BookmarkNode)node).Checked = !isOneChecked;
				}
			}
		}
		
		void AddMark(SDBookmark mark)
		{
			if (!ShowBookmarkInThisPad(mark))
				return;
			if (!fileNodes.ContainsKey(mark.FileName)) {
				BookmarkFolderNode folderNode = new BookmarkFolderNode(mark.FileName);
				fileNodes.Add(mark.FileName, folderNode);
				bookmarkTreeView.Nodes.Add(folderNode);
			}
			fileNodes[mark.FileName].AddMark(mark);
			fileNodes[mark.FileName].Expand();
		}
		
		protected virtual bool ShowBookmarkInThisPad(SDBookmark mark)
		{
			return mark.IsVisibleInBookmarkPad && !(mark is Debugging.BreakpointBookmark);
		}
		
		void BookmarkManagerAdded(object sender, BookmarkEventArgs e)
		{
			AddMark((SDBookmark)e.Bookmark);
		}
		
		void BookmarkManagerRemoved(object sender, BookmarkEventArgs e)
		{
			if (fileNodes.ContainsKey(e.Bookmark.FileName)) {
				fileNodes[e.Bookmark.FileName].RemoveMark(e.Bookmark);
				if (fileNodes[e.Bookmark.FileName].Marks.Count == 0) {
					bookmarkTreeView.Nodes.Remove(fileNodes[e.Bookmark.FileName]);
					fileNodes.Remove(e.Bookmark.FileName);
				}
			}
		}
		
		void TreeViewDoubleClick(object sender, EventArgs e)
		{
			TreeNode node = bookmarkTreeView.SelectedNode;
			if (node != null) {
				SDBookmark mark = node.Tag as SDBookmark;
				if (mark != null) {
					FileService.JumpToFilePosition(mark.FileName, mark.LineNumber, 0);
				}
			}
		}
		
	}
}
