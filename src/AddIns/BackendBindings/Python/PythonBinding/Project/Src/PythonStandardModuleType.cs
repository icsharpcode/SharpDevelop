// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
