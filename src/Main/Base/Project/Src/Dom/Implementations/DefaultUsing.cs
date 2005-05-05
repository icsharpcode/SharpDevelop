// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class DefaultUsing : MarshalByRefObject, IUsing
	{
		IRegion region;
		IProjectContent projectContent;
		
		public DefaultUsing(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
		}
		
		public DefaultUsing(IProjectContent projectContent, IRegion region) : this(projectContent)
		{
			this.region = region;
		}
		
		List<string> usings  = new List<string>();
		SortedList<string, string> aliases = new SortedList<string, string>();
		
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
		
		public SortedList<string, string> Aliases {
			get {
				return aliases;
			}
		}
		
		public string SearchNamespace(string partitialNamespaceName)
		{
			foreach (KeyValuePair<string, string> entry in aliases) {
				string aliasString = entry.Key;
				// TODO: case insensitive: partitialNamespaceName.ToLower().StartsWith(aliasString.ToLower())
				if (partitialNamespaceName.StartsWith(aliasString)) {
					if (aliasString.Length >= 0) {
						string nsName = nsName = String.Concat(entry.Value, partitialNamespaceName.Remove(0, aliasString.Length));
						if (projectContent.NamespaceExists(nsName)) {
							return nsName;
						}
					}
				}
			}
			return null;
		}
		
		public IClass SearchType(string partitialTypeName)
		{
			foreach (string str in usings) {
				string possibleType = String.Concat(str, ".", partitialTypeName);
				IClass c = projectContent.GetClass(possibleType);
				if (c != null) {
					return c;
				}
			}
			
			foreach (KeyValuePair<string, string> entry in aliases) {
				string aliasString = entry.Key;
				// TODO: case insensitive:  : partitialTypeName.ToLower().StartsWith(aliasString.ToLower())
				if (partitialTypeName.StartsWith(aliasString)) {
					string className = null;
					if (aliasString.Length > 0) {
						className = String.Concat(entry.Value, partitialTypeName.Remove(0, aliasString.Length));
						IClass c = projectContent.GetClass(className);
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
			StringBuilder builder = new StringBuilder("[AbstractUsing: ");
			foreach (string str in usings) {
				builder.Append(str);
				builder.Append(", ");
			}
			foreach (KeyValuePair<string, string> p in aliases) {
				builder.Append(p.Key);
				builder.Append("=");
				builder.Append(p.Value);
				builder.Append(", ");
			}
			builder.Length -= 2; // remove last ", "
			builder.Append("]");
			return builder.ToString();
		}
	}
}
