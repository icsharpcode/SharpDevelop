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
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	/// <summary>
	/// View-Model class for a property grid category.
	/// </summary>
	public class Category : INotifyPropertyChanged
	{
		// don't warn on missing XML comments in View-Model
		#pragma warning disable 1591
		
		public Category(string name)
		{
			Name = name;
			Properties = new PropertyNodeCollection();
			//MoreProperties = new ObservableCollection<PropertyNode>();
		}

		public string Name { get; private set; }
		public PropertyNodeCollection Properties { get; private set; }
		//public ObservableCollection<PropertyNode> MoreProperties { get; private set; }

		bool isExpanded = true;

		public bool IsExpanded {
			get {
				return isExpanded;
			}
			set {
				isExpanded = value;
				RaisePropertyChanged("IsExpanded");
			}
		}

		//bool showMore;
		//internal bool ShowMoreByFilter;

		//public bool ShowMore {
		//    get {
		//        return showMore;
		//    }
		//    set {
		//        showMore = value;
		//        RaisePropertyChanged("ShowMore");
		//    }
		//}

		bool isVisible;

		public bool IsVisible
		{
			get
			{
				return isVisible;
			}
			set
			{
				isVisible = value;
				RaisePropertyChanged("IsVisible");
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
