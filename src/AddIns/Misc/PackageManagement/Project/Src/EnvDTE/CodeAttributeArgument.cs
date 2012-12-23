// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttributeArgument : CodeElement, global::EnvDTE.CodeAttributeArgument
	{
		string name;
		string value;
		
		public CodeAttributeArgument()
		{
		}
		
		public CodeAttributeArgument(string name, object value)
		{
			this.name = name;
			this.value = GetValue(value);
		}
		
		string GetValue(object value)
		{
			if (value is string) {
				return String.Format("\"{0}\"", value);
			}
			return value.ToString();
		}
		
		public override string Name {
			get { return name; }
		}
		
		public virtual string Value {
			get { return value; }
		}
	}
}
