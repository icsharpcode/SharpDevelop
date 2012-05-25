// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class NamespaceName
	{
		public NamespaceName(string qualifiedName)
		{
			this.QualifiedName = qualifiedName;
			FirstPart = GetFirstPartOfNamespace();
			LastPart = GetLastPartOfNamespace();
		}
		
		string GetFirstPartOfNamespace()
		{
			int index = QualifiedName.IndexOf('.');
			if (index >= 0) {
				return QualifiedName.Substring(0, index);
			}
			return QualifiedName;
		}
		
		string GetLastPartOfNamespace()
		{
			int index = QualifiedName.LastIndexOf('.');
			return QualifiedName.Substring(index + 1);
		}
		
		public string FirstPart { get; private set; }
		public string LastPart { get; private set; }
		public string QualifiedName { get; private set; }
		
		public string GetChildNamespaceName(string namespaceName)
		{
			if (QualifiedName == String.Empty) {
				return GetChildNamespaceNameForRootNamespace(namespaceName);
			}
			
			string dottedQualifiedName = QualifiedName + ".";
			if (namespaceName.StartsWith(dottedQualifiedName)) {
				int nextIndex = namespaceName.IndexOf('.', dottedQualifiedName.Length);
				if (nextIndex >= 0) {
					return namespaceName.Substring(0, nextIndex);
				}
				return namespaceName;
			}
			return null;
		}
		
		string GetChildNamespaceNameForRootNamespace(string namespaceName)
		{
			if (!String.IsNullOrEmpty(namespaceName)) {
				return new NamespaceName(namespaceName).FirstPart;
			}
			return null;
		}
	}
}
