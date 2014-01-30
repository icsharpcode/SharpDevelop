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
using System.Collections.Generic;
using System.Windows.Media;

using ICSharpCode.NRefactory.Editor;
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
				int offset = document.GetOffset(node.LineNumber + 1, node.LinePosition + 1);
				if (markerService != null) {
					ITextMarker marker = markerService.Create(offset, node.Value.Length);
					marker.Tag = typeof(XPathNodeTextMarker);
					marker.BackgroundColor = MarkerBackColor;
				}
			}
		}
		
		/// <summary>
		/// Removes all markers from the given view content or text editor.
		/// </summary>
		public static void RemoveMarkers(IServiceProvider serviceProvider)
		{
			ITextMarkerService markerService = serviceProvider.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			if (markerService != null) {
				markerService.RemoveAll(IsXPathNodeTextMarker);
			}
		}
		
		static bool IsXPathNodeTextMarker(ITextMarker marker)
		{
			return (Type)marker.Tag == typeof(XPathNodeTextMarker);
		}
	}
}
