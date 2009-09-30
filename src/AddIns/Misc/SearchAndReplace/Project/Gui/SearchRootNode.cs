// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;

namespace SearchAndReplace
{
	sealed class SearchRootNode : SearchNode
	{
		IList<SearchResultNode> results;
		int fileCount;
		
		public string Title { get; private set; }
		
		public SearchRootNode(string title, IList<SearchResultMatch> results)
		{
			this.Title = title;
			this.results = results.Select(r => new SearchResultNode(r)).ToArray();
			
			fileCount = results.GroupBy(r => r.FileName).Count();
			this.Children = this.results;
			this.IsExpanded = true;
		}
		
		public int Occurrences {
			get { return results.Count; }
		}
		
		protected override object CreateText()
		{
			return new TextBlock {
				Inlines = {
					new Bold(new Run(this.Title)),
					new Run(" (" + GetOccurrencesString(results.Count)
					        + StringParser.Parse(" ${res:MainWindow.Windows.SearchResultPanel.In} ")
					        + GetFileCountString(fileCount) + ")")
				}
			};
		}
		
		public static string GetOccurrencesString(int count)
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
	}
}
