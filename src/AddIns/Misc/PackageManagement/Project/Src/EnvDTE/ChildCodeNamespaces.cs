// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ChildCodeNamespaces : List<CodeNamespace>
	{
		IProjectContent projectContent;
		CodeNamespace parentNamespace;
		HashSet<string> namespacesAdded = new HashSet<string>();
		
		public ChildCodeNamespaces(IProjectContent projectContent, CodeNamespace parentNamespace)
		{
			this.projectContent = projectContent;
			this.parentNamespace = parentNamespace;
			AddChildNamespaces();
		}
		
		void AddChildNamespaces()
		{
			foreach (string namespaceName in GetUniqueQualifiedChildNamespaceNames()) {
				AddCodeNamespace(namespaceName);
			}
		}
		
		IEnumerable<string> GetUniqueQualifiedChildNamespaceNames()
		{
			foreach (string namespaceName in projectContent.NamespaceNames) {
				string qualifiedChildNamespaceName = parentNamespace.GetChildNamespaceName(namespaceName);
				if (IsUniqueChildNamespaceName(qualifiedChildNamespaceName)) {
					namespacesAdded.Add(qualifiedChildNamespaceName);
					yield return qualifiedChildNamespaceName;
				}
			}
		}
		
		bool IsUniqueChildNamespaceName(string name)
		{
			if (name != null) {
				return !namespacesAdded.Contains(name);
			}
			return false;
		}
		
		void AddCodeNamespace(string namespaceName)
		{
			Add(new CodeNamespace(projectContent, namespaceName));
		}
	}
}
