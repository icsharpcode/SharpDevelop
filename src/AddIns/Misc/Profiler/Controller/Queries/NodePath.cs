// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.Profiler.Controller.Queries
{
	/// <summary>
	/// Describes an absolute path to an CallTreeNode.
	/// </summary>
	public class NodePath : IEquatable<NodePath>, IEnumerable<int>
	{
		/// <summary>
		/// Describes an empty NodePath.
		/// </summary>
		public static readonly NodePath Empty = new NodePath(0, null);
		
		int lastId;
		NodePath previous;
		
		/// <summary>
		/// Gets the top-most/last NameId in the path.
		/// </summary>
		public int LastId {
			get { return lastId; }
		}
		
		/// <summary>
		/// Gets a reference to the previous segment of the path.
		/// </summary>
		public NodePath Previous {
			get { return previous; }
		}
		
		NodePath(int id, NodePath previous)
		{
			this.lastId = id;
			this.previous = previous;
		}
		
		/// <summary>
		/// Creates a new NodePath from this with a new Name Id segment attached.
		/// </summary>
		public NodePath Append(int newVal)
		{
			return new NodePath(newVal, this);
		}
		
		/// <summary>
		/// Returns whether the other NodePath is equal to this NodePath.
		/// </summary>
		public bool Equals(NodePath other)
		{
			if (other == null)
				return false;
			
			return other.lastId == this.lastId && object.Equals(other.previous, this.previous);
		}
		
		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			return Equals(obj as NodePath);
		}
		
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			// (a[0] * p + a[1]) * p + a[2]
			const int hashPrime = 1000000007;
			
			unchecked {
				return ((previous != null) ? previous.GetHashCode() : 0) * hashPrime + lastId;
			}
		}
		
		/// <inheritdoc/>
		public IEnumerator<int> GetEnumerator()
		{
			var list = new List<int>();
			var me = this;
			while (me != null) {
				list.Add(me.lastId);
				me = me.previous;
			}
			list.Reverse();
			return list.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public override string ToString()
		{
			return ((previous != null) ? previous.ToString() + "->" : "") + lastId;
		}
	}
}
