using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace Bookmark
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class BookmarkFolderNode : ExtFolderNode
	{
		List<Bookmark> marks = new List<Bookmark>();
		string fileName;
		string occurences;
		Image icon;
		
		public List<Bookmark> Marks { 
			get {
				return marks;
			}
		}
		
		public BookmarkFolderNode(string fileName)
		{
			drawDefault = false;
			this.fileName = fileName;
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
			this.Text = fileName + occurences;
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int x = MeasureTextWidth(g, fileName, Font);
			x += MeasureTextWidth(g, occurences, ItalicFont);
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
				x += icon.Width;
			}
			DrawText(g, fileName, Brushes.Black, Font, ref x, e.Bounds.Y);
			DrawText(g, occurences, Brushes.Gray,  ItalicFont, ref x, e.Bounds.Y);
		}
		
		public void AddMark(Bookmark mark)
		{
			marks.Add(mark);
			
			if (isInitialized) {
				BookmarkNode newNode = new BookmarkNode(mark);
				Nodes.Add(newNode);
				newNode.EnsureVisible();
			}
			SetText();
		}
		
		public void RemoveMark(Bookmark mark)
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
				IDocument document = marks[0].Document;
				if (document.HighlightingStrategy == null) {
					document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);
				}
				foreach (Bookmark mark in marks) {
					TreeNode newResult = new BookmarkNode(mark);
					Nodes.Add(newResult);
				}
			}
		}
	}
}
