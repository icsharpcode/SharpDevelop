// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class BookmarkFolderNode : ExtFolderNode
	{
		List<SDBookmark> marks = new List<SDBookmark>();
		string fileName;
		string fileNameText;
		string occurences;
		Image icon;
		
		public List<SDBookmark> Marks {
			get {
				return marks;
			}
		}
		
		public BookmarkFolderNode(string fileName)
		{
			drawDefault = false;
			this.fileName = fileName;
			fileNameText = Path.GetFileName(fileName) + StringParser.Parse(" ${res:MainWindow.Windows.SearchResultPanel.In} ") + Path.GetDirectoryName(fileName);
			icon = IconService.GetBitmap(IconService.GetImageForFile(fileName));
			Nodes.Add(new TreeNode());
		}
		
		public void SetText()
		{
			if (marks.Count == 1) {
				occurences = " (1 bookmark)";
			} else {
				occurences = " (" + marks.Count + " bookmarks)";
			}
			this.Text = fileNameText + occurences;
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int x = MeasureTextWidth(g, fileNameText, RegularBigFont);
			x += MeasureTextWidth(g, occurences, ItalicBigFont);
			if (icon != null) {
				x += icon.Width;
			}
			return x + 3;
		}
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			float x = e.Bounds.X;
			if (icon != null) {
				g.DrawImage(icon, x, e.Bounds.Y, icon.Width, icon.Height);
				x += icon.Width + 2;
			}
			DrawText(e, fileNameText, SystemBrushes.WindowText, RegularBigFont, ref x);
			DrawText(e, occurences, SystemBrushes.GrayText,  ItalicBigFont, ref x);
		}
		
		public void AddMark(SDBookmark mark)
		{
			int index = -1;
			for (int i = 0; i < marks.Count; ++i) {
				if (mark.LineNumber < marks[i].LineNumber) {
					index = i;
					break;
				}
			}
			if (index < 0)
				marks.Add(mark);
			else
				marks.Insert(index, mark);
			
			if (isInitialized) {
				BookmarkNode newNode = new BookmarkNode(mark);
				if (index < 0)
					Nodes.Add(newNode);
				else
					Nodes.Insert(index, newNode);
				newNode.EnsureVisible();
			}
			SetText();
		}
		
		public void RemoveMark(SDBookmark mark)
		{
			marks.Remove(mark);
			if (isInitialized) {
				for (int i = 0; i < Nodes.Count; ++i) {
					if (((BookmarkNode)Nodes[i]).Bookmark == mark) {
						Nodes.RemoveAt(i);
						break;
					}
				}
			}
			SetText();
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();
			if (marks.Count > 0) {
//				IDocument document = marks[0].Document;
//				if (document != null && document.HighlightingStrategy == null) {
//					document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);
//				}
				foreach (SDBookmark mark in marks) {
					TreeNode newResult = new BookmarkNode(mark);
					Nodes.Add(newResult);
				}
			}
		}
	}
}
