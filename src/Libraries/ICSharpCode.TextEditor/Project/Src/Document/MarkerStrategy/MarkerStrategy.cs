// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.TextEditor.Document
{	
	/// <summary>
	/// Description of MarkerStrategy.	
	/// </summary>
	public class MarkerStrategy
	{
		List<TextMarker> textMarker = new List<TextMarker>();
		IDocument document;
		
		public IDocument Document {
			get {
				return document;
			}
		}
		
		public List<TextMarker> TextMarker {
			get {
				return textMarker;
			}
		}
		
		public MarkerStrategy(IDocument document)
		{
			this.document = document;
			document.DocumentChanged += new DocumentEventHandler(DocumentChanged);
		}
		
//// Alex: minimize GC allocations - it's heavy on heap
		Dictionary<int, List<TextMarker>> markersTable = new Dictionary<int, List<TextMarker>>();
		
		public List<TextMarker> GetMarkers(int offset)
		{
			if (!markersTable.ContainsKey(offset)) {
				List<TextMarker> markers = new List<TextMarker>();
				for (int i = 0; i < textMarker.Count; ++i) {
					TextMarker marker = (TextMarker)textMarker[i];
					if (marker.Offset <= offset && offset <= marker.Offset + marker.Length) {
						markers.Add(marker);
					}
				}
				markersTable[offset] = markers;
			}
			return markersTable[offset];
		}
		
		public List<TextMarker> GetMarkers(int offset, int length)
		{
			List<TextMarker> markers = new List<TextMarker>();
			for (int i = 0; i < textMarker.Count; ++i) {
				TextMarker marker = (TextMarker)textMarker[i];
				if (marker.Offset <= offset && offset <= marker.Offset + marker.Length ||
				    marker.Offset <= offset + length && offset + length <= marker.Offset + marker.Length ||
				    offset <= marker.Offset && marker.Offset <= offset + length ||
				    offset <= marker.Offset + marker.Length && marker.Offset + marker.Length <= offset + length
				    ) {
					markers.Add(marker);
				}
			}
			return markers;
		}
		
		public List<TextMarker> GetMarkers(Point position)
		{
			List<TextMarker> markers = new List<TextMarker>();
			if (position.Y >= document.TotalNumberOfLines || position.Y < 0) {
				return markers;
			}
			LineSegment segment = document.GetLineSegment(position.Y);
			return GetMarkers(segment.Offset + position.X);
		}
		
		void DocumentChanged(object sender, DocumentEventArgs e)
		{
			// reset markers table
			markersTable.Clear();
			document.UpdateSegmentListOnDocumentChange(textMarker, e);
		}
	}
}
