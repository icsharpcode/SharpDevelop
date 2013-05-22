// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public class BreakpointBookmark : SDMarkerBookmark
	{
		bool isHealthy = true;
		bool isEnabled = true;
		string condition;
		
		public event EventHandler<EventArgs> ConditionChanged;
		
		public string Condition {
			get { return condition; }
			set {
				if (condition != value) {
					condition = value;
					if (ConditionChanged != null)
						ConditionChanged(this, EventArgs.Empty);
					Redraw();
				}
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object InternalBreakpointObject { get; set; }
		
		[DefaultValue(true)]
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
		
		[DefaultValue(true)]
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
		
		/// <summary>
		/// parameter-less ctor is necessary for deserialization
		/// </summary>
		public BreakpointBookmark()
		{
		}
		
		public BreakpointBookmark(FileName fileName, TextLocation location)
		{
			this.Location = location;
			this.FileName = fileName;
		}
		
		public const string BreakpointMarker = "Breakpoint";
		
		public static readonly Color DefaultBackground = Color.FromRgb(180, 38, 38);
		public static readonly Color DefaultForeground = Colors.White;
		
		public static IImage BreakpointImage {
			get { return SD.ResourceService.GetImage("Bookmarks.Breakpoint"); }
		}
		public static IImage BreakpointConditionalImage {
			get {  return SD.ResourceService.GetImage("Bookmarks.BreakpointConditional"); }
		}
		public static IImage DisabledBreakpointImage {
			get { return SD.ResourceService.GetImage("Bookmarks.DisabledBreakpoint"); }
		}
		public static IImage UnhealthyBreakpointImage {
			get { return SD.ResourceService.GetImage("Bookmarks.UnhealthyBreakpoint"); }
		}
		public static IImage UnhealthyBreakpointConditionalImage {
			get { return SD.ResourceService.GetImage("Bookmarks.UnhealthyBreakpointConditional"); }
		}
		
		public override IImage Image {
			get {
				if (!this.IsEnabled)
					return DisabledBreakpointImage;
				else if (this.IsHealthy)
					return string.IsNullOrEmpty(this.Condition) ? BreakpointImage : BreakpointConditionalImage;
				else
					return string.IsNullOrEmpty(this.Condition) ? UnhealthyBreakpointImage : UnhealthyBreakpointConditionalImage;
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
