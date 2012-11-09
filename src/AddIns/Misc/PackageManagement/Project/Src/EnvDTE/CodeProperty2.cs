// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeProperty2 : CodeProperty, global::EnvDTE.CodeProperty2
	{
		public CodeProperty2()
		{
		}
		
		public CodeProperty2(IProperty property)
			: base(property)
		{
		}
		
		public global::EnvDTE.vsCMPropertyKind ReadWrite { 
			get { return GetPropertyKind(); }
		}
		
		global::EnvDTE.vsCMPropertyKind GetPropertyKind()
		{
			if (Property.CanSet && Property.CanGet) {
				return global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindReadWrite;
			} else if (Property.CanSet) {
				return global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindWriteOnly;
			}
			return global::EnvDTE.vsCMPropertyKind.vsCMPropertyKindReadOnly;
		}
		
		public global::EnvDTE.CodeElements Parameters {
			get { return new CodeParameters(null, Property.Parameters); }
		}
	}
}
