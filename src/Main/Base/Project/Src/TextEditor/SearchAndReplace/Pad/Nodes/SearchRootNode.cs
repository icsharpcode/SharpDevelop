// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class SearchRootNode : ExtTreeNode
	{
		List<SearchResult> results;
		string             pattern;
		int fileCount;
		
		public List<SearchResult> Results {
			get {
				return results;
			}
		}
		
		public SearchRootNode(string pattern, List<SearchResult> results, int fileCount)
		{
			drawDefault = false;
			this.results = results;
			this.pattern = pattern;
			this.fileCount = fileCount;
			Text = GetText();
		}
		
		public static string GetOccurencesString(int count)
		{
			if (count == 1) {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OneOccurrence}");
			} else {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OccurrencesCount}", new string[,] {{"Count", count.ToString()}});
			}
		}
		
		public static string GetFileCountString(int count)
		{
			if (count == 1) {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OneFile}");
			} else {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.FileCount}", new string[,] {{"Count", count.ToString()}});
			}
		}
		
		string GetText()
		{
			return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OccurrencesOf}",
			                          new string[,] {{ "Pattern", pattern }})
				+ " (" + GetOccurencesString(results.Count) + " in " + GetFileCountString(fileCount) + ")";
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			return MeasureTextWidth(e.Graphics, GetText(), BoldFont);
		}
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			float x = e.Bounds.X;
			DrawText(g, GetText(), Brushes.Black, BoldFont, ref x, e.Bounds.Y);
		}
	}
}
