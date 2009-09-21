using ICSharpCode.Profiler.Controller.Data;
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
				this.txtQuery.IsReadOnly = !isQueryModifiable;
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
				if (this.list == null)
					return new List<CallTreeNodeViewModel>().AsEnumerable();
				return from i in this.list where i.IsSelected select i;
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
				int start = this.RangeStart;
				int end = this.RangeEnd;
				var provider = this.Provider;
				
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
			var parent = list.Roots.Where(i => i.Node.Equals(info.ResultRoot)).FirstOrDefault();

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

		public QueryView()
		{
			InitializeComponent();
			this.IsVisibleChanged += delegate { this.ExecuteQuery(); };
			this.DataContext = this;
			this.task = new SingleTask(this.Dispatcher);
			this.searchTask = new SingleTask(this.Dispatcher);
			
			this.Translation = new ControlsTranslation();
			
			this.treeView.SizeChanged += delegate(object sender, SizeChangedEventArgs e) {
				if (e.NewSize.Width > 0 && e.PreviousSize.Width > 0) {
					double adjustedNameColumnWidth = nameColumn.Width + e.NewSize.Width - e.PreviousSize.Width;
					double matchingNameColumnWidth = e.NewSize.Width - this.callCountColumn.Width
						- this.percentColumn.Width - this.timeSpentColumn.Width
						- this.timeSpentSelfColumn.Width - this.timeSpentPerCallColumn.Width
						- this.timeSpentSelfPerCallColumn.Width - 25;
					
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
			};
		}
		
		public void SetRange(int start, int end)
		{
			if (this.Provider == null)
				return;
			
			this.RangeStart = start;
			this.RangeEnd = end;
			this.Invalidate();
			this.InvalidateArrange();
		}
		
		public void Invalidate()
		{
			this.isDirty = true;
			if (this.IsVisible)
				ExecuteQuery();
		}
		
		void ExecuteQuery()
		{
			if (!this.isDirty || this.Provider == null)
				return;
			
			if (RangeStart > RangeEnd) {
				int help = RangeStart;
				RangeStart = RangeEnd;
				RangeEnd = help;
			}

			if ((RangeEnd < 0 && RangeEnd >= Provider.DataSets.Count) &&
			    (RangeStart < 0 && RangeStart >= Provider.DataSets.Count))
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
			this.isDirty = false;
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
			string query = this.CurrentQuery;
			
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
					var data = compiler.ExecuteQuery(provider, rangeStart, rangeEnd);
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
					currentItem.RequestBringIntoView += (sender, e) => this.treeView.ScrollIntoView(e.Node);
				}
			}
		}
		
		public string CurrentQuery {
			get { return this.txtQuery.Text; }
			set {
				if (IsQueryModifiable)
					this.txtQuery.Text = value;
			}
		}

		void txtQueryTextChanged(object sender, TextChangedEventArgs e)
		{
			this.CurrentQuery = this.txtQuery.Text;
		}
		
		void txtQueryKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnExecuteQueryClick(null, null);
		}
		
		void btnExecuteQueryClick(object sender, RoutedEventArgs e)
		{
			OnCurrentQueryChanged(EventArgs.Empty);
			this.Invalidate();
		}
		
		void BtnExpandHotPathSubtreeClick(object sender, RoutedEventArgs e)
		{
			foreach (CallTreeNodeViewModel node in this.SelectedItems.ToArray()) {
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
			get { return this.treeView.ContextMenu; }
			set { this.treeView.ContextMenu = this.ringDiagram.ContextMenu = value; }
		}
	}
}