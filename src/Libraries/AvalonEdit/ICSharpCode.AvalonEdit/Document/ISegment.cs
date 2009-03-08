// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// An (Offset,Length)-pair.
	/// </summary>
	public interface ISegment
	{
		/// <summary>
		/// Gets the start offset of the segment.
		/// </summary>
		int Offset { get; }
		
		/// <summary>
		/// Gets the length of the segment.
		/// </summary>
		int Length { get; }
	}
	
	/// <summary>
	/// Represents a simple segment (Offset,Length pair) that is not automatically updated
	/// on document changes.
	/// </summary>
	struct SimpleSegment : IEquatable<SimpleSegment>, ISegment
	{
		public static readonly SimpleSegment Invalid = new SimpleSegment(-1, -1);
		
		public readonly int Offset, Length;
		
		int ISegment.Offset {
			get { return Offset; }
		}
		
		int ISegment.Length {
			get { return Length; }
		}
		
		internal int GetEndOffset()
		{
			return Offset + Length;
		}
		
		public SimpleSegment(int offset, int length)
		{
			this.Offset = offset;
			this.Length = length;
		}
		
		public SimpleSegment(ISegment segment)
		{
			Debug.Assert(segment != null);
			this.Offset = segment.Offset;
			this.Length = segment.Length;
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return Offset + 10301 * Length;
			}
		}
		
		public override bool Equals(object obj)
		{
			return (obj is SimpleSegment) && Equals((SimpleSegment)obj);
		}
		
		public bool Equals(SimpleSegment other)
		{
			return this.Offset == other.Offset && this.Length == other.Length;
		}
		
		public static bool operator ==(SimpleSegment left, SimpleSegment right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(SimpleSegment left, SimpleSegment right)
		{
			return !left.Equals(right);
		}
	}
	
	/// <summary>
	/// A segment using text anchors as start and end positions.
	/// </summary>
	sealed class AnchorSegment : ISegment
	{
		readonly TextAnchor start, end;
		
		public int Offset {
			get { return start.Offset; }
		}
		
		public int Length {
			get { return end.Offset - start.Offset; }
		}
		
		internal int GetEndOffset()
		{
			return end.Offset;
		}
		
		public AnchorSegment(TextAnchor start, TextAnchor end)
		{
			Debug.Assert(start != null);
			Debug.Assert(end != null);
			Debug.Assert(start.SurviveDeletion);
			Debug.Assert(end.SurviveDeletion);
			this.start = start;
			this.end = end;
		}
		
		public AnchorSegment(TextDocument document, ISegment segment)
			: this(document, segment.Offset, segment.Length)
		{
		}
		
		public AnchorSegment(TextDocument document, int offset, int length)
		{
			Debug.Assert(document != null);
			this.start = document.CreateAnchor(offset);
			this.start.SurviveDeletion = true;
			this.end = document.CreateAnchor(offset + length);
			this.end.SurviveDeletion = true;
		}
	}
}
