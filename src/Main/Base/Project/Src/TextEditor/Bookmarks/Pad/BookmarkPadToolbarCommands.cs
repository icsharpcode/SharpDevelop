// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Actions;

using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	#region Goto Commands
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
	
	#endregion Goto Commands	
	
	#region Delete BookMark(s) commands
	
	public abstract class AbstractDeleteMarkClass : AbstractMenuCommand
	{	
		protected void deleteBookMark (BookmarkNode node) {
			if (node.Bookmark.Document != null) {
				node.Bookmark.Document.BookmarkManager.RemoveMark(node.Bookmark);
			} else {
				ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.RemoveMark(node.Bookmark);
			}
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
			WorkbenchSingleton.MainForm.Refresh();			
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
				WorkbenchSingleton.MainForm.Refresh();
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
