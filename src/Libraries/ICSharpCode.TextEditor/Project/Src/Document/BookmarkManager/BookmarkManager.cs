// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.TextEditor.Document
{
	public interface IBookmarkFactory
	{
		Bookmark CreateBookmark(IDocument document, int lineNumber);
	}
	
	/// <summary>
	/// This class handles the bookmarks for a buffer
	/// </summary>
	public class BookmarkManager
	{
		IDocument      document;
		List<Bookmark> bookmark = new List<Bookmark>();
		
		/// <value>
		/// Contains all bookmarks as int values
		/// </value>
		public List<Bookmark> Marks {
			get {
				return bookmark;
			}
		}
		
		public IDocument Document {
			get {
				return document;
			}
		}
		
		/// <summary>
		/// Creates a new instance of <see cref="BookmarkManager"/>
		/// </summary>
		public BookmarkManager(IDocument document, ILineManager lineTracker)
		{
			this.document = document;
			lineTracker.LineCountChanged += new LineManagerEventHandler(MoveIndices);
		}
		
		IBookmarkFactory factory;
		
		public IBookmarkFactory Factory {
			get {
				return factory;
			}
			set {
				factory = value;
			}
		}
		
		/// <summary>
		/// Sets the mark at the line <code>lineNr</code> if it is not set, if the
		/// line is already marked the mark is cleared.
		/// </summary>
		public void ToggleMarkAt(int lineNr)
		{
			for (int i = 0; i < bookmark.Count; ++i) {
				Bookmark mark = bookmark[i];
				if (mark.LineNumber == lineNr && mark.CanToggle) {
					bookmark.RemoveAt(i);
					OnRemoved(new BookmarkEventArgs(mark));
					OnChanged(EventArgs.Empty);
					return;
				}
			}
			Bookmark newMark;
			if (factory != null)
				newMark = factory.CreateBookmark(document, lineNr);
			else
				newMark = new Bookmark(document, lineNr);
			bookmark.Add(newMark);
			OnAdded(new BookmarkEventArgs(newMark));
			OnChanged(EventArgs.Empty);
		}
		
		public void AddMark(Bookmark mark)
		{
			bookmark.Add(mark);
			OnAdded(new BookmarkEventArgs(mark));
			OnChanged(EventArgs.Empty);
		}
		
		public void RemoveMark(Bookmark mark)
		{
			bookmark.Remove(mark);
			OnRemoved(new BookmarkEventArgs(mark));
			OnChanged(EventArgs.Empty);
		}
		
		public void RemoveMarks(Predicate<Bookmark> predicate)
		{
			for (int i = 0; i < bookmark.Count; ++i) {
				Bookmark bm = bookmark[i];
				if (predicate(bm)) {
					bookmark.RemoveAt(i--);
					OnRemoved(new BookmarkEventArgs(bm));
				}
			}
			OnChanged(EventArgs.Empty);
		}
		
		/// <returns>
		/// true, if a mark at mark exists, otherwise false
		/// </returns>
		public bool IsMarked(int lineNr)
		{
			for (int i = 0; i < bookmark.Count; ++i) {
				if (bookmark[i].LineNumber == lineNr) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// This method moves all indices from index upward count lines
		/// (useful for deletion/insertion of text)
		/// </summary>
		void MoveIndices(object sender,LineManagerEventArgs e)
		{
			bool changed = false;
			for (int i = 0; i < bookmark.Count; ++i) {
				Bookmark mark = bookmark[i];
				if (e.LinesMoved < 0 && mark.LineNumber == e.LineStart) {
					bookmark.RemoveAt(i);
					OnRemoved(new BookmarkEventArgs(mark));
					--i;
					changed = true;
				} else if (mark.LineNumber > e.LineStart + 1 || (e.LinesMoved < 0 && mark.LineNumber > e.LineStart))  {
					changed = true;
					bookmark[i].LineNumber = mark.LineNumber + e.LinesMoved;
				}
			}
			
			if (changed) {
				OnChanged(EventArgs.Empty);
			}
		}
		
		/// <remarks>
		/// Clears all bookmark
		/// </remarks>
		public void Clear()
		{
			foreach (Bookmark mark in bookmark) {
				OnRemoved(new BookmarkEventArgs(mark));
			}
			bookmark.Clear();
			OnChanged(EventArgs.Empty);
		}
		
		/// <value>
		/// The lowest mark, if no marks exists it returns -1
		/// </value>
		public Bookmark GetFirstMark(Predicate<Bookmark> predicate) 
		{
			if (bookmark.Count < 1) {
				return null;
			}
			Bookmark first = null;
			for (int i = 0; i < bookmark.Count; ++i) {
				if (predicate(bookmark[i]) && bookmark[i].IsEnabled && (first == null || bookmark[i].LineNumber < first.LineNumber)) {
					first = bookmark[i];
				}
			}
			return first;
		}
		
		/// <value>
		/// The highest mark, if no marks exists it returns -1
		/// </value>
		public Bookmark GetLastMark(Predicate<Bookmark> predicate)
		{
			if (bookmark.Count < 1) {
				return null;
			}
			Bookmark last = null;
			for (int i = 0; i < bookmark.Count; ++i) {
				if (predicate(bookmark[i]) && bookmark[i].IsEnabled && (last == null || bookmark[i].LineNumber > last.LineNumber)) {
					last = bookmark[i];
				}
			}
			return last;
		}
		bool AcceptAnyMarkPredicate(Bookmark mark)
		{
			return true;
		}
		public Bookmark GetNextMark(int curLineNr)
		{
			return GetNextMark(curLineNr, AcceptAnyMarkPredicate);
		}
		
		/// <remarks>
		/// returns first mark higher than <code>lineNr</code>
		/// </remarks>
		/// <returns>
		/// returns the next mark > cur, if it not exists it returns FirstMark()
		/// </returns>
		public Bookmark GetNextMark(int curLineNr, Predicate<Bookmark> predicate)
		{
			if (bookmark.Count == 0) {
				return null;
			}
			
			Bookmark next = GetFirstMark(predicate);
			foreach (Bookmark mark in bookmark) {
				if (predicate(mark) && mark.IsEnabled && mark.LineNumber > curLineNr) {
					if (mark.LineNumber < next.LineNumber || next.LineNumber <= curLineNr) {
						next = mark;
					}
				}
			}
			return next;
		}
		
		public Bookmark GetPrevMark(int curLineNr)
		{
			return GetPrevMark(curLineNr, AcceptAnyMarkPredicate);
		}
		/// <remarks>
		/// returns first mark lower than <code>lineNr</code>
		/// </remarks>
		/// <returns>
		/// returns the next mark lower than cur, if it not exists it returns LastMark()
		/// </returns>
		public Bookmark GetPrevMark(int curLineNr, Predicate<Bookmark> predicate)
		{
			if (bookmark.Count == 0) {
				return null;
			}
			
			Bookmark prev = GetLastMark(predicate);
			
			foreach (Bookmark mark in bookmark) {
				if (predicate(mark) && mark.IsEnabled && mark.LineNumber < curLineNr) {
					if (mark.LineNumber > prev.LineNumber || prev.LineNumber >= curLineNr) {
						prev = mark;
					}
				}
			}
			return prev;
		}
		
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		
		protected virtual void OnRemoved(BookmarkEventArgs e)
		{
			if (Removed != null) {
				Removed(this, e);
			}
		}
		
		
		protected virtual void OnAdded(BookmarkEventArgs e)
		{
			if (Added != null) {
				Added(this, e);
			}
		}
		
		public event BookmarkEventHandler Removed;
		public event BookmarkEventHandler Added;
		
		/// <summary>
		/// Is fired after the bookmarks change
		/// </summary>
		public event EventHandler Changed;
	}
}
