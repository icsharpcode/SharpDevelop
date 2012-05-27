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
		IEntity entity;
		
		public CodeAttributes(IEntity entity)
		{
			this.entity = entity;
			GetAttributes();
		}
		
		void GetAttributes()
		{
			foreach (IAttribute attribute in entity.Attributes) {
				AddAttribute(attribute);
			}
		}
		
		void AddAttribute(IAttribute attribute)
		{
			AddCodeElement(new CodeAttribute2(attribute));
		}
	}
}
