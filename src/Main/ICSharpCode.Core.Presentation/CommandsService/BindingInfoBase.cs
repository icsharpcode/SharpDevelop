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
    abstract public class BindingInfoBase : IBindingInfo
    {
		private BindingGroupCollection _groups;
		private string _ownerInstanceName;
		private string _ownerTypeName;
		private string routedCommandName;
		
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
		/// Stores name of named instance to which this binding belongs. When this binding is registered a
		/// <see cref="InputBinding" /> is assigned to owner instance
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerType" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public virtual string OwnerInstanceName {
			get {
				return _ownerInstanceName;
			}
			set {
				if(_ownerInstanceName != null || _ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				_ownerInstanceName = value;
			}
		}
		
		/// <summary>
		/// Stores owner instance to which this binding belongs. When this binding is registered a
		/// <see cref="InputBinding" /> is assigned to owner instance
		/// 
		/// If this attribute is used <see cref="OwnerInstanceName" />, <see cref="OwnerType" /> and
		/// <see cref="OwnerTypeName" /> can not be set
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
		/// Stores name of owner type. Full name with assembly should be used. When this binding is 
		/// registered <see cref="InputBinding" /> is assigned to all instances of provided class
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerInstanceName" /> and
		/// <see cref="OwnerType" /> can not be set
		/// </summary>
		public virtual string OwnerTypeName 
		{
			get {
				return _ownerTypeName;
			}
			set {
				if(_ownerInstanceName != null || _ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				_ownerTypeName = value;
			}
		}
		
		/// <summary>
		/// Stores owner type. When this binding is registered <see cref="InputBinding" /> 
		/// is assigned to all instances of provided class
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerInstanceName" /> and
		/// <see cref="OwnerTypeName" /> can not be set
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
		/// Routed command text
		/// 
		/// Override routed command text when displaying to user
		/// </summary>
		/// <seealso cref="RoutedCommand"></seealso>
		public string RoutedCommandText { 
			get; set;
		}
		
		
		/// <summary>
		/// Name of the routed command which will be invoked when this binding is triggered
		/// </summary>
		public virtual string RoutedCommandName { 
			get {
				return routedCommandName;
			}
			set {
				routedCommandName = value;
			}
		}
		
		/// <summary>
		/// Routed command instance which will be invoked when this binding is triggered
		/// </summary>
		public RoutedUICommand RoutedCommand { 
			get { 
				return SDCommandManager.GetRoutedUICommand(RoutedCommandName);
			}
		}
		
		public bool IsRegistered
		{
			get; set;
		}
		
		/// <summary>
		/// Add-in to which registered this input binding
		/// </summary>
		public AddIn AddIn {
			get; set;
		}
		
		public void SetCollectionChanged<T>(IObservableCollection<T> oldObservableCollection, IObservableCollection<T> newObservableCollection, NotifyCollectionChangedEventHandler handler) 
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
		/// Updates owner bindings
		/// </summary>
		public void BindingsChangedHandler(object sender, NotifyBindingsChangedEventArgs args)
		{
			if(!IsRegistered || RoutedCommand == null || (OwnerTypes == null && OwnerInstances == null)) {
				return;
			}
			
			if( (args.Action == NotifyBindingsChangedAction.BindingInfoModified && args.ModifiedBindingInfoTemplates.Contains(BindingInfoTemplate.CreateFromIBindingInfo(this)))
			 || (args.Action == NotifyBindingsChangedAction.NamedInstanceModified && OwnerInstanceName == args.UIElementName)
			 || (args.Action == NotifyBindingsChangedAction.RoutedUICommandModified && routedCommandName == args.RoutedCommandName)
			 || (args.Action == NotifyBindingsChangedAction.NamedTypeModified && OwnerTypeName == args.TypeName)
			 || (args.Action == NotifyBindingsChangedAction.GroupAttachmendsModified && ((OwnerTypeName != null && OwnerTypes.Any(t1 => args.AttachedInstances.Any(t2 => t1 == t2.GetType())))
			 	 || (OwnerInstanceName != null && OwnerInstances.Any(t1 => args.AttachedInstances.Any(t2 => t1 == t2)))))
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
		
		public void RemoveActiveBindings()
		{
			PopulateOwnerInstancesWithBindings(null);
			PopulateOwnerTypesWithBindings(null);
		}
		
		protected abstract void GenerateBindings();
		
		protected abstract void PopulateOwnerInstancesWithBindings(ICollection<UIElement> newInstances);
		
		protected abstract void PopulateOwnerTypesWithBindings(ICollection<Type> newtTypes);
    }
}
