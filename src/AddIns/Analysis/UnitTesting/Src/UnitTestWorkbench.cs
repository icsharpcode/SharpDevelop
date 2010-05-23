// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestWorkbench : IUnitTestWorkbench
	{
		public PadDescriptor GetPad(Type type)
		{
			return WorkbenchSingleton.Workbench.GetPad(type);
		}
		
		public void SafeThreadAsyncCall(Action method)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(method);
		}
		
		public void SafeThreadAsyncCall<A>(Action<A> method, A arg1)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(method, arg1);
		}
	}
}
