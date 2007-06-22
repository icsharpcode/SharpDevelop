// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace XamlBinding
{
	/// <summary>
	/// Description of XamlCompilationUnit.
	/// </summary>
	public class XamlCompilationUnit : DefaultCompilationUnit
	{
		public XamlCompilationUnit(IProjectContent projectContent) : base(projectContent)
		{
		}
		
		public IReturnType CreateType(string xmlNamespace, string className)
		{
			if (xmlNamespace.StartsWith("clr-namespace:")) {
				return CreateClrNamespaceType(xmlNamespace, className);
			} else {
				return new XamlClassReturnType(this, xmlNamespace, className);
			}
		}
		
		IReturnType CreateClrNamespaceType(string xmlNamespace, string className)
		{
			string namespaceName = xmlNamespace.Substring("clr-namespace:".Length);
			int pos = namespaceName.IndexOf(';');
			if (pos >= 0) {
				// we expect that the target type is also a reference of the project, so we
				// can ignore the assembly part after the ;
				namespaceName = namespaceName.Substring(0, pos);
			}
			return new GetClassReturnType(this.ProjectContent, namespaceName + "." + className, 0);
		}
		
		public IReturnType FindType(string xmlNamespace, string className)
		{
			if (xmlNamespace.StartsWith("clr-namespace:")) {
				return CreateClrNamespaceType(xmlNamespace, className);
			} else {
				IReturnType type = FindTypeInAssembly(this.ProjectContent, xmlNamespace, className);
				if (type != null)
					return type;
				foreach (IProjectContent p in this.ProjectContent.ReferencedContents) {
					type = FindTypeInAssembly(this.ProjectContent, xmlNamespace, className);
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
				    && att.AttributeType.FullyQualifiedName == "System.Windows.Markup.XmlnsDefinitionAttribute")
				{
					string namespaceName = att.PositionalArguments[1] as string;
					if (xmlNamespace.Equals(att.PositionalArguments[0]) && namespaceName != null) {
						IClass c = projectContent.GetClass(namespaceName + "." + className, 0, LanguageProperties.CSharp, false);
						if (c != null)
							return c.DefaultReturnType;
					}
				}
			}
			return null;
		}
	}
}
