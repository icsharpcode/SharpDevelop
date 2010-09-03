// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PythonBinding
{
	public class PythonStandardModuleType
	{
		Type type;
		string name;
		
		public PythonStandardModuleType(Type type, string name)
		{
			this.type = type;
			this.name = name;
		}
		
		public Type Type {
			get { return type; }
		}
		
		public string Name {
			get { return name; }
		}
	}
}
