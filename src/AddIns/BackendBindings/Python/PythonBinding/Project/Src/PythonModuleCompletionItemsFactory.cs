// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
