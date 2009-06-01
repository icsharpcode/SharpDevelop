// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
 	/// <summary>
	/// A text marker for an XPath query match. Wraps ITextMarker.
	/// </summary>
	public class XPathNodeTextMarker
	{	
		public static readonly Color MarkerBackColor = Color.FromArgb(159, 255, 162);
		static List<XPathNodeTextMarker> markers = new List<XPathNodeTextMarker>();
		ITextMarker marker;
		
		XPathNodeTextMarker(IDocument document, int offset, XPathNodeMatch node)
		{
			ITextMarkerService markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			marker = markerService.Create(offset, node.Value.Length);
			marker.Tag = this;
		}
		
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
				markers.Add(new XPathNodeTextMarker(document, document.PositionToOffset(node.LineNumber + 1, node.LinePosition + 1), node));
			}
		}
		
		/// <summary>
		/// Removes all the XPathNodeMarkers from the marker strategy.
		/// </summary>
		public static void RemoveMarkers(IDocument document)
		{
			ITextMarkerService markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			
			ITextMarker[] list = markerService.TextMarkers.ToArray();
			
			foreach (ITextMarker item in list) {
				if (item.Tag is XPathNodeTextMarker)
					item.Delete();
			}
		}
	}
}
