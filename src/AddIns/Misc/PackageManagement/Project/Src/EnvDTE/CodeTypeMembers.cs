// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypeMembers : CodeElementsList
	{
		IProjectContent projectContent;
		IClass c;
		
		public CodeTypeMembers(IProjectContent projectContent, IClass c)
		{
			this.projectContent = projectContent;
			this.c = c;
			AddMembers();
		}
		
		void AddMembers()
		{
			foreach (IProperty property in c.Properties) {
				AddProperty(property);
			}
			foreach (IField field in c.Fields) {
				AddField(field);
			}
			foreach (IMethod method in c.Methods) {
				AddMethod(method);
			}
		}
		
		void AddMethod(IMethod method)
		{
			AddCodeElement(new CodeFunction2(method));
		}
		
		void AddField(IField field)
		{
			AddCodeElement(new CodeVariable(field));
		}
		
		void AddProperty(IProperty property)
		{
			AddCodeElement(new CodeProperty2(property));
		}
	}
}
