// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A <see cref="IModelCollection{T}"/> that works by concatening multiple
	/// other model collections.
	/// </summary>
	public sealed class ConcatModelCollection<T> : IModelCollection<T>
	{
		sealed class InputCollection : Collection<IModelCollection<T>>
		{
			readonly ConcatModelCollection<T> owner;
			
			public InputCollection(ConcatModelCollection<T> owner)
			{
				this.owner = owner;
			}
			
			protected override void ClearItems()
			{
				if (owner.collectionChanged != null) {
					foreach (var input in Items) {
						input.CollectionChanged -= owner.OnInputCollectionChanged;
					}
				}
				base.ClearItems();
				owner.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			
			protected override void InsertItem(int index, IModelCollection<T> item)
			{
				if (owner.collectionChanged != null)
					item.CollectionChanged += owner.OnInputCollectionChanged;
				base.InsertItem(index, item);
				owner.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, item.ToArray(), owner.GetCount(index)));
			}
			
			protected override void RemoveItem(int index)
			{
				if (owner.collectionChanged != null)
					Items[index].CollectionChanged -= owner.OnInputCollectionChanged;
				var oldItems = Items[index].ToArray();
				base.RemoveItem(index);
				owner.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Remove, oldItems, owner.GetCount(index)));
			}
			
			protected override void SetItem(int index, IModelCollection<T> item)
			{
				RemoveItem(index);
				InsertItem(index, item);
			}
		}
		
		InputCollection inputs;
		
		public ConcatModelCollection()
		{
			this.inputs = new InputCollection(this);
		}
		
		public IList<IModelCollection<T>> Inputs {
			get { return inputs; }
		}
		
		NotifyCollectionChangedEventHandler collectionChanged;
		
		public event NotifyCollectionChangedEventHandler CollectionChanged {
			add {
				var oldEventHandlers = collectionChanged;
				collectionChanged += value;
				if (oldEventHandlers == null && collectionChanged != null) {
					foreach (var input in inputs)
						input.CollectionChanged += OnInputCollectionChanged;
				}
			}
			remove {
				var oldEventHandlers = collectionChanged;
				collectionChanged -= value;
				if (oldEventHandlers != null && collectionChanged == null) {
					foreach (var input in inputs)
						input.CollectionChanged -= OnInputCollectionChanged;
				}
			}
		}
		
		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (collectionChanged != null)
				collectionChanged(this, e);
		}
		
		void OnInputCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			int inputIndex = inputs.IndexOf((IModelCollection<T>)sender);
			int startIndex = GetCount(inputIndex);
			NotifyCollectionChangedEventArgs newEventArgs;
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					newEventArgs = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, startIndex + e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Remove:
					newEventArgs = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, startIndex + e.OldStartingIndex);
					break;
				case NotifyCollectionChangedAction.Replace:
					newEventArgs = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, e.NewItems, startIndex + e.OldStartingIndex);
					break;
				case NotifyCollectionChangedAction.Move:
					newEventArgs = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, startIndex + e.OldStartingIndex, startIndex + e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Reset:
					newEventArgs = new NotifyCollectionChangedEventArgs(e.Action);
					break;
				default:
					throw new NotSupportedException("Invalid value for NotifyCollectionChangedAction");
			}
			collectionChanged(this, newEventArgs);
		}
		
		public int Count {
			get { return GetCount(inputs.Count); }
		}
		
		int GetCount(int inputIndex)
		{
			int count = 0;
			for (int i = 0; i < inputIndex; i++) {
				count += inputs[i].Count;
			}
			return count;
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return inputs.SelectMany(i => i).GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public T this[int index] {
			get {
				int inputIndex = 0;
				while (index >= inputs[inputIndex].Count) {
					index -= inputs[inputIndex].Count;
					inputIndex++;
				}
				return inputs[inputIndex][index];
			}
		}
	}
}
