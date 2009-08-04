// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

using ICSharpCode.AvalonEdit.Utils;
using System.Text;

namespace ICSharpCode.AvalonEdit.Document
{
	[Serializable]
	sealed class RopeNode<T>
	{
		internal const int NodeSize = 256;
		
		internal static readonly RopeNode<T> emptyRopeNode = new RopeNode<T> { isShared = true, contents = new T[RopeNode<T>.NodeSize] };
		
		// Fields for pointers to sub-nodes. Only non-null for concat nodes (height>=1)
		internal RopeNode<T> left, right;
		internal volatile bool isShared; // specifies whether this node is shared between multiple ropes
		// the total length of all text in this subtree
		internal int length;
		// the height of this subtree: 0 for leaf nodes; 1+max(left.height,right.height) for concat nodes
		internal byte height;
		
		// The character data. Only non-null for leaf nodes (height=0).
		internal T[] contents;
		
		internal int Balance {
			get { return right.height - left.height; }
		}
		
		[Conditional("DEBUG")]
		internal void CheckInvariants()
		{
			if (height == 0) {
				Debug.Assert(left == null && right == null);
				Debug.Assert(contents != null && contents.Length == NodeSize);
				Debug.Assert(length >= 0 && length <= NodeSize);
			} else {
				Debug.Assert(left != null && right != null);
				Debug.Assert(contents == null);
				Debug.Assert(length == left.length + right.length);
				Debug.Assert(height == 1 + Math.Max(left.height, right.height));
				Debug.Assert(Math.Abs(this.Balance) <= 1);
				
				// this is an additional invariant that forces the tree to combine small leafs to prevent excessive memory usage:
				Debug.Assert(length > NodeSize);
				// note that the this invariant ensures that all nodes except for the empty rope's single node have at least length 1
				
				if (isShared)
					Debug.Assert(left.isShared && right.isShared);
				left.CheckInvariants();
				right.CheckInvariants();
			}
		}
		
		internal RopeNode<T> Clone()
		{
			if (height == 0) {
				T[] newContents = new T[NodeSize];
				contents.CopyTo(newContents, 0);
				return new RopeNode<T> {
					length = this.length,
					contents = newContents
				};
			} else {
				return new RopeNode<T> {
					left = this.left,
					right = this.right,
					length = this.length,
					height = this.height
				};
			}
		}
		
		internal RopeNode<T> CloneIfShared()
		{
			if (isShared)
				return Clone();
			else
				return this;
		}
		
		internal void Publish()
		{
			if (!isShared) {
				if (left != null)
					left.Publish();
				if (right != null)
					right.Publish();
				// it's important that isShared=true is set at the end:
				// Publish() must not return until the whole subtree is marked as shared, even when
				// Publish() is called concurrently.
				isShared = true;
			}
		}
		
		internal static RopeNode<T> CreateFromArray(T[] arr, int index, int length)
		{
			if (length == 0) {
				return emptyRopeNode;
			}
			int nodeCount = (length + NodeSize - 1) / NodeSize;
			RopeNode<T> node = CreateNodes(nodeCount);
			node.StoreElements(arr, index, length);
			return node;
		}
		
		internal static RopeNode<T> CreateNodes(int leafCount)
		{
			Debug.Assert(leafCount > 0);
			if (leafCount == 1) {
				return new RopeNode<T> { contents = new T[NodeSize] };
			} else {
				int rightSide = leafCount / 2;
				int leftSide = leafCount - rightSide;
				RopeNode<T> result = new RopeNode<T>();
				result.left = CreateNodes(leftSide);
				result.right = CreateNodes(rightSide);
				result.height = (byte)(1 + Math.Max(result.left.height, result.right.height));
				return result;
			}
		}
		
		/// <summary>
		/// Balances this node and recomputes the 'height' field.
		/// This method assumes that the children of this node are already balanced and have an up-to-date 'height' value.
		/// </summary>
		internal void Rebalance()
		{
			// the tree is always balanced before sharing, so a shared node cannot be unbalanced
			if (isShared)
				return;
			// leaf nodes are always balanced (we don't use 'height' to detect leaf nodes here because Balance is supposed to recompute the height).
			if (left == null)
				return;
			
			Debug.Assert(this.length > NodeSize);
			
			// We need to loop until it's balanced. Rotations might cause two small leaves to combine to a larger one,
			// which changes the height and might mean we need additional balancing steps.
			while (Math.Abs(this.Balance) > 1) {
				// AVL balancing
				// note: because we don't care about the identity of concat nodes, this works a little different than usual
				// tree rotations: in our implementation, the "this" node will stay at the top, only its children are rearranged
				if (this.Balance > 1) {
					if (right.Balance < 0) {
						right = right.CloneIfShared();
						right.RotateRight();
					}
					this.RotateLeft();
					// If 'this' was unbalanced by more than 2, we've shifted some of the inbalance to the left node; so rebalance that.
					this.left.Rebalance();
				} else if (this.Balance < -1) {
					if (left.Balance > 0) {
						left = left.CloneIfShared();
						left.RotateLeft();
					}
					this.RotateRight();
					// If 'this' was unbalanced by more than 2, we've shifted some of the inbalance to the right node; so rebalance that.
					this.right.Rebalance();
				}
			}
			
			Debug.Assert(Math.Abs(this.Balance) <= 1);
			this.height = (byte)(1 + Math.Max(left.height, right.height));
		}
		
