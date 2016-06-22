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

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Anything that has recursive children. Used by <see cref="TreeFlattener">.
	/// </summary>
	public interface ITreeNode<T>
	{
		IEnumerable<T> Children { get; }
	}
	
	/// <summary>
	/// Transforms a tree structure into a flat list.
	/// </summary>
	public class TreeFlattener
	{
		/// <summary>
		/// Flattens a tree by preorder walk.
		/// Does not make copies of the nodes, just returns List of refences to nodes in original tree.
		/// </summary>
		public static List<T> Flatten<T>(T root) where T : ITreeNode<T>
		{
			return Flatten(root, (node) => { return true; });
		}
		
		/// <summary>
		/// Flattens a tree by preorder walk.
		/// Does not make copies of the nodes, just returns List of refences to nodes in original tree.
		/// </summary>
		/// <param name="root">Tree root.</param>
		/// <param name="selectNode">Selector telling which nodes be processed.</param>
		public static List<T> Flatten<T>(T root, Func<T, bool> selectNode) where T : ITreeNode<T>
		{
			if (root == null) throw new ArgumentNullException("root");
			List<T> flattened = new List<T>();
			flattenRecursive(root, selectNode, flattened);
			return flattened;
		}
		
		/// <summary>
		/// Flattens a tree by preorder walk.
		/// Does not make copies of the nodes, just returns List of refences to nodes in original tree.
		/// </summary>
		/// <param name="root">Tree root.</param>
		/// <param name="selectChildren">Selector telling which nodes should have children processed.</param>
		public static List<T> FlattenSelectChildrenIf<T>(T root, Func<T, bool> selectChildren) where T : ITreeNode<T>
		{
			if (root == null) throw new ArgumentNullException("root");
			if (selectChildren == null) throw new ArgumentNullException("selectChildren");
			List<T> flattened = new List<T>();
			flattenSelectChildrenRecursive(root, selectChildren, flattened);
			return flattened;
		}
		
		public static List<T> FlattenSelectChildrenIf<T>(IEnumerable<T> roots, Func<T, bool> selectChildren) where T : ITreeNode<T>
		{
			if (roots == null) throw new ArgumentNullException("root");
			if (selectChildren == null) throw new ArgumentNullException("selectChildren");
			List<T> flattened = new List<T>();
			foreach (T root in roots) {
				flattenSelectChildrenRecursive(root, selectChildren, flattened);
			}
			return flattened;
		}
		
		private static void flattenRecursive<T>(T root, Func<T, bool> selectNode, IList<T> flattened) where T : ITreeNode<T>
		{
			if (selectNode(root)) {
				flattened.Add(root);
				foreach (T child in root.Children) {
					flattenRecursive(child, selectNode, flattened);
				}
			}
		}
		
		private static void flattenSelectChildrenRecursive<T>(T root, Func<T, bool> selectChildren, IList<T> flattened) where T : ITreeNode<T>
		{
			flattened.Add(root);
			if (selectChildren(root)) {
				foreach (T child in root.Children) {
					flattenSelectChildrenRecursive(child, selectChildren, flattened);
				}
			}
		}
	}
}
