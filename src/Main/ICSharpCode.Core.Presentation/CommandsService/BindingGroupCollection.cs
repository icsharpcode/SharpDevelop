using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

using ICSharpCode.Core;

namespace ICSharpCode.Core.Presentation
{
    public class BindingGroupCollection : ICollection<BindingGroup>
    {
    	private ObservableCollection<BindingGroup> _bindingGroups = new ObservableCollection<BindingGroup>();
    	
    	public event NotifyCollectionChangedEventHandler CollectionChanged
    	{
    		add {
    			_bindingGroups.CollectionChanged += value;
    		}
    		remove {
    			_bindingGroups.CollectionChanged -= value;
    		}
    	}
    	
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
    	
    	public bool IsAttachedTo(UIElement instance)
    	{
    		foreach(var bindingGroup in FlatNesteGroups) {
    			if(bindingGroup.IsAttachedTo(instance)) {
    				return true;
    			}
    		}
    		
    		return false;
    	}
    	
    	public bool IsAttachedToAny(ICollection<UIElement> instances)
    	{
    		if(instances != null && instances.Count > 0) {
    			foreach(var instance in instances) {
    				if(IsAttachedTo(instance)) {
    					return true;
    				}
    			}
    		}
    		
    		return false;
    	}
    	
		public int Count {
			get {
				return _bindingGroups.Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public void Add(BindingGroup bindingGroup)
		{
			if(bindingGroup == null) {
				throw new ArgumentNullException("bindingGroup");
			}
			
			if(!_bindingGroups.Contains(bindingGroup)) {
				_bindingGroups.Add(bindingGroup);
			}
		}
		
		public void Clear()
		{
			_bindingGroups.Clear();
		}
		
		public bool ContainsNestedAny(BindingGroupCollection bindingGroups)
		{
			return FlatNesteGroups.ContainsAnyFromCollection(bindingGroups);
		}
		
		public bool ContainsNested(BindingGroup bindingGroup)
		{
			return FlatNesteGroups.Contains(bindingGroup);
		}
		
		public bool ContainsAny(BindingGroupCollection bindingGroups)
		{
			return FlatNesteGroups.ContainsAnyFromCollection(bindingGroups);
		}
		
		public bool Contains(BindingGroup bindingGroup)
		{
			return _bindingGroups.Contains(bindingGroup);
		}
		
		public void AddRange(IEnumerable<BindingGroup> bindingGroups)
		{
			foreach(var bindingGroup in bindingGroups) {
				Add(bindingGroup);
			}
		}
		
		public void CopyTo(BindingGroup[] array, int arrayIndex)
		{
			_bindingGroups.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(BindingGroup bindingGroup)
		{
			return _bindingGroups.Remove(bindingGroup);
		}
		
		public IEnumerator<BindingGroup> GetEnumerator()
		{
			return _bindingGroups.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _bindingGroups.GetEnumerator();
		}
    }
}