		void RotateLeft()
		{
			Debug.Assert(!isShared);
			
			/* Rotate tree to the left
			 * 
			 *       this               this
			 *       /  \               /  \
			 *      A   right   ===>  left  C
			 *           / \          / \
			 *          B   C        A   B
			 */
			RopeNode<T> a = left;
			RopeNode<T> b = right.left;
			RopeNode<T> c = right.right;
			// reuse right concat node, if possible
			this.left = right.isShared ? new RopeNode<T>() : right;
			this.left.left = a;
			this.left.right = b;
			this.left.length = a.length + b.length;
			this.left.height = (byte)(1 + Math.Max(a.height, b.height));
			this.right = c;
			
			this.left.MergeIfPossible();
		}
		
		void RotateRight()
		{
			Debug.Assert(!isShared);
			
			/* Rotate tree to the right
			 * 
			 *       this             this
			 *       /  \             /  \
			 *     left  C   ===>    A  right
			 *     / \                   /  \
			 *    A   B                 B    C
			 */
			RopeNode<T> a = left.left;
			RopeNode<T> b = left.right;
			RopeNode<T> c = right;
			// reuse left concat node, if possible
			this.right = left.isShared ? new RopeNode<T>() : left;
			this.right.left = b;
			this.right.right = c;
			this.right.length = b.length + c.length;
			this.right.height = (byte)(1 + Math.Max(b.height, c.height));
			this.left = a;
			
			this.right.MergeIfPossible();
		}
		
		void MergeIfPossible()
		{
			Debug.Assert(!isShared);
			
			if (this.length <= NodeSize) {
				// convert this concat node to leaf node
				this.height = 0;
				int lengthOnLeftSide = this.left.length;
				if (this.left.isShared) {
					this.contents = new T[NodeSize];
					Array.Copy(left.contents, 0, this.contents, 0, lengthOnLeftSide);
				} else {
					// steal buffer from left side
					this.contents = this.left.contents;
				}
				this.left = null;
				Array.Copy(right.contents, 0, this.contents, lengthOnLeftSide, this.right.length);
				this.right = null;
			}
		}
		
		/// <summary>
		/// Stores the specified text in this node.
		/// </summary>
		internal void StoreElements(T[] array, int arrayIndex, int count)
		{
			Debug.Assert(!isShared);
			if (height == 0) {
				length = Math.Min(NodeSize, count);
				Array.Copy(array, arrayIndex, contents, 0, length);
			} else {
				left.StoreElements(array, arrayIndex, count);
				right.StoreElements(array, arrayIndex + left.length, count - left.length);
				length = left.length + right.length;
			}
		}
		
		internal void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			if (height == 0) {
				Array.Copy(this.contents, index, array, arrayIndex, count);
			} else {
				if (index + count <= this.left.length) {
					this.left.CopyTo(index, array, arrayIndex, count);
				} else if (index >= this.left.length) {
					this.right.CopyTo(index - this.left.length, array, arrayIndex, count);
				} else {
					int amountInLeft = this.left.length - index;
					this.left.CopyTo(index, array, arrayIndex, amountInLeft);
					this.right.CopyTo(0, array, arrayIndex + amountInLeft, count - amountInLeft);
				}
			}
		}
		
		internal RopeNode<T> SetElement(int offset, T value)
		{
			RopeNode<T> result = CloneIfShared();
			if (result.height == 0) {
				result.contents[offset] = value;
			} else if (offset < result.left.length) {
				result.left = result.left.SetElement(offset, value);
			} else {
				result.right = result.right.SetElement(offset - result.left.length, value);
			}
			return result;
		}
		
		internal static RopeNode<T> Concat(RopeNode<T> left, RopeNode<T> right)
		{
			if (left.length == 0)
				return right;
			if (right.length == 0)
				return left;
			
			if (left.length + right.length <= NodeSize) {
				left = left.CloneIfShared();
				Array.Copy(right.contents, 0, left.contents, left.length, right.length);
				left.length += right.length;
				return left;
			} else {
				RopeNode<T> concatNode = new RopeNode<T>();
				concatNode.left = left;
				concatNode.right = right;
				concatNode.length = left.length + right.length;
				concatNode.Rebalance();
				return concatNode;
			}
		}
		
