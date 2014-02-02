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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;

namespace Debugger.AddIn.Breakpoints
{
	public class BreakpointBookmark : SDMarkerBookmark, IHaveStateEnabled
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
			marker.BackgroundColor = BookmarkBase.BreakpointDefaultBackground;
			marker.ForegroundColor = BookmarkBase.BreakpointDefaultForeground;
			marker.MarkerColor = BookmarkBase.BreakpointDefaultBackground;
			marker.MarkerTypes = TextMarkerTypes.CircleInScrollBar;
			
			if (highlighter != null) {
				var color = highlighter.GetNamedColor(BookmarkBase.BreakpointMarkerName);
				if (color != null) {
					marker.BackgroundColor = color.Background.GetColor(null);
					marker.MarkerColor = color.Background.GetColor(null) ?? BookmarkBase.BreakpointDefaultForeground;
					marker.ForegroundColor = color.Foreground.GetColor(null);
				}
			}
			return marker;
		}
		
		public override string ToString()
		{
			return string.Format("{0} @{1}", this.FileName, this.LineNumber);
		}
		
		public override object CreateTooltipContent()
		{
			return new Popup {
				StaysOpen = false,
				Child = new Border {
					Child = new BreakpointEditor(this),
					BorderBrush = Brushes.Black,
					BorderThickness = new Thickness(1)
				}
			};
		}
	}
}
