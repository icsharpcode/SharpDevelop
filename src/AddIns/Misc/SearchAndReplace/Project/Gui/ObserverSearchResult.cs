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
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
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
			SD.MainThread.VerifyAccess();
			if (resultsTreeViewInstance == null)
				resultsTreeViewInstance = new ResultsTreeView();
			rootNode.GroupResultsBy(ResultsTreeView.GroupingKind);
			resultsTreeViewInstance.ItemsSource = new object[] { rootNode };
			return resultsTreeViewInstance;
		}
		
		public override void OnDeactivate()
		{
			if (!finished)
				StopButtonClick(null, null);
			base.OnDeactivate();
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
			if (error == null)
				throw new ArgumentNullException("error");
			// flatten AggregateException and
			// filter OperationCanceledException
			try {
				if (error is AggregateException)
					((AggregateException)error).Flatten().Handle(ex => ex is OperationCanceledException);
				else if (!(error is OperationCanceledException))
					MessageService.ShowException(error);
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
