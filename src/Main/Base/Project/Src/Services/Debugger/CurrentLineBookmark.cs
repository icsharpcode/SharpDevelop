// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.SharpDevelop.Debugging
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
			
			if (startLine < 1 || startLine > document.TotalNumberOfLines)
				return;
			if (endLine < 1 || endLine > document.TotalNumberOfLines) {
				endLine = startLine;
				endColumn = int.MaxValue;
			}
			if (startColumn < 1)
				startColumn = 1;
			
			LineSegment line = document.GetLineSegment(startLine - 1);
			if (endColumn < 1 || endColumn > line.Length)
				endColumn = line.Length;
			instance = new CurrentLineBookmark(fileName, document, new TextLocation(startColumn - 1, startLine - 1));
			document.BookmarkManager.AddMark(instance);
			document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, startLine - 1, endLine - 1));
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
		
		public CurrentLineBookmark(string fileName, IDocument document, TextLocation location) : base(fileName, document, location)
		{
			this.IsSaved = false;
			this.IsVisibleInBookmarkPad = false;
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawArrow(g, p.Y);
		}
		
		protected override TextMarker CreateMarker()
		{
			LineSegment lineSeg = Document.GetLineSegment(startLine - 1);
			TextMarker marker = new TextMarker(lineSeg.Offset + startColumn - 1, Math.Max(endColumn - startColumn, 1), TextMarkerType.SolidBlock, Color.Yellow, Color.Blue);
			Document.MarkerStrategy.InsertMarker(0, marker);
			return marker;
		}
	}
}
