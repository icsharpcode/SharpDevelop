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
    /// Description of BindingInfoBase.
    /// </summary>
    abstract public class BindingInfoBase : IBindingInfo, IBindingInfoTemplate
    {
		private BindingGroupCollection _groups;
		
		public BindingGroupCollection Groups
		{
			get {
				return _groups;
			}
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}

				_groups = value;
			}
		}
		
		public string _ownerInstanceName;
		
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
					
		private string _ownerTypeName;
		
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
		
		private string routedCommandName;
		
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
		
		public InputBindingIdentifier Identifier {
			get {
				var identifier = new InputBindingIdentifier();
				identifier.OwnerInstanceName = OwnerInstanceName;
				identifier.OwnerTypeName = OwnerTypeName;
				identifier.RoutedCommandName = RoutedCommandName;
				
				return identifier;
			}
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
		
		private BindingsUpdatedHandler defaultCommandBindingHandler;
		
		/// <summary>
		/// Updates owner bindings
		/// </summary>
		public BindingsUpdatedHandler DefaultBindingsUpdateHandler
		{
			get {
				if(defaultCommandBindingHandler == null && OwnerTypeName != null) {
					defaultCommandBindingHandler = delegate(object sender, BindingsUpdatedHandlerArgs args) {
						var ownerTypes = OwnerTypes;
						
						if(RoutedCommand != null && OwnerTypes != null && IsRegistered) {
							GenerateBindings();
							
							if(Groups.Count == 0) {
								var groupInstances = Groups.GetAttachedInstances(ownerTypes);
								SetInstanceBindings(groupInstances, null);
								
								var removedOwnerTypes = new List<Type>(ownerTypes);
								if(args.RemovedTypes != null) {
									removedOwnerTypes.AddRange(args.RemovedTypes);
								}
								
								SetClassBindings(removedOwnerTypes, ownerTypes);
							} else {
								SetClassBindings(ownerTypes, null);
								
								var groupInstances = Groups.GetAttachedInstances(ownerTypes);
								var removedOwnerInstances = new List<UIElement>(groupInstances);
								if(args.RemovedInstances != null) {
									removedOwnerInstances.AddRange(args.RemovedInstances);
								}
										
								SetInstanceBindings(removedOwnerInstances, groupInstances);
							}
						}
					};
				} else if(defaultCommandBindingHandler == null && OwnerInstanceName != null) {
		 			defaultCommandBindingHandler = delegate(object sender, BindingsUpdatedHandlerArgs args) {
						if(RoutedCommand != null && OwnerInstances != null && IsRegistered) {
							GenerateBindings();
							
							if(Groups.Count == 0) {
								SetInstanceBindings(OwnerInstances, OwnerInstances);
							} else {
								var removedInstances = new List<UIElement>(OwnerInstances);
								if(args.RemovedInstances != null) {
									removedInstances.AddRange(args.RemovedInstances);
								}
								
								SetInstanceBindings(removedInstances, Groups.GetAttachedInstances(OwnerInstances));
							}
						}
					};
				}
				
				return defaultCommandBindingHandler;
			}
		}
		
		public void RemoveActiveBindings()
		{
			if(OwnerTypeName != null) {
				if(Groups.Count > 0) {
					SetInstanceBindings(Groups.GetAttachedInstances(OwnerTypes), null);
				}
				
				SetClassBindings(OwnerTypes, null);
			} else if(OwnerInstanceName != null) {
				SetInstanceBindings(OwnerInstances, null);
			}
		}
		
		protected abstract void GenerateBindings();
		
		protected abstract void SetInstanceBindings(ICollection<UIElement> oldInstances, ICollection<UIElement> newInstances);
		
		protected abstract void SetClassBindings(ICollection<Type> oldTypes, ICollection<Type> newtTypes);
    }
}
