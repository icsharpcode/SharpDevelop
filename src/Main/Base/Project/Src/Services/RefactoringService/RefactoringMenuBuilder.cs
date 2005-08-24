// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of RefactoringMenuBuilder
	/// </summary>
	public class RefactoringMenuBuilder : ISubmenuBuilder
	{
		/*
		<MenuItem id = "GoToDefinition"
		          label = "${res:ICSharpCode.NAntAddIn.GotoDefinitionMenuLabel}"
		          shortcut = "Control|Enter"
		          class = "ICSharpCode.SharpDevelop.DefaultEditor.Commands.GoToDefinition" />
		 */
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			TextEditorControl textEditorControl = (TextEditorControl)owner;
			if (textEditorControl.FileName == null)
				return new ToolStripItem[0];
			List<ToolStripItem> resultItems = new List<ToolStripItem>();
			TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			IDocument doc = textArea.Document;
			int caretLine = textArea.Caret.Line;
			// Include definitions (use the bookmarks which should already be present)
			foreach (Bookmark mark in doc.BookmarkManager.Marks) {
				if (mark != null && mark.LineNumber == caretLine) {
					string path = null;
					int iconIndex = 0;
					string name = null;
					ClassMemberBookmark cmb = mark as ClassMemberBookmark;
					ClassBookmark cb = mark as ClassBookmark;
					if (cmb != null) {
						path = ClassMemberBookmark.ContextMenuPath;
						iconIndex = cmb.IconIndex;
						name = cmb.Member.Name;
					} else if (cb != null) {
						path = ClassBookmark.ContextMenuPath;
						iconIndex = ClassBrowserIconService.GetIcon(cb.Class);
						name = cb.Class.Name;
					}
					if (path != null) {
						ToolStripMenuItem item = new ToolStripMenuItem(name, ClassBrowserIconService.ImageList.Images[iconIndex]);
						MenuService.AddItemsToMenu(item.DropDown.Items, mark, path);
						resultItems.Add(item);
					}
				}
			}
			
			/*
			ResolveResult rr = ResolveAtCaret(textEditorControl, textArea);
			if (rr != null) {
				//XML.TextAreaContextMenu.Refactoring
			}
			 */
			
			if (resultItems.Count == 0) {
				return new ToolStripItem[0];
			} else {
				resultItems.Add(new MenuSeparator());
				return resultItems.ToArray();
			}
		}
		
		ResolveResult ResolveAtCaret(TextEditorControl textEditorControl, TextArea textArea)
		{
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textEditorControl.FileName);
			if (expressionFinder == null)
				return null;
			IDocument doc = textArea.Document;
			string textContent = doc.TextContent;
			ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, textArea.Caret.Offset);
			if (expressionResult.Expression != null) {
				return ParserService.Resolve(expressionResult, textArea.Caret.Line + 1, textArea.Caret.Column + 1, textArea.MotherTextEditorControl.FileName, textContent);
			}
			return null;
		}
	}
}
