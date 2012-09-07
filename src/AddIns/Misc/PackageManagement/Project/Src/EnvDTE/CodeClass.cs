// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass : CodeType
	{
		public CodeClass(IProjectContent projectContent, IClass c)
			: base(projectContent, c)
		{
		}
		
		public CodeClass()
		{
		}
		
		public override vsCMElement Kind {
			get { return vsCMElement.vsCMElementClass; }
		}
		
		public virtual CodeElements ImplementedInterfaces {
			get { return new ImplementedInterfacesOnClass(ProjectContent, Class); }
		}
		
		public virtual CodeVariable AddVariable(string name, object type, object Position = null, vsCMAccess Access = vsCMAccess.vsCMAccessPublic, object Location = null)
		{
			var codeGenerator = new ClassCodeGenerator(Class);
			return codeGenerator.AddPublicVariable(name, (string)type);
		}
	}
}
