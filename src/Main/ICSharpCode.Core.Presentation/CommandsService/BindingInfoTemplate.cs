using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Reflection;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of BindingInfoTemplate.
	/// </summary>
	public struct BindingInfoTemplate
	{
    	static List<PropertyInfo> properties;
    	
		static PropertyInfo OwnerInstanceNameProperty;
		static PropertyInfo OwnerTypeNameProperty;
		static PropertyInfo RoutedCommandNameProperty;
		static PropertyInfo GroupProperty;

		static BindingInfoTemplate()
		{
			var t = typeof(BindingInfoTemplate);
			properties = new List<PropertyInfo>(4);
			properties.Add(OwnerInstanceNameProperty = t.GetProperty("OwnerInstanceName"));
			properties.Add(OwnerTypeNameProperty = t.GetProperty("OwnerTypeName"));
			properties.Add(RoutedCommandNameProperty = t.GetProperty("RoutedCommandName"));
			properties.Add(GroupProperty = t.GetProperty("Group"));
		} 
		
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
		
		public BindingGroup Group
		{
			get; set;
		}
        
		public List<BindingInfoTemplate> GetWildCardTemplates()
		{
			var notNullProperties = GetNotNullProperties();
			var notNullPropertiesCopy = new List<PropertyInfo>(notNullProperties);
			var generatedTemplates = new List<BindingInfoTemplate>((int)Math.Pow(2, notNullProperties.Count));
			generatedTemplates.Add(this);
			
			GetWildCardTemplatesRecursive(notNullPropertiesCopy, this, generatedTemplates);
			
			return generatedTemplates;
		}
		
		private void GetWildCardTemplatesRecursive(List<PropertyInfo> notNullPropertiesCollection, BindingInfoTemplate rootTemplate, List<BindingInfoTemplate> generatedTemplates)
		{
			foreach(var property in notNullPropertiesCollection) {
				var nestedNotNullPropertiesCollection = new List<PropertyInfo>(notNullPropertiesCollection);
				nestedNotNullPropertiesCollection.Remove(property);
				
				var template = new BindingInfoTemplate();
		
				template.OwnerInstanceName = rootTemplate.OwnerInstanceName;
				template.OwnerTypeName = rootTemplate.OwnerTypeName;
				template.RoutedCommandName = rootTemplate.RoutedCommandName;
				template.Group = rootTemplate.Group;
				
				if(property == OwnerInstanceNameProperty) {
					template.OwnerInstanceName = null;
				} else if(property == OwnerTypeNameProperty) {
					template.OwnerTypeName = null;
				} else if(property == RoutedCommandNameProperty) {
					template.RoutedCommandName = null;
				} else if(property == GroupProperty) {
					template.Group = null;
				}
				
				generatedTemplates.Add(template);
				
				GetWildCardTemplatesRecursive(nestedNotNullPropertiesCollection, template, generatedTemplates);
				GetWildCardTemplatesRecursive(nestedNotNullPropertiesCollection, rootTemplate, generatedTemplates);
			}
		}
		
    	public bool IsTemplateFor(IBindingInfo binding, BindingInfoMatchType matchType)
    	{
			var bindingOwnerInstanceName = binding.OwnerInstanceName;
    		var bindingOwnerTypeName = binding.OwnerTypeName;
    		var bindingRoutedCommandName = binding.RoutedCommandName;
    		var bindingGroups = binding.Groups;
    			
    		var templateOwnerInstanceName = this.OwnerInstanceName;
    		var templateOwnerTypeName = this.OwnerTypeName;
    		var templateRoutedCommandName = this.RoutedCommandName;
    		var templateGroup = this.Group;
    		
    		var groupsOverlap = templateGroup != null && bindingGroups != null && bindingGroups.Contains(templateGroup);
			
    		var superSetMatch = false;
    		if((matchType & BindingInfoMatchType.SuperSet) == BindingInfoMatchType.SuperSet) {
	    		superSetMatch = (templateOwnerInstanceName == null || templateOwnerInstanceName == bindingOwnerInstanceName)
					&& (templateOwnerTypeName == null || templateOwnerTypeName == bindingOwnerTypeName)
					&& (templateRoutedCommandName == null || templateRoutedCommandName == bindingRoutedCommandName)
					&& (templateGroup == null || groupsOverlap);
    		}
    		
    		var subSetMatch = false;
    		if((matchType & BindingInfoMatchType.SubSet) == BindingInfoMatchType.SubSet) {
    			subSetMatch = (templateOwnerInstanceName != null && templateOwnerInstanceName == bindingOwnerInstanceName)
					|| (templateOwnerTypeName != null && templateOwnerTypeName == bindingOwnerTypeName)
					|| (templateRoutedCommandName != null && templateRoutedCommandName == bindingRoutedCommandName)
					|| (templateGroup != null && groupsOverlap);
    		}
    		
    		return subSetMatch || superSetMatch;
    	}
		
		public List<PropertyInfo> GetNotNullProperties()
		{
			var notNullProperties = new List<PropertyInfo>();
			foreach(var property in properties) {
				if(property.GetValue(this, null) != null) {
					notNullProperties.Add(property);
				}
			}
			
			return notNullProperties;
		}
    }
}
