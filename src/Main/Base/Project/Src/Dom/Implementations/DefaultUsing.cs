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
				if (projectContent.Language.NameComparer.Equals(partitialNamespaceName, aliasString))
					return entry.Value;
				if (partitialNamespaceName.Length > aliasString.Length) {
					if (projectContent.Language.NameComparer.Equals(partitialNamespaceName.Substring(0, aliasString.Length + 1), aliasString + ".")) {
						string nsName = nsName = String.Concat(entry.Value, partitialNamespaceName.Remove(0, aliasString.Length));
						if (projectContent.NamespaceExists(nsName)) {
							return nsName;
						}
					}
				}
			}
			if (projectContent.Language.ImportNamespaces) {
				foreach (string str in usings) {
					string possibleNamespace = String.Concat(str, ".", partitialNamespaceName);
					if (projectContent.NamespaceExists(possibleNamespace))
						return possibleNamespace;
				}
			}
			return null;
		}
		
		public IClass SearchType(string partitialTypeName)
		{
			foreach (KeyValuePair<string, string> entry in aliases) {
				string aliasString = entry.Key;
				if (partitialTypeName.Length > aliasString.Length) {
					if (projectContent.Language.NameComparer.Equals(partitialTypeName.Substring(0, aliasString.Length + 1), aliasString + ".")) {
						string className = String.Concat(entry.Value, partitialTypeName.Remove(0, aliasString.Length));
						IClass c = projectContent.GetClass(className);
						if (c != null) {
							return c;
						}
					}
				}
			}
			if (projectContent.Language.ImportNamespaces) {
				foreach (string str in usings) {
					IClass c = projectContent.GetClass(str + "." + partitialTypeName);
					if (c != null) {
						return c;
					}
				}
			} else {
				int pos = partitialTypeName.IndexOf('.');
				string className, subClassName;
				if (pos < 0) {
					className = partitialTypeName;
					subClassName = null;
				} else {
					className = partitialTypeName.Substring(0, pos);
					subClassName = partitialTypeName.Substring(pos + 1);
				}
				foreach (string str in usings) {
					IClass c = projectContent.GetClass(str + "." + className);
					if (c != null) {
						c = projectContent.GetClass(str + "." + partitialTypeName);
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
			StringBuilder builder = new StringBuilder("[DefaultUsing: ");
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
