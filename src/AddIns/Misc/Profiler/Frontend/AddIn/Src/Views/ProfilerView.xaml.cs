using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

using ICSharpCode.Core.Presentation;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Controller.Queries;
using ICSharpCode.Profiler.Controls;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Profiler.AddIn.Views
{
	public partial class ProfilerView : UserControl
	{
		ProfilingDataProvider provider;

		public ProfilerView(ProfilingDataProvider provider)
		{
			InitializeComponent();
			this.provider = provider;
			
			this.timeLine.IsEnabled = true;
			this.timeLine.ValuesList.Clear();
			this.timeLine.ValuesList.AddRange(this.provider.DataSets.Select(i => new TimeLineInfo() { value = i.CpuUsage / 100, displayMarker = i.IsFirst }));
			this.timeLine.SelectedStartIndex = 0;
			this.timeLine.SelectedEndIndex = this.timeLine.ValuesList.Count;
			
			var translation = new SharpDevelopTranslation();
			
			foreach (TabItem item in this.tabView.Items) {
				if (item.Content != null) {
					QueryView view = item.Content as QueryView;
					view.Reporter = new ErrorReporter(UpdateErrorList);
					view.Provider = provider;
					view.Translation = translation;
					view.SetRange(this.timeLine.SelectedStartIndex, this.timeLine.SelectedEndIndex);
					view.ContextMenuOpening += delegate(object sender, ContextMenuEventArgs e) {
						object source = (e.OriginalSource is Shape) ? e.OriginalSource : view;
						MenuService.CreateContextMenu(source, "/AddIns/Profiler/QueryView/ContextMenu").IsOpen = true;
					};
				}
			}
			
			this.dummyTab.Header = new Image { Source = PresentationResourceService.GetImage("Icons.16x16.NewDocumentIcon").Source, Height = 16, Width = 16 };
			
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, ExecuteSelectAll, CanExecuteSelectAll));
			
			InitializeLastItems();
			InitializeOldTabs();
		}

		void TimeLineRangeChanged(object sender, RangeEventArgs e)
		{
			foreach (TabItem item in this.tabView.Items) {
				if (item != null && item.Content != null)
					((QueryView)item.Content).SetRange(e.StartIndex, e.EndIndex);
			}
		}
		
		void ExecuteSelectAll(object sender, ExecutedRoutedEventArgs e)
		{
			DoSelectAll();
			e.Handled = true;
		}

		void DoSelectAll()
		{
			if (this.timeLine.IsEnabled) {
				this.timeLine.SelectedStartIndex = 0;
				this.timeLine.SelectedEndIndex = this.timeLine.ValuesList.Count;
			}
		}
		
		void CanExecuteSelectAll(object sender, CanExecuteRoutedEventArgs e)
		{
			CanDoSelectAll(e);
			e.Handled = true;
		}

		void CanDoSelectAll(CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.timeLine.IsEnabled && this.timeLine.ValuesList.Count > 0;
		}

		void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			int index = tabView.Items.IndexOf(((Button)sender).Tag);
			if (index == tabView.SelectedIndex)
				tabView.SelectedItem = (tabView.SelectedIndex > 1) ? tabView.Items[tabView.SelectedIndex - 1] : tabView.SelectedItem;
			tabView.Items.Remove(((Button)sender).Tag);
		}
		
		void UpdateErrorList(IEnumerable<CompilerError> errors)
		{
			Dispatcher.Invoke(
				(Action)(
					() => {
						WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
						TaskService.ClearExceptCommentTasks();
						TaskService.AddRange(errors.Select(error => new Task("", error.ErrorText, error.Column, error.Line, (error.IsWarning) ? TaskType.Warning : TaskType.Error)));
					}
				)
			);
		}
		
		void TabViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dummyTab.IsSelected)
				Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => CreateTab("New Tab", string.Empty)));
			e.Handled = true;
		}
		
		void InitializeLastItems()
		{
			string queryHistory = provider.GetProperty("queryHistory");
			if (queryHistory != null) {
				foreach (string query in queryHistory.SplitSeparatedString()) {
					string queryCopy = query;
					MenuItem item = new MenuItem();
					mnuQueryHistory.Items.Add(item);
					item.Header = query;
					item.Click += delegate { CreateTab(queryCopy, queryCopy); };
				}
			}
		}
		
		void InitializeOldTabs()
		{
			string tabs = provider.GetProperty("tabs");
			if (tabs != null) {
				foreach (string query in tabs.SplitSeparatedString()) {
					CreateTab(query, query, false);
				}
			}
		}
		
		public void SaveUserState()
		{
			List<string> tabs = new List<string>();
			for (int i = 2; i < tabView.Items.Count; i++) {
				TabItem item = (TabItem)tabView.Items[i];
				if (item.Content is QueryView && !string.IsNullOrEmpty(((QueryView)item.Content).CurrentQuery))
					tabs.Add(((QueryView)item.Content).CurrentQuery);
			}
			provider.SetProperty("tabs", tabs.CreateSeparatedString());
			
			List<string> queryHistory = new List<string>();
			for (int i = 2; i < this.mnuQueryHistory.Items.Count; i++)
				tabs.Add((this.mnuQueryHistory.Items[i] as MenuItem).Header.ToString() ?? string.Empty);
			provider.SetProperty("queryHistory", queryHistory.CreateSeparatedString());
		}

		public TabItem CreateTab(string title, string query)
		{
			return CreateTab(title, query, true);
		}
		
		TabItem CreateTab(string title, string query, bool switchToNewTab)
		{
			TabItem newTab = new TabItem();
			Button closeButton = new Button { Style = this.Resources["CloseButton"] as Style };
			TextBlock header = new TextBlock { Margin = new Thickness(0, 0, 4, 0), MaxWidth = 120, MinWidth = 40 };

			newTab.Header = new StackPanel { Orientation = Orientation.Horizontal, Children = { header, closeButton } };

			closeButton.Click += new RoutedEventHandler(CloseButtonClick);
			closeButton.Tag = newTab;

			QueryView view;

			newTab.Content = view = new QueryView();

			view.Provider = this.provider;
			view.Reporter = new ErrorReporter(UpdateErrorList);
			view.SetRange(this.timeLine.SelectedStartIndex, this.timeLine.SelectedEndIndex);
			
			view.CurrentQuery = query;
			view.ShowQueryItems = true;
			view.ContextMenuOpening += delegate(object sender, ContextMenuEventArgs e) {
				object source = (e.OriginalSource is Shape) ? e.OriginalSource : view;
				MenuService.CreateContextMenu(source, "/AddIns/Profiler/QueryView/ContextMenu").IsOpen = true;
			};
			view.CurrentQueryChanged += delegate { ViewCurrentQueryChanged(header, view); };

			header.Text = title;
			header.TextTrimming = TextTrimming.CharacterEllipsis;
			header.TextWrapping = TextWrapping.NoWrap;
			tabView.Items.Insert(tabView.Items.Count - 1, newTab);
			if (switchToNewTab)
				tabView.SelectedItem = newTab;
			
			return newTab;
		}

		void ViewCurrentQueryChanged(TextBlock header, QueryView view)
		{
			header.Text = view.CurrentQuery;
			int index;
			while ((index = GetLastMatch(view.CurrentQuery)) != -1)
				mnuQueryHistory.Items.RemoveAt(index);

			MenuItem item = new MenuItem { Header = view.CurrentQuery };

			item.Click += delegate { CreateTab(header.Text, header.Text); };

			mnuQueryHistory.Items.Insert(2, item);

			if (mnuQueryHistory.Items.Count > 12) mnuQueryHistory.Items.RemoveAt(12);
		}
		
		int GetLastMatch(string query)
		{
			int index = -1;
			
			for (int i = 2; i < mnuQueryHistory.Items.Count; i++) {
				if ((mnuQueryHistory.Items[i] as MenuItem).Header.ToString() == query)
					index = i;
			}
			
			return index;
		}
		
		void ClearQueryHistoryClick(object sender, RoutedEventArgs e)
		{
			while (mnuQueryHistory.Items.Count > 2)
				mnuQueryHistory.Items.RemoveAt(2);
		}
	}
}
