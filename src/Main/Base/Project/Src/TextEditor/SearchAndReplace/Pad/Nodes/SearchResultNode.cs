using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class SearchResultNode : ExtTreeNode
	{
		IDocument    document;
		SearchResult result;
		SizeF  spaceSize;
		static StringFormat sf = (StringFormat)System.Drawing.StringFormat.GenericTypographic.Clone();
		
		LineSegment line;
		
		Point startPosition;
		Point endPosition;
		string positionText;
		bool showFileName = false;
		public bool ShowFileName {
			get {
				return showFileName;
			}
			set {
				showFileName = value;
				if (showFileName) {
					Text = positionText + document.GetText(line) + FileNameText;
				} else {
					Text = positionText + document.GetText(line);
				}
			}
		}
		
		string FileNameText {
			get {
				return " in " + Path.GetFileName(result.FileName) + "(" + Path.GetDirectoryName(result.FileName) +")";
			}
		}
		public SearchResultNode(IDocument document, SearchResult result)
		{
			drawDefault = false;
			this.document = document;
			this.result = result;
			startPosition = document.OffsetToPosition(result.Offset);
			endPosition   = document.OffsetToPosition(result.Offset + result.Length);
			positionText =  "(" + startPosition.Y + ", " + startPosition.X + ") ";
			
			line = document.GetLineSegment(startPosition.Y);
			Text = positionText + document.GetText(line);
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int x = MeasureTextWidth(g, positionText, BoldMonospacedFont);
			x += MeasureTextWidth(g, document.GetText(line).Replace("\t", "   "), BoldMonospacedFont);
			if (ShowFileName) {
				x += MeasureTextWidth(g, FileNameText, ItalicFont);
			}
			return x;
		}
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			float x = e.Bounds.X;
			DrawText(g, positionText, Brushes.Black, Font, ref x, e.Bounds.Y);
			
			spaceSize = g.MeasureString("-", Font,  new PointF(0, 0), StringFormat.GenericTypographic);
			
			x += DrawLine(g, line, e.Bounds.Y, x);
			
		}
		
		public override void ActivateItem()
		{
			FileService.JumpToFilePosition(result.FileName, startPosition.Y, startPosition.X);
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
		float DrawLine(Graphics g, LineSegment line, float yPos, float xPos)
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
							int offset1 = Math.Min(word.Length, Math.Max(0, startPosition.X - logicalX));
							int offset2 = Math.Max(offset1, Math.Min(word.Length, endPosition.X - logicalX));
							
							xPos += DrawDocumentWord(g,
							                         word.Word.Substring(0, offset1),
							                         new PointF(xPos, yPos),
							                         MonospacedFont,
							                         word.Color
							                        );
							
							xPos += DrawDocumentWord(g,
							                         word.Word.Substring(offset1, offset2 - offset1),
							                         new PointF(xPos, yPos),
							                         BoldMonospacedFont,
							                         word.Color
							                        );
							
							xPos += DrawDocumentWord(g,
							                         word.Word.Substring(offset2),
							                         new PointF(xPos, yPos),
							                         MonospacedFont,
							                         word.Color
							                        );
							
							logicalX += word.Word.Length;
							break;
					}
				}
			} else {
				xPos += DrawDocumentWord(g,
				                 document.GetText(line),
				                 new PointF(xPos, yPos),
				                 MonospacedFont,
				                 Color.Black
				                );
			}
			if (ShowFileName) {
				xPos += DrawDocumentWord(g,
				                 FileNameText,
				                 new PointF(xPos, yPos),
				                 ItalicMonospacedFont,
				                 Color.Gray
				                );
			}
			return xPos;
		}
	}
}
