// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.WixBinding
{
	public class WixDocumentLineSegment : ISegment
	{
		int offset;
		int length;
		
		public WixDocumentLineSegment(int offset, int length)
		{
			this.offset = offset;
			this.length = length;
		}
		
		public int Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		
		public int Length {
			get {
				return length;
			}
			set {
				length = value;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[LineSegment: Offset {0}, Length {1}]", offset, length);
		}
	
		public override int GetHashCode()
		{
			return offset.GetHashCode() ^ length.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			WixDocumentLineSegment lineSegment = obj as WixDocumentLineSegment;
			if (lineSegment == null) return false; 
			if (this == lineSegment) return true;
			return offset == lineSegment.offset && length == lineSegment.length;
		}
	}
}
