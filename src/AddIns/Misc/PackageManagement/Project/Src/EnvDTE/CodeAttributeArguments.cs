// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttributeArguments : CodeElementsList
	{
		IAttribute attribute;
		
		public CodeAttributeArguments(IAttribute attribute)
		{
			this.attribute = attribute;
			AddCodeElements();
		}
		
		void AddCodeElements()
		{
			foreach (object arg in attribute.PositionalArguments) {
				AddAttributeArgument(String.Empty, arg);
			}
			foreach (KeyValuePair<string, object> namedArg in attribute.NamedArguments) {
				AddAttributeArgument(namedArg.Key, namedArg.Value);
			}
		}
		
		void AddAttributeArgument(string name, object value)
		{
			AddCodeElement(new CodeAttributeArgument(name, value));
		}
	}
}
