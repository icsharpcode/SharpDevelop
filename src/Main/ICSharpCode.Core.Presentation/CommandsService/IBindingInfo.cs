using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Reflection;

namespace ICSharpCode.Core.Presentation
{
    public interface IBindingInfo
    {
        string OwnerInstanceName
        {
        	get; set;
        }
        
        ICollection<UIElement> OwnerInstances
        {
        	get;
        }
        
        string OwnerTypeName
        {
        	get; set;
        }
        
        ICollection<Type> OwnerTypes
        {
        	get;
        }
        
        string RoutedCommandName
        {
        	get;
        }
        
        RoutedUICommand RoutedCommand
        {
        	get;
        }
        
        BindingGroupCollection Groups 
        {
        	get;
        }
        
        void BindingsChangedHandler(object sender, NotifyBindingsChangedEventArgs args);
        
        void RemoveActiveBindings();
    }
}
