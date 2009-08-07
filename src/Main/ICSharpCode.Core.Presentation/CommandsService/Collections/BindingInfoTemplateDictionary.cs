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
	/// Holds objects of type T which can be searched by <see cref="BindingInfoTemplate" />
	/// </summary>
    public class BindingInfoTemplateDictionary<T>
    {
		static List<PropertyInfo> properties;
		static PropertyInfo OwnerInstanceNameProperty;
		static PropertyInfo OwnerTypeNameProperty;
		static PropertyInfo RoutedCommandNameProperty;

    	private Dictionary<BindingInfoTemplate, HashSet<T>> dictionary = new Dictionary<BindingInfoTemplate, HashSet<T>>();
		
		/// <summary>
		/// Creates new instance of <see cref="BindingInfoTemplateDictionary{T}" />
		/// </summary>
		static BindingInfoTemplateDictionary()
		{
			var t = typeof(BindingInfoTemplate);
			properties = new List<PropertyInfo>(4);
			properties.Add(OwnerInstanceNameProperty = t.GetProperty("OwnerInstanceName"));
			properties.Add(OwnerTypeNameProperty = t.GetProperty("OwnerTypeName"));
			properties.Add(RoutedCommandNameProperty = t.GetProperty("RoutedCommandName"));
		}
		
		/// <summary>
		/// Add new item of type T to the dictionary
		/// </summary>
		/// <param name="template">Template which can be used to search for an item</param>
		/// <param name="item">Added item</param>
		public void Add(BindingInfoTemplate template, T item)
		{
			foreach(var wildCardTemplate in GetWildCardTemplates(template)) {
				if(!dictionary.ContainsKey(wildCardTemplate)) {
					dictionary.Add(wildCardTemplate, new HashSet<T>());
				}
				
				dictionary[wildCardTemplate].Add(item);
			}
		}
		
		/// <summary>
		/// Find items using <see cref="BindingInfoTemplate" />
		/// 
		/// <code>null</code> values in the template will act as wildcards
		/// </summary>
		/// <param name="template">Template</param>
		/// <returns>Collection of found items identified by provided <see cref="BindingInfoTemplate" /></returns>
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
		
		private HashSet<T> FindBucket(BindingInfoTemplate template) 
		{				
			HashSet<T> bucket;
			dictionary.TryGetValue(template, out bucket);
			if(bucket == null) {
				bucket = new HashSet<T>();
			}
			
			return bucket;
		}
		
		/// <summary>
		/// Remove item from template
		/// </summary>
		/// <param name="item"></param>
		public void Remove(T item) 
		{
			foreach(var pair in dictionary) {
				pair.Value.Remove(item);
			}
		}

		/// <summary>
		/// Remove all registered items from dictionary
		/// </summary>
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
				
				var template = BindingInfoTemplate.Create(
					property != OwnerInstanceNameProperty ? rootTemplate.OwnerInstanceName : null, 
					property != OwnerTypeNameProperty ? rootTemplate.OwnerTypeName : null, 
					property != RoutedCommandNameProperty ? rootTemplate.RoutedCommandName : null);
				
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
