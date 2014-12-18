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
using System.Windows;
using ICSharpCode.WpfDesign.Designer.themes;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	public partial class RightClickContextMenu
	{
		private DesignItem designItem;
		
		public RightClickContextMenu(DesignItem designItem)
		{
			this.designItem = designItem;
			
			SpecialInitializeComponent();
		}
		
		/// <summary>
		/// Fixes InitializeComponent with multiple Versions of same Assembly loaded
		/// </summary>
		public void SpecialInitializeComponent()
		{
			if (!this._contentLoaded) {
				this._contentLoaded = true;
				Uri resourceLocator = new Uri(VersionedAssemblyResourceDictionary.GetXamlNameForType(this.GetType()), UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
			
			this.InitializeComponent();
		}

		void Click_BringToFront(object sender, RoutedEventArgs e)
		{
			if ((designItem.ParentProperty == null) || !designItem.ParentProperty.IsCollection)
				return;
			
			var collection = this.designItem.ParentProperty.CollectionElements;
			collection.Remove(this.designItem);
			collection.Add(this.designItem);
		}

		void Click_SendToBack(object sender, RoutedEventArgs e)
		{
			if ((designItem.ParentProperty == null) || !designItem.ParentProperty.IsCollection)
				return;
			
			var collection = this.designItem.ParentProperty.CollectionElements;
			collection.Remove(this.designItem);
			collection.Insert(0, this.designItem);
		}
		
		void Click_Backward(object sender, RoutedEventArgs e)
		{
			if ((designItem.ParentProperty == null) || !designItem.ParentProperty.IsCollection)
				return;
			
			var collection = this.designItem.ParentProperty.CollectionElements;
			var idx = collection.IndexOf(this.designItem);
			collection.RemoveAt(idx);
			collection.Insert((--idx < 0 ? 0 : idx), this.designItem);
		}

		void Click_Forward(object sender, RoutedEventArgs e)
		{
			if ((designItem.ParentProperty == null) || !designItem.ParentProperty.IsCollection)
				return;
			
			var collection = this.designItem.ParentProperty.CollectionElements;
			var idx = collection.IndexOf(this.designItem);
			collection.RemoveAt(idx);
			var cnt = collection.Count;
			collection.Insert((++idx > cnt ? cnt : idx), this.designItem);
		}
	}
}
