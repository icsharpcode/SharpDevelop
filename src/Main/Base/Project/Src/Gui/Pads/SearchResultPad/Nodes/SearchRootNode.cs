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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class SearchRootNode : ExtTreeNode
	{
		IList<SearchResultMatch> results;
		string pattern;
		int fileCount;
		
		public IList<SearchResultMatch> Results {
			get {
				return results;
			}
		}
		
		public SearchRootNode(string pattern, IList<SearchResultMatch> results, int fileCount)
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
				+ " (" + GetOccurencesString(results.Count)
				+ StringParser.Parse(" ${res:MainWindow.Windows.SearchResultPanel.In} ")
				+ GetFileCountString(fileCount) + ")";
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			return MeasureTextWidth(e.Graphics, GetText(), BoldBigFont);
		}
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			DrawText(e, GetText(), SystemBrushes.WindowText, BoldBigFont);
		}
	}
}
