using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.Core.Presentation
{
	public delegate void NotifyBindingsChangedEventHandler(object sender, NotifyBindingsChangedEventArgs args);
	
	public class NotifyBindingsChangedEventArgs : EventArgs
	{	
		private ICollection<BindingInfoTemplate> _modifiedBindingInfoTemplates;
		public ICollection<BindingInfoTemplate> ModifiedBindingInfoTemplates
		{
			get {
				return _modifiedBindingInfoTemplates;
			}
		}
		
		private string _routedCommandName;
		public string RoutedCommandName
		{
			get {
				return _routedCommandName;
			}
		}
		
		private string _typeName;
		public string TypeName
		{
			get {
				return _typeName;
			}
		}
		
		private ICollection<Type> _oldNamedTypes;
		public ICollection<Type> OldNamedTypes
		{
			get {
				return _oldNamedTypes;
			}
		}
		
		private ICollection<Type> _newNamedTypes;
		public ICollection<Type> NewNamedTypes
		{
			get {
				return _newNamedTypes;
			}
		}
		
		private string _uiElementName;
		public string UIElementName
		{
			get {
				return _uiElementName;
			}
		}
		
		private ICollection<UIElement> _oldNamedUIElements;
		public ICollection<UIElement> OldNamedUIElements
		{
			get {
				return _oldNamedUIElements;
			}
		}
		
		private ICollection<UIElement> _newNamedUIElements;
		public ICollection<UIElement> NewNamedUIElements
		{
			get {
				return _newNamedUIElements;
			}
		}
		
		private ICollection<UIElement> _attachedInstances;
		public ICollection<UIElement> AttachedInstances
		{
			get {
				return _attachedInstances;
			}
		}
		
		private BindingGroupCollection _groups;
		public BindingGroupCollection Groups
		{
			get {
				return _groups;
			}
		}
		
		private NotifyBindingsChangedAction _action;
		public NotifyBindingsChangedAction Action
		{
			get {
				return _action;
			}
		}
		
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action)
		{
			if(action != NotifyBindingsChangedAction.EnforceUpdates) {
				throw new ArgumentException(
					string.Format(
						"This constructor only supports '{0}' action (got '{1}')",
					    Enum.GetName(typeof(NotifyBindingsChangedAction), NotifyBindingsChangedAction.EnforceUpdates),
					    Enum.GetName(typeof(NotifyBindingsChangedAction), action)
					   ));
			}
			
			_action = action;
		}
		
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, string typeName, ICollection<Type> oldTypes, ICollection<Type> newTypes)
		{
			if(action != NotifyBindingsChangedAction.NamedTypeModified) {
				throw new ArgumentException(
					string.Format(
						"This constructor only supports '{0}' action (got '{1}')",
					    Enum.GetName(typeof(NotifyBindingsChangedAction), NotifyBindingsChangedAction.NamedTypeModified),
					    Enum.GetName(typeof(NotifyBindingsChangedAction), action)
					   ));
			}
			
			if(typeName == null) {
				throw new ArgumentNullException("typeName");
			}

			if(oldTypes == null) {
				throw new ArgumentNullException("oldTypes");
			}

			if(newTypes == null) {
				throw new ArgumentNullException("newTypes");
			}

			var oldTypesArray = new Type[0];
			if(oldTypes != null) {
				oldTypesArray = new Type[oldTypes.Count];
				oldTypes.CopyTo(oldTypesArray, 0);
			}
			
			var newTypesArray = new Type[0];
			if(newTypes != null) {
				newTypesArray = new Type[newTypes.Count];
				newTypes.CopyTo(newTypesArray, 0);
			}
			
			_action = action;
			_typeName = typeName;
			_oldNamedTypes = oldTypesArray;
			_newNamedTypes = newTypesArray;
		}
		
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, string elementName, ICollection<UIElement> oldElements, ICollection<UIElement> newElements)
		{
			if(action != NotifyBindingsChangedAction.NamedInstanceModified) {
				throw new ArgumentException(
					string.Format(
						"This constructor only supports '{0}' action (got '{1}')",
					    Enum.GetName(typeof(NotifyBindingsChangedAction), NotifyBindingsChangedAction.NamedInstanceModified),
					    Enum.GetName(typeof(NotifyBindingsChangedAction), action)
					   ));
			}
			
			if(elementName == null) {
				throw new ArgumentNullException("elementName");
			}
			
			if(oldElements == null) {
				throw new ArgumentNullException("oldElements");
			}
			
			if(newElements == null) {
				throw new ArgumentNullException("newElements");
			}
			
			var oldElementsArray = new UIElement[0];
			if(oldElements != null) {
				oldElementsArray = new UIElement[oldElements.Count];
				oldElements.CopyTo(oldElementsArray, 0);
			}
			
			var newElementsArray = new UIElement[0];
			if(newElements != null) {
				newElementsArray = new UIElement[newElements.Count];
				newElements.CopyTo(newElementsArray, 0);
			}
			
			_action = action;
			_uiElementName = elementName;
			_oldNamedUIElements = oldElementsArray;
			_newNamedUIElements = newElementsArray;
		}
		
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, string routedCommandName)
		{
			if(action != NotifyBindingsChangedAction.RoutedUICommandModified) {
				throw new ArgumentException(
					string.Format(
						"This constructor only supports '{0}' action (got '{1}')",
					    Enum.GetName(typeof(NotifyBindingsChangedAction), NotifyBindingsChangedAction.RoutedUICommandModified),
					    Enum.GetName(typeof(NotifyBindingsChangedAction), action)
					   ));
			}
			
			if(routedCommandName == null) {
				throw new ArgumentNullException("routedCommandName");
			}
			
			_action = action;
			_routedCommandName = routedCommandName;
		}
		
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, IEnumerable<BindingInfoTemplate> templates)
		{
			if(action != NotifyBindingsChangedAction.BindingInfoModified) {
				throw new ArgumentException(
					string.Format(
						"This constructor only supports '{0}' action (got '{1}')",
					    Enum.GetName(typeof(NotifyBindingsChangedAction), NotifyBindingsChangedAction.BindingInfoModified),
					    Enum.GetName(typeof(NotifyBindingsChangedAction), action)
					   ));
			}
			
			if(templates == null) {
				throw new ArgumentNullException("templates");
			}
			
			_action = action;
			_modifiedBindingInfoTemplates = new HashSet<BindingInfoTemplate>(templates);
		}
		
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, BindingGroupCollection groups, ICollection<UIElement> attachedInstances)
		{
			if(action != NotifyBindingsChangedAction.GroupAttachmendsModified) {
				throw new ArgumentException(
					string.Format(
						"This constructor only supports '{0}' action (got '{1}')",
					    Enum.GetName(typeof(NotifyBindingsChangedAction), NotifyBindingsChangedAction.GroupAttachmendsModified),
					    Enum.GetName(typeof(NotifyBindingsChangedAction), action)
					   ));
			}
			
			if(groups == null) {
				throw new ArgumentNullException("groups");
			}
			
			if(attachedInstances == null) {
				throw new ArgumentNullException("attachedInstances");
			}
			
			_action = action;
			_groups = new BindingGroupCollection();
			_groups.AddRange(groups);
			_attachedInstances = new HashSet<UIElement>(attachedInstances);
		}
		
	}
	
	public enum NotifyBindingsChangedAction
	{
		EnforceUpdates,
		BindingInfoModified,
		NamedInstanceModified,
		NamedTypeModified,
		RoutedUICommandModified,
		GroupAttachmendsModified
	}
}
