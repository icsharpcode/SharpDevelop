// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Controller.Queries;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// Interaktionslogik für QueryView.xaml
	/// </summary>
	public partial class QueryView : UserControl
	{
		#region Properties
		/// <remarks>
		/// Provider should only be changed once!
		/// </remarks>
		public ProfilingDataProvider Provider { get; set; }
		HierarchyList<CallTreeNodeViewModel> list;
		CallTreeNodeViewModel oldSearchResult;
		SingleTask task;
		SingleTask searchTask;
		
		bool isQueryModifiable = true;
		
		public int RangeStart { get; set; }
		public int RangeEnd { get; set; }
		
		public bool IsQueryModifiable {
			get { return isQueryModifiable; }
			set {
				isQueryModifiable = value;
				txtQuery.IsReadOnly = !isQueryModifiable;
			}
		}
		
		bool isDirty;
		
		public ErrorReporter Reporter { get; set; }
		
		public event EventHandler CurrentQueryChanged;
		
		protected virtual void OnCurrentQueryChanged(EventArgs e)
		{
			if (CurrentQueryChanged != null) {
				CurrentQueryChanged(this, e);
			}
		}
		
		public IEnumerable<CallTreeNodeViewModel> SelectedItems {
			get {
				if (list == null)
					return new List<CallTreeNodeViewModel>().AsEnumerable();
				return from i in list where i.IsSelected select i;
			}
		}
		
		public static readonly DependencyProperty ShowQueryItemsProperty = DependencyProperty.Register(
			"ShowQueryItems", typeof(bool), typeof(QueryView));
		
		public bool ShowQueryItems {
			set { SetValue(ShowQueryItemsProperty, value); }
			get { return (bool)GetValue(ShowQueryItemsProperty); }
		}
		
		public static readonly DependencyProperty TranslationProperty = DependencyProperty.Register(
			"Translation", typeof(ControlsTranslation), typeof(QueryView));
		
		public ControlsTranslation Translation {
			set { SetValue(TranslationProperty, value); }
			get { return (ControlsTranslation)GetValue(TranslationProperty); }
		}
		#endregion

		void txtSearchKeyDown(object sender, KeyEventArgs e)
		{
			if (!string.IsNullOrEmpty(txtSearch.Text) && list.Count > 0) {
				searchTask.Cancel();
				string text = txtSearch.Text;
				int start = RangeStart;
				int end = RangeEnd;
				var provider = Provider;
				
				AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
				OverlayAdorner ad = new OverlayAdorner(this);
				WaitBar bar = new WaitBar(Translation.WaitBarText);
				ad.Child = bar;
				layer.Add(ad);
				
				searchTask.Execute(
					() => DoSearchInBackground(list.Roots.Select(i => i.Node).ToList(), start, end, text, true),
					result => SearchCompleted(result, layer, ad),
					delegate { layer.Remove(ad); });
			}
		}
		
		struct SearchInfo {
			public CallTreeNode Result;
			public CallTreeNode ResultRoot;
		}
		
		static SearchInfo? DoSearchInBackground(IList<CallTreeNode> nodes, int startIndex, int endIndex, string text, bool recursive)
		{
			foreach (var item in nodes) {
				CallTreeNode result = item.Descendants.FirstOrDefault(desc => desc.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
				if (result != null) {
					return new SearchInfo() { Result = result, ResultRoot = item };
				}
			}
			return null;
		}
		
		void SearchCompleted(SearchInfo? result, AdornerLayer layer, OverlayAdorner ad)
		{
			if (!result.HasValue)
				return;
			
			CallTreeNodeViewModel item = GetViewModelFromPath(result.Value.Result.GetPathRelativeTo(result.Value.ResultRoot), result.Value);
			if (item != null) {
				item.IsSelected = true;
				if (oldSearchResult != null)
					oldSearchResult.IsSelected = false;
				oldSearchResult = item;
			}
			layer.Remove(ad);
		}
		
		CallTreeNodeViewModel GetViewModelFromPath(IEnumerable<NodePath> paths, SearchInfo info)
		{
			CallTreeNodeViewModel result = null;
			var parent = list.Roots.FirstOrDefault(i => i.Node.Equals(info.ResultRoot));

			foreach (var path in paths) {
				var items = parent.Children;
				foreach (var pathId in path.Skip(1)) {
					foreach (var item in items) {
						if (item.Node.NameMapping.Id == pathId) {
							items = item.Children;
							result = item;
							break;
						}
					}
					if (result == null)
						break;
				}
			}
			
			return result;
		}
		
		class GridViewColumnModel
		{
			public GridViewColumnModel(GridViewColumn column)
			{
				this.Column = column;
			}
			
			public GridViewColumn Column { get; private set; }
			double width;
			
			public bool IsVisible {
				get { return Column.Width > 0.1; }
				set {
					if (value)
						Column.Width = width;
					else {
						width = Column.Width;
						Column.Width = 0;
					}
				}
			}
		}

		public QueryView()
		{
			InitializeComponent();
			this.IsVisibleChanged += delegate { this.ExecuteQuery(); };
			this.DataContext = this;
			this.task = new SingleTask(this.Dispatcher);
			this.searchTask = new SingleTask(this.Dispatcher);
			
			this.Translation = new ControlsTranslation();
			this.visibleColumnsSelection.ItemsSource = this.gridView.Columns.Select(col => new GridViewColumnModel(col));
			
			this.treeView.SizeChanged += QueryView_SizeChanged;
		}

		void QueryView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width > 0 && e.PreviousSize.Width > 0) {
				double adjustedNameColumnWidth = nameColumn.Width + e.NewSize.Width - e.PreviousSize.Width;
				double matchingNameColumnWidth = e.NewSize.Width - callCountColumn.Width
					- percentColumn.Width - timeSpentColumn.Width
					- timeSpentSelfColumn.Width - timeSpentPerCallColumn.Width
					- timeSpentSelfPerCallColumn.Width - 25;
				
				// always keep name column at least 75 pixels wide
				if (matchingNameColumnWidth < 75)
					matchingNameColumnWidth = 75;
				
				if (e.NewSize.Width >= e.PreviousSize.Width) {
					// treeView got wider: also make name column wider if there's space free
					if (adjustedNameColumnWidth <= matchingNameColumnWidth)
						nameColumn.Width = adjustedNameColumnWidth;
				} else {
					// treeView got smaller: make column smaller unless there's space free
					if (adjustedNameColumnWidth >= matchingNameColumnWidth)
						nameColumn.Width = adjustedNameColumnWidth;
				}
			}
		}
		
		public void SetRange(int start, int end)
		{
			if (Provider == null)
				return;
			
			RangeStart = start;
			RangeEnd = end;
			Invalidate();
			InvalidateArrange();
		}
		
		public void Invalidate()
		{
			isDirty = true;
			if (IsVisible)
				ExecuteQuery();
		}
		
		void ExecuteQuery()
		{
			if (!isDirty || Provider == null)
				return;
			
			if (Provider.DataSets.Count == 0)
				return;
			
			RangeEnd = Math.Min(RangeEnd, Provider.DataSets.Count - 1);
			RangeStart = Math.Max(RangeStart, 0);
			
			Stopwatch watch = new Stopwatch();
			watch.Start();
			LoadData();
			watch.Stop();
			Debug.Print("update finished in {0}ms", watch.ElapsedMilliseconds);
			isDirty = false;
		}
		
		void LoadData()
		{
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
			OverlayAdorner ad = new OverlayAdorner(this);
			WaitBar bar = new WaitBar(Translation.WaitBarText);
			ad.Child = bar;
			layer.Add(ad);
			int rangeStart = RangeStart;
			int rangeEnd = RangeEnd;
			string query = CurrentQuery;
			
			ProfilingDataProvider provider = Provider;
			QueryCompiler compiler = new QueryCompiler(Reporter, query);
			ringDiagram.SelectedRoot = null;
			
			task.Execute(() => LoadWorker(provider, compiler, rangeStart, rangeEnd),
			             list => LoadCompleted(list, layer, ad),
			             delegate { layer.Remove(ad); });
		}
		
		public RingDiagramControl RingDiagram {
			get { return ringDiagram; }
		}

		static HierarchyList<CallTreeNodeViewModel> LoadWorker(ProfilingDataProvider provider, QueryCompiler compiler, int rangeStart, int rangeEnd)
		{
			try {
				if (compiler.Compile()) {
					IEnumerable<CallTreeNode> data = compiler.ExecuteQuery(provider, rangeStart, rangeEnd);
					#if DEBUG
					data = data.WithQueryLog(Console.Out);
					#endif
					var nodes = data.Select(i => new CallTreeNodeViewModel(i, null)).ToList();
					return new HierarchyList<CallTreeNodeViewModel>(nodes);
				}
			} catch (ObjectDisposedException) {
				return null;
			}
			
			return null;
		}
		
		void LoadCompleted(HierarchyList<CallTreeNodeViewModel> list, AdornerLayer layer, OverlayAdorner ad)
		{
			layer.Remove(ad);
			treeView.ItemsSource = this.list = list;
			if (list != null && list.Count > 0) {
				ringDiagram.SelectedRoot = this.list[0];
				
				foreach (var item in list) {
					var currentItem = item;
					currentItem.RequestBringIntoView += (sender, e) => treeView.ScrollIntoView(e.Node);
				}
			}
		}
		
		public string CurrentQuery {
			get { return txtQuery.Text; }
			set {
				if (IsQueryModifiable)
					txtQuery.Text = value;
			}
		}

		void txtQueryTextChanged(object sender, TextChangedEventArgs e)
		{
			CurrentQuery = txtQuery.Text;
		}
		
		void txtQueryKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnExecuteQueryClick(null, null);
		}
		
		void btnExecuteQueryClick(object sender, RoutedEventArgs e)
		{
			OnCurrentQueryChanged(EventArgs.Empty);
			Invalidate();
		}
		
		void BtnExpandHotPathSubtreeClick(object sender, RoutedEventArgs e)
		{
			foreach (CallTreeNodeViewModel node in SelectedItems.ToArray()) {
				ExpandHotPathItems(node, node);
			}
		}
		
		void ExpandHotPathItems(CallTreeNodeViewModel parent, CallTreeNodeViewModel selectedRoot)
		{
			if ((parent.CpuCyclesSpent / (double)selectedRoot.CpuCyclesSpent) >= 0.2) {
				parent.IsExpanded = true;
				
				foreach (CallTreeNodeViewModel node in parent.Children)
					ExpandHotPathItems(node, selectedRoot);
			}
		}
		
		public ContextMenu TreeViewContextMenu {
			get { return treeView.ContextMenu; }
			set { treeView.ContextMenu = ringDiagram.ContextMenu = value; }
		}
	}
}
