// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypeRef : MarshalByRefObject
	{
		IProjectContent projectContent;
		CodeElement parent;
		
		public CodeTypeRef()
		{
		}
		
		public CodeTypeRef(IProjectContent projectContent, CodeElement parent, IReturnType returnType)
		{
			this.parent = parent;
			this.projectContent = projectContent;
			this.ReturnType = returnType;
		}
		
		protected IReturnType ReturnType { get; private set; }
		
		public virtual string AsFullName {
			get { return ReturnType.GetFullName(); }
		}
		
		public virtual string AsString {
			get {
				if (projectContent.Language == LanguageProperties.VBNet) {
					return ReturnType.AsVisualBasicString();
				}
				return ReturnType.AsCSharpString();
			}
		}
		
		public virtual CodeElement Parent {
			get { return parent; }
		}
		
		public virtual CodeType CodeType {
			get { return new CodeClass2(projectContent, ReturnType.GetUnderlyingClass()); }
		}
	}
}
