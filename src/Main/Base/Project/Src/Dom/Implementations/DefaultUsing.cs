// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
		DomRegion region;
		IProjectContent projectContent;
		
		public DefaultUsing(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
		}
		
		public DefaultUsing(IProjectContent projectContent, DomRegion region) : this(projectContent)
		{
			this.region = region;
		}
		
		List<string> usings  = new List<string>();
		SortedList<string, IReturnType> aliases = null;
		
		public DomRegion Region {
			get {
				return region;
			}
		}
		
		public List<string> Usings {
			get {
				return usings;
			}
		}
		
		public SortedList<string, IReturnType> Aliases {
			get {
				return aliases;
			}
		}
		
		public bool HasAliases {
			get {
				return aliases != null && aliases.Count > 0;
			}
		}
		
		public void AddAlias(string alias, IReturnType type)
		{
			if (aliases == null) aliases = new SortedList<string, IReturnType>();
			aliases.Add(alias, type);
		}
		
		public string SearchNamespace(string partitialNamespaceName)
		{
			if (HasAliases) {
				foreach (KeyValuePair<string, IReturnType> entry in aliases) {
					if (!entry.Value.IsDefaultReturnType)
						continue;
					string aliasString = entry.Key;
					string nsName;
					if (projectContent.Language.NameComparer.Equals(partitialNamespaceName, aliasString)) {
						nsName = entry.Value.FullyQualifiedName;
						if (projectContent.NamespaceExists(nsName))
							return nsName;
					}
					if (partitialNamespaceName.Length > aliasString.Length) {
						if (projectContent.Language.NameComparer.Equals(partitialNamespaceName.Substring(0, aliasString.Length + 1), aliasString + ".")) {
							nsName = String.Concat(entry.Value.FullyQualifiedName, partitialNamespaceName.Remove(0, aliasString.Length));
							if (projectContent.NamespaceExists(nsName)) {
								return nsName;
							}
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
		
		public IReturnType SearchType(string partitialTypeName, int typeParameterCount)
		{
			if (HasAliases) {
				foreach (KeyValuePair<string, IReturnType> entry in aliases) {
					string aliasString = entry.Key;
					if (projectContent.Language.NameComparer.Equals(partitialTypeName, aliasString)) {
						if (entry.Value.IsDefaultReturnType && entry.Value.GetUnderlyingClass() == null)
							continue; // type not found, maybe entry was a namespace
						return entry.Value;
					}
					if (partitialTypeName.Length > aliasString.Length) {
						if (projectContent.Language.NameComparer.Equals(partitialTypeName.Substring(0, aliasString.Length + 1), aliasString + ".")) {
							string className = entry.Value.FullyQualifiedName + partitialTypeName.Remove(0, aliasString.Length);
							IClass c = projectContent.GetClass(className, typeParameterCount);
							if (c != null) {
								return c.DefaultReturnType;
							}
						}
					}
				}
			}
			if (projectContent.Language.ImportNamespaces) {
				foreach (string str in usings) {
					IClass c = projectContent.GetClass(str + "." + partitialTypeName, typeParameterCount);
					if (c != null) {
						return c.DefaultReturnType;
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
					IClass c = projectContent.GetClass(str + "." + className, typeParameterCount);
					if (c != null) {
						c = projectContent.GetClass(str + "." + partitialTypeName, typeParameterCount);
						if (c != null) {
							return c.DefaultReturnType;
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
			if (HasAliases) {
				foreach (KeyValuePair<string, IReturnType> p in aliases) {
					builder.Append(p.Key);
					builder.Append("=");
					builder.Append(p.Value.ToString());
					builder.Append(", ");
				}
			}
			builder.Length -= 2; // remove last ", "
			builder.Append("]");
			return builder.ToString();
		}
	}
}
