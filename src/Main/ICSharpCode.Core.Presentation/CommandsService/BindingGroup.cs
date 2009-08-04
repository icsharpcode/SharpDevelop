using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;


namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of BindingGroup.
	/// </summary>
	public class BindingGroup
	{
		private HashSet<WeakReference> _attachedInstances = new HashSet<WeakReference>(new WeakReferenceEqualirtyComparer());
		
		private BindingGroupCollection _nestedGroups = new BindingGroupCollection();
		
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
			SDCommandManager.InvokeBindingsChanged(
				this, 
				new NotifyBindingsChangedEventArgs(
					NotifyBindingsChangedAction.GroupAttachmendsModified, 
					FlatNestedGroups, 
					new []{instance}));
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
		
		public BindingGroupCollection NestedGroups
		{
			get {
				return _nestedGroups;
			}
		}
		
		public BindingGroupCollection FlatNestedGroups
		{
			get {
				var foundNestedGroups = new HashSet<BindingGroup>();
				FlattenNestedGroups(this, foundNestedGroups);
				
				var groups = new BindingGroupCollection();
				groups.AddRange(foundNestedGroups);
				return groups;
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
