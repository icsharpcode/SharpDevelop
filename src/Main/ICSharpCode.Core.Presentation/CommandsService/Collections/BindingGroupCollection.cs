using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

using ICSharpCode.Core;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Observable collection of <see cref="BindingGroup" />
	/// </summary>
	public class BindingGroupCollection : IObservableCollection<BindingGroup>
	{
		private ObservableCollection<BindingGroup> _bindingGroups = new ObservableCollection<BindingGroup>();
		private event NotifyCollectionChangedEventHandler _bindingGroupsCollectionChanged;
		
		/// <inheritdoc />
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add {
				_bindingGroupsCollectionChanged += value;
				_bindingGroups.CollectionChanged += value;
			}
			remove {
				_bindingGroupsCollectionChanged -= value;
				_bindingGroups.CollectionChanged -= value;
			}
		}
		
		/// <summary>
		/// Gets <see cref="BindingGroupCollection" /> containing all nested binding groups
		/// </summary>
		public BindingGroupCollection FlatNesteGroups
		{
			get {
				var flatGroups = new HashSet<BindingGroup>();
				foreach(var bindingGroup in this) {
					bindingGroup.FlattenNestedGroups(bindingGroup, flatGroups);
				}
				
				var flatBindingGroupCollection = new BindingGroupCollection();
				flatBindingGroupCollection.AddRange(flatGroups);
				
				return flatBindingGroupCollection;
			}
		}
		
		/// <summary>
		/// Determines whether <see cref="UIElement" /> instance is handled by any group in this collection
		/// </summary>
		/// <param name="instance">Examined instance</param>
		/// <returns><code>true</code> if registered; otherwise <code>false</code></returns>
		public bool IsInstanceRegistered(UIElement instance)
		{
			foreach(var bindingGroup in FlatNesteGroups) {
				if(bindingGroup.IsInstanceRegistered(instance)) {
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Determines whether any <see cref="UIElement" /> instance from provided collection
		/// is handled by any group in this collection
		/// </summary>
		/// <param name="instance">Examined instance</param>
		/// <returns><code>true</code> if registered; otherwise <code>false</code></returns>
		public bool IsAnyInstanceRegistered(ICollection<UIElement> instances)
		{
			if(instances != null && instances.Count > 0) {
				foreach(var instance in instances) {
					if(IsInstanceRegistered(instance)) {
						return true;
					}
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// From provided <see cref="ICollection{Type}" /> generate <see cref="ICollection{UIElement}" /> containing 
		/// instances created from one of the provided types and registered in any group included in this <see cref="BindingGroupCollection" />
		/// </summary>
		/// <param name="instances">Collection of examined types</param>
		/// <returns>Generated instances</returns>
		public ICollection<UIElement> GetAttachedInstances(ICollection<Type> types)
		{
			var instances = new HashSet<UIElement>();
			foreach(var group in FlatNesteGroups) {
				foreach(var instance in group.FilterAttachedInstances(types)) {
					instances.Add(instance);
				}
			}
			
			return instances;
		}
		
		/// <summary>
		/// From provided <see cref="ICollection{UIElement}" /> filter <see cref="ICollection{UIElement}" /> containing 
		/// only instances registered in any group included in this <see cref="BindingGroupCollection" />
		/// </summary>
		/// <param name="instances">Collection of examined instances</param>
		/// <returns>Filtered instances</returns>
		public ICollection<UIElement> GetAttachedInstances(ICollection<UIElement> instances)
		{
			var attachedInstances = new HashSet<UIElement>();
			foreach(var group in FlatNesteGroups) {
				foreach(var instance in group.GetAttachedInstances(instances)) {
					attachedInstances.Add(instance);
				}
			}
			
			return attachedInstances;
		}
		
		/// <summary>
		/// Number of <see cref="BindingGroup" /> instances in this collection
		/// </summary>
		public int Count {
			get {
				return _bindingGroups.Count;
			}
		}
		
		/// <inheritdoc />
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Add <see cref="BindingGroup" /> to this collection
		/// </summary>
		/// <param name="bindingGroup"></param>
		public void Add(BindingGroup bindingGroup)
		{
			if(bindingGroup == null) {
				throw new ArgumentNullException("bindingGroup");
			}
			
			if(!_bindingGroups.Contains(bindingGroup)) {
				_bindingGroups.Add(bindingGroup);
			}
		}
		
		/// <summary>
		/// Remove all instances of <see cref="BindingGroup" /> from this collection
		/// </summary>
		public void Clear()
		{
			var itemsBackup = _bindingGroups;
			
			_bindingGroups = new ObservableCollection<BindingGroup>();
			foreach(NotifyCollectionChangedEventHandler handler in _bindingGroupsCollectionChanged.GetInvocationList()) {
				_bindingGroups.CollectionChanged += handler;
			}
			
			if(_bindingGroupsCollectionChanged != null) {
				_bindingGroupsCollectionChanged.Invoke(
					this,
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Remove,
						itemsBackup));
			}
		}
		
		/// <summary>
		/// Add many instances of <see cref="BindingGroup" /> to this collection
		/// </summary>
		/// <param name="bindingGroups"></param>
		public void AddRange(IEnumerable<BindingGroup> bindingGroups)
		{
			foreach(var bindingGroup in bindingGroups) {
				Add(bindingGroup);
			}
		}
		
		/// <summary>
		/// Determines whether this <see cref="BindingGroupCollection" /> contains
		/// provided instance of <see cref="BindingGroup" />
		/// </summary>
		/// <param name="bindingGroup">Instance of <see cref="BindingGroup" /></param>
		/// <returns>Returns <code>true</code> if binding group is present in collection; otherwise returns <code>false</code></returns>
		public bool Contains(BindingGroup bindingGroup)
		{
			return _bindingGroups.Contains(bindingGroup);
		}
		
		/// <summary>
		/// Copy this collection to array
		/// </summary>
		/// <param name="array">Array of <see cref="BindingGroup" /> instances</param>
		/// <param name="arrayIndex">Copy start index</param>
		public void CopyTo(BindingGroup[] array, int arrayIndex)
		{
			_bindingGroups.CopyTo(array, arrayIndex);
		}
		
		/// <summary>
		/// Remove instance of <see cref="BindingGroup" />
		/// </summary>
		/// <param name="bindingGroup">Added instance of <see cref="BindingGroup" /></param>
		/// <returns><code>True</code> if item was added; otherwise <code>false</code></returns>
		public bool Remove(BindingGroup bindingGroup)
		{
			return _bindingGroups.Remove(bindingGroup);
		}
		
		/// <inheritdoc />
		public IEnumerator<BindingGroup> GetEnumerator()
		{
			return _bindingGroups.GetEnumerator();
		}
		
		/// <inheritdoc />
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _bindingGroups.GetEnumerator();
		}
	}
}
