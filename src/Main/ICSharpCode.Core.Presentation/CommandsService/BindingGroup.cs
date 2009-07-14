/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 7/13/2009
 * Time: 5:10 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;


namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Description of BindingGroup.
    /// </summary>
    public class BindingGroup
    {
        private ObservableCollection<InputBindingInfo> _inputBindings;
        private ObservableCollection<CommandBindingInfo> _commandBindings;

        private List<string> _attachedInstances = new List<string>();
        
        private List<BindingGroup> _nestedGroups = new List<BindingGroup>();
        
        public BindingGroup()
        {
        	_inputBindings = new ObservableCollection<InputBindingInfo>();
        	_inputBindings.CollectionChanged += inputBindings_CollectionChanged;
        	
        	_commandBindings = new ObservableCollection<CommandBindingInfo>();
        	_commandBindings.CollectionChanged += commandBindings_CollectionChanged;
        }
        
        public string Name
        {
        	get; set;
        }
        
        public bool IsAttachedTo(string instanceName) 
        {
        	return _attachedInstances.Contains(instanceName);
        }
        
        private void AttachToWithoutInvoke(string instanceName)
        {
        	if(!_attachedInstances.Contains(instanceName)) {
        		_attachedInstances.Add(instanceName);
        		
        		foreach(var nestedGroup in _nestedGroups) {
        			nestedGroup.AttachToWithoutInvoke(instanceName);
        		}
        	}
        }
        
        public void AttachTo(string instanceName)
        {
        	if(!_attachedInstances.Contains(instanceName)) {
        		AttachToWithoutInvoke(instanceName);
        		
        		CommandManager.InvokeCommandBindingUpdateHandlers(null, instanceName);
        		CommandManager.InvokeInputBindingUpdateHandlers(null, instanceName);
        	}
        }
        
        public void DetachFromWithoutInvoke(string instanceName)
        {
        	if(_attachedInstances.Contains(instanceName)) {
        		_attachedInstances.Remove(instanceName);
        		
        		foreach(var nestedGroup in _nestedGroups) {
        			nestedGroup.DetachFrom(instanceName);
        		}
        	}
        }
        
        public void DetachFrom(string instanceName)
        {
        	if(_attachedInstances.Contains(instanceName)) {
        		DetachFromWithoutInvoke(instanceName);
        		
        		CommandManager.InvokeCommandBindingUpdateHandlers(null, instanceName);
        		CommandManager.InvokeInputBindingUpdateHandlers(null, instanceName);
        	}
        }
        
        public List<BindingGroup> NestedGroups
        {
        	get {
        		return _nestedGroups;
        	}
        }
        
        public ICollection<BindingGroup> FlatNestedGroups
        {
        	get {
        		var foundNestedGroups = new List<BindingGroup>();
        		FlattenNestedGroups(this, foundNestedGroups);
        		
        		return foundNestedGroups.AsReadOnly();
        	}
        }
        
        internal void FlattenNestedGroups(BindingGroup rootGroup, ICollection<BindingGroup> foundGroups)
        {
        	if(!foundGroups.Contains(rootGroup)) {
        		foundGroups.Add(rootGroup);
        	}
        	
        	foreach(var nestedGroup in NestedGroups) {
        		if(!foundGroups.Contains(nestedGroup)) {
        			foundGroups.Add(nestedGroup);
        			FlattenNestedGroups(nestedGroup, foundGroups);
        		}
        	}
        }
        
        public ICollection<InputBindingInfo> InputBindings
        {
        	get {
        		return _inputBindings;
        	}
        }
        
        public ICollection<CommandBindingInfo> CommandBindings
        {
        	get {
        		return _commandBindings;
        	}
        }
        
        private void commandBindings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        	
        }
        
        private void inputBindings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        	
        }
    }
}
