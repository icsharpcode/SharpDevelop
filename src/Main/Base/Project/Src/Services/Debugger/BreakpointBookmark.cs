// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public enum BreakpointAction
	{
		Break,
		Trace,
		Condition
	}
	
	public class BreakpointBookmark : SDMarkerBookmark
	{
		bool isHealthy = true;
		bool isEnabled = true;
		string tooltip;
		
		BreakpointAction action = BreakpointAction.Break;
		string condition;
		string scriptLanguage;
		
		public event EventHandler<EventArgs> ConditionChanged;
		
		public string ScriptLanguage {
			get { return scriptLanguage; }
			set { scriptLanguage = value; }
		}
		
		public string Condition {
			get { return condition; }
			set {
				if (condition != value) {
					condition = value;
					this.Action = string.IsNullOrEmpty(condition) ? BreakpointAction.Break : BreakpointAction.Condition;
					if (ConditionChanged != null)
						ConditionChanged(this, EventArgs.Empty);
					Redraw();
				}
			}
		}
		
		public BreakpointAction Action {
			get {
				return action;
			}
			set {
				if (action != value) {
					action = value;
					Redraw();
				}
			}
		}
		
		public object InternalBreakpointObject { get; set; }
		
		public virtual bool IsHealthy {
			get {
				return isHealthy;
			}
			set {
				if (isHealthy != value) {
					isHealthy = value;
					Redraw();
				}
			}
		}
		
		public virtual bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				if (isEnabled != value) {
					isEnabled = value;
					if (IsEnabledChanged != null)
						IsEnabledChanged(this, EventArgs.Empty);
					Redraw();
				}
			}
		}
		
		public event EventHandler IsEnabledChanged;
		
		public string Tooltip {
			get { return tooltip; }
			set { tooltip = value; }
		}
		
		/// <summary>
		/// parameter-less ctor is necessary for deserialization
		/// </summary>
		public BreakpointBookmark()
		{
		}
		
		public BreakpointBookmark(FileName fileName, TextLocation location, BreakpointAction action, string scriptLanguage, string script)
		{
			this.Location = location;
			this.FileName = fileName;
			this.action = action;
			this.scriptLanguage = scriptLanguage;
			this.condition = script;
		}
		
		public const string BreakpointMarker = "Breakpoint";
		
		public static readonly Color DefaultBackground = Color.FromRgb(180, 38, 38);
		public static readonly Color DefaultForeground = Colors.White;
		
		public static readonly IImage BreakpointImage = new ResourceServiceImage("Bookmarks.Breakpoint");
		public static readonly IImage BreakpointConditionalImage = new ResourceServiceImage("Bookmarks.BreakpointConditional");
		public static readonly IImage DisabledBreakpointImage = new ResourceServiceImage("Bookmarks.DisabledBreakpoint");
		public static readonly IImage UnhealthyBreakpointImage = new ResourceServiceImage("Bookmarks.UnhealthyBreakpoint");
		public static readonly IImage UnhealthyBreakpointConditionalImage = new ResourceServiceImage("Bookmarks.UnhealthyBreakpointConditional");
		
		public override IImage Image {
			get {
				if (!this.IsEnabled)
					return DisabledBreakpointImage;
				else if (this.IsHealthy)
					return this.Action == BreakpointAction.Break ? BreakpointImage : BreakpointConditionalImage;
				else
					return this.Action == BreakpointAction.Break ? UnhealthyBreakpointImage : UnhealthyBreakpointConditionalImage;
			}
		}
		
		protected override ITextMarker CreateMarker(ITextMarkerService markerService)
		{
			IDocumentLine line = this.Document.GetLineByNumber(this.LineNumber);
			ITextMarker marker = markerService.Create(line.Offset, line.Length);
			IHighlighter highlighter = this.Document.GetService(typeof(IHighlighter)) as IHighlighter;
			marker.BackgroundColor = DefaultBackground;
			marker.ForegroundColor = DefaultForeground;
			marker.MarkerColor = DefaultBackground;
			marker.MarkerTypes = TextMarkerTypes.CircleInScrollBar;
			
			if (highlighter != null) {
				var color = highlighter.GetNamedColor(BreakpointMarker);
				if (color != null) {
					marker.BackgroundColor = color.Background.GetColor(null);
					marker.MarkerColor = color.Background.GetColor(null) ?? DefaultBackground;
					marker.ForegroundColor = color.Foreground.GetColor(null);
				}
			}
			return marker;
		}
		
		public override string ToString()
		{
			return string.Format("{0} @{1}", this.FileName, this.LineNumber);
		}
	}
}
