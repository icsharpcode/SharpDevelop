// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockTextMarker : ITextMarker
	{
		public event EventHandler Deleted { add {} remove {} }
		
		List<ITextMarker> markers;
		int start, end, length;
		
		public MockTextMarker(List<ITextMarker> markers, int start, int end, int length)
		{
			this.markers = markers;
			this.start = start;
			this.end = end;
			this.length = length;
		}
		
		public int StartOffset {
			get {
				return this.start;
			}
		}
		
		public int EndOffset {
			get {
				return this.end;
			}
		}
		
		public int Length {
			get {
				return this.length;
			}
		}
		
		public bool IsDeleted {
			get {
				return !markers.Contains(this);
			}
		}
		
		public Nullable<Color> BackgroundColor { get; set; }
		public Nullable<Color> ForegroundColor { get; set; }
		public TextMarkerType MarkerType { get; set; }
		public Color MarkerColor { get; set; }
		public object Tag { get; set; }
		public object ToolTip { get; set; }
		
		public void Delete()
		{
			this.markers.Remove(this);
		}
	}
}
