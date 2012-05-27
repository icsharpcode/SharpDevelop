// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttributeArguments : CodeElements
	{
		List<CodeElement> elements = new List<CodeElement>();
		IAttribute attribute;
		
		public CodeAttributeArguments(IAttribute attribute)
		{
			this.attribute = attribute;
			GetCodeElements();
		}
		
		void GetCodeElements()
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
			elements.Add(new CodeAttributeArgument(name, value));
		}
		
		public int Count {
			get { return elements.Count; }
		}
		
		public IEnumerator GetEnumerator()
		{
			return elements.GetEnumerator();
		}
		
		public CodeElement Item(object index)
		{
			if (index is int) {
				return Item((int)index);
			}
			return Item((string)index);
		}
		
		CodeElement Item(int index)
		{
			return elements[index - 1];
		}
		
		CodeElement Item(string name)
		{
			return elements.Single(item => item.Name == name);
		}
	}
}
