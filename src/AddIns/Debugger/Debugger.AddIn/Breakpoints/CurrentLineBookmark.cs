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
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public class CurrentLineBookmark : SDMarkerBookmark
	{
		static object syncObject = new object();
		static CurrentLineBookmark instance;
		
		static int startLine;
		static int startColumn;
		static int endLine;
		static int endColumn;
		
		public static void SetPosition(IViewContent viewContent, int markerStartLine, int markerStartColumn, int markerEndLine, int markerEndColumn)
		{
			if (viewContent == null)
				return;
			ITextEditor editor = viewContent.GetService<ITextEditor>();
			if (editor != null) {
				SetPosition(editor.FileName, editor.Document, markerStartLine, markerStartColumn, markerEndLine, markerEndColumn);
			}
		}
		
		public static void SetPosition(FileName fileName, IDocument document, int markerStartLine, int markerStartColumn, int markerEndLine, int markerEndColumn)
		{
			if (document == null)
				return;
			Remove();
			
			startLine   = markerStartLine;
			startColumn = markerStartColumn;
			endLine     = markerEndLine;
			endColumn   = markerEndColumn;
			
			if (startLine < 1 || startLine > document.LineCount)
				return;
			if (endLine < 1 || endLine > document.LineCount) {
				endLine = startLine;
				endColumn = int.MaxValue;
			}
			if (startColumn < 1)
				startColumn = 1;
			
			instance = new CurrentLineBookmark();
			instance.Location = new TextLocation(startLine, startColumn);
			instance.FileName = fileName;
			SD.BookmarkManager.AddMark(instance);
		}
		
		public static void Remove()
		{
			if (instance != null) {
				SD.BookmarkManager.RemoveMark(instance);
				instance = null;
			}
		}
		
		public override bool CanToggle {
			get { return false; }
		}
		
		public override int ZOrder {
			get { return 100; }
		}
		
		public override bool IsSaved {
			get { return false; }
		}
		
		public override bool ShowInPad(BookmarkPadBase pad)
		{
			return false;
		}
		
		public override IImage Image {
			get { return SD.ResourceService.GetImage("Bookmarks.CurrentLine"); }
		}
		
		protected override ITextMarker CreateMarker(ITextMarkerService markerService)
		{
			IDocumentLine sLine = this.Document.GetLineByNumber(startLine);
			IDocumentLine eLine = this.Document.GetLineByNumber(endLine);
			int sOffset = Math.Min(sLine.Offset + startColumn - 1, sLine.EndOffset);
			int eOffset = Math.Min(eLine.Offset + endColumn - 1, eLine.EndOffset);
			ITextMarker marker = markerService.Create(sOffset, Math.Max(eOffset - sOffset, 1));
			IHighlighter highlighter = this.Document.GetService(typeof(IHighlighter)) as IHighlighter;
			marker.BackgroundColor = BookmarkBase.CurrentLineDefaultBackground;
			marker.ForegroundColor = BookmarkBase.CurrentLineDefaultForeground;
			
			if (highlighter != null) {
				var color = highlighter.GetNamedColor(BookmarkBase.CurrentLineBookmarkName);
				if (color != null) {
					marker.BackgroundColor = color.Background.GetColor(null);
					marker.ForegroundColor = color.Foreground.GetColor(null);
				}
			}
			return marker;
		}
		
		public override bool CanDragDrop {
			get { return true; }
		}
		
		public override void Drop(int lineNumber)
		{
			// call async because the Debugger seems to use Application.DoEvents(), but we don't want to process events
			// because Drag'N'Drop operation has finished
			SD.MainThread.InvokeAsyncAndForget(delegate {
				SD.Debugger.SetInstructionPointer(this.FileName, lineNumber, 1, false);
			});
		}
	}
}
