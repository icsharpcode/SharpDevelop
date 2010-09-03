// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
