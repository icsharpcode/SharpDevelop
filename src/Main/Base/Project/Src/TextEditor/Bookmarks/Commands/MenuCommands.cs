// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public class ToggleBookmark : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ToggleBookmark();
			}
		}
	}
	
	public class PrevBookmark : AbstractEditActionMenuCommand
	{
		
		public static bool AcceptOnlyStandardBookmarks(Bookmark mark)
		{
			return (mark is SDBookmark);
		}
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoPrevBookmark(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
		
	public class NextBookmark : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoNextBookmark(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
		
	public class ClearBookmarks : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ClearAllBookmarks(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
}
