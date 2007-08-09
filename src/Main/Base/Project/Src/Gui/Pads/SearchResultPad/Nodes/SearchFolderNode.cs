// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class SearchFolderNode : ExtFolderNode
	{
		List<SearchResultMatch> results = new List<SearchResultMatch>();
		string fileName;
		string occurences;
		Image icon;
		
		public List<SearchResultMatch> Results { 
			get {
				return results;
			}
		}
		
		public SearchFolderNode(string fileName)
		{
			drawDefault = false;
			this.fileName = fileName;
			icon = IconService.GetBitmap(IconService.GetImageForFile(fileName));
			Nodes.Add(new TreeNode());
		}
		
		public void SetText()
		{
			if (results.Count == 1) {
				occurences = " (1 occurence)";
			} else {
				occurences = " (" + results.Count + " occurences)";
			}
			
			this.Text = fileName + occurences;
		}
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			int x = MeasureTextWidth(g, fileName, RegularBigFont);
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
			DrawText(e, fileName, SystemBrushes.WindowText, RegularBigFont, ref x);
			DrawText(e, occurences, SystemBrushes.GrayText,  ItalicBigFont, ref x);
		}
			
		protected override void Initialize()
		{
			Nodes.Clear();
			IDocument document = results[0].CreateDocument();
			if (document.HighlightingStrategy == null) {
				document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);
			}
			foreach (SearchResultMatch result in results) {
				TreeNode newResult = new SearchResultNode(document, result);
				Nodes.Add(newResult);
			}
		}
	}
}
