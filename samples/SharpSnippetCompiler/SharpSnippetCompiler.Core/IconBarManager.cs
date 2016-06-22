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

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpSnippetCompiler.Core
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
		
		[Obsolete("Please provide a TextDocument; this is necessary so that the bookmarks can move when lines are inserted/removed")]
		public void UpdateClassMemberBookmarks(ParseInformation parseInfo)
		{
			UpdateClassMemberBookmarks(parseInfo, null);
		}
		
		public void UpdateClassMemberBookmarks(ParseInformation parseInfo, TextDocument document)
		{
			for (int i = bookmarks.Count - 1; i >= 0; i--) {
				if (IsClassMemberBookmark(bookmarks[i]))
					bookmarks.RemoveAt(i);
			}
			if (parseInfo == null)
				return;
			foreach (IClass c in parseInfo.CompilationUnit.Classes) {
				AddClassMemberBookmarks(c, document);
			}
		}
		
		void AddClassMemberBookmarks(IClass c, TextDocument document)
		{
			if (c.IsSynthetic) return;
			if (!c.Region.IsEmpty) {
				bookmarks.Add(new ClassBookmark(c, document));
			}
			foreach (IClass innerClass in c.InnerClasses) {
				AddClassMemberBookmarks(innerClass, document);
			}
			foreach (IMethod m in c.Methods) {
				if (m.Region.IsEmpty || m.IsSynthetic) continue;
				bookmarks.Add(new ClassMemberBookmark(m, document));
			}
			foreach (IProperty p in c.Properties) {
				if (p.Region.IsEmpty || p.IsSynthetic) continue;
				bookmarks.Add(new ClassMemberBookmark(p, document));
			}
			foreach (IField f in c.Fields) {
				if (f.Region.IsEmpty || f.IsSynthetic) continue;
				bookmarks.Add(new ClassMemberBookmark(f, document));
			}
			foreach (IEvent e in c.Events) {
				if (e.Region.IsEmpty || e.IsSynthetic) continue;
				bookmarks.Add(new ClassMemberBookmark(e, document));
			}
		}
		
		static bool IsClassMemberBookmark(IBookmark b)
		{
			return b is ClassMemberBookmark || b is ClassBookmark;
		}
	}
}
