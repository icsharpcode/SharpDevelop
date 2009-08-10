using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Represent a method that will handle <see cref="ICSharpCode.Core.Presentation.CommandManager.BindingsChanged" /> event
	/// </summary>
	public delegate void NotifyBindingsChangedEventHandler(object sender, NotifyBindingsChangedEventArgs args);
	
	/// <summary>
	/// Provides data for <see cref="ICSharpCode.Core.Presentation.CommandManager.BindingsChanged" /> event
	/// </summary>
	public class NotifyBindingsChangedEventArgs : EventArgs
	{	
		private ICollection<BindingInfoTemplate> _modifiedBindingInfoTemplates;
		private string _routedCommandName;
		private string _typeName;
		private ICollection<Type> _oldNamedTypes;
		private ICollection<Type> _newNamedTypes;
		private string _uiElementName;
		private ICollection<UIElement> _oldNamedUIElements;
		private ICollection<UIElement> _newNamedUIElements;
		private ICollection<UIElement> _attachedInstances;
		private BindingGroupCollection _modifiedGroups;
		private NotifyBindingsChangedAction _action;
		
		/// <summary>
		/// Gets collection of templates which can identify changed 
		/// <see cref="CommandBindingInfo" /> and <see cref="InputBindingInfo" /> objects
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.BindingInfoModified" />
		/// </summary>
		public ICollection<BindingInfoTemplate> ModifiedBindingInfoTemplates
		{
			get {
				return _modifiedBindingInfoTemplates;
			}
		}
		
		/// <summary>
		/// Gets name of changed named <see cref="System.Windows.Input.RoutedUICommand" /> as registered using
		/// <see cref="CommandManager.RegisterRoutedUICommand" /> 
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.RoutedUICommandModified" />
		/// </summary>
		public string RoutedCommandName
		{
			get {
				return _routedCommandName;
			}
		}
		
		/// <summary>
		/// Gets name of modified named <see cref="Type" /> as registered using <see cref="CommandManager.RegisterNamedUIType" /> 
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.NamedTypeModified" />
		/// </summary>
		public string TypeName
		{
			get {
				return _typeName;
			}
		}
		
		/// <summary>
		/// Gets collection of types associated with name provided in <see cref="NotifyBindingsChangedEventArgs.TypeName" />
		/// before the modification event
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.NamedTypeModified" />
		/// </summary>
		public ICollection<Type> OldNamedTypes
		{
			get {
				return _oldNamedTypes;
			}
		}
		
		
		/// <summary>
		/// Gets collection of types associated with name provided in <see cref="NotifyBindingsChangedEventArgs.TypeName" /> 
		/// after the modification event
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.NamedTypeModified" />
		/// </summary>
		public ICollection<Type> NewNamedTypes
		{
			get {
				return _newNamedTypes;
			}
		}
		
		
		/// <summary>
		/// Gets name of modified named <see cref="UIElement" /> instance as registered using <see cref="CommandManager.RegisterNamedUIElement" /> 
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.NamedInstanceModified" />
		/// </summary>
		public string UIElementName
		{
			get {
				return _uiElementName;
			}
		}
	
		/// <summary>
		/// Gets collection of <see cref="UIElement" /> instances associated with name provided in <see cref="NotifyBindingsChangedEventArgs.UIElementName" /> property 
		/// before the modification event
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.NamedInstanceModified" />
		/// </summary>
		public ICollection<UIElement> OldNamedUIElements
		{
			get {
				return _oldNamedUIElements;
			}
		}
	
		/// <summary>
		/// Gets collection of <see cref="UIElement" /> instances associated with name provided in <see cref="NotifyBindingsChangedEventArgs.UIElementName" /> property 
		/// after the modification event
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.NamedInstanceModified" />
		/// </summary>	
		public ICollection<UIElement> NewNamedUIElements
		{
			get {
				return _newNamedUIElements;
			}
		}
		
		/// <summary>
		/// Gets collection of collection of modified <see cref="BindingInfoGroup" />
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.GroupAttachmendsModified" />
		/// </summary>
		public BindingGroupCollection ModifiedGroups
		{
			get {
				return _modifiedGroups;
			}
		}
		
		/// <summary>
		/// Gets collection of instances whose <see cref="UIElement.CommandBindings" />
		/// and <see cref="UIElement.InputBindings" /> collections are controled by
		/// groups provided in <see cref="NotifyBindingsChangedEventArgs.ModifiedGroups" /> property 
		/// 
		/// Provided only when <see cref="NotifyBindingsChangedEventArgs.Action" /> is equal to <see cref="NotifyBindingsChangedAction.GroupAttachmendsModified" />
		/// </summary>
		public ICollection<UIElement> GroupHandledInstances
		{
			get {
				return _attachedInstances;
			}
		}
		
		/// <summary>
		/// Gets action that caused an event
		/// </summary>
		public NotifyBindingsChangedAction Action
		{
			get {
				return _action;
			}
		}
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyBindingsChangedEventArgs" /> that describes
		/// <see cref="NotifyBindingsChangedAction.EnforceUpdates" /> event
		/// </summary>
		/// <param name="action">The action that caused this event. This can only be set to <see cref="NotifyBindingsChangedAction.EnforceUpdates" /></param>
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action)
		{
			ValidateAction(NotifyBindingsChangedAction.EnforceUpdates, action);
			
			_action = action;
		}
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyBindingsChangedEventArgs" /> that describes
		/// <see cref="NotifyBindingsChangedAction.NamedTypeModified" /> event
		/// </summary>
		/// <param name="action">The action that caused this event. This can only be set to <see cref="NotifyBindingsChangedAction.NamedTypeModified" /></param>
		/// <param name="elementName">Registered <see cref="Type" /> name (Can be different from <see cref="System.Type.Name")</param>
		/// <param name="oldElements">Collection of <see cref="Type" />s associated with provided name before modification</param>
		/// <param name="newElements">Collection of <see cref="Type" />s associated with provided name after modification</param>
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, string typeName, ICollection<Type> oldTypes, ICollection<Type> newTypes)
		{
			ValidateAction(NotifyBindingsChangedAction.NamedTypeModified, action);
			
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
		
		
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyBindingsChangedEventArgs" /> that describes
		/// <see cref="NotifyBindingsChangedAction.NamedInstanceModified" /> event
		/// </summary>
		/// <param name="action">The action that caused this event. This can only be set to <see cref="NotifyBindingsChangedAction.NamedInstanceModified" /></param>
		/// <param name="elementName"><see cref="UIElement" /> instance name</param>
		/// <param name="oldElements">Collection of <see cref="UIElement" /> instances associated with provided name before modification</param>
		/// <param name="newElements">Collection of <see cref="UIElement" /> instances associated with provided name after modification</param>
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, string elementName, ICollection<UIElement> oldElements, ICollection<UIElement> newElements)
		{
			ValidateAction(NotifyBindingsChangedAction.NamedInstanceModified, action);
			
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
		
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyBindingsChangedEventArgs" /> that describes
		/// <see cref="NotifyBindingsChangedAction.RoutedUICommandModified" /> event
		/// </summary>
		/// <param name="action">The action that caused this event. This can only be set to <see cref="NotifyBindingsChangedAction.RoutedUICommandModified" /></param>
		/// <param name="routedCommandName">Registered or unregistered <see cref="RoutedUICommand" /> name</param>
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, string routedCommandName)
		{
			ValidateAction(NotifyBindingsChangedAction.RoutedUICommandModified, action);
			
			if(routedCommandName == null) {
				throw new ArgumentNullException("routedCommandName");
			}
			
			_action = action;
			_routedCommandName = routedCommandName;
		}
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyBindingsChangedEventArgs" /> that describes
		/// <see cref="NotifyBindingsChangedAction.BindingInfoModified" /> event
		/// </summary>
		/// <param name="action">The action that caused this event. This can only be set to <see cref="NotifyBindingsChangedAction.BindingInfoModified" /></param>
		/// <param name="templates">Collection of <see cref="BindingInfoTemplate" />s describing modified <see cref="CommandBindingInfo" />s or <see cref="InputBindingInfo" />s</param>
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, IEnumerable<BindingInfoTemplate> templates)
		{
			ValidateAction(NotifyBindingsChangedAction.BindingInfoModified, action);
			
			if(templates == null) {
				throw new ArgumentNullException("templates");
			}
			
			_action = action;
			_modifiedBindingInfoTemplates = new HashSet<BindingInfoTemplate>(templates);
		}
		
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyBindingsChangedEventArgs" /> that describes
		/// <see cref="NotifyBindingsChangedAction.GroupAttachmendsModified" /> change event
		/// </summary>
		/// <param name="action">The action that caused this event. This can only be set to <see cref="NotifyBindingsChangedAction.GroupAttachmendsModified" /></param>
		/// <param name="groups">Modified groups</param>
		/// <param name="attachedInstances">Collection instances (un)registered in <see cref="BindingGroup" /></param>
		public NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction action, BindingGroupCollection groups, ICollection<UIElement> attachedInstances)
		{
			ValidateAction(NotifyBindingsChangedAction.GroupAttachmendsModified, action);
			
			if(groups == null) {
				throw new ArgumentNullException("groups");
			}
			
			if(attachedInstances == null) {
				throw new ArgumentNullException("attachedInstances");
			}
			
			_action = action;
			_modifiedGroups = new BindingGroupCollection();
			_modifiedGroups.AddRange(groups);
			_attachedInstances = new HashSet<UIElement>(attachedInstances);
		}
		
		private void ValidateAction(NotifyBindingsChangedAction expectedAction, NotifyBindingsChangedAction receivedAction)
		{
			if(receivedAction != expectedAction) {
				throw new ArgumentException(
					string.Format(
						"This constructor only supports '{0}' action (got '{1}')",
					    Enum.GetName(typeof(NotifyBindingsChangedAction), expectedAction),
					    Enum.GetName(typeof(NotifyBindingsChangedAction), receivedAction)
					   ));
			}
		}
	}
	
	/// <summary>
	/// Describes the action that caused a <see cref="ICSharpCode.Core.Presentation.CommandManager.BindingsChanged" /> event.
	/// </summary>
	public enum NotifyBindingsChangedAction
	{
		/// <summary>
		/// Implicitly enforce executing all handlers
		/// </summary>
		EnforceUpdates,
		
		/// <summary>
		/// Binding info modified
		/// </summary>
		BindingInfoModified,
		
		/// <summary>
		/// Named instance modfied (new <see cref="UIElement" /> added/removed)
		/// </summary>
		NamedInstanceModified,
		
		/// <summary>
		/// Named type modfied (new <see cref="Type" /> added/removed)
		/// </summary>
		NamedTypeModified,
		
		/// <summary>
		/// <see cref="RoutedUICommand" /> registered/unregistered
		/// </summary>
		RoutedUICommandModified,
		
		/// <summary>
		/// Groups modified (group attached/detached to binding info or new <see cref="UIElement" />
		/// is registered to be handled by <see cref="BindingGroup" />
		/// </summary>
		GroupAttachmendsModified
	}
}
