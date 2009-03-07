// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// A tree of TextAnchorNodes.
	/// </summary>
	sealed class TextAnchorTree
	{
		readonly TextDocument document;
		readonly List<TextAnchorNode> nodesToDelete = new List<TextAnchorNode>();
		TextAnchorNode root;
		
		public TextAnchorTree(TextDocument document)
		{
			this.document = document;
		}
		
		[Conditional("DEBUG")]
		static void Log(string text)
		{
			Debug.WriteLine("TextAnchorTree: " + text);
		}
		
		#region Insert Text
		public void InsertText(int offset, int length)
		{
			//Log("InsertText(" + offset + ", " + length + ")");
			if (length == 0 || root == null || offset > root.totalLength)
				return;
			
			// find the range of nodes that are placed exactly at offset
			// beginNode is inclusive, endNode is exclusive
			if (offset == root.totalLength) {
				PerformInsertText(root.RightMost, null, length);
			} else {
				TextAnchorNode endNode = FindNode(ref offset);
				Debug.Assert(endNode.length > 0);
				
				if (offset > 0) {
					// there are no nodes exactly at offset
					endNode.length += length;
					UpdateAugmentedData(endNode);
				} else {
					PerformInsertText(endNode.Predecessor, endNode, length);
				}
			}
			DeleteMarkedNodes();
		}
		
		void PerformInsertText(TextAnchorNode beginNode, TextAnchorNode endNode, int length)
		{
			// now find the actual beginNode
			while (beginNode != null && beginNode.length == 0)
				beginNode = beginNode.Predecessor;
			if (beginNode == null) {
				// no predecessor = beginNode is first node in tree
				beginNode = root.LeftMost;
			}
			// now we need to sort the nodes in the range [beginNode, endNode); putting those with
			// MovementType.BeforeInsertion in front of those with MovementType.AfterInsertion
			List<TextAnchorNode> beforeInsert = new List<TextAnchorNode>();
			//List<TextAnchorNode> afterInsert = new List<TextAnchorNode>();
			TextAnchorNode temp = beginNode;
			while (temp != endNode) {
				TextAnchor anchor = (TextAnchor)temp.Target;
				if (anchor == null) {
					// afterInsert.Add(temp);
					MarkNodeForDelete(temp);
				} else if (anchor.MovementType == AnchorMovementType.AfterInsertion) {
					// afterInsert.Add(temp);
				} else {
					beforeInsert.Add(temp);
				}
				temp = temp.Successor;
			}
			// now again go through the range and swap the nodes with those in the beforeInsert list
			temp = beginNode;
			foreach (TextAnchorNode node in beforeInsert) {
				SwapAnchors(node, temp);
				temp = temp.Successor;
			}
			// now temp is pointing to the first node that is afterInsert,
			// or to endNode, if there is no afterInsert node at the offset
			// So add the length to temp
			if (temp == null) {
				// temp might be null if endNode==null and no afterInserts
				Debug.Assert(endNode == null);
			} else {
				temp.length += length;
				UpdateAugmentedData(temp);
			}
		}
		
		/// <summary>
		/// Swaps the anchors stored in the two nodes.
		/// </summary>
		void SwapAnchors(TextAnchorNode n1, TextAnchorNode n2)
		{
			if (n1 != n2) {
				TextAnchor anchor1 = (TextAnchor)n1.Target;
				TextAnchor anchor2 = (TextAnchor)n2.Target;
				if (anchor1 == null && anchor2 == null) {
					// -> no swap required
					return;
				}
				n1.Target = anchor2;
				n2.Target = anchor1;
				if (anchor1 == null) {
					// unmark n1 from deletion, mark n2 for deletion
					nodesToDelete.Remove(n1);
					MarkNodeForDelete(n2);
					anchor2.node = n1;
				} else if (anchor2 == null) {
					// unmark n2 from deletion, mark n1 for deletion
					nodesToDelete.Remove(n2);
					MarkNodeForDelete(n1);
					anchor1.node = n2;
				} else {
					anchor1.node = n2;
					anchor2.node = n1;
				}
			}
		}
		#endregion
		
		#region Remove Text
		public void RemoveText(int offset, int length, DelayedEvents delayedEvents)
		{
			//Log("RemoveText(" + offset + ", " + length + ")");
			if (length == 0 || root == null || offset >= root.totalLength)
				return;
			TextAnchorNode node = FindNode(ref offset);
			while (node != null && offset + length > node.length) {
				TextAnchor anchor = (TextAnchor)node.Target;
				if (anchor != null && anchor.SurviveDeletion) {
					// shorten node
					length -= node.length - offset;
					node.length = offset;
					offset = 0;
					UpdateAugmentedData(node);
					node = node.Successor;
				} else {
					// delete node
					TextAnchorNode s = node.Successor;
					length -= node.length;
					RemoveNode(node);
					// we already deleted the node, don't delete it twice
					nodesToDelete.Remove(node);
					if (anchor != null)
						anchor.OnDeleted(delayedEvents);
					node = s;
				}
			}
			if (node != null) {
				node.length -= length;
				UpdateAugmentedData(node);
			}
			DeleteMarkedNodes();
		}
		#endregion
		
		#region Node removal when TextAnchor was GC'ed
		void MarkNodeForDelete(TextAnchorNode node)
		{
			if (!nodesToDelete.Contains(node))
				nodesToDelete.Add(node);
		}
		
		void DeleteMarkedNodes()
		{
			CheckProperties();
			while (nodesToDelete.Count > 0) {
				int pos = nodesToDelete.Count - 1;
				TextAnchorNode n = nodesToDelete[pos];
				// combine section of n with the following section
				TextAnchorNode s = n.Successor;
				if (s != null) {
					s.length += n.length;
				}
				RemoveNode(n);
				if (s != null) {
					UpdateAugmentedData(s);
				}
				nodesToDelete.RemoveAt(pos);
				CheckProperties();
			}
			CheckProperties();
		}
		#endregion
		
		#region FindNode
		/// <summary>
		/// Finds the node at the specified offset.
		/// After the method has run, offset is relative to the beginning of the returned node.
		/// </summary>
		TextAnchorNode FindNode(ref int offset)
		{
			TextAnchorNode n = root;
			while (true) {
				if (n.left != null) {
					if (offset < n.left.totalLength) {
						n = n.left; // descend into left subtree
						continue;
					} else {
						offset -= n.left.totalLength; // skip left subtree
					}
				}
				if (!n.IsAlive)
					MarkNodeForDelete(n);
				if (offset < n.length) {
					return n; // found correct node
				} else {
					offset -= n.length; // skip this node
				}
				if (n.right != null) {
					n = n.right; // descend into right subtree
				} else {
					// didn't find any node containing the offset
					return null;
				}
			}
		}
		#endregion
		
		#region UpdateAugmentedData
		void UpdateAugmentedData(TextAnchorNode n)
		{
			if (!n.IsAlive)
				MarkNodeForDelete(n);
			
			int totalLength = n.length;
			if (n.left != null)
				totalLength += n.left.totalLength;
			if (n.right != null)
				totalLength += n.right.totalLength;
			if (n.totalLength != totalLength) {
				n.totalLength = totalLength;
				if (n.parent != null)
					UpdateAugmentedData(n.parent);
			}
		}
		#endregion
		
		#region CreateAnchor
		public TextAnchor CreateAnchor(int offset)
		{
			Log("CreateAnchor(" + offset + ")");
			TextAnchor anchor = new TextAnchor(document);
			anchor.node = new TextAnchorNode(anchor);
			if (root == null) {
				// creating the first text anchor
				root = anchor.node;
				root.totalLength = root.length = offset;
			} else if (offset >= root.totalLength) {
				// append anchor at end of tree
				anchor.node.totalLength = anchor.node.length = offset - root.totalLength;
				InsertAsRight(root.RightMost, anchor.node);
			} else {
				// insert anchor in middle of tree
				TextAnchorNode n = FindNode(ref offset);
				Debug.Assert(offset < n.length);
				// split segment 'n' at offset
				anchor.node.totalLength = anchor.node.length = offset;
				n.length -= offset;
				InsertBefore(n, anchor.node);
			}
			DeleteMarkedNodes();
			return anchor;
		}
		
		void InsertBefore(TextAnchorNode node, TextAnchorNode newNode)
		{
			if (node.left == null) {
				InsertAsLeft(node, newNode);
			} else {
				InsertAsRight(node.left.RightMost, newNode);
			}
		}
		#endregion
		
		#region Red/Black Tree
		internal const bool RED = true;
		internal const bool BLACK = false;
		
		void InsertAsLeft(TextAnchorNode parentNode, TextAnchorNode newNode)
		{
			Debug.Assert(parentNode.left == null);
			parentNode.left = newNode;
			newNode.parent = parentNode;
			newNode.color = RED;
			UpdateAugmentedData(parentNode);
			FixTreeOnInsert(newNode);
		}
		
		void InsertAsRight(TextAnchorNode parentNode, TextAnchorNode newNode)
		{
			Debug.Assert(parentNode.right == null);
			parentNode.right = newNode;
			newNode.parent = parentNode;
			newNode.color = RED;
			UpdateAugmentedData(parentNode);
			FixTreeOnInsert(newNode);
		}
		
		void FixTreeOnInsert(TextAnchorNode node)
		{
			Debug.Assert(node != null);
			Debug.Assert(node.color == RED);
			Debug.Assert(node.left == null || node.left.color == BLACK);
			Debug.Assert(node.right == null || node.right.color == BLACK);
			
			TextAnchorNode parentNode = node.parent;
			if (parentNode == null) {
				// we inserted in the root -> the node must be black
				// since this is a root node, making the node black increments the number of black nodes
				// on all paths by one, so it is still the same for all paths.
				node.color = BLACK;
				return;
			}
			if (parentNode.color == BLACK) {
				// if the parent node where we inserted was black, our red node is placed correctly.
				// since we inserted a red node, the number of black nodes on each path is unchanged
				// -> the tree is still balanced
				return;
			}
			// parentNode is red, so there is a conflict here!
			
			// because the root is black, parentNode is not the root -> there is a grandparent node
			TextAnchorNode grandparentNode = parentNode.parent;
			TextAnchorNode uncleNode = Sibling(parentNode);
			if (uncleNode != null && uncleNode.color == RED) {
				parentNode.color = BLACK;
				uncleNode.color = BLACK;
				grandparentNode.color = RED;
				FixTreeOnInsert(grandparentNode);
				return;
			}
			// now we know: parent is red but uncle is black
			// First rotation:
			if (node == parentNode.right && parentNode == grandparentNode.left) {
				RotateLeft(parentNode);
				node = node.left;
			} else if (node == parentNode.left && parentNode == grandparentNode.right) {
				RotateRight(parentNode);
				node = node.right;
			}
			// because node might have changed, reassign variables:
			parentNode = node.parent;
			grandparentNode = parentNode.parent;
			
			// Now recolor a bit:
			parentNode.color = BLACK;
			grandparentNode.color = RED;
			// Second rotation:
			if (node == parentNode.left && parentNode == grandparentNode.left) {
				RotateRight(grandparentNode);
			} else {
				// because of the first rotation, this is guaranteed:
				Debug.Assert(node == parentNode.right && parentNode == grandparentNode.right);
				RotateLeft(grandparentNode);
			}
		}
		
		void RemoveNode(TextAnchorNode removedNode)
		{
			if (removedNode.left != null && removedNode.right != null) {
				// replace removedNode with it's in-order successor
				
				TextAnchorNode leftMost = removedNode.right.LeftMost;
				RemoveNode(leftMost); // remove leftMost from its current location
				
				// and overwrite the removedNode with it
				ReplaceNode(removedNode, leftMost);
				leftMost.left = removedNode.left;
				if (leftMost.left != null) leftMost.left.parent = leftMost;
				leftMost.right = removedNode.right;
				if (leftMost.right != null) leftMost.right.parent = leftMost;
				leftMost.color = removedNode.color;
				
				UpdateAugmentedData(leftMost);
				if (leftMost.parent != null) UpdateAugmentedData(leftMost.parent);
				return;
			}
			
			// now either removedNode.left or removedNode.right is null
			// get the remaining child
			TextAnchorNode parentNode = removedNode.parent;
			TextAnchorNode childNode = removedNode.left ?? removedNode.right;
			ReplaceNode(removedNode, childNode);
			if (parentNode != null) UpdateAugmentedData(parentNode);
			if (removedNode.color == BLACK) {
				if (childNode != null && childNode.color == RED) {
					childNode.color = BLACK;
				} else {
					FixTreeOnDelete(childNode, parentNode);
				}
			}
		}
		
		void FixTreeOnDelete(TextAnchorNode node, TextAnchorNode parentNode)
		{
			Debug.Assert(node == null || node.parent == parentNode);
			if (parentNode == null)
				return;
			
			// warning: node may be null
			TextAnchorNode sibling = Sibling(node, parentNode);
			if (sibling.color == RED) {
				parentNode.color = RED;
				sibling.color = BLACK;
				if (node == parentNode.left) {
					RotateLeft(parentNode);
				} else {
					RotateRight(parentNode);
				}
				
				sibling = Sibling(node, parentNode); // update value of sibling after rotation
			}
			
			if (parentNode.color == BLACK
			    && sibling.color == BLACK
			    && GetColor(sibling.left) == BLACK
			    && GetColor(sibling.right) == BLACK)
			{
				sibling.color = RED;
				FixTreeOnDelete(parentNode, parentNode.parent);
				return;
			}
			
			if (parentNode.color == RED
			    && sibling.color == BLACK
			    && GetColor(sibling.left) == BLACK
			    && GetColor(sibling.right) == BLACK)
			{
				sibling.color = RED;
				parentNode.color = BLACK;
				return;
			}
			
			if (node == parentNode.left &&
			    sibling.color == BLACK &&
			    GetColor(sibling.left) == RED &&
			    GetColor(sibling.right) == BLACK)
			{
				sibling.color = RED;
				sibling.left.color = BLACK;
				RotateRight(sibling);
			}
			else if (node == parentNode.right &&
			         sibling.color == BLACK &&
			         GetColor(sibling.right) == RED &&
			         GetColor(sibling.left) == BLACK)
			{
				sibling.color = RED;
				sibling.right.color = BLACK;
				RotateLeft(sibling);
			}
			sibling = Sibling(node, parentNode); // update value of sibling after rotation
			
			sibling.color = parentNode.color;
			parentNode.color = BLACK;
			if (node == parentNode.left) {
				if (sibling.right != null) {
					Debug.Assert(sibling.right.color == RED);
					sibling.right.color = BLACK;
				}
				RotateLeft(parentNode);
			} else {
				if (sibling.left != null) {
					Debug.Assert(sibling.left.color == RED);
					sibling.left.color = BLACK;
				}
				RotateRight(parentNode);
			}
		}
		
		void ReplaceNode(TextAnchorNode replacedNode, TextAnchorNode newNode)
		{
			if (replacedNode.parent == null) {
				Debug.Assert(replacedNode == root);
				root = newNode;
			} else {
				if (replacedNode.parent.left == replacedNode)
					replacedNode.parent.left = newNode;
				else
					replacedNode.parent.right = newNode;
			}
			if (newNode != null) {
				newNode.parent = replacedNode.parent;
			}
			replacedNode.parent = null;
		}
		
		void RotateLeft(TextAnchorNode p)
		{
			// let q be p's right child
			TextAnchorNode q = p.right;
			Debug.Assert(q != null);
			Debug.Assert(q.parent == p);
			// set q to be the new root
			ReplaceNode(p, q);
			
			// set p's right child to be q's left child
			p.right = q.left;
			if (p.right != null) p.right.parent = p;
			// set q's left child to be p
			q.left = p;
			p.parent = q;
			UpdateAugmentedData(p);
			UpdateAugmentedData(q);
		}
		
		void RotateRight(TextAnchorNode p)
		{
			// let q be p's left child
			TextAnchorNode q = p.left;
			Debug.Assert(q != null);
			Debug.Assert(q.parent == p);
			// set q to be the new root
			ReplaceNode(p, q);
			
			// set p's left child to be q's right child
			p.left = q.right;
			if (p.left != null) p.left.parent = p;
			// set q's right child to be p
			q.right = p;
			p.parent = q;
			UpdateAugmentedData(p);
			UpdateAugmentedData(q);
		}
		
		static TextAnchorNode Sibling(TextAnchorNode node)
		{
			if (node == node.parent.left)
				return node.parent.right;
			else
				return node.parent.left;
		}
		
		static TextAnchorNode Sibling(TextAnchorNode node, TextAnchorNode parentNode)
		{
			Debug.Assert(node == null || node.parent == parentNode);
			if (node == parentNode.left)
				return parentNode.right;
			else
				return parentNode.left;
		}
		
		static bool GetColor(TextAnchorNode node)
		{
			return node != null ? node.color : BLACK;
		}
		#endregion
		
		#region CheckProperties
		[Conditional("DATACONSISTENCYTEST")]
		internal void CheckProperties()
		{
			#if DEBUG
			if (root != null) {
				CheckProperties(root);
				
				// check red-black property:
				int blackCount = -1;
				CheckNodeProperties(root, null, RED, 0, ref blackCount);
			}
			#endif
		}
		
		#if DEBUG
		void CheckProperties(TextAnchorNode node)
		{
			int totalLength = node.length;
			if (node.left != null) {
				CheckProperties(node.left);
				totalLength += node.left.totalLength;
			}
			if (node.right != null) {
				CheckProperties(node.right);
				totalLength += node.right.totalLength;
			}
			Debug.Assert(node.totalLength == totalLength);
		}
		
		/*
		1. A node is either red or black.
		2. The root is black.
		3. All leaves are black. (The leaves are the NIL children.)
		4. Both children of every red node are black. (So every red node must have a black parent.)
		5. Every simple path from a node to a descendant leaf contains the same number of black nodes. (Not counting the leaf node.)
		 */
		void CheckNodeProperties(TextAnchorNode node, TextAnchorNode parentNode, bool parentColor, int blackCount, ref int expectedBlackCount)
		{
			if (node == null) return;
			
			Debug.Assert(node.parent == parentNode);
			
			if (parentColor == RED) {
				Debug.Assert(node.color == BLACK);
			}
			if (node.color == BLACK) {
				blackCount++;
			}
			if (node.left == null && node.right == null) {
				// node is a leaf node:
				if (expectedBlackCount == -1)
					expectedBlackCount = blackCount;
				else
					Debug.Assert(expectedBlackCount == blackCount);
			}
			CheckNodeProperties(node.left, node, node.color, blackCount, ref expectedBlackCount);
			CheckNodeProperties(node.right, node, node.color, blackCount, ref expectedBlackCount);
		}
		#endif
		#endregion
		
		#region GetTreeAsString
		#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public string GetTreeAsString()
		{
			if (root == null)
				return "<empty tree>";
			StringBuilder b = new StringBuilder();
			AppendTreeToString(root, b, 0);
			return b.ToString();
		}
		
		static void AppendTreeToString(TextAnchorNode node, StringBuilder b, int indent)
		{
			if (node.color == RED)
				b.Append("RED   ");
			else
				b.Append("BLACK ");
			b.AppendLine(node.ToString());
			indent += 2;
			if (node.left != null) {
				b.Append(' ', indent);
				b.Append("L: ");
				AppendTreeToString(node.left, b, indent);
			}
			if (node.right != null) {
				b.Append(' ', indent);
				b.Append("R: ");
				AppendTreeToString(node.right, b, indent);
			}
		}
		#endif
		#endregion
	}
}
