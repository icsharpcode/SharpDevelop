/*
 * Created by SharpDevelop.
 * User: Sergej Andrejev
 * Date: 7/17/2009
 * Time: 10:55 AM
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

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
    }
    
    
    public static class IBindingInfoExtensions
    {
    	public static bool IsTemplateFor(this IBindingInfo template, IBindingInfo binding)
    	{
			return (template.OwnerInstanceName == null || template.OwnerInstanceName == binding.OwnerInstanceName)
				&& (template.OwnerInstances == null || (binding.OwnerInstances != null && template.OwnerInstances.ContainsAnyFromCollection(binding.OwnerInstances)))
				&& (template.OwnerTypeName == null || template.OwnerTypeName == binding.OwnerTypeName)
				&& (template.OwnerTypes == null || (binding.OwnerTypes != null && template.OwnerTypes.ContainsAnyFromCollection(binding.OwnerTypes)))		
				&& (template.RoutedCommandName == null || template.RoutedCommandName == binding.RoutedCommandName)
				&& (template.RoutedCommand == null || template.RoutedCommand == binding.RoutedCommand);
    	}
    }
}
