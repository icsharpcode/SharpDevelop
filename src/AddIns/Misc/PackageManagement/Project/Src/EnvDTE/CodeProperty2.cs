// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeProperty2 : CodeProperty
	{
		public CodeProperty2()
		{
		}
		
		public CodeProperty2(IProperty property)
			: base(property)
		{
		}
		
		public vsCMPropertyKind ReadWrite { 
			get { return GetPropertyKind(); }
		}
		
		vsCMPropertyKind GetPropertyKind()
		{
			if (Property.CanSet && Property.CanGet) {
				return vsCMPropertyKind.vsCMPropertyKindReadWrite;
			} else if (Property.CanSet) {
				return vsCMPropertyKind.vsCMPropertyKindWriteOnly;
			}
			return vsCMPropertyKind.vsCMPropertyKindReadOnly;
		}
		
		public CodeElements Parameters {
			get { return new CodeParameters(null, Property.Parameters); }
		}
	}
}
