/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 7/14/2009
 * Time: 1:20 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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
    			var flatGroups = new BindingGroupCollection();
    			foreach(var bindingGroup in this) {
    				bindingGroup.FlattenNestedGroups(bindingGroup, flatGroups);
    			}
    			
    			return flatGroups;
    		}
    	}
    	
    	public bool IsAttachedTo(string instanceName)
    	{
    		foreach(var bindingGroup in FlatNesteGroups) {
    			if(bindingGroup.IsAttachedTo(instanceName)) {
    				return true;
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
			foreach(var bindingGroup in _bindingGroups) {
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
