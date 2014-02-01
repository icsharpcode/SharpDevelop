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
		public TextMarkerTypes MarkerTypes { get; set; }
		public Color MarkerColor { get; set; }
		public object Tag { get; set; }
		public object ToolTip { get; set; }
		public Nullable<System.Windows.FontWeight> FontWeight { get; set; }
		public Nullable<System.Windows.FontStyle> FontStyle { get; set; }
		
		public void Delete()
		{
			this.markers.Remove(this);
		}
	}
}
