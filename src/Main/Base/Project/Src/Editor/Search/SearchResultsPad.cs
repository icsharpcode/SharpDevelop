// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	/// <summary>
	/// Pad that displays search results.
	/// </summary>
	public class SearchResultsPad : AbstractPadContent
	{
		static SearchResultsPad instance;
		
		public static SearchResultsPad Instance {
			get {
				if (instance == null) {
					WorkbenchSingleton.Workbench.GetPad(typeof(SearchResultsPad)).CreatePad();
				}
				return instance;
			}
		}
		
		DockPanel dockPanel;
		ToolBar toolBar;
		ContentControl contentPlaceholder;
		IList defaultToolbarItems;
		
		public SearchResultsPad()
		{
			if (instance != null)
				throw new InvalidOperationException("Cannot create multiple instances");
			instance = this;
			toolBar = new ToolBar();
			ToolBarTray.SetIsLocked(toolBar, true);
			defaultToolbarItems = ToolBarService.CreateToolBarItems(this, "/SharpDevelop/Pads/SearchResultPad/Toolbar");
			foreach (object toolBarItem in defaultToolbarItems) {
				toolBar.Items.Add(toolBarItem);
			}
			
			DockPanel.SetDock(toolBar, Dock.Top);
			contentPlaceholder = new ContentControl();
			dockPanel = new DockPanel {
				Children = { toolBar, contentPlaceholder }
			};
		}
		
		public override object Control {
			get {
				return dockPanel;
			}
		}
		
		List<ISearchResult> lastSearches = new List<ISearchResult>();
		
		public IEnumerable<ISearchResult> LastSearches {
			get { return lastSearches; }
		}
		
		public void ClearLastSearchesList()
		{
			lastSearches.Clear();
		}
		
		public void ShowSearchResults(ISearchResult result)
		{
			if (result == null)
				throw new ArgumentNullException("result");
			
			// move result to top of last searches
			lastSearches.Remove(result);
			lastSearches.Insert(0, result);
			
			contentPlaceholder.SetContent(result.GetControl());
			
			toolBar.Items.Clear();
			foreach (object toolBarItem in defaultToolbarItems) {
				toolBar.Items.Add(toolBarItem);
			}
			IList additionalToolbarItems = result.GetToolbarItems();
			if (additionalToolbarItems != null) {
				toolBar.Items.Add(new Separator());
				foreach (object toolBarItem in additionalToolbarItems) {
					toolBar.Items.Add(toolBarItem);
				}
			}
			
			SearchResultsShown.RaiseEvent(this, EventArgs.Empty);
		}
		
		public void ShowSearchResults(string pattern, IEnumerable<SearchResultMatch> matches)
		{
			ShowSearchResults(CreateSearchResult(pattern, matches));
		}
		
		public event EventHandler SearchResultsShown;
		
		public static ISearchResult CreateSearchResult(string pattern, IEnumerable<SearchResultMatch> matches)
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			if (matches == null)
				throw new ArgumentNullException("matches");
			foreach (ISearchResultFactory factory in AddInTree.BuildItems<ISearchResultFactory>("/SharpDevelop/Pads/SearchResultPad/Factories", null)) {
				ISearchResult result = factory.CreateSearchResult(pattern, matches);
				if (result != null)
					return result;
			}
			return new DummySearchResult { Text = pattern };
		}
		
		sealed class DummySearchResult : ISearchResult
		{
			public string Text {
				get; set;
			}
			
			public object GetControl()
			{
				return "Could not find ISearchResultFactory. Did you disable the search+replace addin?";
			}
			
			public IList GetToolbarItems()
			{
				return null;
			}
		}
	}
}
