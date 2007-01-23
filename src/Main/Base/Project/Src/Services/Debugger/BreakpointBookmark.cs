// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public class BreakpointBookmark : SDMarkerBookmark
	{
		bool willBeHit = true;
		
		static readonly Color defaultColor = Color.FromArgb(180, 38, 38);
		
		public virtual bool WillBeHit {
			get {
				return willBeHit;
			}
			set {
				willBeHit = value;
				if (Document != null) {
					Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, LineNumber));
					Document.CommitUpdate();
				}
			}
		}
		
		public BreakpointBookmark(string fileName, IDocument document, int lineNumber) : base(fileName, document, lineNumber)
		{
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawBreakpoint(g, p.Y, IsEnabled, WillBeHit);
		}
		
		protected override TextMarker CreateMarker()
		{
			if (LineNumber >= Document.TotalNumberOfLines)
				LineNumber = Document.TotalNumberOfLines - 1;
			LineSegment lineSeg = Document.GetLineSegment(LineNumber);
			TextMarker marker = new TextMarker(lineSeg.Offset, lineSeg.Length, TextMarkerType.SolidBlock, defaultColor, Color.White);
			Document.MarkerStrategy.AddMarker(marker);
			return marker;
		}
	}
}
