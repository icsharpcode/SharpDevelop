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
		/// <remarks>
		/// Provider should only be changed once!
		/// </remarks>
		public ProfilingDataProvider Provider { get; set; }
		HierarchyList<CallTreeNodeViewModel> list;
		CallTreeNodeViewModel searchRoot, oldSearchResult;
		SingleTask task;
		
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

		void txtSearchKeyDown(object sender, KeyEventArgs e)
		{
			if (!string.IsNullOrEmpty(txtSearch.Text) && list.Count > 0) {
				CallTreeNodeViewModel result;
				// TODO: should we perform search in background?
				
				if (list.First().Search(txtSearch.Text, true, out result)) {
					result.IsSelected = true;
					if (oldSearchResult != null)
						oldSearchResult.IsSelected = false;
					oldSearchResult = result;
				}
			}
		}

		public QueryView()
		{
			InitializeComponent();
			this.IsVisibleChanged += delegate { this.ExecuteQuery(); };
			this.DataContext = this;
			this.task = new SingleTask(this.Dispatcher);
			this.treeView.SizeChanged += delegate(object sender, SizeChangedEventArgs e) {
				if (e.NewSize.Width > 0 && e.PreviousSize.Width > 0 &&
				    (nameColumn.Width + (e.NewSize.Width - e.PreviousSize.Width)) > 0) {
					if ((nameColumn.Width + (e.NewSize.Width - e.PreviousSize.Width)) >=
					    (e.NewSize.Width - this.callCountColumn.Width - this.percentColumn.Width - this.timeSpentColumn.Width))
						this.nameColumn.Width = e.NewSize.Width - this.callCountColumn.Width - this.percentColumn.Width - this.timeSpentColumn.Width - 25;
					else
						nameColumn.Width += (e.NewSize.Width - e.PreviousSize.Width);
				}
			};
		}
		
		public void SetRange(int start, int end)
		{
			if (this.Provider == null)
				return;
			
			this.RangeStart = start;
			this.RangeEnd = end;
			this.searchRoot = new CallTreeNodeViewModel(this.Provider.GetRoot(start, end), null);
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
			if (!this.isDirty)
				return;
			
			if (RangeStart > RangeEnd) {
				int help = RangeStart;
				RangeStart = RangeEnd;
				RangeEnd = help;
			}

			if ((RangeEnd < 0 && RangeEnd >= Provider.DataSets.Count) &&
			    (RangeStart < 0 && RangeStart >= Provider.DataSets.Count))
				return;

			Debug.Assert(RangeStart <= RangeEnd);
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
			WaitBar bar = new WaitBar("Refreshing view, please wait ...");
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
			if (compiler.Compile()) {
				var data = compiler.ExecuteQuery(provider, rangeStart, rangeEnd - 1);
				var nodes = data.Select(i => new CallTreeNodeViewModel(i, null)).ToList();
				return new HierarchyList<CallTreeNodeViewModel>(nodes);
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
		
		public ContextMenu TreeViewContextMenu {
			get { return this.treeView.ContextMenu; }
			set { this.treeView.ContextMenu = this.ringDiagram.ContextMenu = value; }
		}
	}
}