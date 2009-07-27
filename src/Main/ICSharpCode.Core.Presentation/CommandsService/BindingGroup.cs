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

        private HashSet<UIElement> _attachedInstances = new HashSet<UIElement>();
        
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
        
        public static bool IsActive(IBindingInfo bindingInfo)
        {
			if(bindingInfo.OwnerInstances != null && bindingInfo.Groups != null && bindingInfo.Groups.Count > 0) {
				return bindingInfo.Groups.IsAttachedToAny(bindingInfo.OwnerInstances);
			}
			
			return true;
        }
        
        public bool IsAttachedTo(UIElement instance) 
        {
        	return _attachedInstances.Contains(instance);
        }
        
        public void AttachTo(UIElement instance)
        {
        	AttachToWithoutInvoke(instance);
        	InvokeBindingUpdateHandlers(instance);
        }
        
        private void AttachToWithoutInvoke(UIElement instance)
        {
    		_attachedInstances.Add(instance);
    		
    		foreach(var nestedGroup in _nestedGroups) {
    			nestedGroup.AttachToWithoutInvoke(instance);
    		}
        }
        
        public void DetachFromWithoutInvoke(UIElement instance)
        {
    		_attachedInstances.Remove(instance);
    		
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
        	
        	CommandManager.InvokeCommandBindingUpdateHandlers(BindingInfoMatchType.SubSet, bindingInfoTemplates);
        	CommandManager.InvokeInputBindingUpdateHandlers(BindingInfoMatchType.SubSet, bindingInfoTemplates);
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
