// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3494 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using System.Collections;
using System.Collections.Generic;

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

		public IReturnType CreateType(string xmlNamespace, string className)
		{
			if (xmlNamespace.StartsWith("clr-namespace:")) {
				return CreateClrNamespaceType(this.ProjectContent, xmlNamespace, className);
			}
			else {
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
			if (xmlNamespace.StartsWith("clr-namespace:")) {
				return CreateClrNamespaceType(pc, xmlNamespace, className);
			}
			else {
				IReturnType type = FindTypeInAssembly(pc, xmlNamespace, className);
				if (type != null)
					return type;
				foreach (IProjectContent p in pc.ReferencedContents) {
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

		public static ArrayList GetNamespaceMembers(IProjectContent pc, string xmlNamespace)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			if (xmlNamespace == null)
				return null;
			if (xmlNamespace.StartsWith("clr-namespace:")) {
				return pc.GetNamespaceContents(GetNamespaceNameFromClrNamespace(xmlNamespace));
			}
			else {
				ArrayList list = new ArrayList();
				AddNamespaceMembersInAssembly(pc, xmlNamespace, list);
				foreach (IProjectContent p in pc.ReferencedContents) {
					AddNamespaceMembersInAssembly(p, xmlNamespace, list);
				}
				return list;
			}
		}

		static void AddNamespaceMembersInAssembly(IProjectContent projectContent, string xmlNamespace, ArrayList list)
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
