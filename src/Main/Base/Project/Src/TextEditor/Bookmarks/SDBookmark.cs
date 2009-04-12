// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// A bookmark that is persistant across SharpDevelop sessions.
	/// </summary>
	[TypeConverter(typeof(BookmarkConverter))]
	public class SDBookmark : Bookmark
	{
		public SDBookmark(string fileName, IDocument document, TextLocation location) : base(document, location)
		{
			this.fileName = fileName;
		}
		
		string fileName;
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				if (fileName != value) {
					fileName = value;
					OnFileNameChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler FileNameChanged;
		
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			if (FileNameChanged != null) {
				FileNameChanged(this, e);
			}
		}
		
		public event EventHandler LineNumberChanged;
		
		internal void RaiseLineNumberChanged()
		{
			if (LineNumberChanged != null)
				LineNumberChanged(this, EventArgs.Empty);
		}
		
		bool isSaved = true;
		
		/// <summary>
		/// Gets/Sets if the bookmark should be saved to the project memento file.
		/// </summary>
		/// <remarks>
		/// Default is true, set this property to false if you are using the bookmark for
		/// something special like like "CurrentLineBookmark" in the debugger.
		/// </remarks>
		public bool IsSaved {
			get {
				return isSaved;
			}
			set {
				isSaved = value;
			}
		}
		
		bool isVisibleInBookmarkPad = true;
		
		/// <summary>
		/// Gets/Sets if the bookmark is shown in the bookmark pad.
		/// </summary>
		/// <remarks>
		/// Default is true, set this property to false if you are using the bookmark for
		/// something special like like "CurrentLineBookmark" in the debugger.
		/// </remarks>
		public bool IsVisibleInBookmarkPad {
			get {
				return isVisibleInBookmarkPad;
			}
			set {
				isVisibleInBookmarkPad = value;
			}
		}
	}
	
	/// <summary>
	/// A bookmark that is persistant across SharpDevelop sessions and has a text marker assigned to it.
	/// </summary>
	public abstract class SDMarkerBookmark : SDBookmark
	{
		public SDMarkerBookmark(string fileName, IDocument document, TextLocation location) : base(fileName, document, location)
		{
			SetMarker();
		}
		
		IDocument oldDocument;
		TextMarker oldMarker;
		
		protected abstract TextMarker CreateMarker();
		
		void SetMarker()
		{
			RemoveMarker();
			if (Document != null) {
				TextMarker marker = CreateMarker();
				// Perform editor update
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, LineNumber));
				Document.CommitUpdate();
				oldMarker = marker;
			}
			oldDocument = Document;
		}
		
		protected override void OnDocumentChanged(EventArgs e)
		{
			base.OnDocumentChanged(e);
			SetMarker();
		}
		
		public void RemoveMarker()
		{
			if (oldDocument != null) {
				int from = SafeGetLineNumberForOffset(oldDocument, oldMarker.Offset);
				int to = SafeGetLineNumberForOffset(oldDocument, oldMarker.Offset + oldMarker.Length);
				oldDocument.MarkerStrategy.RemoveMarker(oldMarker);
				oldDocument.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, from, to));
				oldDocument.CommitUpdate();
			}
			oldDocument = null;
			oldMarker = null;
		}
		
		static int SafeGetLineNumberForOffset(IDocument document, int offset)
		{
			if (offset <= 0)
				return 0;
			if (offset >= document.TextLength)
				return document.TotalNumberOfLines;
			return document.GetLineNumberForOffset(offset);
		}
	}
	
	public class SDBookmarkFactory : IBookmarkFactory
	{
		string fileName;
		ICSharpCode.TextEditor.Document.BookmarkManager manager;
		
		public SDBookmarkFactory(ICSharpCode.TextEditor.Document.BookmarkManager manager)
		{
			this.manager = manager;
		}
		
		public void ChangeFilename(string newFileName)
		{
			fileName = newFileName;
			foreach (Bookmark mark in manager.Marks) {
				SDBookmark sdMark = mark as SDBookmark;
				if (sdMark != null) {
					sdMark.FileName = newFileName;
				}
			}
		}
		
		public Bookmark CreateBookmark(IDocument document, TextLocation location)
		{
			return new SDBookmark(fileName, document, location);
		}
	}
}