		/// <summary>
		/// Splits this leaf node at offset and returns a new node with the part of the text after offset.
		/// </summary>
		RopeNode<T> SplitAfter(int offset)
		{
			Debug.Assert(!isShared && height == 0);
			RopeNode<T> newPart = new RopeNode<T>();
			newPart.contents = new T[NodeSize];
			newPart.length = this.length - offset;
			Array.Copy(this.contents, offset, newPart.contents, 0, newPart.length);
			this.length = offset;
			return newPart;
		}
		
		internal RopeNode<T> Insert(int offset, RopeNode<T> newElements)
		{
			if (offset == 0) {
				return Concat(newElements, this);
			} else if (offset == this.length) {
				return Concat(this, newElements);
			}
			
			if (height == 0) {
				// we'll need to split this node
				RopeNode<T> left = CloneIfShared();
				RopeNode<T> right = left.SplitAfter(offset);
				return Concat(Concat(left, newElements), right);
			} else {
				RopeNode<T> result = CloneIfShared();
				if (offset < result.left.length) {
					result.left = result.left.Insert(offset, newElements);
				} else {
					result.right = result.right.Insert(offset - result.left.length, newElements);
				}
				result.length += newElements.length;
				result.Rebalance();
				return result;
			}
		}
		
		internal RopeNode<T> Insert(int offset, T[] array, int arrayIndex, int count)
		{
			Debug.Assert(count > 0);
			
			if (height == 0) {
				if (this.length + count < RopeNode<char>.NodeSize) {
					RopeNode<T> result = CloneIfShared();
					int lengthAfterOffset = this.length - offset;
					T[] resultContents = result.contents;
					for (int i = lengthAfterOffset; i >= 0; i--) {
						resultContents[i + offset + count] = resultContents[i + offset];
					}
					Array.Copy(array, arrayIndex, resultContents, offset, count);
					result.length += count;
					return result;
				} else {
					// TODO: implement this more efficiently?
					return Insert(offset, CreateFromArray(array, arrayIndex, count));
				}
			} else {
				RopeNode<T> result = CloneIfShared();
				if (offset < result.left.length) {
					result.left = result.left.Insert(offset, array, arrayIndex, count);
				} else {
					result.right = result.right.Insert(offset - result.left.length, array, arrayIndex, count);
				}
				result.length += count;
				result.Rebalance();
				return result;
			}
		}
		
		internal RopeNode<T> RemoveRange(int index, int count)
		{
			Debug.Assert(count > 0);
			
			// produce empty node when one node is deleted completely
			if (index == 0 && count == this.length)
				return emptyRopeNode;
			
			int endIndex = index + count;
			if (height == 0) {
				RopeNode<T> result = CloneIfShared();
				int remainingAfterEnd = result.length - endIndex;
				for (int i = 0; i < remainingAfterEnd; i++) {
					result.contents[index + i] = result.contents[endIndex + i];
				}
				result.length -= count;
				return result;
			} else {
				RopeNode<T> result = CloneIfShared();
				if (endIndex <= this.left.length) {
					// deletion is only within the left part
					result.left = result.left.RemoveRange(index, count);
				} else if (index >= this.left.length) {
					// deletion is only within the right part
					result.right = result.right.RemoveRange(index - this.left.length, count);
				} else {
					// deletion overlaps both parts
					int deletionAmountOnLeftSide = this.left.length - index;
					result.left = result.left.RemoveRange(index, deletionAmountOnLeftSide);
					result.right = result.right.RemoveRange(0, count - deletionAmountOnLeftSide);
				}
				// The deletion might have introduced empty nodes. Those must be removed.
				if (result.left.length == 0)
					return result.right;
				if (result.right.length == 0)
					return result.left;
				
				result.length -= count;
				result.MergeIfPossible();
				result.Rebalance();
				return result;
			}
		}
		
		#region Debug Output
		#if DEBUG
		void AppendTreeToString(StringBuilder b, int indent)
		{
			b.AppendLine(ToString());
			indent += 2;
			if (left != null) {
				b.Append(' ', indent);
				b.Append("L: ");
				left.AppendTreeToString(b, indent);
			}
			if (right != null) {
				b.Append(' ', indent);
				b.Append("R: ");
				right.AppendTreeToString(b, indent);
			}
		}
		
		public override string ToString()
		{
			if (contents != null) {
				char[] charContents = contents as char[];
				if (charContents != null)
					return "[Leaf length=" + length + ", isShared=" + isShared + ", text=\"" + new string(charContents, 0, length) + "\"]";
				else
					return "[Leaf length=" + length + ", isShared=" + isShared + "\"]";
			} else {
				return "[Concat length=" + length + ", isShared=" + isShared + ", height=" + height + ", Balance=" + this.Balance + "]";
			}
		}
		
		internal string GetTreeAsString()
		{
			StringBuilder b = new StringBuilder();
			AppendTreeToString(b, 0);
			return b.ToString();
		}
		#endif
		#endregion
	}
}
