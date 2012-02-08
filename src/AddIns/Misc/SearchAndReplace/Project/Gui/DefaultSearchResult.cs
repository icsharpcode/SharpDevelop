// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	/// <summary>
	/// Implements ISearchResult and provides the ResultsTreeView.
	/// </summary>
	public class DefaultSearchResult : ISearchResult
	{
		IList<SearchResultMatch> matches;
		protected SearchRootNode rootNode;
		
		protected DefaultSearchResult()
		{
		}
		
		public DefaultSearchResult(string title, IEnumerable<SearchResultMatch> matches)
		{
			if (title == null)
				throw new ArgumentNullException("title");
			if (matches == null)
				throw new ArgumentNullException("matches");
			this.matches = matches.ToArray();
			rootNode = new SearchRootNode(title, this.matches);
		}
		
		public string Text {
			get {
				return rootNode.Title + " (" + SearchRootNode.GetOccurrencesString(rootNode.Occurrences) + SearchRootNode.GetWasCancelledString(rootNode.WasCancelled) + ")";
			}
		}
		
		protected static ResultsTreeView resultsTreeViewInstance;
		
		public virtual object GetControl()
		{
			WorkbenchSingleton.AssertMainThread();
			if (resultsTreeViewInstance == null)
				resultsTreeViewInstance = new ResultsTreeView();
			rootNode.GroupResultsByFile(ResultsTreeView.GroupResultsByFile);
			resultsTreeViewInstance.ItemsSource = new object[] { rootNode };
			return resultsTreeViewInstance;
		}
		
		static IList toolbarItems;
		static MenuItem flatItem, perFileItem;
		
		public virtual IList GetToolbarItems()
		{
			WorkbenchSingleton.AssertMainThread();
			return GetDefaultToolbarItems();
		}
		
		protected IList GetDefaultToolbarItems()
		{
			if (toolbarItems == null) {
				toolbarItems = new List<object>();
				DropDownButton perFileDropDown = new DropDownButton();
				perFileDropDown.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.FindIcon") };
				perFileDropDown.SetValueToExtension(DropDownButton.ToolTipProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.SelectViewMode.ToolTip"));
				
				flatItem = new MenuItem();
				flatItem.SetValueToExtension(MenuItem.HeaderProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.Flat"));
				flatItem.Click += delegate { SetPerFile(false); };
				
				perFileItem = new MenuItem();
				perFileItem.SetValueToExtension(MenuItem.HeaderProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.PerFile"));
				perFileItem.Click += delegate { SetPerFile(true); };
				
				perFileDropDown.DropDownMenu = new ContextMenu();
				perFileDropDown.DropDownMenu.Items.Add(flatItem);
				perFileDropDown.DropDownMenu.Items.Add(perFileItem);
				toolbarItems.Add(perFileDropDown);
				toolbarItems.Add(new Separator());
				
				Button expandAll = new Button();
				expandAll.SetValueToExtension(Button.ToolTipProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.ExpandAll.ToolTip"));
				expandAll.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.OpenAssembly") };
				expandAll.Click += delegate { ExpandCollapseAll(true); };
				toolbarItems.Add(expandAll);
				
				Button collapseAll = new Button();
				collapseAll.SetValueToExtension(Button.ToolTipProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.CollapseAll.ToolTip"));
				collapseAll.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.Assembly") };
				collapseAll.Click += delegate { ExpandCollapseAll(false); };
				toolbarItems.Add(collapseAll);
			}
			return toolbarItems;
		}
		
		static void ExpandCollapseAll(bool newIsExpanded)
		{
			if (resultsTreeViewInstance != null) {
				foreach (SearchNode node in resultsTreeViewInstance.ItemsSource.OfType<SearchNode>().Flatten(n => n.Children)) {
					node.IsExpanded = newIsExpanded;
				}
			}
		}
		
		static void SetPerFile(bool perFile)
		{
			ResultsTreeView.GroupResultsByFile = perFile;
			if (resultsTreeViewInstance != null) {
				foreach (SearchRootNode node in resultsTreeViewInstance.ItemsSource.OfType<SearchRootNode>()) {
					node.GroupResultsByFile(perFile);
				}
			}
		}
	}
	
	public class DefaultSearchResultFactory : ISearchResultFactory
	{
		public ISearchResult CreateSearchResult(string title, IEnumerable<SearchResultMatch> matches)
		{
			return new DefaultSearchResult(title, matches);
		}
		
		public ISearchResult CreateSearchResult(string title, IObservable<SearchedFile> matches)
		{
			var osr = new ObserverSearchResult(title);
			osr.Registration = matches.ObserveOnUIThread().Subscribe(osr);
			return osr;
		}
	}
}
