// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass : CodeType, global::EnvDTE.CodeClass
	{
		public CodeClass(IProjectContent projectContent, IClass c)
			: base(projectContent, c)
		{
		}
		
		public CodeClass()
		{
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementClass; }
		}
		
		public virtual global::EnvDTE.CodeElements ImplementedInterfaces {
			get { return new ImplementedInterfacesOnClass(ProjectContent, Class); }
		}
		
		public virtual global::EnvDTE.CodeVariable AddVariable(string name, object type, object Position = null, global::EnvDTE.vsCMAccess Access = global::EnvDTE.vsCMAccess.vsCMAccessPublic, object Location = null)
		{
			var codeGenerator = new ClassCodeGenerator(Class);
			return codeGenerator.AddPublicVariable(name, (string)type);
		}
	}
}
