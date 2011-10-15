// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;

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
