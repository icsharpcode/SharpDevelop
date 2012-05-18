// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlCompilationUnit.
	/// </summary>
	public class XamlCompilationUnit : DefaultCompilationUnit
	{
		public XamlCompilationUnit(IProjectContent projectContent)
			: base(projectContent)
		{
		}
		
		public NodeWrapper TreeRootNode { get; set; }

		/// <summary>
		/// Creates a IReturnType looking for a class referenced in XAML.
		/// </summary>
		/// <param name="xmlNamespace">The XML namespace</param>
		/// <param name="className">The class name</param>
		/// <returns>A new IReturnType that will search the referenced type on demand.</returns>
		public IReturnType CreateType(string xmlNamespace, string className)
		{
			if (string.IsNullOrEmpty(className) || className.Contains("."))
				return null;
			
			if (xmlNamespace.StartsWith("clr-namespace:", StringComparison.OrdinalIgnoreCase)) {
				return CreateClrNamespaceType(this.ProjectContent, xmlNamespace, className);
			} else {
				return new XamlClassReturnType(this, xmlNamespace, className);
			}
		}

		static IReturnType CreateClrNamespaceType(IProjectContent pc, string xmlNamespace, string className)
		{
			string namespaceName = GetNamespaceNameFromClrNamespace(xmlNamespace);
			return new GetClassReturnType(pc, namespaceName + "." + className, 0);
		}

		static string GetNamespaceNameFromClrNamespace(string xmlNamespace)
		{
			string namespaceName = xmlNamespace.Substring("clr-namespace:".Length);
			int pos = namespaceName.IndexOf(';');
			if (pos >= 0) {
				// we expect that the target type is also a reference of the project, so we
				// can ignore the assembly part after the ;
				namespaceName = namespaceName.Substring(0, pos);
			}
			return namespaceName;
		}

		/// <summary>
		/// Finds a type referenced in XAML.
		/// </summary>
		/// <param name="xmlNamespace">The XML namespace</param>
		/// <param name="className">The class name</param>
		/// <returns>Returns the referenced type, or null if it cannot be found.</returns>
		public IReturnType FindType(string xmlNamespace, string className)
		{
			return FindType(this.ProjectContent, xmlNamespace, className);
		}

		public static IReturnType FindType(IProjectContent pc, string xmlNamespace, string className)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			if (xmlNamespace == null || className == null)
				return null;
			if (xmlNamespace.StartsWith("clr-namespace:", StringComparison.OrdinalIgnoreCase)) {
				return CreateClrNamespaceType(pc, xmlNamespace, className);
			}
			else {
				IReturnType type = FindTypeInAssembly(pc, xmlNamespace, className);
				if (type != null)
					return type;
				foreach (IProjectContent p in pc.ThreadSafeGetReferencedContents()) {
					type = FindTypeInAssembly(p, xmlNamespace, className);
					if (type != null)
						return type;
				}
				return null;
			}
		}

		static IReturnType FindTypeInAssembly(IProjectContent projectContent, string xmlNamespace, string className)
		{
			foreach (IAttribute att in projectContent.GetAssemblyAttributes()) {
				if (att.PositionalArguments.Count == 2
				    && att.AttributeType.FullyQualifiedName == "System.Windows.Markup.XmlnsDefinitionAttribute") {
					string namespaceName = att.PositionalArguments[1] as string;
					if (xmlNamespace.Equals(att.PositionalArguments[0]) && namespaceName != null) {
						IClass c = projectContent.GetClass(namespaceName + "." + className, 0);
						if (c != null)
							return c.DefaultReturnType;
					}
				}
			}
			return null;
		}

		public static IEnumerable<IClass> GetNamespaceMembers(IProjectContent pc, string xmlNamespace)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			
			if (!string.IsNullOrEmpty(xmlNamespace)) {
				if (xmlNamespace.StartsWith("clr-namespace:", StringComparison.OrdinalIgnoreCase))
					return pc.GetNamespaceContents(GetNamespaceNameFromClrNamespace(xmlNamespace)).OfType<IClass>();
				else {
					var list = new List<ICompletionEntry>();
					AddNamespaceMembersInAssembly(pc, xmlNamespace, list);
					foreach (IProjectContent p in pc.ThreadSafeGetReferencedContents()) {
						AddNamespaceMembersInAssembly(p, xmlNamespace, list);
					}
					return list.OfType<IClass>();
				}
			}
			
			return Enumerable.Empty<IClass>();
		}

		static void AddNamespaceMembersInAssembly(IProjectContent projectContent, string xmlNamespace, List<ICompletionEntry> list)
		{
			foreach (IAttribute att in projectContent.GetAssemblyAttributes()) {
				if (att.PositionalArguments.Count == 2
				    && att.AttributeType.FullyQualifiedName == "System.Windows.Markup.XmlnsDefinitionAttribute") {
					string namespaceName = att.PositionalArguments[1] as string;
					if (xmlNamespace.Equals(att.PositionalArguments[0]) && namespaceName != null) {
						projectContent.AddNamespaceContents(list, namespaceName, projectContent.Language, false);
					}
				}
			}
		}
	}
}
