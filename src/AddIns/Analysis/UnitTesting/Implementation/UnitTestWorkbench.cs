// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
