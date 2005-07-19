// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.Core
{
	public class Breakpoint 
	{
		BreakpointBookmark bookmark;
		object tag;
		
		public BreakpointBookmark Bookmark {
			get {
				return bookmark;
			}
		}
		
		public string FileName {
			get {
				return bookmark.FileName;
			}
			set {
				bookmark.FileName = value;
			}
		}
		
		public int LineNumber {
			get {
				return bookmark.LineNumber + 1;
			}
			set {
				bookmark.LineNumber = value - 1;
			}
		}
		
		public event EventHandler LineNumberChanged {
			add    { bookmark.LineNumberChanged += value; }
			remove { bookmark.LineNumberChanged -= value; }
		}

		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
		
		public bool IsEnabled {
			get {
				return bookmark.IsEnabled;
			}
			set {
				bookmark.IsEnabled = value;
			}
		}
		
		public Breakpoint(ICSharpCode.TextEditor.Document.IDocument document, string fileName, int lineNumber)
		{
			bookmark = new BreakpointBookmark(this, fileName, document, lineNumber - 1);
		}
	}
}

