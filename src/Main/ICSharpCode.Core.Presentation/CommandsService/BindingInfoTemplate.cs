using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Description of BindingInfoTemplate.
    /// </summary>
    public struct BindingInfoTemplate : IBindingInfo
    {
        public string OwnerInstanceName
        {
        	get; set;
        }
        
        public ICollection<UIElement> OwnerInstances
        {
        	get; set;
        }
        
        public string OwnerTypeName
        {
        	get; set;
        }
        
        public ICollection<Type> OwnerTypes
        {
        	get; set;
        }
        
        public string RoutedCommandName
        {
        	get; set;
        }
        
        public RoutedUICommand RoutedCommand
        {
        	get; set;
        }
        
        public BindingGroupCollection Groups 
        {
        	get; set;
        } 
    }
}
