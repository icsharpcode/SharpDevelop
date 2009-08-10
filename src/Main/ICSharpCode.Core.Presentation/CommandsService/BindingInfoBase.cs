using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Base class for <see cref="CommandBindingInfo" /> and <see cref="InputBindingInfo" />
    /// </summary>
    abstract public class BindingInfoBase
    {
		private BindingGroupCollection _groups;
		private string _ownerInstanceName;
		private string _ownerTypeName;
		private string routedCommandName;
		private AddIn _addIn;
		
		/// <summary>
		/// Creates instance of <see cref="BindingInfoBase" />
		/// </summary>
		public BindingInfoBase() 
		{
			Groups = new BindingGroupCollection();
		}
			 
		/// <summary>
		/// Get or sets binding groups
		/// 
		/// If <see cref="BindingInfoBase" /> instance has a group assigned 
		/// then <see cref="InputBindingCollection" /> or <see cref="CommandBindingCollection" />
		/// is applied only for instances attached to the same group
		/// </summary>
		public BindingGroupCollection Groups
		{
			get {
				return _groups;
			}
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}

				var oldGroups = _groups;
				_groups = value;
				
				SetCollectionChanged<BindingGroup>(oldGroups, value, Groups_CollectionChanged);
			}
		}
		
		private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {  
			if(IsRegistered) {
				var modifiedGroups = new BindingGroupCollection();
				if(e.NewItems != null) {						
					modifiedGroups.AddRange(e.NewItems.Cast<BindingGroup>());
				}
				
				if(e.OldItems != null) {
					modifiedGroups.AddRange(e.OldItems.Cast<BindingGroup>());
				}
				
				ICollection<UIElement> attachedInstances = null;
				if(OwnerInstanceName != null) {
					attachedInstances = modifiedGroups.FlatNesteGroups.GetAttachedInstances(OwnerInstances);
				} else {
					attachedInstances = modifiedGroups.FlatNesteGroups.GetAttachedInstances(OwnerTypes);
				}
				
				SDCommandManager.InvokeBindingsChanged(this, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.GroupAttachmendsModified, modifiedGroups, attachedInstances));
			}
		}
		
		/// <summary>
		/// Gets name of named owner instance as registered using <see cref="ICSharpCode.Core.Presentation.CommandManager.RegisterNamedUIElement" />
		///
		/// If this attribute is used <see cref="OwnerTypeName" /> can not be set
		/// 
		/// This attribute can't be set while <see cref="InputBindingInfo" /> or <see cref="CommandBindingInfo" />
		/// is registered in <see cref="ICSharpCode.Core.Presentation.CommandManager" />
		/// </summary>
		public virtual string OwnerInstanceName {
			get {
				return _ownerInstanceName;
			}
			set {
				if(_ownerTypeName != null) {
					throw new ArgumentException("This binding info already has an owner");
				}
				
				if(IsRegistered) {
					throw new ArgumentException("Can not change owner while binding info is registered");
				}
				
				_ownerInstanceName = value;
			}
		}
		
		/// <summary>
		/// Gets collection of instances registered in <see cref="ICSharpCode.Core.Presentation.CommandManager" /> by name found in <see cref="OwnerInstanceName" /> property
		/// </summary>
		public ICollection<UIElement> OwnerInstances {
			get {
				if(_ownerInstanceName != null) {
					return SDCommandManager.GetNamedUIElementCollection(_ownerInstanceName);
				}
				
				return null;
			}
		}
					
		/// <summary>
		/// Gets name of named owner type as registered using <see cref="ICSharpCode.Core.Presentation.CommandManager.RegisterNamedUIType" />
		///
		/// If this attribute is used <see cref="OwnerInstanceName" /> can not be set
		/// 
		/// This attribute can't be set while <see cref="InputBindingInfo" /> or <see cref="CommandBindingInfo" />
		/// is registered in <see cref="ICSharpCode.Core.Presentation.CommandManager" />
		/// </summary>
		public virtual string OwnerTypeName 
		{
			get {
				return _ownerTypeName;
			}
			set {
				if(_ownerTypeName != null) {
					throw new ArgumentException("This binding info already has an owner");
				}
				
				if(IsRegistered) {
					throw new ArgumentException("Can not change owner while binding info is registered");
				}
				
				_ownerTypeName = value;
			}
		}
		
		/// <summary>
		/// Gets collection of types registered in <see cref="ICSharpCode.Core.Presentation.CommandManager" /> by name found in <see cref="OwnerInstanceName" /> property
		/// </summary>
		public ICollection<Type> OwnerTypes { 
			get {
				if(_ownerTypeName != null) {
					return SDCommandManager.GetNamedUITypeCollection(_ownerTypeName);
				}
				
				return null;
			}
		}
		
		/// <summary>
		/// Gets name of <see cref="RoutedUICommand" /> associated with binding info as registered using 
		/// <see cref="ICSharpCode.Core.Presentation.CommandManager.RegisterRoutedUICommand" />
		/// 
		/// This attribute can't be set while <see cref="InputBindingInfo" /> or <see cref="CommandBindingInfo" />
		/// is registered in <see cref="ICSharpCode.Core.Presentation.CommandManager" />
		/// </summary>
		public virtual string RoutedCommandName { 
			get {
				return routedCommandName;
			}
			set {
				if(IsRegistered) {
					throw new ArgumentException("Can not change routed command while binding info is registered");
				}
				
				routedCommandName = value;
			}
		}
		
		/// <summary>
		/// Gets <see cref="RoutedUICommand" /> instance registered in <see cref="ICSharpCode.Core.Presentation.CommandManager" /> by name found in <see cref="RoutedCommandName" /> property
		/// </summary>
		public RoutedUICommand RoutedCommand { 
			get { 
				return SDCommandManager.GetRoutedUICommand(RoutedCommandName);
			}
		}
		
		/// <summary>
		/// Gets whether <see cref="CommandBindingInfo" /> or <see cref="InputBindingInfo" /> is registered in 
		/// <see cref="ICSharpCode.Core.Presentation.CommandManager" /> 
		/// </summary>
		public bool IsRegistered
		{
			get; internal set;
		}
		
		/// <summary>
		/// Gets or sets add-in which created this <see cref="CommandBindingInfo" /> or <see cref="InputBindingInfo" />
		/// 
		/// In case of <see cref="CommandBindingInfo" /> this reference is used to create an instance of
		/// associated command when doing lazy loading
		/// 
		/// This attribute can't be set while <see cref="InputBindingInfo" /> or <see cref="CommandBindingInfo" />
		/// is registered in <see cref="ICSharpCode.Core.Presentation.CommandManager" />
		/// </summary>
		public AddIn AddIn {
			get {
				return _addIn;
			} 
			set {
				if(IsRegistered) {
					throw new ArgumentException("Can not change add-in while binding info is registered");
				}
				
				_addIn = value;
			}
		}
		
		protected void SetCollectionChanged<T>(IObservableCollection<T> oldObservableCollection, IObservableCollection<T> newObservableCollection, NotifyCollectionChangedEventHandler handler) 
		{
			newObservableCollection.CollectionChanged += handler;
			
			if(oldObservableCollection != null) {
				oldObservableCollection.CollectionChanged -= handler;
			
				var oldItems = new System.Collections.ArrayList();
				foreach(var oldItem in oldObservableCollection) {
					oldItems.Add(oldItem);
				}
				
				var newItems = new System.Collections.ArrayList();
				foreach(var newItem in newObservableCollection) {
					newItems.Add(newItem);
				}
				
				handler.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems, 0));
			}
		}
		
		/// <summary>
		/// Handles <see cref="ICSharpCode.Core.Presentation.CommandManager.BindingsChanged" /> event
		/// </summary>
		/// <param name="sender">Sender object</param>
		/// <param name="args">Event data</param>
		public void BindingsChangedHandler(object sender, NotifyBindingsChangedEventArgs args)
		{
			if(!IsRegistered || RoutedCommand == null || (OwnerTypes == null && OwnerInstances == null)) {
				return;
			}
			
			if( (args.Action == NotifyBindingsChangedAction.BindingInfoModified && args.ModifiedBindingInfoTemplates.Contains(BindingInfoTemplate.CreateFromIBindingInfo(this)))
			 || (args.Action == NotifyBindingsChangedAction.NamedInstanceModified && OwnerInstanceName == args.UIElementName)
			 || (args.Action == NotifyBindingsChangedAction.RoutedUICommandModified && routedCommandName == args.RoutedCommandName)
			 || (args.Action == NotifyBindingsChangedAction.NamedTypeModified && OwnerTypeName == args.TypeName)
			 || (args.Action == NotifyBindingsChangedAction.GroupAttachmendsModified && ((OwnerTypeName != null && OwnerTypes.Any(t1 => args.GroupHandledInstances.Any(t2 => t1 == t2.GetType())))
			 	 || (OwnerInstanceName != null && OwnerInstances.Any(t1 => args.GroupHandledInstances.Any(t2 => t1 == t2)))))
			 ) {
				GenerateBindings();
				
				if(OwnerInstanceName != null) {
					PopulateOwnerInstancesWithBindings(Groups.Count == 0 ? OwnerInstances : Groups.GetAttachedInstances(OwnerInstances));
				} else {
					if(Groups.Count == 0) {
						PopulateOwnerInstancesWithBindings(null);
						PopulateOwnerTypesWithBindings(OwnerTypes);
					} else {
						PopulateOwnerTypesWithBindings(null);
						PopulateOwnerInstancesWithBindings(Groups.GetAttachedInstances(OwnerTypes));
					}
				}
			}
		}
		
		/// <summary>
		/// Remove bindings currently assigned to <see cref="UIElement" /> or <see cref="Type" />
		/// collection described in this binding info
		/// </summary>
		public void RemoveActiveBindings()
		{
			PopulateOwnerInstancesWithBindings(null);
			PopulateOwnerTypesWithBindings(null);
		}
		
		/// <summary>
		/// Represents instance of <see cref="BindingInfoBase" /> as string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(
				"[{0}={1}, RoutedCommandName={2}, Hash={3}",
				OwnerInstanceName != null ? "OwnerInstanceName" : "OwnerTypeName",
				OwnerInstanceName != null ? OwnerInstanceName : OwnerTypeName,
				RoutedCommandName,
				GetHashCode());
		}
		
		/// <summary>
		/// Generate up to date <see cref="InputBindings" /> collection in case of <see cref="InputBindingInfo" />
		/// or <see cref="CommandBindings" /> collection in case of <see cref="CommandBindingInfo" />
		/// </summary>
		protected abstract void GenerateBindings();
		
		/// <summary>
		/// Fills <see cref="UIElement.CommandBindings" /> or <see cref="UIElement.InputBindings" /> collections 
		/// with generated <see cref="CommandBinding" /> or <see cref="InputBinding" /> collections
		/// </summary>
		/// <param name="newInstances">Collection of <see cref="UIElement" /></param>
		protected abstract void PopulateOwnerInstancesWithBindings(ICollection<UIElement> newInstances);
		
		/// <summary>
		/// Using <see cref="System.Windows.Input.CommandManager.RegisterClassCommandBinding" /> or <see cref="System.Windows.Input.CommandManager.RegisterClassCommandBinding" /> methods
		/// registers generated <see cref="CommandBinding" /> or <see cref="InputBinding" /> collections to collection of provided types
		/// </summary>
		/// <param name="newInstances">Collection of <see cref="Type" /></param>
		protected abstract void PopulateOwnerTypesWithBindings(ICollection<Type> newtTypes);
    }
}
