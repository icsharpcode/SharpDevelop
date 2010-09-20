// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Walks tree structure to produce flat list.
	/// </summary>
	public class TreeFlattener
	{
		/// <summary>
		/// Flattens tree by preorder walk.
		/// Does not make copies of the nodes, just returns List of refences to nodes in original tree.
		/// </summary>
		public static List<T> Flatten<T>(T root) where T : ITreeNode<T>
		{
			return Flatten(root, (node) => { return true; });
		}
		
		/// <summary>
		/// Flattens tree by preorder walk.
		/// Does not make copies of the nodes, just returns List of refences to nodes in original tree.
		/// </summary>
		/// <param name="root">Tree root.</param>
		/// <param name="selectNode">Selector telling which nodes be processed.</param>
		public static List<T> Flatten<T>(T root, Func<T, bool> selectNode) where T : ITreeNode<T>
		{
			if (root == null)
				throw new ArgumentNullException("root");
			
			List<T> flattened = new List<T>();
			flattenRecursive(root, selectNode, flattened);
			return flattened;
		}
		
		/// <summary>
		/// Flattens tree by preorder walk.
		/// Does not make copies of the nodes, just returns List of refences to nodes in original tree.
		/// </summary>
		/// <param name="root">Tree root.</param>
		/// <param name="selectChildren">Selector telling which nodes should have children processed.</param>
		public static List<T> FlattenSelectChildrenIf<T>(T root, Func<T, bool> selectChildren) where T : ITreeNode<T>
		{
			if (root == null)
				throw new ArgumentNullException("root");
			if (selectChildren == null)
				throw new ArgumentNullException("selectChildren");
			
			List<T> flattened = new List<T>();
			flattenSelectChildrenRecursive(root, selectChildren, flattened);
			return flattened;
		}
		
		public static List<T> FlattenSelectChildrenIf<T>(IEnumerable<T> roots, Func<T, bool> selectChildren) where T : ITreeNode<T>
		{
			if (roots == null)
				throw new ArgumentNullException("root");
			if (selectChildren == null)
				throw new ArgumentNullException("selectChildren");
			
			List<T> flattened = new List<T>();
			foreach (T root in roots)
			{
				flattenSelectChildrenRecursive(root, selectChildren, flattened);
			}
			return flattened;
		}
		
		private static void flattenRecursive<T>(T root, Func<T, bool> selectNode, IList<T> flattened) where T : ITreeNode<T>
		{
			if (selectNode(root))
			{
				flattened.Add(root);
				foreach (T child in root.Children)
				{
					flattenRecursive(child, selectNode, flattened);
				}
			}
		}
		
		private static void flattenSelectChildrenRecursive<T>(T root, Func<T, bool> selectChildren, IList<T> flattened) where T : ITreeNode<T>
		{
			flattened.Add(root);
			if (selectChildren(root))
			{
				foreach (T child in root.Children)
				{
					flattenSelectChildrenRecursive(child, selectChildren, flattened);
				}
			}
		}
	}
}
