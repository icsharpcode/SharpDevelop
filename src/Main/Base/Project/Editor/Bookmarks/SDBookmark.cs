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
using System.ComponentModel;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	/// <summary>
	/// A bookmark that is persistant across SharpDevelop sessions.
	/// </summary>
	public abstract class SDBookmark : BookmarkBase
	{
		FileName fileName;
		
		public FileName FileName {
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
		
		public string FileNameAndLineNumber {
			get { return string.Format(StringParser.Parse("${res:MainWindow.Windows.BookmarkPad.LineText}"), Path.GetFileName(this.FileName), this.LineNumber); }
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
		
		/// <summary>
		/// Gets/Sets if the bookmark should be saved to the project memento file.
		/// </summary>
		/// <remarks>
		/// Default is true; override this property if you are using the bookmark for
		/// something special like like "CurrentLineBookmark" in the debugger.
		/// </remarks>
		public virtual bool IsSaved {
			get {
				return true;
			}
		}
		
		/// <summary>
		/// Gets whether the bookmark should be displayed in the given pad.
		/// </summary>
		public virtual bool ShowInPad(BookmarkPadBase pad)
		{
			return true;
		}
		
		protected override void RemoveMark()
		{
			SD.BookmarkManager.RemoveMark(this);
		}
	}
	
	/// <summary>
	/// Bookmark class.
	/// </summary>
	public sealed class Bookmark : SDBookmark
	{ 
	}
}
