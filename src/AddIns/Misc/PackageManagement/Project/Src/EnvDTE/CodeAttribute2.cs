// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttribute2 : CodeAttribute
	{
		CodeAttributeArguments arguments;
		IAttribute attribute;
		
		public CodeAttribute2()
		{
		}
		
		public CodeAttribute2(IAttribute attribute)
			: base(attribute)
		{
			this.attribute = attribute;
		}
		
		public virtual CodeElements Arguments {
			get {
				if (arguments == null) {
					arguments = new CodeAttributeArguments(attribute);
				}
				return arguments;
			}
		}
	}
}
