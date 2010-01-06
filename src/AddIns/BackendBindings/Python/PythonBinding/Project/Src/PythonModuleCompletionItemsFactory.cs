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
		public static PythonModuleCompletionItems Create(Type type)
		{
			if (IsSysModule(type)) {
				return new SysModuleCompletionItems(type);
			}
			return new PythonModuleCompletionItems(type);
		}
		
		static bool IsSysModule(Type type)
		{
			return type == typeof(SysModule);
		}
	}
}
