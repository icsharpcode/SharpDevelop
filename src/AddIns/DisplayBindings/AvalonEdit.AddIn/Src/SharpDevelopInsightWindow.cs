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
using System.Collections.Specialized;
using System.ComponentModel;

using System.Windows.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	class SharpDevelopInsightWindow : OverloadInsightWindow
	{
		public SharpDevelopInsightWindow(TextArea textArea) : base(textArea)
		{
			this.Style = ICSharpCode.Core.Presentation.GlobalStyles.WindowStyle;
			AttachEvents();
		}
		
		internal SharpDevelopInsightWindowAdapter activeAdapter;

		public void SetActiveAdapter(SharpDevelopInsightWindowAdapter adapter, bool isNewWindow)
		{
			if (activeAdapter != null) {
				// tell the previous adapter that its window was closed,
				// but actually reuse the window for the new adapter
				activeAdapter.OnClosed();
			}
			activeAdapter = adapter;
			this.Provider = adapter.Provider;
			if (!isNewWindow) {
				// reset insight window to initial state
				CloseAutomatically = true;
				StartOffset = EndOffset = this.TextArea.Caret.Offset;
				Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(SetPositionToStartOffset));
			}
		}
		
		void SetPositionToStartOffset()
		{
			if (document != null && this.StartOffset != this.TextArea.Caret.Offset) {
				SetPosition(new TextViewPosition(document.GetLocation(this.StartOffset)));
			} else {
				SetPosition(this.TextArea.Caret.Position);
			}
		}
		
		TextDocument document;
		Caret caret;
		
		void AttachEvents()
		{
			document = this.TextArea.Document;
			caret = this.TextArea.Caret;
			if (document != null)
				document.Changed += document_Changed;
			if (caret != null)
				caret.PositionChanged += caret_PositionChanged;
			this.Closed += OnClosed;
		}

		/// <inheritdoc/>
		protected override void DetachEvents()
		{
			if (document != null)
				document.Changed -= document_Changed;
			if (caret != null)
				caret.PositionChanged -= caret_PositionChanged;
			this.Closed -= OnClosed;
			base.DetachEvents();
		}
		
		void OnClosed(object sender, EventArgs e)
		{
			if (activeAdapter != null)
				activeAdapter.OnClosed();
		}
		
		void caret_PositionChanged(object sender, EventArgs e)
		{
			// It is possible that the insight window is not initialized correctly
			// due to an exception, and then caret_PositionChanged is called in a finally block
			// during exception handling.
			// Check for a null adapter to avoid a NullReferenceException that hides the first exception.
			if (activeAdapter != null)
				activeAdapter.OnCaretPositionChanged(e);
		}
		
		void document_Changed(object sender, DocumentChangeEventArgs e)
		{
			if (activeAdapter != null)
				activeAdapter.OnDocumentChanged(e);
		}
	}
	
	/// <summary>
	/// Adapter between AvalonEdit InsightWindow and SharpDevelop IInsightWindow interface.
	/// </summary>
	class SharpDevelopInsightWindowAdapter : IInsightWindow
	{
		sealed class SDItemProvider : IOverloadProvider
		{
			readonly SharpDevelopInsightWindowAdapter insightWindow;
			int selectedIndex;
			
			public SDItemProvider(SharpDevelopInsightWindowAdapter insightWindow)
			{
				this.insightWindow = insightWindow;
				insightWindow.items.CollectionChanged += insightWindow_items_CollectionChanged;
			}
			
			void insightWindow_items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				OnPropertyChanged("Count");
				OnPropertyChanged("CurrentHeader");
				OnPropertyChanged("CurrentContent");
				OnPropertyChanged("CurrentIndexText");
				insightWindow.OnSelectedItemChanged(EventArgs.Empty);
			}
			
			public event PropertyChangedEventHandler PropertyChanged;
			
			public int SelectedIndex {
				get {
					return selectedIndex;
				}
				set {
					if (selectedIndex != value) {
						selectedIndex = value;
						OnPropertyChanged("SelectedIndex");
						OnPropertyChanged("CurrentHeader");
						OnPropertyChanged("CurrentContent");
						OnPropertyChanged("CurrentIndexText");
					}
				}
			}
			
			public int Count {
				get { return insightWindow.Items.Count; }
			}
			
			public string CurrentIndexText {
				get { return (selectedIndex + 1).ToString() + " of " + this.Count.ToString(); }
			}
			
			public object CurrentHeader {
				get {
					IInsightItem item = insightWindow.SelectedItem;
					return item != null ? item.Header : null;
				}
			}
			
			public object CurrentContent {
				get {
					IInsightItem item = insightWindow.SelectedItem;
					return item != null ? item.Content : null;
				}
			}
			
			internal void OnPropertyChanged(string propertyName)
			{
				if (PropertyChanged != null) {
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}
		
		readonly ObservableCollection<IInsightItem> items = new ObservableCollection<IInsightItem>();
		SharpDevelopInsightWindow insightWindow;
		readonly SDItemProvider provider;
		
		internal IOverloadProvider Provider {
			get { return provider; }
		}
		
		internal SharpDevelopInsightWindowAdapter(SharpDevelopInsightWindow insightWindow)
		{
			this.insightWindow = insightWindow;
			provider = new SDItemProvider(this);
			provider.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
				if (e.PropertyName == "SelectedIndex")
					OnSelectedItemChanged(EventArgs.Empty);
			};
		}
		
		public IList<IInsightItem> Items {
			get { return items; }
		}
		
		public IInsightItem SelectedItem {
			get {
				int index = provider.SelectedIndex;
				if (index < 0 || index >= items.Count)
					return null;
				else
					return items[index];
			}
			set {
				provider.SelectedIndex = items.IndexOf(value);
				OnSelectedItemChanged(EventArgs.Empty);
			}
		}
		
		IInsightItem oldSelectedItem;
		
		public event EventHandler<TextChangeEventArgs> DocumentChanged;

		internal void OnDocumentChanged(DocumentChangeEventArgs e)
		{
			if (DocumentChanged != null)
				DocumentChanged(this, e);
		}
		
		public event EventHandler SelectedItemChanged;
		
		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			if (oldSelectedItem != null)
				oldSelectedItem.PropertyChanged -= SelectedItemPropertyChanged;
			oldSelectedItem = SelectedItem;
			if (oldSelectedItem != null)
				oldSelectedItem.PropertyChanged += SelectedItemPropertyChanged;
			if (SelectedItemChanged != null) {
				SelectedItemChanged(this, e);
			}
		}
		
		void SelectedItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName) {
				case "Header":
					provider.OnPropertyChanged("CurrentHeader");
					break;
				case "Content":
					provider.OnPropertyChanged("CurrentContent");
					break;
			}
		}
		
		public event EventHandler CaretPositionChanged;

		internal void OnCaretPositionChanged(EventArgs e)
		{
			if (CaretPositionChanged != null) {
				CaretPositionChanged(this, e);
			}
		}

		public event EventHandler Closed;

		internal void OnClosed()
		{
			if (Closed != null)
				Closed(this, EventArgs.Empty);
			insightWindow = null;
		}
		
		public void Close()
		{
			if (insightWindow != null)
				insightWindow.Close();
		}

		public double Width {
			get {
				return insightWindow != null ? insightWindow.Width : 0;
			}
			set {
				if (insightWindow != null)
					insightWindow.Width = value;
			}
		}

		public double Height {
			get {
				return insightWindow != null ? insightWindow.Height : 0;
			}
			set {
				if (insightWindow != null)
					insightWindow.Height = value;
			}
		}

		public bool CloseAutomatically {
			get {
				return insightWindow != null && insightWindow.CloseAutomatically;
			}
			set {
				if (insightWindow != null)
					insightWindow.CloseAutomatically = value;
			}
		}

		public int StartOffset {
			get {
				return insightWindow != null ? insightWindow.StartOffset : 0;
			}
			set {
				if (insightWindow != null)
					insightWindow.StartOffset = value;
			}
		}

		public int EndOffset {
			get {
				return insightWindow != null ? insightWindow.EndOffset : 0;
			}
			set {
				if (insightWindow != null)
					insightWindow.EndOffset = value;
			}
		}
	}
}
