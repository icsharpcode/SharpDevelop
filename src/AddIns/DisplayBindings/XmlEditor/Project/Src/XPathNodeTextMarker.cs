// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// A text marker for an XPath query match.
	/// </summary>
	public class XPathNodeTextMarker : TextMarker
	{	
		public static readonly Color MarkerBackColor = Color.FromArgb(159, 255, 162);
		
		public XPathNodeTextMarker(int offset, XPathNodeMatch node) : base(offset, node.Value.Length, TextMarkerType.SolidBlock, MarkerBackColor)
		{
		}
		
		/// <summary>
		/// Adds markers for each XPathNodeMatch.
		/// </summary>
		public static void AddMarkers(MarkerStrategy markerStrategy, XPathNodeMatch[] nodes)
		{
			foreach (XPathNodeMatch node in nodes) {
				AddMarker(markerStrategy, node);
			}
		}
		
		/// <summary>
		/// Adds a single marker for the XPathNodeMatch.
		/// </summary>
		public static void AddMarker(MarkerStrategy markerStrategy, XPathNodeMatch node)
		{
			if (node.HasLineInfo() && node.Value.Length > 0) {
				LineSegment lineSegment = markerStrategy.Document.GetLineSegment(node.LineNumber);
				markerStrategy.AddMarker(new XPathNodeTextMarker(lineSegment.Offset + node.LinePosition, node));
			}
		}
		
		/// <summary>
		/// Removes all the XPathNodeMarkers from the marker strategy.
		/// </summary>
		public static void RemoveMarkers(MarkerStrategy markerStrategy)
		{
			markerStrategy.RemoveAll(IsXPathNodeTextMarkerMatch);
		}
		
		static bool IsXPathNodeTextMarkerMatch(TextMarker marker)
		{
			return marker is XPathNodeTextMarker;
		}
	}
}
