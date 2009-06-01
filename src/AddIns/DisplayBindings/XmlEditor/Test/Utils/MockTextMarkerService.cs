// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;
using System.Windows.Media;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Description of MockTextEditor.
	/// </summary>
	public class MockTextMarkerService : ITextMarkerService
	{
		List<ITextMarker> markers;
		
		public MockTextMarkerService()
		{
			this.markers = new List<ITextMarker>();
		}
		
		public System.Collections.Generic.IEnumerable<ITextMarker> TextMarkers {
			get {
				return this.markers;
			}
		}
		
		public ITextMarker Create(int startOffset, int length)
		{
			ITextMarker m = new MockTextMarker(this.markers, startOffset, startOffset + length, length);
			this.markers.Add(m);
			return m;
		}
	}
}
