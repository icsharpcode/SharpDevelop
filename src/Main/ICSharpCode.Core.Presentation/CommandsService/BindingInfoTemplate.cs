using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Reflection;

namespace ICSharpCode.Core.Presentation
{
	public class BindingInfoTemplate : IBindingInfoTemplate
	{
		public string OwnerInstanceName
		{
			get; set;
		}
		
		public string OwnerTypeName
		{
			get; set;
		}
		
		public string RoutedCommandName
		{
			get; set;
		}
		
		public BindingGroupCollection Groups
		{
			get; set;
		}
		
		public BindingInfoTemplate()
		{
			
		}
		
		
		public BindingInfoTemplate(InputBindingIdentifier identifier)
		{
			OwnerInstanceName = identifier.OwnerInstanceName;
			OwnerTypeName = identifier.OwnerTypeName;
			RoutedCommandName = identifier.RoutedCommandName;
		}
		
		public BindingInfoTemplate(IBindingInfoTemplate template, bool includeGroup)
		{
			OwnerInstanceName = template.OwnerInstanceName;
			OwnerTypeName = template.OwnerTypeName;
			RoutedCommandName = template.RoutedCommandName;
			if(includeGroup) {
				Groups = template.Groups;
			}
		}
	}
	
	public interface IBindingInfoTemplate
	{
		string OwnerInstanceName
		{
			get; set;
		}
		
		string OwnerTypeName
		{
			get; set;
		}
		
		string RoutedCommandName
		{
			get; set;
		}
		
		BindingGroupCollection Groups
		{
			get; set;
		}
        
//		public List<BindingInfoTemplate> GetWildCardTemplates()
//		{
//			var notNullProperties = GetNotNullProperties();
//			var notNullPropertiesCopy = new List<PropertyInfo>(notNullProperties);
//			var generatedTemplates = new List<BindingInfoTemplate>((int)Math.Pow(2, notNullProperties.Count));
//			generatedTemplates.Add(this);
//			
//			GetWildCardTemplatesRecursive(notNullPropertiesCopy, this, generatedTemplates);
//			
//			return generatedTemplates;
//		}
//		
//    	static List<PropertyInfo> properties;
//    	
//		static PropertyInfo OwnerInstanceNameProperty;
//		static PropertyInfo OwnerTypeNameProperty;
//		static PropertyInfo RoutedCommandNameProperty;
//		static PropertyInfo GroupProperty;
//
//		static BindingInfoTemplate()
//		{
//			var t = typeof(BindingInfoTemplate);
//			properties = new List<PropertyInfo>(4);
//			properties.Add(OwnerInstanceNameProperty = t.GetProperty("OwnerInstanceName"));
//			properties.Add(OwnerTypeNameProperty = t.GetProperty("OwnerTypeName"));
//			properties.Add(RoutedCommandNameProperty = t.GetProperty("RoutedCommandName"));
//			properties.Add(GroupProperty = t.GetProperty("Group"));
//		} 
//		
//		private void GetWildCardTemplatesRecursive(List<PropertyInfo> notNullPropertiesCollection, BindingInfoTemplate rootTemplate, List<BindingInfoTemplate> generatedTemplates)
//		{
//			foreach(var property in notNullPropertiesCollection) {
//				var nestedNotNullPropertiesCollection = new List<PropertyInfo>(notNullPropertiesCollection);
//				nestedNotNullPropertiesCollection.Remove(property);
//				
//				var template = new BindingInfoTemplate();
//		
//				template.OwnerInstanceName = rootTemplate.OwnerInstanceName;
//				template.OwnerTypeName = rootTemplate.OwnerTypeName;
//				template.RoutedCommandName = rootTemplate.RoutedCommandName;
//				template.Group = rootTemplate.Group;
//				
//				if(property == OwnerInstanceNameProperty) {
//					template.OwnerInstanceName = null;
//				} else if(property == OwnerTypeNameProperty) {
//					template.OwnerTypeName = null;
//				} else if(property == RoutedCommandNameProperty) {
//					template.RoutedCommandName = null;
//				} else if(property == GroupProperty) {
//					template.Group = null;
//				}
//				
//				generatedTemplates.Add(template);
//				
//				GetWildCardTemplatesRecursive(nestedNotNullPropertiesCollection, template, generatedTemplates);
//				GetWildCardTemplatesRecursive(nestedNotNullPropertiesCollection, rootTemplate, generatedTemplates);
//			}
//		}
//				
//		public List<PropertyInfo> GetNotNullProperties()
//		{
//			var notNullProperties = new List<PropertyInfo>();
//			foreach(var property in properties) {
//				if(property.GetValue(this, null) != null) {
//					notNullProperties.Add(property);
//				}
//			}
//			
//			return notNullProperties;
//		}
    }
	
	public static class IBindingInfoTemplateExtensions
	{
    	public static bool IsTemplateFor(this IBindingInfoTemplate thisTemplate, IBindingInfoTemplate binding, BindingInfoMatchType matchType)
    	{
			var bindingOwnerInstanceName = binding.OwnerInstanceName;
    		var bindingOwnerTypeName = binding.OwnerTypeName;
    		var bindingRoutedCommandName = binding.RoutedCommandName;
    		var bindingGroups = binding.Groups;
    			
    		var templateOwnerInstanceName = thisTemplate.OwnerInstanceName;
    		var templateOwnerTypeName = thisTemplate.OwnerTypeName;
    		var templateRoutedCommandName = thisTemplate.RoutedCommandName;
    		var templateGroups = thisTemplate.Groups;
    		
    		if(bindingOwnerTypeName == "ICSharpCode.AvalonEdit.Editing.TextArea, ICSharpCode.AvalonEdit") 
    		{
    		
    		}
    		
    		var groupsOverlap = templateGroups != null && bindingGroups != null && bindingGroups.ContainsAnyFromCollection(templateGroups);
    		
    		var exactMatch = false;
    		if((matchType & BindingInfoMatchType.Exact) == BindingInfoMatchType.Exact) {
	    		exactMatch = templateOwnerInstanceName == bindingOwnerInstanceName
					&& templateOwnerTypeName == bindingOwnerTypeName
					&& templateRoutedCommandName == bindingRoutedCommandName
	    			&& ((templateGroups == null && bindingGroups == null) || groupsOverlap);
    		}
    		
    		var superSetMatch = false;
    		if((matchType & BindingInfoMatchType.SuperSet) == BindingInfoMatchType.SuperSet) {
	    		superSetMatch = (templateOwnerInstanceName == null || templateOwnerInstanceName == bindingOwnerInstanceName)
					&& (templateOwnerTypeName == null || templateOwnerTypeName == bindingOwnerTypeName)
					&& (templateRoutedCommandName == null || templateRoutedCommandName == bindingRoutedCommandName)
					&& (templateGroups == null || templateGroups.Count == 0 || groupsOverlap);
    		}
    		
    		var subSetMatch = false;
    		if((matchType & BindingInfoMatchType.SubSet) == BindingInfoMatchType.SubSet) {
    			subSetMatch = (bindingOwnerInstanceName == null || templateOwnerInstanceName == bindingOwnerInstanceName)
					&& (bindingOwnerTypeName == null || templateOwnerTypeName == bindingOwnerTypeName)
					&& (bindingRoutedCommandName == null || templateRoutedCommandName == bindingRoutedCommandName)
    				&& (bindingGroups == null || bindingGroups.Count == 0 || groupsOverlap);
    		}
    		
    		return subSetMatch || superSetMatch;
    	}
	}
}
