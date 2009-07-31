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
        
        BindingsUpdatedHandler DefaultBindingsUpdateHandler
        {
        	get;
        }
        
        void RemoveActiveBindings();
    }
    
    internal static class IBindingInfoExtensions
    {
//    	public static IBindingInfoTemplate[] GenerateTemplates(this IBindingInfo thisObject, bool includeGroup)
//    	{
//    		var invokeTemplates = new BindingInfoTemplate[thisObject.Groups.Count == 0 || !includeGroup ? 1 : thisObject.Groups.Count];
//    		var groupEnumerator = thisObject.Groups.Count == 0 ? null : thisObject.Groups.GetEnumerator();
//			var groupCounter = 0;
//			while(groupEnumerator == null || groupEnumerator.MoveNext()) {
//				var invokeTemplate = new BindingInfoTemplate();
//				invokeTemplate.RoutedCommandName = thisObject.RoutedCommandName;
//				invokeTemplate.OwnerInstanceName = thisObject.OwnerInstanceName;
//				invokeTemplate.OwnerTypeName = thisObject.OwnerTypeName;
//				if(includeGroup && groupEnumerator != null) {
//					invokeTemplate.Group = groupEnumerator.Current;
//				}
//				
//				invokeTemplates[groupCounter++] = invokeTemplate;
//				
//				if(groupEnumerator == null || !includeGroup) {
//					break;
//				}
//			}
//			
//			return invokeTemplates;
//    	}
    }
    
    [FlagsAttribute]
    public enum BindingInfoMatchType
    {
    	SubSet = 1,
    	SuperSet = 2,
    	Exact
    }
}
