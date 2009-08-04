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
	/// A segment that can be put into a TextSegmentCollection.
	/// </summary>
	/// <remarks>
	/// On insertions at the start or end offset of the text segment, a TextSegmentCollection handling the document
	/// changes will keep the TextSegment small;
	/// i.e. use AfterInsertion for the start position and BeforeInsertion for the end position.
	/// </remarks>
	public class TextSegment : ISegment
	{
		internal ISegmentTree ownerTree;
		internal TextSegment left, right, parent;
		
		/// <summary>
		/// The color of the segment in the red/black tree.
		/// </summary>
		internal bool color;
		
		/// <summary>
		/// The "length" of the node (distance to previous node)
		/// </summary>
		internal int nodeLength;
		
		/// <summary>
		/// The total "length" of this subtree.
		/// </summary>
		internal int totalNodeLength; // totalNodeLength = nodeLength + left.totalNodeLength + right.totalNodeLength
		
		/// <summary>
		/// The length of the segment (do not confuse with nodeLength).
		/// </summary>
		internal int segmentLength;
		
		/// <summary>
		/// distanceToMaxEnd = Max(segmentLength,
		///                        left.distanceToMaxEnd + left.Offset - Offset,
		///                        left.distanceToMaxEnd + right.Offset - Offset)
		/// </summary>
		internal int distanceToMaxEnd;
		
		int ISegment.Offset {
			get { return StartOffset; }
		}
		
		/// <summary>
		/// Gets whether this segment is connected to a TextSegmentCollection and will automatically
		/// update its offsets.
		/// </summary>
		protected bool IsConnectedToCollection {
			get {
				return ownerTree != null;
			}
		}
		
		/// <summary>
		/// Gets/Sets the start offset of the segment.
		/// </summary>
		public int StartOffset {
			get {
				// If the segment is not connected to a tree, we store the offset in "nodeLength".
				// Otherwise, "nodeLength" contains the distance to the start offset of the previous node
				Debug.Assert(!(ownerTree == null && parent != null));
				Debug.Assert(!(ownerTree == null && left != null));
				
				TextSegment n = this;
				int offset = n.nodeLength;
				if (n.left != null)
					offset += n.left.totalNodeLength;
				while (n.parent != null) {
					if (n == n.parent.right) {
						if (n.parent.left != null)
							offset += n.parent.left.totalNodeLength;
						offset += n.parent.nodeLength;
					}
					n = n.parent;
				}
				return offset;
			}
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "Offset must not be negative");
				if (this.StartOffset != value) {
					// need a copy of the variable because ownerTree.Remove() sets this.ownerTree to null
					ISegmentTree ownerTree = this.ownerTree;
					if (ownerTree != null) {
						ownerTree.Remove(this);
						nodeLength = value;
						ownerTree.Add(this);
					} else {
						nodeLength = value;
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the end offset of the segment.
		/// </summary>
		public int EndOffset {
			get {
				return StartOffset + Length;
			}
		}
		
		/// <summary>
		/// Gets/Sets the length of the segment.
		/// </summary>
		public int Length {
			get {
				return segmentLength;
			}
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "Length must not be negative");
				segmentLength = value;
				if (ownerTree != null)
					ownerTree.UpdateAugmentedData(this);
			}
		}
		
		internal TextSegment LeftMost {
			get {
				TextSegment node = this;
				while (node.left != null)
					node = node.left;
				return node;
			}
		}
		
		internal TextSegment RightMost {
			get {
				TextSegment node = this;
				while (node.right != null)
					node = node.right;
				return node;
			}
		}
		
		/// <summary>
		/// Gets the inorder successor of the node.
		/// </summary>
		internal TextSegment Successor {
			get {
				if (right != null) {
					return right.LeftMost;
				} else {
					TextSegment node = this;
					TextSegment oldNode;
					do {
						oldNode = node;
						node = node.parent;
						// go up until we are coming out of a left subtree
					} while (node != null && node.right == oldNode);
					return node;
				}
			}
		}
		
		/// <summary>
		/// Gets the inorder predecessor of the node.
		/// </summary>
		internal TextSegment Predecessor {
			get {
				if (left != null) {
					return left.RightMost;
				} else {
					TextSegment node = this;
					TextSegment oldNode;
					do {
						oldNode = node;
						node = node.parent;
						// go up until we are coming out of a right subtree
					} while (node != null && node.left == oldNode);
					return node;
				}
			}
		}
		
		#if DEBUG
		internal string ToDebugString()
		{
			return "[nodeLength=" + nodeLength + " totalNodeLength=" + totalNodeLength
				+ " distanceToMaxEnd=" + distanceToMaxEnd + " MaxEndOffset=" + (StartOffset + distanceToMaxEnd) + "]";
		}
		#endif
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[" + GetType().Name + " Offset=" + StartOffset + " Length=" + Length + " EndOffset=" + EndOffset + "]";
		}
	}
}
