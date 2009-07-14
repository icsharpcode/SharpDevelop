/*
 * Created by SharpDevelop.
 * User: Sergej Andrejev
 * Date: 7/14/2009
 * Time: 2:30 PM
 */
using System;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
    public static class ExtensionMethods
    {
    	/// <summary>
    	/// Determines whether collection contains any elements from other collection
    	/// </summary>
    	/// <param name="thisCollection">This collection</param>
    	/// <param name="otherCollection">Collection of objects to locate in other collection</param>
    	/// <returns><code>true</code> if item is found; otherwise false</returns>
    	public static bool ContainsAnyFromCollection<T>(this ICollection<T> thisCollection, ICollection<T> otherCollection) 
    	{
			foreach(var thisCollectionItem in thisCollection) {
    			if(otherCollection.Contains(thisCollectionItem)) {
    				return true;
    			}
			}
			
			return false;
    	}
    }
}
