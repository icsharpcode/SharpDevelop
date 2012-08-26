// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttributes : CodeElementsList
	{
		public CodeAttributes(IEntity entity)
			: this(entity.Attributes)
		{
		}
		
		public CodeAttributes(IParameter parameter)
			: this(parameter.Attributes)
		{
		}
		
		public CodeAttributes(IEnumerable<IAttribute> attributes)
		{
			AddAttributes(attributes);
		}
		
		void AddAttributes(IEnumerable<IAttribute> attributes)
		{
			foreach (IAttribute attribute in attributes) {
				AddAttribute(attribute);
			}
		}
		
		void AddAttribute(IAttribute attribute)
		{
			AddCodeElement(new CodeAttribute2(attribute));
		}
	}
}
