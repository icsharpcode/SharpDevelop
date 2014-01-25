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

using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Stores the entries in the icon bar margin. Multiple icon bar margins
	/// can use the same manager if split view is used.
	/// </summary>
	public class IconBarManager : IBookmarkMargin
	{
		ObservableCollection<IBookmark> bookmarks = new ObservableCollection<IBookmark>();
		
		public IconBarManager()
		{
			bookmarks.CollectionChanged += bookmarks_CollectionChanged;
		}
		
		public IList<IBookmark> Bookmarks {
			get { return bookmarks; }
		}
		
		void bookmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Redraw();
		}
		
		public void Redraw()
		{
			if (RedrawRequested != null)
				RedrawRequested(this, EventArgs.Empty);
		}
		
		public event EventHandler RedrawRequested;
		
		public void UpdateClassMemberBookmarks(IUnresolvedFile parseInfo, IDocument document)
		{
			for (int i = bookmarks.Count - 1; i >= 0; i--) {
				if (bookmarks[i] is EntityBookmark)
					bookmarks.RemoveAt(i);
			}
			if (parseInfo == null)
				return;
			foreach (var c in parseInfo.TopLevelTypeDefinitions) {
				AddEntityBookmarks(c, document);
			}
		}
		
		void AddEntityBookmarks(IUnresolvedTypeDefinition c, IDocument document)
		{
			if (c.IsSynthetic) return;
			if (!c.Region.IsEmpty) {
				bookmarks.Add(new EntityBookmark(c, document));
			}
			foreach (var innerClass in c.NestedTypes) {
				AddEntityBookmarks(innerClass, document);
			}
			foreach (var m in c.Members) {
				if (m.Region.IsEmpty || m.IsSynthetic) continue;
				bookmarks.Add(new EntityBookmark(m, document));
			}
		}
	}
}
