/*
 * Created by SharpDevelop.
 * User: Sergej Andrejev
 * Date: 7/24/2009
 * Time: 3:35 PM
 */
using System;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Description of AtomicBindingInfoTemplateDictionary.
    /// </summary>
    public class BindingInfoTemplateDictionary<T>
    {
		private Dictionary<AtomicBindingInfoTemplate, HashSet<T>> superSetDictionary = new Dictionary<AtomicBindingInfoTemplate, HashSet<T>>();
	
		private Dictionary<AtomicBindingInfoTemplate, HashSet<T>> subSetDictionary = new Dictionary<AtomicBindingInfoTemplate, HashSet<T>>();
	
		public void Add(BindingInfoTemplate template, T handler)
		{
			template.Normalize();
			
			foreach(var atomicTemplate in template.GetAtomicTemplates()) {
				if(!superSetDictionary.ContainsKey(atomicTemplate)) {
					superSetDictionary.Add(atomicTemplate, new HashSet<T>());
				}
				
				superSetDictionary[atomicTemplate].Add(handler);
				
				foreach(var wildCardTemplate in atomicTemplate.GetWildCardTemplates()) {
					if(!subSetDictionary.ContainsKey(wildCardTemplate)) {
						subSetDictionary.Add(wildCardTemplate, new HashSet<T>());
					}
					
					subSetDictionary[wildCardTemplate].Add(handler);
				}
			}
		}
		
		public IEnumerable InvokeHandlers(BindingInfoTemplate template, BindingInfoMatchType matchType) 
		{
			var handlersToInvoke = new HashSet<BindingsUpdatedHandler>();
			
			template.Normalize();
			var atomicTemplates = template.GetAtomicTemplates().ToList();
			
			if((matchType & BindingInfoMatchType.SubSet) == BindingInfoMatchType.SubSet) {
				foreach(var atomicTemplate in atomicTemplates) {
					foreach(var wildCardTemplate in atomicTemplate.GetWildCardTemplates()) {
						HashSet<BindingsUpdatedHandler> handlers;
						superSetUpdatedsHandler.TryGetValue(wildCardTemplate, out handlers);
						
						if(handlers != null) {
							foreach(var handler in handlers) {
								if(handler != null) {
									handlersToInvoke.Add(handler);
								}
							}
						}
					}
				}
			}
			
			if((matchType & BindingInfoMatchType.SuperSet) == BindingInfoMatchType.SuperSet) {
				foreach(var atomicTemplate in atomicTemplates) {
					HashSet<BindingsUpdatedHandler> handlers;
					subSetUpdatedsHandler.TryGetValue(atomicTemplate, out handlers);
					
					if(handlers != null) {
						foreach(var handler in handlers) {
							if(handler != null) {
								handlersToInvoke.Add(handler);
							}
						}
					}
				}
			}
			
			foreach(var handler in handlersToInvoke) {
				handler.Invoke();
			}
		}

   
	    
	    public void Clear() 
	    {
	    	superSetUpdatedsHandler.Clear();
	    }
    }
}
