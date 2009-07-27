using System;
using System.Linq;
using System.Windows;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Description of AtomicBindingInfoTemplateDictionary.
    /// </summary>
    public class BindingInfoTemplateDictionary<T>
    {
		private Dictionary<BindingInfoTemplate, HashSet<T>> superSetDictionary = new Dictionary<BindingInfoTemplate, HashSet<T>>();
		private Dictionary<BindingInfoTemplate, HashSet<T>> subSetDictionary = new Dictionary<BindingInfoTemplate, HashSet<T>>();
	
		public void Add(BindingInfoTemplate template, T item)
		{
			if(!superSetDictionary.ContainsKey(template)) {
				superSetDictionary.Add(template, new HashSet<T>());
			}
			
			superSetDictionary[template].Add(item);
			
			foreach(var wildCardTemplate in template.GetWildCardTemplates()) {
				if(!subSetDictionary.ContainsKey(wildCardTemplate)) {
					subSetDictionary.Add(wildCardTemplate, new HashSet<T>());
				}
				
				subSetDictionary[wildCardTemplate].Add(item);
			}
		}
		
		public HashSet<T> FindItems(BindingInfoTemplate template, BindingInfoMatchType matchType) 
		{
			var allItems = new HashSet<T>();
			
			foreach(var bucket in FindBuckets(template, matchType)) {
				foreach(var item in bucket) {
					allItems.Add(item);
				}
			}
			
			return allItems;
		}
		
		public IEnumerable<HashSet<T>> FindBuckets(BindingInfoTemplate template, BindingInfoMatchType matchType) 
		{
			if((matchType & BindingInfoMatchType.Exact) == BindingInfoMatchType.Exact) {
				HashSet<T> items;
				superSetDictionary.TryGetValue(template, out items);
				
				if(items != null) {
					yield return items;
				}
			}
			   
			if((matchType & BindingInfoMatchType.SubSet) == BindingInfoMatchType.SubSet) {
				foreach(var wildCardTemplate in template.GetWildCardTemplates()) {
					HashSet<T> items;
					superSetDictionary.TryGetValue(wildCardTemplate, out items);
					
					if(items != null) {
						yield return items;
					}
				}
			}
			
			if((matchType & BindingInfoMatchType.SuperSet) == BindingInfoMatchType.SuperSet) {
				HashSet<T> items;
				subSetDictionary.TryGetValue(template, out items);
				
				if(items != null) {
					yield return items;
				}
			}
		}
		
		public void Remove(T item) 
		{
			foreach(var pair in subSetDictionary) {
				pair.Value.Remove(item);
			}
			
			foreach(var pair in superSetDictionary) {
				pair.Value.Remove(item);
			}
		}
		
		public void Remove(BindingInfoTemplate template, BindingInfoMatchType matchType, T item) 
		{
			foreach(var bucket in FindBuckets(template, matchType)) {
				bucket.Remove(item);
			}
		}

	    public void Clear() 
	    {
	    	superSetDictionary.Clear();
	    	subSetDictionary.Clear();
	    }
	    
    }
}
