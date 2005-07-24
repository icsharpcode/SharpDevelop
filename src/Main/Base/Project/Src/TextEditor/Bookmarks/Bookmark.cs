// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ICSharpCode.Core;
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
		string fileName;
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public SDBookmark(string fileName, IDocument document, int lineNumber) : base(document, lineNumber)
		{
			this.fileName = fileName;
		}
	}
	
	/// <summary>
	/// A bookmark that is persistant across SharpDevelop sessions and has a text marker assigned to it.
	/// </summary>
	public abstract class SDMarkerBookmark : SDBookmark
	{
		public SDMarkerBookmark(string fileName, IDocument document, int lineNumber) : base(fileName, document, lineNumber)
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
				oldDocument.MarkerStrategy.RemoveMarker(oldMarker);
			}
			oldDocument = null;
			oldMarker = null;
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
		
		public Bookmark CreateBookmark(IDocument document, int lineNumber)
		{
			return new SDBookmark(fileName, document, lineNumber);
		}
	}
}
