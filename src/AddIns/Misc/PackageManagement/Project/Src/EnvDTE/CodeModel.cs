// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeModel : MarshalByRefObject, global::EnvDTE.CodeModel
	{
		IProjectContent projectContent;
		CodeElementsInNamespace codeElements;
		
		public CodeModel(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
		}
		
		public global::EnvDTE.CodeElements CodeElements {
			get {
				if (codeElements == null) {
					codeElements = new CodeElementsInNamespace(projectContent, String.Empty);
				}
				return codeElements;
			}
		}
		
		public global::EnvDTE.CodeType CodeTypeFromFullName(string name)
		{
			IClass matchedClass = projectContent.GetClass(name, 0);
			if (matchedClass != null) {
				return CreateCodeTypeForClass(matchedClass);
			}
			return null;
		}
		
		CodeType CreateCodeTypeForClass(IClass c)
		{
			if (c.ClassType == ClassType.Interface) {
				return new CodeInterface(projectContent, c);
			}
			return new CodeClass2(projectContent, c);
		}
		
		public string Language {
			get { return projectContent.GetCodeModelLanguage(); }
		}
	}
}
