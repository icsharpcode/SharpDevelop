// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeControlDispatcher : IControlDispatcher
	{
		public Delegate MethodInvoked;
		public object[] MethodInvokedArgs;
		public bool CheckAccessReturnValue;
		public object InvokeReturnValue;
		
		public bool CheckAccess()
		{
			return CheckAccessReturnValue;
		}
		
		public object Invoke(Delegate method, params object[] args)
		{
			MethodInvoked = method;
			MethodInvokedArgs = args;
			return InvokeReturnValue;
		}
	}
}
