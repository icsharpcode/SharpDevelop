// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;

namespace Bookmark
{
	public class BookmarkPad : AbstractPadContent
	{
		static BookmarkPad instance;
		
		public static BookmarkPad Instance {
			get {
				return instance;
			}
		}
		
		Panel       myPanel          = new Panel();
		ExtTreeView bookmarkTreeView = new ExtTreeView();
		
		Dictionary<string, BookmarkFolderNode> fileNodes = new Dictionary<string, BookmarkFolderNode>();
			
		public override Control Control {
			get {
				return myPanel;
			}
		}
		
		public BookmarkPad()
		{
			instance = this;
			
			bookmarkTreeView.Dock = DockStyle.Fill;
			bookmarkTreeView.CheckBoxes = true;
			bookmarkTreeView.HideSelection = false;
			bookmarkTreeView.Font = ExtTreeNode.Font;
			bookmarkTreeView.IsSorted = false;
			
			ToolStrip toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/BookmarkPad/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			myPanel.Controls.AddRange(new Control[] { bookmarkTreeView, toolStrip} );
			BookmarkManager.Added   += new BookmarkEventHandler(BookmarkManagerAdded);
			BookmarkManager.Removed += new BookmarkEventHandler(BookmarkManagerRemoved);
			
			foreach (Bookmark mark in BookmarkManager.Bookmarks) {
				AddMark(mark);
			}
		}
		
		void AddMark(Bookmark mark)
		{
			if (!fileNodes.ContainsKey(mark.FileName)) {
				BookmarkFolderNode folderNode = new BookmarkFolderNode(mark.FileName);
				fileNodes.Add(mark.FileName, folderNode);
				bookmarkTreeView.Nodes.Add(folderNode);
			}
			fileNodes[mark.FileName].AddMark(mark);
			fileNodes[mark.FileName].Expand();
		}
		
		void BookmarkManagerAdded(object sender, BookmarkEventArgs e) 
		{
			AddMark(e.Bookmark);
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
				Bookmark mark = node.Tag as Bookmark;
				if (mark != null) {
					FileService.JumpToFilePosition(mark.FileName, mark.LineNumber, 0);
				}
			}
		}
		
	}
}
