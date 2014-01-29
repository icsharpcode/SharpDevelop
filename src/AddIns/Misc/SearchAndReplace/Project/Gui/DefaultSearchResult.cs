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
			SD.MainThread.VerifyAccess();
			if (resultsTreeViewInstance == null)
				resultsTreeViewInstance = new ResultsTreeView();
			rootNode.GroupResultsBy(ResultsTreeView.GroupingKind);
			resultsTreeViewInstance.ItemsSource = new object[] { rootNode };
			return resultsTreeViewInstance;
		}
		
		public virtual void OnDeactivate()
		{
		}
		
		static IList<object> toolbarItems;
		
		public virtual IList GetToolbarItems()
		{
			SD.MainThread.VerifyAccess();
			return GetDefaultToolbarItems();
		}
		
		protected IList GetDefaultToolbarItems()
		{
			if (toolbarItems == null) {
				toolbarItems = new List<object>();
				DropDownButton perFileDropDown = new DropDownButton();
				perFileDropDown.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.FindIcon") };
				perFileDropDown.SetValueToExtension(DropDownButton.ToolTipProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.SelectViewMode.ToolTip"));
				
				MenuItem flatItem = new MenuItem();
				flatItem.IsCheckable = true;
				flatItem.SetValueToExtension(MenuItem.HeaderProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.Flat"));
				flatItem.Click += delegate {
					SetResultGrouping();
					SetCheckedItem(flatItem, perFileDropDown.DropDownMenu);
				};
				
				MenuItem perFileItem = new MenuItem();
				perFileItem.IsCheckable = true;
				perFileItem.SetValueToExtension(MenuItem.HeaderProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.PerFile"));
				perFileItem.Click += delegate {
					SetResultGrouping(SearchResultGroupingKind.PerFile);
					SetCheckedItem(perFileItem, perFileDropDown.DropDownMenu);
				};
				
				MenuItem perProjectItem = new MenuItem();
				perProjectItem.IsCheckable = true;
				perProjectItem.SetValueToExtension(MenuItem.HeaderProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.PerProject"));
				perProjectItem.Click += delegate {
					SetResultGrouping(SearchResultGroupingKind.PerProject);
					SetCheckedItem(perProjectItem, perFileDropDown.DropDownMenu);
				};
				
				MenuItem perProjectAndFileItem = new MenuItem();
				perProjectAndFileItem.IsCheckable = true;
				perProjectAndFileItem.SetValueToExtension(MenuItem.HeaderProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.PerProjectAndFile"));
				perProjectAndFileItem.Click += delegate {
					SetResultGrouping(SearchResultGroupingKind.PerProjectAndFile);
					SetCheckedItem(perProjectAndFileItem, perFileDropDown.DropDownMenu);
				};
				
				perFileDropDown.DropDownMenu = new ContextMenu();
				perFileDropDown.DropDownMenu.Items.Add(flatItem);
				perFileDropDown.DropDownMenu.Items.Add(perFileItem);
				perFileDropDown.DropDownMenu.Items.Add(perProjectItem);
				perFileDropDown.DropDownMenu.Items.Add(perProjectAndFileItem);
				toolbarItems.Add(perFileDropDown);
				toolbarItems.Add(new Separator());
				
				Button expandAll = new Button();
				expandAll.SetValueToExtension(Button.ToolTipProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.ExpandAll.ToolTip"));
				expandAll.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.OpenCollection") };
				expandAll.Click += delegate { ExpandCollapseAll(true); };
				toolbarItems.Add(expandAll);
				
				Button collapseAll = new Button();
				collapseAll.SetValueToExtension(Button.ToolTipProperty, new LocalizeExtension("MainWindow.Windows.SearchResultPanel.CollapseAll.ToolTip"));
				collapseAll.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.Collection") };
				collapseAll.Click += delegate { ExpandCollapseAll(false); };
				toolbarItems.Add(collapseAll);
			}
			// Copy the list to avoid modifications to the static list
			return new List<object>(toolbarItems);
		}
		
		static void SetCheckedItem(MenuItem newTarget, ContextMenu menu)
		{
			foreach (var item in menu.Items.OfType<MenuItem>()) {
				item.IsChecked = false;
			}
			newTarget.IsChecked = true;
		}
		
		static void ExpandCollapseAll(bool newIsExpanded)
		{
			if (resultsTreeViewInstance != null) {
				foreach (SearchNode node in resultsTreeViewInstance.ItemsSource.OfType<SearchNode>().Flatten(n => n.Children)) {
					node.IsExpanded = newIsExpanded;
				}
			}
		}
		
		static void SetResultGrouping(SearchResultGroupingKind grouping = SearchResultGroupingKind.Flat)
		{
			ResultsTreeView.GroupingKind = grouping;
			if (resultsTreeViewInstance != null) {
				foreach (SearchRootNode node in resultsTreeViewInstance.ItemsSource.OfType<SearchRootNode>()) {
					node.GroupResultsBy(grouping);
				}
			}
		}
	}
	
	public enum SearchResultGroupingKind
	{
		Flat,
		PerFile,
		PerProject,
		PerProjectAndFile
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
