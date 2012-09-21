// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		/// Gets/Sets if the bookmark is shown in the bookmark pad.
		/// </summary>
		/// <remarks>
		/// Default is true, override this property if you are using the bookmark for
		/// something special like like "CurrentLineBookmark" in the debugger.
		/// </remarks>
		public virtual bool IsVisibleInBookmarkPad {
			get {
				return true;
			}
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
