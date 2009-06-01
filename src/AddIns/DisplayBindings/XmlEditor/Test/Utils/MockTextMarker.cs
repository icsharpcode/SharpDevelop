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
	public class MockTextMarker : ITextMarker
	{
		public event EventHandler Deleted;
		
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
		
		public Nullable<Color> BackgroundColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public Nullable<Color> ForegroundColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public object Tag { get; set; }
		
		public void Delete()
		{
			this.markers.Remove(this);
		}
	}
}
