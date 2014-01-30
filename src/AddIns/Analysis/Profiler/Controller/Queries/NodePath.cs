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
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.Profiler.Controller.Queries
{
	/// <summary>
	/// Describes an absolute path to an CallTreeNode.
	/// </summary>
	public sealed class NodePath : IEquatable<NodePath>, IEnumerable<int>
	{
		/// <summary>
		/// Describes an empty NodePath.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
		                                                 Justification = "NodePath is immutable")]
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
		public NodePath Append(int newValue)
		{
			return new NodePath(newValue, this);
		}
		
		/// <summary>
		/// Returns whether the other NodePath is equal to this NodePath.
		/// </summary>
		public bool Equals(NodePath other)
		{
			if (other == null)
				return false;
			
			return other.lastId == lastId && object.Equals(other.previous, previous);
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
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return ((previous != null) ? previous.ToString() + "->" : "") + lastId;
		}
	}
}
