// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Creates and removes text marker for an XPath query match.
	/// </summary>
	public static class XPathNodeTextMarker
	{
		public static readonly Color MarkerBackColor = Color.FromArgb(255, 159, 255, 162);
		
		/// <summary>
		/// Adds markers for each XPathNodeMatch.
		/// </summary>
		public static void AddMarkers(IDocument document, XPathNodeMatch[] nodes)
		{
			foreach (XPathNodeMatch node in nodes) {
				AddMarker(document, node);
			}
		}
		
		/// <summary>
		/// Adds a single marker for the XPathNodeMatch.
		/// </summary>
		public static void AddMarker(IDocument document, XPathNodeMatch node)
		{
			if (node.HasLineInfo() && node.Value.Length > 0) {
				int offset = document.PositionToOffset(node.LineNumber + 1, node.LinePosition + 1);
				ITextMarkerService markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
				if (markerService != null) {
					ITextMarker marker = markerService.Create(offset, node.Value.Length);
					marker.Tag = typeof(XPathNodeTextMarker);
					marker.BackgroundColor = MarkerBackColor;
				}
			}
		}
		
		/// <summary>
		/// Removes all the XPathNodeMarkers from the marker strategy.
		/// </summary>
		public static void RemoveMarkers(IDocument document)
		{
			ITextMarkerService markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			if (markerService != null) {
				markerService.RemoveAll(marker => (Type)marker.Tag == typeof(XPathNodeTextMarker));
			}
		}
	}
}
