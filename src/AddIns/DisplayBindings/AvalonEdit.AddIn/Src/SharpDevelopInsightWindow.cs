// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Adapter between AvalonEdit InsightWindow and SharpDevelop IInsightWindow interface.
	/// </summary>
	public class SharpDevelopInsightWindow : OverloadInsightWindow, IInsightWindow
	{
		sealed class SDItemProvider : IOverloadProvider
		{
			readonly SharpDevelopInsightWindow insightWindow;
			int selectedIndex;
			
			public SDItemProvider(SharpDevelopInsightWindow insightWindow)
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
		
		public SharpDevelopInsightWindow(TextArea textArea) : base(textArea)
		{
			this.Provider = new SDItemProvider(this);
			this.Provider.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
				if (e.PropertyName == "SelectedIndex")
					OnSelectedItemChanged(EventArgs.Empty);
			};
			this.Style = ICSharpCode.Core.Presentation.GlobalStyles.WindowStyle;
			AttachEvents();
		}
		
		public IList<IInsightItem> Items {
			get { return items; }
		}
		
		public IInsightItem SelectedItem {
			get {
				int index = this.Provider.SelectedIndex;
				if (index < 0 || index >= items.Count)
					return null;
				else
					return items[index];
			}
			set {
				this.Provider.SelectedIndex = items.IndexOf(value);
				OnSelectedItemChanged(EventArgs.Empty);
			}
		}
		
		TextDocument document;
		Caret caret;
		IInsightItem oldSelectedItem;
		
		void AttachEvents()
		{
			document = this.TextArea.Document;
			caret = this.TextArea.Caret;
			if (document != null)
				document.Changed += document_Changed;
			if (caret != null)
				caret.PositionChanged += caret_PositionChanged;
		}

		void caret_PositionChanged(object sender, EventArgs e)
		{
			OnCaretPositionChanged(e);
		}
		
		/// <inheritdoc/>
		protected override void DetachEvents()
		{
			if (document != null)
				document.Changed -= document_Changed;
			if (caret != null)
				caret.PositionChanged -= caret_PositionChanged;
			base.DetachEvents();
		}
		
		void document_Changed(object sender, DocumentChangeEventArgs e)
		{
			if (DocumentChanged != null)
				DocumentChanged(this, e);
		}
		
		public event EventHandler<TextChangeEventArgs> DocumentChanged;
		
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
			var provider = Provider as SDItemProvider;
			if (provider == null) return;
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
		
		protected virtual void OnCaretPositionChanged(EventArgs e)
		{
			if (CaretPositionChanged != null) {
				CaretPositionChanged(this, e);
			}
		}
	}
}
