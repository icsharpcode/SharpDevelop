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
    	private Dictionary<IBindingInfoTemplate, HashSet<T>> dictionary = new Dictionary<IBindingInfoTemplate, HashSet<T>>(new IBindingInfoTemplateEqualityComparer());
		                              
		public void Add(IBindingInfoTemplate template, T item)
		{
			if(!dictionary.ContainsKey(template)) {
				dictionary.Add(template, new HashSet<T>());
			}
			
			dictionary[template].Add(item);
		}
		
		public HashSet<T> FindItems(IBindingInfoTemplate template, BindingInfoMatchType matchType) 
		{
			var allItems = new HashSet<T>();
			
			foreach(var bucket in FindBuckets(template, matchType)) {
				foreach(var item in bucket) {
					allItems.Add(item);
				}
			}
			
			return allItems;
		}
		
		public IEnumerable<HashSet<T>> FindBuckets(IBindingInfoTemplate template, BindingInfoMatchType matchType) 
		{
			foreach(var pair in dictionary) {
				if(template.IsTemplateFor(pair.Key, matchType)) {
					yield return pair.Value;
				}
			}
		}
		
		public void Remove(T item) 
		{
			foreach(var pair in dictionary) {
				pair.Value.Remove(item);
			}
		}
		
		public void Remove(IBindingInfoTemplate template, BindingInfoMatchType matchType, T item) 
		{
			foreach(var bucket in FindBuckets(template, matchType)) {
				bucket.Remove(item);
			}
		}

	    public void Clear() 
	    {
	    	dictionary.Clear();
	    }
	    
    }
}
