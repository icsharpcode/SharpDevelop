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
    	public static bool IsTemplateFor(this IBindingInfo template, IBindingInfo binding, BindingInfoMatchType matchType)
    	{
			var bindingOwnerInstanceName = binding.OwnerInstanceName;
    		var bindingOwnerInstances = binding.OwnerInstances;
    		var bindingOwnerTypeName = binding.OwnerTypeName;
    		var bindingOwnerTypes = binding.OwnerTypes;
    		var bindingRoutedCommandName = binding.RoutedCommandName;
    		var bindingRoutedCommand = binding.RoutedCommand;
    		var bindingGroups = binding.Groups;
    			
    		var templateOwnerInstanceName = template.OwnerInstanceName;
    		var templateOwnerInstances = template.OwnerInstances;
    		var templateOwnerTypeName = template.OwnerTypeName;
    		var templateOwnerTypes = template.OwnerTypes;
    		var templateRoutedCommandName = template.RoutedCommandName;
    		var templateRoutedCommand = template.RoutedCommand;
    		var templateGroups = template.Groups;
    		
    		var instancesOverlap = templateOwnerInstances != null && bindingOwnerInstances != null && templateOwnerInstances.ContainsAnyFromCollection(bindingOwnerInstances);
    		var typesOverlap = templateOwnerTypes != null && bindingOwnerTypes != null && templateOwnerTypes.ContainsAnyFromCollection(bindingOwnerTypes);
    		var groupsOverlap = templateGroups != null && bindingGroups != null && templateGroups.ContainsAnyFromCollection(bindingGroups);
			
    		var superSetMatch = false;
    		if((matchType & BindingInfoMatchType.SuperSet) == BindingInfoMatchType.SuperSet) {
	    		superSetMatch = (templateOwnerInstanceName == null || templateOwnerInstanceName == bindingOwnerInstanceName)
					&& (templateOwnerInstances == null || instancesOverlap)
					&& (templateOwnerTypeName == null || templateOwnerTypeName == bindingOwnerTypeName)
					&& (templateOwnerTypes == null || typesOverlap)		
					&& (templateRoutedCommandName == null || templateRoutedCommandName == bindingRoutedCommandName)
					&& (templateRoutedCommand == null || templateRoutedCommand == bindingRoutedCommand)
	    			&& (templateGroups == null || groupsOverlap);
    		}
    		
    		var subSetMatch = false;
    		if((matchType & BindingInfoMatchType.SubSet) == BindingInfoMatchType.SubSet) {
    			subSetMatch = (templateOwnerInstanceName != null && templateOwnerInstanceName == bindingOwnerInstanceName)
					|| (templateOwnerInstances != null && instancesOverlap)
					|| (templateOwnerTypeName != null && templateOwnerTypeName == bindingOwnerTypeName)
					|| (templateOwnerTypes != null && typesOverlap)		
					|| (templateRoutedCommandName != null && templateRoutedCommandName == bindingRoutedCommandName)
					|| (templateRoutedCommand != null && templateRoutedCommand == bindingRoutedCommand)
	    			|| (templateGroups != null && groupsOverlap);
    		}
    		
    		return subSetMatch || superSetMatch;
    	}
    }
    
    [FlagsAttribute]
    public enum BindingInfoMatchType
    {
    	SubSet = 1,
    	SuperSet = 2,
    }
}
