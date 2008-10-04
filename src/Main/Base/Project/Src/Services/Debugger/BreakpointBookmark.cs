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
	public enum BreakpointAction {
		Break, Trace, Condition
	}
	
	public class BreakpointBookmark : SDMarkerBookmark
	{
		bool isHealthy = true;
		string tooltip;
		
		static readonly Color defaultColor = Color.FromArgb(180, 38, 38);
		
		BreakpointAction action = BreakpointAction.Break;
		string condition;
		string scriptLanguage;
		
		public string ScriptLanguage {
			get { return scriptLanguage; }
			set { scriptLanguage = value; }
		}
		
		public string Condition {
			get { return condition; }
			set { condition = value; }
		}
		
		public BreakpointAction Action {
			get { return action; }
			set { this.action = value; }
		}
		
		public virtual bool IsHealthy {
			get {
				return isHealthy;
			}
			set {
				isHealthy = value;
				if (Document != null && !Anchor.IsDeleted) {
					Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, LineNumber));
					Document.CommitUpdate();
				}
			}
		}
		
		public string Tooltip {
			get { return tooltip; }
			set { tooltip = value; }
		}
		
		public BreakpointBookmark(string fileName, IDocument document, TextLocation location, BreakpointAction action, string scriptLanguage, string script) : base(fileName, document, location)
		{
			this.action = action;
			this.scriptLanguage = scriptLanguage;
			this.condition = script;
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawBreakpoint(g, p.Y, IsEnabled, IsHealthy);
		}
		
		protected override TextMarker CreateMarker()
		{
			LineSegment lineSeg = Anchor.Line;
			TextMarker marker = new TextMarker(lineSeg.Offset, lineSeg.Length, TextMarkerType.SolidBlock, defaultColor, Color.White);
			Document.MarkerStrategy.AddMarker(marker);
			return marker;
		}
	}
}
