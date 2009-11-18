// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.SharpDevelop.Editor;

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
			
			void OnPropertyChanged(string propertyName)
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
			this.Style = ICSharpCode.Core.Presentation.GlobalStyles.WindowStyle;
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
			}
		}
	}
}
