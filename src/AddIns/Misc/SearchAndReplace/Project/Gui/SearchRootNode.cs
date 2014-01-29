// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;

namespace SearchAndReplace
{
	public sealed class SearchRootNode : SearchNode
	{
		ObservableCollection<SearchResultNode> resultNodes;
		ObservableCollection<SearchFileNode> fileNodes;
		ObservableCollection<SearchProjectNode> projectNodes;
		ObservableCollection<SearchProjectNode> projectAndFileNodes;
		bool wasCancelled;
		
		public string Title { get; private set; }
		
		public bool WasCancelled {
			get {
				return wasCancelled;
			}
			set {
				wasCancelled = value;
				InvalidateText();
			}
		}
		
		public SearchRootNode(string title, IList<SearchResultMatch> results)
		{
			this.Title = title;
			this.resultNodes = new ObservableCollection<SearchResultNode>(results.Select(r => new SearchResultNode(r)));
			this.fileNodes = new ObservableCollection<SearchFileNode>(
				resultNodes.GroupBy(r => r.FileName)
				.Select(g => new SearchFileNode(g.Key, g.ToList()))
			);
			this.projectNodes = new ObservableCollection<SearchProjectNode>(
				resultNodes.GroupBy(r => SD.ProjectService.FindProjectContainingFile(r.FileName))
				.Select(g => new SearchProjectNode(g.Key, g.OfType<SearchNode>().ToList()))
			);
			this.projectAndFileNodes = new ObservableCollection<SearchProjectNode>(
				resultNodes.GroupBy(r => SD.ProjectService.FindProjectContainingFile(r.FileName))
				.Select(g => new SearchProjectNode(g.Key, g.GroupBy(r => r.FileName).Select(g2 => new SearchFileNode(g2.Key, g2.ToList())).OfType<SearchNode>().ToList()))
			);
			this.IsExpanded = true;
		}

		public void Add(SearchedFile searchedFile)
		{
			var results = searchedFile.Matches.Select(m => new SearchResultNode(m)).ToList();
			resultNodes.AddRange(results);
			this.fileNodes.Add(new SearchFileNode(searchedFile.FileName, results));
			foreach (var g in results.GroupBy(r => SD.ProjectService.FindProjectContainingFile(r.FileName))) {
				var p = projectNodes.FirstOrDefault(n => n.Project == g.Key);
				var p2 = projectAndFileNodes.FirstOrDefault(n => n.Project == g.Key);
				if (p == null) {
					projectNodes.Add(new SearchProjectNode(g.Key, g.OfType<SearchNode>().ToList()));
				} else {
					p.Children = new List<SearchNode>(p.Children.Concat(g.AsEnumerable()));
				}
				if (p2 == null) {
					projectAndFileNodes.Add(new SearchProjectNode(g.Key, g.GroupBy(r => r.FileName).Select(g2 => new SearchFileNode(g2.Key, g2.ToList())).OfType<SearchNode>().ToList()));
				} else {
					var f = p2.Children.OfType<SearchFileNode>().FirstOrDefault(n => n.FileName == searchedFile.FileName);
					if (f == null) {
						var list = new List<SearchNode>(p2.Children);
						list.Add(new SearchFileNode(searchedFile.FileName, g.ToList()));
						p2.Children = list;
					} else {
						f.Children = new List<SearchNode>(f.Children.Concat(g.AsEnumerable()));
					}
				}
			}
			InvalidateText();
		}
		
		public void GroupResultsBy(SearchResultGroupingKind kind)
		{
			bool perFile = false;
			switch (kind) {
				case SearchResultGroupingKind.Flat:
					this.Children = resultNodes;
					perFile = false;
					break;
				case SearchResultGroupingKind.PerFile:
					this.Children = fileNodes;
					perFile = true;
					break;
				case SearchResultGroupingKind.PerProject:
					this.Children = projectNodes;
					perFile = false;
					break;
				case SearchResultGroupingKind.PerProjectAndFile:
					this.Children = projectAndFileNodes;
					perFile = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
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
					        + GetFileCountString(fileNodes.Count) + GetWasCancelledString(WasCancelled) + ")")
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
		
		public static string GetWasCancelledString(bool wasCancelled)
		{
			if (wasCancelled)
				return "; was cancelled";
			
			return "";
		}
	}
}
