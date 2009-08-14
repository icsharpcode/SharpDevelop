// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.XmlParser
{
	/// <summary>
	/// Abstact base class for all types that can contain child nodes
	/// </summary>
	public abstract class RawContainer: RawObject
	{
		/// <summary>
		/// Children of the node.  It is read-only.
		/// Note that is has CollectionChanged event.
		/// </summary>
		public ChildrenCollection<RawObject> Children { get; private set; }
		
		/// <summary> Create new container </summary>
		public RawContainer()
		{
			this.Children = new ChildrenCollection<RawObject>();
		}
		
		#region Helpper methods
		
		ObservableCollection<RawElement> elements;
		
		/// <summary> Gets direcly nested elements (non-recursive) </summary>
		public ObservableCollection<RawElement> Elements {
			get {
				if (elements == null) {
					elements = new FilteredCollection<RawElement, ChildrenCollection<RawObject>>(this.Children);
				}
				return elements;
			}
		}
		
		internal RawObject FirstChild {
			get {
				return this.Children[0];
			}
		}
		
		internal RawObject LastChild {
			get {
				return this.Children[this.Children.Count - 1];
			}
		}
		
		#endregion
		
		/// <inheritdoc/>
		public override IEnumerable<RawObject> GetSelfAndAllChildren()
		{
			return new RawObject[] { this }.Flatten(i => i is RawContainer ? ((RawContainer)i).Children : null);
		}
		
		/// <summary>
		/// Gets a child fully containg the given offset.
		/// Goes recursively down the tree.
		/// Specail case if at the end of attribute or text
		/// </summary>
		public RawObject GetChildAtOffset(int offset)
		{
			foreach(RawObject child in this.Children) {
				if ((child is RawAttribute || child is RawText) && offset == child.EndOffset) return child;
				if (child.StartOffset < offset && offset < child.EndOffset) {
					if (child is RawContainer) {
						return ((RawContainer)child).GetChildAtOffset(offset);
					} else {
						return child;
					}
				}
			}
			return this; // No childs at offset
		}
		
		// Only these four methods should be used to modify the collection
		
		/// <summary> To be used exlucively by the parser </summary>
		internal void AddChild(RawObject item)
		{
			// Childs can be only added to newly parsed items
			Assert(this.Parent == null, "I have to be new");
			Assert(item.IsInCache, "Added item must be in cache");
			// Do not set parent pointer
			this.Children.InsertItemAt(this.Children.Count, item);
		}
		
		/// <summary> To be used exlucively by the parser </summary>
		internal void AddChildren(IEnumerable<RawObject> items)
		{
			// Childs can be only added to newly parsed items
			Assert(this.Parent == null, "I have to be new");
			// Do not set parent pointer
			this.Children.InsertItemsAt(this.Children.Count, items.ToList());
		}
		
		/// <summary>
		/// To be used exclusively by the children update algorithm.
		/// Insert child and keep links consistent.
		/// </summary>
		void InsertChild(int index, RawObject item)
		{
			LogDom("Inserting {0} at index {1}", item, index);
			
			RawDocument document = this.Document;
			Assert(document != null, "Can not insert to dangling object");
			Assert(item.Parent != this, "Can not own item twice");
			
			SetParentPointersInTree(item);
			
			this.Children.InsertItemAt(index, item);
			
			document.OnObjectInserted(index, item);
		}
		
		/// <summary> Recursively fix all parent pointer in a tree </summary>
		/// <remarks>
		/// Cache constraint:
		///    If cached item has parent set, then the whole subtree must be consistent
		/// </remarks>
		void SetParentPointersInTree(RawObject item)
		{
			// All items come from the parser cache
			
			if (item.Parent == null) {
				// Dangling object - either a new parser object or removed tree (still cached)
				item.Parent = this;
				if (item is RawContainer) {
					foreach(RawObject child in ((RawContainer)item).Children) {
						((RawContainer)item).SetParentPointersInTree(child);
					}
				}
			} else if (item.Parent == this) {
				// If node is attached and then deattached, it will have null parent pointer
				//   but valid subtree - so its children will alredy have correct parent pointer
				//   like in this case
				item.DebugCheckConsistency(false);
				// Rest of the tree is consistent - do not recurse
			} else {
				// From cache & parent set => consitent subtree
				item.DebugCheckConsistency(false);
				// The parent (or any futher parents) can not be part of parsed document
				//   becuase otherwise this item would be included twice => safe to change parents
				DebugAssert(item.Parent.Document == null, "Old parent is part of document as well");
				// Maintain cache constraint by setting parents to null
				foreach(RawObject ancest in item.GetAncestors().ToList()) {
					ancest.Parent = null; 
				}
				item.Parent = this;
				// Rest of the tree is consistent - do not recurse
			}
		}
		
		/// <summary>
		/// To be used exclusively by the children update algorithm.
		/// Remove child, set parent to null and notify the document
		/// </summary>
		void RemoveChild(int index)
		{
			RawObject removed = this.Children[index];
			LogDom("Removing {0} at index {1}", removed, index);
			
			// Null parent pointer
			Assert(removed.Parent == this, "Inconsistent child");
			removed.Parent = null;
			
			this.Children.RemoveItemAt(index);
			
			this.Document.OnObjectRemoved(index, removed);
		}
		
		/// <summary> Verify that the subtree is consistent.  Only in debug build. </summary>
		internal override void DebugCheckConsistency(bool allowNullParent)
		{
			base.DebugCheckConsistency(allowNullParent);
			RawObject prevChild = null;
			int myStartOffset = this.StartOffset;
			int myEndOffset = this.EndOffset;
			foreach(RawObject child in this.Children) {
				Assert(child.Length != 0, "Empty child");
				if (!allowNullParent) {
					Assert(child.Parent != null, "Null parent reference");
				}
				Assert(child.Parent == null || child.Parent == this, "Inccorect parent reference");
				Assert(myStartOffset <= child.StartOffset && child.EndOffset <= myEndOffset, "Child not within parent text range");
				if (this.IsInCache)
					Assert(child.IsInCache, "Child not in cache");
				if (prevChild != null)
					Assert(prevChild.EndOffset <= child.StartOffset, "Overlaping childs");
				child.DebugCheckConsistency(allowNullParent);
				prevChild = child;
			}
		}
		
		internal void UpdateTreeFrom(RawContainer srcContainer)
		{
			RemoveChildrenNotIn(srcContainer.Children);
			InsertAndUpdateChildrenFrom(srcContainer.Children);
		}
		
		void RemoveChildrenNotIn(IList<RawObject> srcList)
		{
			Dictionary<int, RawObject> srcChildren = srcList.ToDictionary(i => i.StartOffset);
			for(int i = 0; i < this.Children.Count;) {
				RawObject child = this.Children[i];
				RawObject srcChild;
				
				if (srcChildren.TryGetValue(child.StartOffset, out srcChild) && child.CanUpdateDataFrom(srcChild)) {
					// Keep only one item with given offset (we might have several due to deletion)
					srcChildren.Remove(child.StartOffset);
					if (child is RawContainer)
						((RawContainer)child).RemoveChildrenNotIn(((RawContainer)srcChild).Children);
					i++;
				} else {
					RemoveChild(i);
				}
			}
		}
		
		void InsertAndUpdateChildrenFrom(IList<RawObject> srcList)
		{
			for(int i = 0; i < srcList.Count; i++) {
				// End of our list?
				if (i == this.Children.Count) {
					InsertChild(i, srcList[i]);
					continue;
				}
				RawObject child = this.Children[i];
				RawObject srcChild = srcList[i];
				
				if (child.CanUpdateDataFrom(srcChild) /* includes offset test */) {
					child.UpdateDataFrom(srcChild);
					if (child is RawContainer)
						((RawContainer)child).InsertAndUpdateChildrenFrom(((RawContainer)srcChild).Children);
				} else {
					InsertChild(i, srcChild);
				}
			}
			Assert(this.Children.Count == srcList.Count, "List lengths differ after update");
		}
	}
}
