// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public class GotoNext : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoNextBookmark(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
	
	public class GotoPrev : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoPrevBookmark(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
	
	public class DeleteMark : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkNode node = BookmarkPad.Instance.CurrentNode;
			if (node != null) {
				node.Bookmark.Document.BookmarkManager.RemoveMark(node.Bookmark);
				WorkbenchSingleton.MainForm.Refresh();
			}
		}
	}
	
	public class EnableDisableAll : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkPad.Instance.EnableDisableAll();
		}
	}
	
}
