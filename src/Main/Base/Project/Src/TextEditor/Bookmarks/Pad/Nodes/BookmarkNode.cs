using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;

namespace Bookmark
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class BookmarkNode : ExtTreeNode
	{
		Bookmark bookmark;
		
		SizeF  spaceSize;
		static StringFormat sf = (StringFormat)System.Drawing.StringFormat.GenericTypographic.Clone();
		
		LineSegment line;
		
		string positionText;
		
		public Bookmark Bookmark {
			get {
				return bookmark;
			}
		}
		
		public BookmarkNode(Bookmark bookmark)
		{
			drawDefault = false;
			this.bookmark = bookmark;
			Tag = bookmark;
			Checked = bookmark.IsEnabled;
			positionText =  "(" + (bookmark.LineNumber + 1) + ") ";
			
			line = bookmark.Document.GetLineSegment(bookmark.LineNumber);
			Text = positionText + bookmark.Document.GetText(line);
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int x = MeasureTextWidth(g, positionText, BoldMonospacedFont);
			x += MeasureTextWidth(g, bookmark.Document.GetText(line).Replace("\t", "   "), BoldMonospacedFont);
			return x;
		}
		
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			float x = e.Bounds.X;
			DrawText(g, positionText, Brushes.Black, Font, ref x, e.Bounds.Y);
			
			spaceSize = g.MeasureString("-", Font,  new PointF(0, 0), StringFormat.GenericTypographic);
			
			DrawLine(g, line, e.Bounds.Y, x);
		}
		
		public override void ActivateItem()
		{
			FileService.JumpToFilePosition(bookmark.FileName, bookmark.LineNumber, 0);
		}
		
		float DrawDocumentWord(Graphics g, string word, PointF position, Font font, Color foreColor)
		{
			if (word == null || word.Length == 0) {
				return 0f;
			}
			SizeF wordSize = g.MeasureString(word, font, 32768, sf);
			
			g.DrawString(word,
			             font,
			             BrushRegistry.GetBrush(foreColor),
			             position,
			             sf);
			return wordSize.Width;
		}
		
		void DrawLine(Graphics g, LineSegment line, float yPos, float xPos)
		{
			int logicalX = 0;
			if (line.Words != null) {
				foreach (TextWord word in line.Words) {
					switch (word.Type) {
						case TextWordType.Space:
							xPos += spaceSize.Width;
							logicalX++;
							break;
						case TextWordType.Tab:
							xPos += spaceSize.Width * 4;
							logicalX++;
							break;
						case TextWordType.Word:
							xPos += DrawDocumentWord(g,
							                         word.Word,
							                         new PointF(xPos, yPos),
							                         word.Font.Style == FontStyle.Bold ? BoldMonospacedFont : MonospacedFont,
							                         word.Color
							                        );
							logicalX += word.Word.Length;
							break;
					}
				}
			} else {
				DrawDocumentWord(g,
				                 bookmark.Document.GetText(line),
				                 new PointF(xPos, yPos),
				                 MonospacedFont,
				                 Color.Black
				                );
			}
		}
	}
}
