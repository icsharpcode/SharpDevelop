// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;

namespace Bookmark
{
	/// <summary>
	/// Description of Bookmark.
	/// </summary>
	public class Bookmark 
	{
		ICSharpCode.TextEditor.Document.Bookmark bookmark;
		string fileName;
		
		public ICSharpCode.TextEditor.Document.IDocument Document {
			get {
				return bookmark.Document;
			}
		}
		
		public ICSharpCode.TextEditor.Document.Bookmark CreateBookmark(ICSharpCode.TextEditor.Document.IDocument document)
		{
			this.bookmark = new ICSharpCode.TextEditor.Document.Bookmark(document, LineNumber, IsEnabled);
			return bookmark;
		}
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public int LineNumber {
			get {
				return bookmark.LineNumber;
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
		
		public Bookmark(string fileName, ICSharpCode.TextEditor.Document.Bookmark bookmark)
		{
			this.fileName = fileName;
			this.bookmark = bookmark;
		}
	}
}
