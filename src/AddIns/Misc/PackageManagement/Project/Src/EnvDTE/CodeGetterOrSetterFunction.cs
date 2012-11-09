// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeGetterOrSetterFunction : CodeFunction
	{
		ModifierEnum modifier;
		
		public CodeGetterOrSetterFunction(IProperty property, ModifierEnum modifier)
			: base(property)
		{
			this.modifier = modifier;
		}
		
		public override global::EnvDTE.vsCMAccess Access {
			get {
				if (modifier == ModifierEnum.None) {
					return base.Access;
				}
				return global::EnvDTE.vsCMAccess.vsCMAccessPrivate;
			}
			set { }
		}
	}
}
