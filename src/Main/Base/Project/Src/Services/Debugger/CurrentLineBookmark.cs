// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace ICSharpCode.Core
{
	public class CurrentLineBookmark: SDMarkerBookmark
	{
		static CurrentLineBookmark instance;

		static int startLine;
		static int startColumn;
		static int endLine;
		static int endColumn;
		
		public static void SetPosition(IViewContent viewContent,  int makerStartLine, int makerStartColumn, int makerEndLine, int makerEndColumn)
		{
			ITextEditorControlProvider tecp = viewContent as ITextEditorControlProvider;
			if (tecp != null)
				SetPosition(tecp.TextEditorControl.FileName, tecp.TextEditorControl.Document, makerStartLine, makerStartColumn, makerEndLine, makerEndColumn);
			else
				Remove();
		}
		
		public static void SetPosition(string fileName, IDocument document, int makerStartLine, int makerStartColumn, int makerEndLine, int makerEndColumn)
		{
			Remove();

			startLine   = makerStartLine;
			startColumn = makerStartColumn;
			endLine     = makerEndLine;
			endColumn   = makerEndColumn;

			LineSegment line = document.GetLineSegment(startLine - 1);
			int offset = line.Offset + startColumn;
			instance = new CurrentLineBookmark(fileName, document, startLine - 1);
			document.BookmarkManager.AddMark(instance);
			document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, startLine - 1));
			document.CommitUpdate();
		}
		
		public static void Remove()
		{
			if (instance != null) {
				instance.Document.BookmarkManager.RemoveMark(instance);
				instance.RemoveMarker();
				instance = null;
			}
		}
		
		public override bool CanToggle {
			get {
				return false;
			}
		}
		
		public CurrentLineBookmark(string fileName, IDocument document,  int startLine) : base(fileName, document, startLine)
		{

		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawArrow(g, p.Y);
		}
		
		protected override TextMarker CreateMarker()
		{
			LineSegment lineSeg = Document.GetLineSegment(startLine - 1);
			TextMarker marker = new TextMarker(lineSeg.Offset + startColumn, endColumn - startColumn, TextMarkerType.SolidBlock, Color.Yellow, Color.Blue);
			Document.MarkerStrategy.InsertMarker(0, marker);
			return marker;
		}
	}
}
