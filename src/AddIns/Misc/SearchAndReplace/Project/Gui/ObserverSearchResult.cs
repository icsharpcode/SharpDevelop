// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public class ObserverSearchResult : DefaultSearchResult, IObserver<SearchedFile>
	{
		Button stopButton;
		bool finished;
		
		public ObserverSearchResult(string title)
		{
			rootNode = new SearchRootNode(title, new List<SearchResultMatch>());
		}
		
		public IDisposable Registration { get; set; }
		
		public override object GetControl()
		{
			WorkbenchSingleton.AssertMainThread();
			if (resultsTreeViewInstance == null)
				resultsTreeViewInstance = new ResultsTreeView();
			rootNode.GroupResultsByFile(ResultsTreeView.GroupResultsByFile);
			resultsTreeViewInstance.ItemsSource = new object[] { rootNode };
			return resultsTreeViewInstance;
		}
		
		public override IList GetToolbarItems()
		{
			var items = base.GetToolbarItems();
			
			if (!finished) {
				stopButton = new Button { Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.Debug.StopProcess") } };
				stopButton.Click += StopButtonClick;
				
				items.Add(stopButton);
			}
			
			return items;
		}
	
		void StopButtonClick(object sender, RoutedEventArgs e)
		{
			try {
				stopButton.Visibility = Visibility.Hidden;
				if (Registration != null) Registration.Dispose();
			} finally {
				rootNode.WasCancelled = true;
				finished = true;
			}
		}
		
		void IObserver<SearchedFile>.OnNext(SearchedFile value)
		{
			rootNode.Add(value);
		}
		
		void IObserver<SearchedFile>.OnError(Exception error)
		{
			// flatten AggregateException and
			// filter OperationCanceledException
			try {
				if (error is AggregateException)
					((AggregateException)error).Flatten().Handle(ex => ex is OperationCanceledException);
				else if (!(error is OperationCanceledException))
					throw error;
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
			OnCompleted();
		}
		
		void OnCompleted()
		{
			try {
				stopButton.Visibility = Visibility.Collapsed;
				if (Registration != null)
					Registration.Dispose();
			} finally {
				finished = true;
			}
		}
		
		void IObserver<SearchedFile>.OnCompleted()
		{
			OnCompleted();
		}
	}
}
