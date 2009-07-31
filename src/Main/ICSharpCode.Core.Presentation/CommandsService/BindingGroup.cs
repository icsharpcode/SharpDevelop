using System;
using System.Linq;
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
			foreach(var nestedGroup in FlatNestedGroups) {
				if(nestedGroup._attachedInstances.Contains(new WeakReference(instance))) {
					return true;
				}
			}
			
			return false;
		}
		
		public void AttachTo(UIElement instance)
		{
			AttachToWithoutInvoke(instance);
			InvokeBindingUpdateHandlers(instance, true);
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
			InvokeBindingUpdateHandlers(instance, false);
		}
		
		private void InvokeBindingUpdateHandlers(UIElement instance, bool attaching)
		{
			var type = instance.GetType();
			
			// Invoke class wide and instance update handlers
			var instanceNames = CommandManager.GetUIElementNameCollection(instance);
			var typeNames = CommandManager.GetUITypeNameCollection(type);
			
			var bindingInfoTemplates = new IBindingInfoTemplate[instanceNames.Count + typeNames.Count + 1];
						
			var i = 0;
				
			bindingInfoTemplates[i++] = new BindingInfoTemplate { Groups = new BindingGroupCollection { this } };
				
			foreach(var instanceName in instanceNames) {
				bindingInfoTemplates[i++] = new BindingInfoTemplate { OwnerInstanceName = instanceName};
			} 

			foreach(var typeName in typeNames) {
				bindingInfoTemplates[i++] = new BindingInfoTemplate { OwnerTypeName = typeName};
			}
			
			var args = new BindingsUpdatedHandlerArgs();
			if(attaching) {
				args.AddedInstances = new List<UIElement>{ instance };
			} else {
				args.RemovedInstances = new List<UIElement>{ instance };
			}
			
			CommandManager.InvokeCommandBindingUpdateHandlers(
				this,
				args,
				BindingInfoMatchType.SubSet | BindingInfoMatchType.SuperSet, 
				bindingInfoTemplates);
			
			CommandManager.InvokeInputBindingUpdateHandlers(
				this,
				args,
				BindingInfoMatchType.SubSet | BindingInfoMatchType.SuperSet, 
				bindingInfoTemplates);
		}
		
		public ICollection<UIElement> GetAttachedInstances(ICollection<Type> types)
		{
			var attachedInstances = new HashSet<UIElement>();
			foreach(var nestedGroup in FlatNestedGroups) {
				foreach(var wr in nestedGroup._attachedInstances) {
					var wrTarget = (UIElement)wr.Target;
					if(wrTarget != null && types.Any(t => t.IsInstanceOfType(wrTarget))) {
						attachedInstances.Add(wrTarget);
					}
				}
			}
			
			return attachedInstances;
		}
		
		public ICollection<UIElement> GetAttachedInstances(ICollection<UIElement> instances)
		{
			var attachedInstances = new HashSet<UIElement>();
			foreach(var nestedGroup in FlatNestedGroups) {
				foreach(var wr in nestedGroup._attachedInstances) {
					var wrTarget = (UIElement)wr.Target;
					if(wrTarget != null && instances.Contains(wrTarget)) {
						attachedInstances.Add(wrTarget);
					}
				}
			}
			
			return attachedInstances;
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
