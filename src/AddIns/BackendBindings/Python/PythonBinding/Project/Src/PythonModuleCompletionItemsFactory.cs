// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using IronPython.Modules;

namespace ICSharpCode.PythonBinding
{
	public static class PythonModuleCompletionItemsFactory
	{
		public static PythonModuleCompletionItems Create(PythonStandardModuleType moduleType)
		{
			if (IsSysModule(moduleType.Type)) {
				return new SysModuleCompletionItems(moduleType);
			}
			return new PythonModuleCompletionItems(moduleType);
		}
		
		static bool IsSysModule(Type type)
		{
			return type == typeof(SysModule);
		}
	}
}
