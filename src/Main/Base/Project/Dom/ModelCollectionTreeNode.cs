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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom
{
	public abstract class ModelCollectionTreeNode : SharpTreeNode
	{
		protected static readonly IComparer<SharpTreeNode> NodeTextComparer = KeyComparer.Create((SharpTreeNode n) => n.Text.ToString(), StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
		protected bool listeningToCollectionChangedEvents;
		
		protected ModelCollectionTreeNode()
		{
			this.LazyLoading = true;
		}
		
		protected abstract IModelCollection<object> ModelChildren { get; }
		protected abstract IComparer<SharpTreeNode> NodeComparer { get; }
		
		protected virtual void InsertSpecialNodes()
		{
		}
		
		protected override void OnIsVisibleChanged()
		{
			base.OnIsVisibleChanged();
			
			if (IsVisible) {
				if (!LazyLoading) {
					if (!listeningToCollectionChangedEvents) {
						ModelChildren.CollectionChanged += ModelChildrenCollectionChanged;
						listeningToCollectionChangedEvents = true;
					}
					SynchronizeModelChildren();
				}
			} else {
				ModelChildren.CollectionChanged -= ModelChildrenCollectionChanged;
				listeningToCollectionChangedEvents = false;
			}
		}
		
		#region Manage Children
		protected override void LoadChildren()
		{
			Children.Clear();
			InsertSpecialNodes();
			InsertChildren(ModelChildren);
			if (!listeningToCollectionChangedEvents) {
				ModelChildren.CollectionChanged += ModelChildrenCollectionChanged;
				listeningToCollectionChangedEvents = true;
			}
		}
		
		protected void InsertChildren(IEnumerable children)
		{
			foreach (object child in children) {
				var treeNode = SD.TreeNodeFactory.CreateTreeNode(child);
				if (treeNode != null)
					Children.OrderedInsert(treeNode, NodeComparer);
			}
		}
		
		protected void SynchronizeModelChildren()
		{
			HashSet<object> set = new HashSet<object>(ModelChildren);
			Children.RemoveAll(n => !set.Contains(n.Model));
			set.ExceptWith(Children.Select(n => n.Model));
			InsertChildren(set);
		}
		
		void ModelChildrenCollectionChanged(IReadOnlyCollection<object> removedItems, IReadOnlyCollection<object> addedItems)
		{
			if (!IsVisible) {
				SwitchBackToLazyLoading();
				return;
			}
			Children.RemoveAll(n => removedItems.Contains(n.Model));
			InsertChildren(addedItems);
		}
		
		protected void SwitchBackToLazyLoading()
		{
			if (listeningToCollectionChangedEvents) {
				ModelChildren.CollectionChanged -= ModelChildrenCollectionChanged;
				listeningToCollectionChangedEvents = false;
			}
			Children.Clear();
			LazyLoading = true;
		}
		#endregion
		
		// TODO: remove this method from ModelCollectionTreeNode; it's unrelated to the core functionality of managing the IModelCollection
		public virtual SharpTreeNode FindChildNodeRecursively(Func<SharpTreeNode, bool> predicate)
		{
			if (predicate == null)
				return null;
			
			SharpTreeNode foundNode = null;
			foreach (var child in Children) {
				if (predicate(child)) {
					// This is our node!
					foundNode = child;
					break;
				}
				
				// Search in all children of this node
				var modelNode = child as ModelCollectionTreeNode;
				if (modelNode != null && modelNode.CanFindChildNodeRecursively) {
					child.EnsureLazyChildren();
					foundNode = modelNode.FindChildNodeRecursively(predicate);
					if (foundNode != null)
						break;
				}
			}
			
			return foundNode;
		}
		
		public virtual bool CanFindChildNodeRecursively {
			get { return true; }
		}
	}
}
