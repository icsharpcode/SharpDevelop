// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		IList<SearchResultNode> resultNodes;
		IList<SearchFileNode> fileNodes;
		
		public string Title { get; private set; }
		
		public SearchRootNode(string title, IList<SearchResultMatch> results)
		{
			this.Title = title;
			this.resultNodes = results.Select(r => new SearchResultNode(r)).ToArray();
			this.fileNodes = resultNodes.GroupBy(r => r.FileName).Select(g => new SearchFileNode(g.Key, g.ToArray())).ToArray();
			
			this.Children = this.resultNodes;
			this.IsExpanded = true;
		}
		
		public void GroupResultsByFile(bool perFile)
		{
			if (perFile)
				this.Children = fileNodes;
			else
				this.Children = resultNodes;
			foreach (SearchResultNode node in resultNodes) {
				node.ShowFileName = !perFile;
			}
		}
		
		public int Occurrences {
			get { return resultNodes.Count; }
		}
		
		protected override object CreateText()
		{
			return new TextBlock {
				Inlines = {
					new Bold(new Run(this.Title)),
					new Run(" (" + GetOccurrencesString(resultNodes.Count)
					        + StringParser.Parse(" ${res:MainWindow.Windows.SearchResultPanel.In} ")
					        + GetFileCountString(fileNodes.Count) + ")")
				}
			};
		}
		
		public static string GetOccurrencesString(int count)
		{
			if (count == 1) {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OneOccurrence}");
			} else {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OccurrencesCount}",
				                          new StringTagPair("Count", count.ToString()));
			}
		}
		
		public static string GetFileCountString(int count)
		{
			if (count == 1) {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OneFile}");
			} else {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.FileCount}",
				                          new StringTagPair("Count", count.ToString()));
			}
		}
	}
}
