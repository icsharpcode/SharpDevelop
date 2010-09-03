// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	#region Goto Commands
	/*
	public class GotoNext : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoNextBookmark(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
	
	public class GotoPrev : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoPrevBookmark(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
	*/
	#endregion Goto Commands
	
	#region Delete BookMark(s) commands
	
	public abstract class AbstractDeleteMarkClass : AbstractMenuCommand
	{
		protected void deleteBookMark (BookmarkNode node) {
			ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.RemoveMark(node.Bookmark);
		}
	}
	
	/// <summary>
	/// Deletes all <see cref="BookmarkNode" />s in the BookMarkPad.
	/// </summary>
	public class DeleteAllMarks : AbstractDeleteMarkClass
	{
		public override void Run()
		{
			IEnumerable<TreeNode> nodes = ((BookmarkPadBase)Owner).AllNodes;
			foreach(TreeNode innerNode in nodes) {
				BookmarkFolderNode folderNode =  innerNode as BookmarkFolderNode;
				// Its problebly not the most effecient way of doing it, but it works.
				if (folderNode != null) {
					for (int i = folderNode.Nodes.Count - 1; i >= 0 ; i--)
					{
						if (folderNode.Nodes[i] is BookmarkNode) {
							deleteBookMark(folderNode.Nodes[i] as BookmarkNode);
						}
					}
				}
			}
		}
	}
	
	
	/// <summary>
	/// Deletes the currently selected <see cref="BookmarkNode" /> or <see cref="BookmarkFolderNode" />
	/// </summary>
	public class DeleteMark : AbstractDeleteMarkClass
	{
		public override void Run()
		{
			TreeNode node = ((BookmarkPadBase)Owner).CurrentNode;
			if (node == null) return;
			if (node is BookmarkNode) {
				deleteBookMark(node as BookmarkNode);
			}
			if (node is BookmarkFolderNode) {
				BookmarkFolderNode folderNode = node as BookmarkFolderNode;
				// We have to start from the top of the array to prevent reordering.
				for (int i = folderNode.Nodes.Count - 1; i >= 0 ; i--)
				{
					if (folderNode.Nodes[i] is BookmarkNode) {
						deleteBookMark(folderNode.Nodes[i] as BookmarkNode);
					}
				}
			}
		}
	}
	
	#endregion Delete BookMark(s) commands
	
	public class EnableDisableAll : AbstractMenuCommand
	{
		public override void Run()
		{
			((BookmarkPadBase)Owner).EnableDisableAll();
		}
	}
}
