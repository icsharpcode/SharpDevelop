// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Creates and removes text marker for an XPath query match.
	/// </summary>
	public class XPathNodeTextMarker
	{
		public static readonly Color MarkerBackColor = Color.FromArgb(255, 159, 255, 162);
		
		IDocument document;
		ITextMarkerService markerService;
		
		public XPathNodeTextMarker(IDocument document)
		{
			this.document = document;
			markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
		}
		
		public void AddMarkers(XPathNodeMatch[] nodes)
		{
			foreach (XPathNodeMatch node in nodes) {
				AddMarker(node);
			}
		}
		
		public void AddMarker(XPathNodeMatch node)
		{
			if (node.HasLineInfo() && node.Value.Length > 0) {
				int offset = document.PositionToOffset(node.LineNumber + 1, node.LinePosition + 1);
				if (markerService != null) {
					ITextMarker marker = markerService.Create(offset, node.Value.Length);
					marker.Tag = typeof(XPathNodeTextMarker);
					marker.BackgroundColor = MarkerBackColor;
				}
			}
		}
		
		public void RemoveMarkers()
		{
			ITextMarkerService markerService = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			if (markerService != null) {
				markerService.RemoveAll(IsXPathNodeTextMarker);
			}
		}
		
		bool IsXPathNodeTextMarker(ITextMarker marker)
		{
			return (Type)marker.Tag == typeof(XPathNodeTextMarker);
		}
	}
}
