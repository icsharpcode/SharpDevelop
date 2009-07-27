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
        private HashSet<WeakReference> _attachedInstances = new HashSet<WeakReference>(new WeakReferenceEqualirtyComparer());
        
        private List<BindingGroup> _nestedGroups = new List<BindingGroup>();
        
        
        public string Name
        {
        	get; set;
        }
        
        public bool IsAttachedTo(UIElement instance) 
        {
        	return _attachedInstances.Contains(new WeakReference(instance));
        }
        
        public void AttachTo(UIElement instance)
        {
        	AttachToWithoutInvoke(instance);
        	InvokeBindingUpdateHandlers(instance);
        }
        
        private void AttachToWithoutInvoke(UIElement instance)
        {
    		_attachedInstances.Add(new WeakReference(instance));
    		
    		foreach(var nestedGroup in _nestedGroups) {
    			nestedGroup.AttachToWithoutInvoke(instance);
    		}
        }
        
        public void DetachFromWithoutInvoke(UIElement instance)
        {
    		_attachedInstances.Remove(new WeakReference(instance));
    		
    		foreach(var nestedGroup in _nestedGroups) {
    			nestedGroup.DetachFrom(instance);
    		}
        }
        
        public void DetachFrom(UIElement instance)
        {
        	DetachFromWithoutInvoke(instance);
        	InvokeBindingUpdateHandlers(instance);
        }
        
        private void InvokeBindingUpdateHandlers(UIElement instance)
        {
        	var i = 0;
        	
        	// Invoke class wide and instance update handlers
        	var instanceNames = CommandManager.GetUIElementNameCollection(instance);
        	var typeNames = CommandManager.GetUITypeNameCollection(instance.GetType());
        	
        	var bindingInfoTemplates = new BindingInfoTemplate[instanceNames.Count + typeNames.Count];
        	
        	foreach(var instanceName in instanceNames) {
        		bindingInfoTemplates[i++] = new BindingInfoTemplate { OwnerInstanceName = instanceName };
        	}
        	
        	foreach(var typeName in typeNames) {
        		bindingInfoTemplates[i++] = new BindingInfoTemplate { OwnerTypeName = typeName };
        	}
        	
        	CommandManager.InvokeCommandBindingUpdateHandlers(
        		BindingInfoMatchType.SubSet | BindingInfoMatchType.SuperSet, 
        		bindingInfoTemplates);
        	
        	CommandManager.InvokeInputBindingUpdateHandlers(
        		BindingInfoMatchType.SubSet | BindingInfoMatchType.SuperSet, 
        		bindingInfoTemplates);
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
        		var foundNestedGroups = new HashSet<BindingGroup>();
        		FlattenNestedGroups(this, foundNestedGroups);
        		
        		return foundNestedGroups;
        	}
        }
        
        internal void FlattenNestedGroups(BindingGroup rootGroup, HashSet<BindingGroup> foundGroups)
        {
        	foundGroups.Add(rootGroup);
        	
        	foreach(var nestedGroup in NestedGroups) {
        		if(foundGroups.Add(nestedGroup)) {
        			FlattenNestedGroups(nestedGroup, foundGroups);
        		}
        	}
        }
    }
}
