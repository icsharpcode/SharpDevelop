// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class NamespaceName
	{
		public NamespaceName(string parentNamespace, string name)
			: this(GetQualifiedNamespaceName(parentNamespace, name))
		{
		}
		
		static string GetQualifiedNamespaceName(string parentNamespace, string name)
		{
			if (String.IsNullOrEmpty(parentNamespace)) {
				return name;
			}
			return String.Format("{0}.{1}", parentNamespace, name);
		}
		
		public NamespaceName(string qualifiedName)
		{
			this.QualifiedName = qualifiedName;
			LastPart = GetLastPartOfNamespace();
		}
		
		string GetLastPartOfNamespace()
		{
			int index = QualifiedName.LastIndexOf('.');
			return QualifiedName.Substring(index + 1);
		}
		
		public string LastPart { get; private set; }
		public string QualifiedName { get; private set; }
		
		public NamespaceName CreateChildNamespaceName(string name)
		{
			return new NamespaceName(QualifiedName, name);
		}
	}
}
