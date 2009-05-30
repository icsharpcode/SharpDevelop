// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core.Presentation;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Pad that displays search results.
	/// </summary>
	public class SearchResultPad : AbstractPadContent
	{
		static SearchResultPad instance;
		
		public static SearchResultPad Instance {
			get {
				if (instance == null) {
					WorkbenchSingleton.Workbench.GetPad(typeof(SearchResultPad)).CreatePad();
				}
				return instance;
			}
		}
		
		DockPanel dockPanel;
		ToolBar toolBar;
		ContentControl contentPlaceholder;
		
		public SearchResultPad()
		{
			if (instance != null)
				throw new InvalidOperationException("Cannot create multiple instances");
			instance = this;
			toolBar = ToolBarService.CreateToolBar(this, "/SharpDevelop/Pads/SearchResultPad/Toolbar");
			
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
			
			SearchResultsShown.RaiseEvent(this, EventArgs.Empty);
		}
		
		public event EventHandler SearchResultsShown;
	}
}
