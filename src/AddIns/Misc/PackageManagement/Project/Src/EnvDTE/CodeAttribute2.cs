// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttribute2 : CodeAttribute, global::EnvDTE.CodeAttribute2
	{
		IAttribute attribute;
		
		public CodeAttribute2()
		{
		}
		
		public CodeAttribute2(IAttribute attribute)
			: base(attribute)
		{
			this.attribute = attribute;
		}
		
		public virtual global::EnvDTE.CodeElements Arguments {
			get { return new CodeAttributeArguments(attribute); }
		}
	}
}
