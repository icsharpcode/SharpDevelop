// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace SearchAndReplace
{
	public abstract class SearchNode : INotifyPropertyChanged
	{
		bool isExpanded;
		
		public bool IsExpanded {
			get { return isExpanded; }
			set {
				if (isExpanded != value) {
					isExpanded = value;
					OnPropertyChanged("IsExpanded");
				}
			}
		}
		
		bool isSelected;
		
		public bool IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value) {
					isSelected = value;
					OnPropertyChanged("IsSelected");
				}
			}
		}
		
		IEnumerable<SearchNode> children;
		
		public IEnumerable<SearchNode> Children {
			get { return children; }
			set {
				if (children != value) {
					children = value;
					OnPropertyChanged("Children");
				}
			}
		}
		
		// Text usually is a TextBlock object, so we don't want too many of them floating in memory
		// (remember, we have a SearchNode for each search result, even for those in the search history).
		// We don't want to create a new TextBlock for each property access, either; so we'll cache them
		// using a WeakReference.
		
		WeakReference cachedText;
		
		public object Text {
			get {
				object text;
				if (cachedText != null) {
					text = cachedText.Target;
					if (text != null)
						return text;
				}
				text = CreateText() ?? string.Empty;
				cachedText = new WeakReference(text);
				return text;
			}
		}
		
		protected abstract object CreateText();
		
		protected internal void InvalidateText()
		{
			cachedText = null;
			OnPropertyChanged("Text");
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		public virtual void ActivateItem()
		{
		}
	}
}
