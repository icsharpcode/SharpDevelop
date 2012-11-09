// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElementsList : MarshalByRefObject, global::EnvDTE.CodeElements
	{
		List<CodeElement> elements = new List<CodeElement>();
		
		public CodeElementsList()
		{
		}
		
		protected virtual void AddCodeElement(CodeElement element)
		{
			elements.Add(element);
		}
		
		public int Count {
			get { return elements.Count; }
		}
		
		public IEnumerator GetEnumerator()
		{
			return elements.GetEnumerator();
		}
		
		public global::EnvDTE.CodeElement Item(object index)
		{
			if (index is int) {
				return Item((int)index);
			}
			return Item((string)index);
		}
		
		global::EnvDTE.CodeElement Item(int index)
		{
			return elements[index - 1];
		}
		
		global::EnvDTE.CodeElement Item(string name)
		{
			return elements.Single(item => item.Name == name);
		}
	}
}
