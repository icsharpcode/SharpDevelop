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
using System.Collections.Generic;

namespace Tools.Diagrams
{
	public class DependencyTreeNode<T>
	{
		private T item;
		private DependencyTreeNode<T> parentNode;
		private List<DependencyTreeNode<T>> dependants = new List<DependencyTreeNode<T>>();
		
		private DependencyTreeNode(T item, DependencyTreeNode<T> parent)
		{
			this.item = item;
			parentNode = parent;
		}
		
		public DependencyTreeNode(T item)
		{
			this.item = item;
		}
		
		public T Item
		{
			get { return item; }
		}
		
		public DependencyTreeNode<T> ParentNode
		{
			get { return parentNode; }
		}
		
		public DependencyTreeNode<T> AddDependency (T item)
		{
			DependencyTreeNode<T> node = new DependencyTreeNode<T>(item, this);
			dependants.Add (node);
			return node;
		}
		
		public void Reparent (DependencyTreeNode<T> parent)
		{
			parent.dependants.Add(this);
			this.parentNode.dependants.Remove(this);
			this.parentNode = parent;
		}
		
		public bool IsLeaf
		{
			get { return dependants.Count == 0; }
		}
		
		public int ChildrenCount
		{
			get { return dependants.Count; }
		}
		
		public int LeafsCount
		{
			get
			{
				int count = 0;
				foreach (DependencyTreeNode<T> node in dependants)
					if (node.IsLeaf)
					count++;
				return count;
			}
		}
		
		public List <DependencyTreeNode<T>> Dependants
		{
			get { return dependants; }
		}
	}
}
