// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public abstract class AbstractUsing : MarshalByRefObject, IUsing
	{
		protected IRegion region;
		
		List<string> usings  = new List<string>();
		SortedList       aliases = new SortedList();
		
		public IRegion Region {
			get {
				return region;
			}
		}
		
		public List<string> Usings {
			get {
				return usings;
			}
		}
		
		public SortedList Aliases {
			get {
				return aliases;
			}
		}
		
		public string SearchNamespace(string partitialNamespaceName)
		{
			if (ParserService.CurrentProjectContent.NamespaceExists(partitialNamespaceName)) {
				return partitialNamespaceName;
			}
			
			// search for partitial namespaces
			string declaringNamespace = (string)aliases[""];
			if (declaringNamespace != null) {
				while (declaringNamespace.Length > 0) {
					// TODO: case insensitive : : declaringNamespace.ToLower().EndsWith(partitialNamespaceName.ToLower()) ) && ParserService.CurrentProjectContent.NamespaceExists(declaringNamespace, caseSensitive)
					if (declaringNamespace.EndsWith(partitialNamespaceName)) {
						return declaringNamespace;
					}
					int index = declaringNamespace.IndexOf('.');
					if (index > 0) {
						declaringNamespace = declaringNamespace.Substring(0, index);
					} else {
						break;
					}
				}
			}
			
			// Remember:
			//     Each namespace has an own using object
			//     The namespace name is an alias which has the key ""
			foreach (DictionaryEntry entry in aliases) {
				string aliasString = entry.Key.ToString();
				// TODO: case insensitive: partitialNamespaceName.ToLower().StartsWith(aliasString.ToLower())
				if (partitialNamespaceName.StartsWith(aliasString)) {
					if (aliasString.Length >= 0) {
						string nsName = nsName = String.Concat(entry.Value.ToString(), partitialNamespaceName.Remove(0, aliasString.Length));
						if (ParserService.CurrentProjectContent.NamespaceExists(nsName)) {
							return nsName;
						}
					}
				}
			}
			return null;
		}

		public IClass SearchType(string partitialTypeName)
		{
			IClass c = ParserService.CurrentProjectContent.GetClass(partitialTypeName);
			if (c != null) {
				return c;
			}
			
			foreach (string str in usings) {
				string possibleType = String.Concat(str, ".", partitialTypeName);
				c = ParserService.CurrentProjectContent.GetClass(possibleType);
				if (c != null) {
					return c;
				}
			}
			
			// search class in partitial namespaces
			string declaringNamespace = (string)aliases[""];
			if (declaringNamespace != null) {
				while (declaringNamespace.Length > 0) {
					string className = String.Concat(declaringNamespace, ".", partitialTypeName);
					c = ParserService.CurrentProjectContent.GetClass(className);
					if (c != null) {
						return c;
					}
					int index = declaringNamespace.IndexOf('.');
					if (index > 0) {
						declaringNamespace = declaringNamespace.Substring(0, index);
					} else {
						break;
					}
				}
			}
			
			foreach (DictionaryEntry entry in aliases) {
				string aliasString = entry.Key.ToString();
				// TODO: case insensitive:  : partitialTypeName.ToLower().StartsWith(aliasString.ToLower())
				if (partitialTypeName.StartsWith(aliasString)) {
					string className = null;
					if (aliasString.Length > 0) {
						className = String.Concat(entry.Value.ToString(), partitialTypeName.Remove(0, aliasString.Length));
						c = ParserService.CurrentProjectContent.GetClass(className);
						if (c != null) {
							return c;
						}
					}
				}
			}
			return null;
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("[AbstractUsing: using list=");
			foreach (string str in usings) {
				builder.Append(str);
				builder.Append(", ");
			}
			builder.Append("]");
			return builder.ToString();
		}
	}
}
