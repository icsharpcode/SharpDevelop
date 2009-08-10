using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
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
		private bool forwardEvents = true;
		
		private ObservableCollection<BindingGroup> _bindingGroups;
		private event NotifyCollectionChangedEventHandler _bindingGroupsCollectionChanged;
		private bool _isReadOnly;
		
		/// <summary>
		/// Creates instance of read-write <see cref="BindingGroupCollection" />
		/// </summary>
		public BindingGroupCollection() : this(false, null)
		{
		}
		
		/// <summary>
		/// Creates instance of <see cref="BindingGroupCollection" />
		/// </summary>
		/// <param name="isReadOnly">Specifies whether collection is read-only or not</param>
		/// <param name="groups">Prefilled groups</param>
		public BindingGroupCollection(bool isReadOnly, ICollection<BindingGroup> groups)
		{
			_isReadOnly = isReadOnly;
			
			var observableBindingGroups = new ObservableCollection<BindingGroup>(groups == null ? new List<BindingGroup>() : groups.ToList());
			observableBindingGroups.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e) {  
				if(_bindingGroupsCollectionChanged != null && forwardEvents) {
					_bindingGroupsCollectionChanged.Invoke(this, e);
				}
			};
			
			_bindingGroups = observableBindingGroups;
		}
		
		/// <summary>
		/// Gets read-only collection of attached instances
		/// </summary>
		public ICollection<UIElement> AttachedInstances
		{
			get {
				var attachments = new HashSet<UIElement>();
				foreach(var group in this) {
					foreach(var groupAttachment in group.AttachedInstances) {
						attachments.Add(groupAttachment);
					}
				}
				
				return new ReadOnlyCollection<UIElement>(attachments.ToList());
			}
		}
		
		/// <summary>
		/// Gets single-level collection containing attached instances from this group and all sub-groups
		/// </summary>
		public ICollection<UIElement> FlatNestedAttachedInstances
		{
			get {
				var attachments = new HashSet<UIElement>();
				foreach(var group in this.FlatNesteGroups) {
					foreach(var groupAttachment in group.AttachedInstances) {
						attachments.Add(groupAttachment);
					}
				}
				
				return new ReadOnlyCollection<UIElement>(attachments.ToArray());
			}
		}
		
		/// <inheritdoc />
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add {
				if(_isReadOnly) {
					throw new ReadOnlyException("This BindingGroupCollection is read-only");
				}
				
				_bindingGroupsCollectionChanged += value;
			}
			remove {
				if(_isReadOnly) {
					throw new ReadOnlyException("This BindingGroupCollection is read-only");
				}
				
				_bindingGroupsCollectionChanged -= value;
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
				
				return new BindingGroupCollection(true, flatGroups);
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
		
		/// <summary>
		/// Returns returns read-only copy of <see cref="BindingGroupCollection" />
		/// </summary>
		/// <returns>Read-only <see cref="BindingGroupCollection" /></returns>
		public BindingGroupCollection AsReadOnly()
		{
			return new BindingGroupCollection(true, this);
		}
		
		/// <inheritdoc />
		public bool IsReadOnly {
			get {
				return _isReadOnly;
			}
		}
		
		/// <summary>
		/// Add <see cref="BindingGroup" /> to this collection
		/// </summary>
		/// <param name="bindingGroup"></param>
		public void Add(BindingGroup bindingGroup)
		{
			if(_isReadOnly) {
				throw new ReadOnlyException("This BindingGroupCollection is read-only");
			}
			
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
			if(_isReadOnly) {
				throw new ReadOnlyException("This BindingGroupCollection is read-only");
			}

			var groupsBackup = FlatNesteGroups.ToArray();
			forwardEvents = false;
			_bindingGroups.Clear();
			forwardEvents = true;
			
			InvokeCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, groupsBackup));
		}
		
		/// <summary>
		/// Add many instances of <see cref="BindingGroup" /> to this collection
		/// </summary>
		/// <param name="bindingGroups"></param>
		public void AddRange(IEnumerable<BindingGroup> bindingGroups)
		{
			if(_isReadOnly) {
				throw new ReadOnlyException("This BindingGroupCollection is read-only");
			}
			
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
			if(_isReadOnly) {
				throw new ReadOnlyException("This BindingGroupCollection is read-only");
			}
			
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
		
		private void InvokeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) 
		{
			if(_bindingGroupsCollectionChanged != null && forwardEvents) {
				_bindingGroupsCollectionChanged.Invoke(this, e);
			}
		}
		
	}
}
