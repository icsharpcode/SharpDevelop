// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace XmlDOM
{
	/// <summary>
	/// Handles the text markers for a code editor.
	/// </summary>
	sealed class TextMarkerService : DocumentColorizingTransformer, IBackgroundRenderer, ITextMarkerService
	{
		internal TextSegmentCollection<TextMarker> markers;
		
		TextArea area;
		
		public TextMarkerService(TextArea area)
		{
			this.area = area;
			markers = new TextSegmentCollection<TextMarker>(area.Document);
			this.area.TextView.BackgroundRenderers.Add(this);
			this.area.TextView.LineTransformers.Add(this);
		}
		
		#region ITextMarkerService
		public ITextMarker Create(int startOffset, int length)
		{
			TextMarker m = new TextMarker(this, startOffset, length);
			markers.Add(m);
			// no need to mark segment for redraw: the text marker is invisible until a property is set
			return m;
		}
		
		public IEnumerable<ITextMarker> TextMarkers {
			get { return markers.Cast<ITextMarker>(); }
		}
		
		public void RemoveAll(Predicate<ITextMarker> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			foreach (TextMarker m in markers.ToArray()) {
				if (predicate(m))
					m.Delete();
			}
		}
		
		internal void Remove(TextMarker marker)
		{
			markers.Remove(marker);
			Redraw(marker);
		}
		
		/// <summary>
		/// Redraws the specified text segment.
		/// </summary>
		internal void Redraw(ISegment segment)
		{
			area.TextView.Redraw(segment, DispatcherPriority.Normal);
		}
		#endregion
		
		#region DocumentColorizingTransformer
		protected override void ColorizeLine(DocumentLine line)
		{
			if (markers == null)
				return;
			int lineStart = line.Offset;
			int lineEnd = lineStart + line.Length;
			foreach (TextMarker marker in markers.FindOverlappingSegments(lineStart, line.Length)) {
				Brush foregroundBrush = null;
				if (marker.ForegroundColor != null) {
					foregroundBrush = new SolidColorBrush(marker.ForegroundColor.Value);
					foregroundBrush.Freeze();
				}
				ChangeLinePart(
					Math.Max(marker.StartOffset, lineStart),
					Math.Min(marker.EndOffset, lineEnd),
					element => {
						if (foregroundBrush != null) {
							element.TextRunProperties.SetForegroundBrush(foregroundBrush);
						}
					}
				);
			}
		}
		#endregion
		
		#region IBackgroundRenderer
		public KnownLayer Layer {
			get {
				// draw behind selection
				return KnownLayer.Selection;
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			if (drawingContext == null)
				throw new ArgumentNullException("drawingContext");
			if (markers == null || !textView.VisualLinesValid)
				return;
			var visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
				return;
			int viewStart = visualLines.First().FirstDocumentLine.Offset;
			int viewEnd = visualLines.Last().LastDocumentLine.Offset + visualLines.Last().LastDocumentLine.Length;
			foreach (TextMarker marker in markers.FindOverlappingSegments(viewStart, viewEnd - viewStart)) {
				if (marker.BackgroundColor != null) {
					BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
					geoBuilder.AddSegment(textView, marker);
					Geometry geometry = geoBuilder.CreateGeometry();
					if (geometry != null) {
						Color color = marker.BackgroundColor.Value;
						SolidColorBrush brush = new SolidColorBrush(color);
						brush.Freeze();
						drawingContext.DrawGeometry(brush, null, geometry);
					}
				}
			}
		}
		#endregion
	}
	
	sealed class TextMarker : TextSegment, ITextMarker
	{
		readonly TextMarkerService service;
		
		public TextMarker(TextMarkerService service, int startOffset, int length)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			this.service = service;
			this.StartOffset = startOffset;
			this.Length = length;
		}
		
		public event EventHandler Deleted;
		
		public bool IsDeleted {
			get { return !this.IsConnectedToCollection; }
		}
		
		public void Delete()
		{
			if (this.IsConnectedToCollection) {
				service.Remove(this);
				if (Deleted != null)
					Deleted(this, EventArgs.Empty);
			}
		}
		
		void Redraw()
		{
			service.Redraw(this);
		}
		
		Color? backgroundColor;
		
		public Color? BackgroundColor {
			get { return backgroundColor; }
			set {
				if (backgroundColor != value) {
					backgroundColor = value;
					Redraw();
				}
			}
		}
		
		Color? foregroundColor;
		
		public Color? ForegroundColor {
			get { return foregroundColor; }
			set {
				if (foregroundColor != value) {
					foregroundColor = value;
					Redraw();
				}
			}
		}
		
		public object Tag { get; set; }
	}
	
	/// <summary>
	/// Represents a text marker.
	/// </summary>
	public interface ITextMarker
	{
		/// <summary>
		/// Gets the start offset of the marked text region.
		/// </summary>
		int StartOffset { get; }
		
		/// <summary>
		/// Gets the end offset of the marked text region.
		/// </summary>
		int EndOffset { get; }
		
		/// <summary>
		/// Gets the length of the marked region.
		/// </summary>
		int Length { get; }
		
		/// <summary>
		/// Deletes the text marker.
		/// </summary>
		void Delete();
		
		/// <summary>
		/// Gets whether the text marker was deleted.
		/// </summary>
		bool IsDeleted { get; }
		
		/// <summary>
		/// Event that occurs when the text marker is deleted.
		/// </summary>
		event EventHandler Deleted;
		
		/// <summary>
		/// Gets/Sets the background color.
		/// </summary>
		Color? BackgroundColor { get; set; }
		
		/// <summary>
		/// Gets/Sets the foreground color.
		/// </summary>
		Color? ForegroundColor { get; set; }
		
		/// <summary>
		/// Gets/Sets an object with additional data for this text marker.
		/// </summary>
		object Tag { get; set; }
	}
	
	public interface ITextMarkerService
	{
		/// <summary>
		/// Creates a new text marker. The text marker will be invisible at first,
		/// you need to set one of the Color properties to make it visible.
		/// </summary>
		ITextMarker Create(int startOffset, int length);
		
		/// <summary>
		/// Gets the list of text markers.
		/// </summary>
		IEnumerable<ITextMarker> TextMarkers { get; }
		
		/// <summary>
		/// Removes all text markers that match the condition.
		/// </summary>
		void RemoveAll(Predicate<ITextMarker> predicate);
	}
}
