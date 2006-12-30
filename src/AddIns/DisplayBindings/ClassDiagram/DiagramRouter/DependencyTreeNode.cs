/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 03/11/2006
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
