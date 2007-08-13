// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class SearchResultNode : ExtTreeNode
	{
		SearchResultMatch result;
		
		TextLocation startPosition;
		string positionText;
		string displayText;
		string specialText;
		bool showFileName = false;
		DrawableLine drawableLine;
		
		public bool ShowFileName {
			get {
				return showFileName;
			}
			set {
				showFileName = value;
				if (showFileName) {
					Text = displayText + FileNameText;
				} else {
					Text = displayText;
				}
			}
		}
		
		string FileNameText {
			get {
				return StringParser.Parse(" ${res:MainWindow.Windows.SearchResultPanel.In} ")
					+ Path.GetFileName(result.FileName) + "(" + Path.GetDirectoryName(result.FileName) +")";
			}
		}
		
		public SearchResultNode(IDocument document, SearchResultMatch result)
		{
			drawDefault = false;
			this.result = result;
			startPosition = result.GetStartPosition(document);
			TextLocation endPosition = result.GetEndPosition(document);
			positionText =  "(" + (startPosition.Y + 1) + ", " + (startPosition.X + 1) + ") ";
			
			LineSegment line = document.GetLineSegment(startPosition.Y);
			drawableLine = new DrawableLine(document, line, RegularMonospacedFont, BoldMonospacedFont);
			drawableLine.SetBold(0, drawableLine.LineLength, false);
			if (startPosition.Y == endPosition.Y) {
				drawableLine.SetBold(startPosition.X, endPosition.X, true);
			}
			
			specialText = result.DisplayText;
			if (specialText != null) {
				displayText = positionText + specialText;
			} else {
				displayText = positionText + document.GetText(line).Replace("\t", "    ");
			}
			Text = displayText;
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int x = MeasureTextWidth(g, displayText, BoldMonospacedFont);
			if (ShowFileName) {
				float tabWidth = drawableLine.GetSpaceSize(g).Width * 6;
				x = (int)((int)((x + 2 + tabWidth) / tabWidth) * tabWidth);
				x += MeasureTextWidth(g, FileNameText, ItalicBigFont);
			}
			return x;
		}
		
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			float x = e.Bounds.X;
			DrawText(e, positionText, SystemBrushes.WindowText, RegularBigFont, ref x);
			
			if (specialText != null) {
				DrawText(e, specialText, SystemBrushes.WindowText, RegularBigFont, ref x);
			} else {
				x -= e.Bounds.X;
				drawableLine.DrawLine(g, ref x, e.Bounds.X, e.Bounds.Y, GetTextColor(e.State, Color.Empty));
			}
			if (ShowFileName) {
				float tabWidth = drawableLine.GetSpaceSize(g).Width * 6;
				x = (int)((int)((x + 2 + tabWidth) / tabWidth) * tabWidth);
				x += e.Bounds.X;
				DrawText(e,
				         FileNameText,
				         SystemBrushes.GrayText,
				         ItalicBigFont,
				         ref x);
			}
		}
		
		public override void ActivateItem()
		{
			FileService.JumpToFilePosition(result.FileName, startPosition.Y, startPosition.X);
		}
	}
}
