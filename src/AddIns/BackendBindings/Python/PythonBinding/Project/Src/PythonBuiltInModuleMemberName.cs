// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.PythonBinding
{
	public class PythonBuiltInModuleMemberName : MemberName
	{
		public const string PythonBuiltInModuleName = "__builtin__";
		
		public PythonBuiltInModuleMemberName(string memberName)
			: base(PythonBuiltInModuleName, memberName)
		{
		}
		
		public static bool IsBuiltInModule(string name)
		{
			return name == PythonBuiltInModuleName;
		}
	}
}
