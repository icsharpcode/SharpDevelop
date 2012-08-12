// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ImplementedInterfacesOnClass : CodeElementsList
	{
		IProjectContent projectContent;
		IClass c;
		
		public ImplementedInterfacesOnClass(IProjectContent projectContent, IClass c)
		{
			this.projectContent = projectContent;
			this.c 	= c;
			AddCodeInterfaces();
		}
		
		void AddCodeInterfaces()
		{
			foreach (IReturnType baseType in c.BaseTypes) {
				CodeInterface codeInterface = CodeInterface.CreateFromBaseType(projectContent, baseType);
				if (codeInterface != null) {
					AddCodeElement(codeInterface);
				}
			}
		}
		
		void AddCodeInterface(IReturnType baseType, IClass baseTypeClass)
		{
			AddCodeElement(CreateCodeInterface(baseType, baseTypeClass));
		}
		
		CodeInterface CreateCodeInterface(IReturnType baseType, IClass baseTypeClass)
		{
			return new CodeInterface(projectContent, baseType, baseTypeClass);
		}
	}
}
