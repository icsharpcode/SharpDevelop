using System;
using System.Linq;
using System.Windows;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Description of AtomicBindingInfoTemplateDictionary.
    /// </summary>
    public class BindingInfoTemplateDictionary<T>
    {
		static List<PropertyInfo> properties;
		static PropertyInfo OwnerInstanceNameProperty;
		static PropertyInfo OwnerTypeNameProperty;
		static PropertyInfo RoutedCommandNameProperty;

		static BindingInfoTemplateDictionary()
		{
			var t = typeof(BindingInfoTemplate);
			properties = new List<PropertyInfo>(4);
			properties.Add(OwnerInstanceNameProperty = t.GetProperty("OwnerInstanceName"));
			properties.Add(OwnerTypeNameProperty = t.GetProperty("OwnerTypeName"));
			properties.Add(RoutedCommandNameProperty = t.GetProperty("RoutedCommandName"));
		}
		
    	private Dictionary<BindingInfoTemplate, HashSet<T>> dictionary = new Dictionary<BindingInfoTemplate, HashSet<T>>();
		                              
		public void Add(BindingInfoTemplate template, T item)
		{
			foreach(var wildCardTemplate in GetWildCardTemplates(template)) {
				if(!dictionary.ContainsKey(wildCardTemplate)) {
					dictionary.Add(wildCardTemplate, new HashSet<T>());
				}
				
				dictionary[wildCardTemplate].Add(item);
			}
		}
		
		public ICollection<T> FindItems(BindingInfoTemplate template) 
		{
			var bucket = FindBucket(template);
			if(bucket != null) {
				var items = new T[bucket.Count];
				bucket.CopyTo(items, 0);
				return items;
			}
			
			return null;
		}
		
		public HashSet<T> FindBucket(BindingInfoTemplate template) 
		{				
			HashSet<T> bucket;
			dictionary.TryGetValue(template, out bucket);
			if(bucket == null) {
				bucket = new HashSet<T>();
			}
			
			return bucket;
		}
		
		public void Remove(T item) 
		{
			foreach(var pair in dictionary) {
				pair.Value.Remove(item);
			}
		}
		
		public void Remove(BindingInfoTemplate template, T item) 
		{
			var bucket = FindBucket(template);
			if(bucket != null) {
				bucket.Remove(item);
			}
		}

	    public void Clear() 
	    {
	    	dictionary.Clear();
	    }
	    
		private List<BindingInfoTemplate> GetWildCardTemplates(BindingInfoTemplate template)
		{
			var notNullProperties = GetNotNullProperties(template);
			var notNullPropertiesCopy = new List<PropertyInfo>(notNullProperties);
			var generatedTemplates = new List<BindingInfoTemplate>((int)Math.Pow(2, notNullProperties.Count));
			generatedTemplates.Add(template);
			
			GetWildCardTemplatesRecursive(notNullPropertiesCopy, template, generatedTemplates);
			
			return generatedTemplates;
		}
		
		private void GetWildCardTemplatesRecursive(List<PropertyInfo> notNullPropertiesCollection, BindingInfoTemplate rootTemplate, List<BindingInfoTemplate> generatedTemplates)
		{
			foreach(var property in notNullPropertiesCollection) {
				var nestedNotNullPropertiesCollection = new List<PropertyInfo>(notNullPropertiesCollection);
				nestedNotNullPropertiesCollection.Remove(property);
				
				var template = new BindingInfoTemplate();
				
				if(property == OwnerInstanceNameProperty) {
					template.OwnerInstanceName = null;
				} else {
					template.OwnerInstanceName = rootTemplate.OwnerInstanceName;
				}
				
				if(property == OwnerTypeNameProperty) {
					template.OwnerTypeName = null;
				} else {
					template.OwnerTypeName = rootTemplate.OwnerTypeName;
				}
				
				if(property == RoutedCommandNameProperty) {
					template.RoutedCommandName = null;
				} else {
					template.RoutedCommandName = rootTemplate.RoutedCommandName;
				}
				
				generatedTemplates.Add(template);
				
				GetWildCardTemplatesRecursive(nestedNotNullPropertiesCollection, template, generatedTemplates);
				GetWildCardTemplatesRecursive(nestedNotNullPropertiesCollection, rootTemplate, generatedTemplates);
			}
		}
		
		private List<PropertyInfo> GetNotNullProperties(BindingInfoTemplate template)
		{
			var notNullProperties = new List<PropertyInfo>();
			foreach(var property in properties) {
				if(property.GetValue(template, null) != null) {
					notNullProperties.Add(property);
				}
			}
			
			return notNullProperties;
		}
    }
}
