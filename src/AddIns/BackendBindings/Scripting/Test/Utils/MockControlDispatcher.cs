// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils.Tests
{
	public class MockControlDispatcher : IControlDispatcher
	{
		Delegate methodInvoked;
		object[] methodInvokedArgs;
		bool checkAccessReturnValue;
		
		public MockControlDispatcher()
		{
		}
		
		public bool CheckAccess()
		{
			return checkAccessReturnValue;
		}
		
		public bool CheckAccessReturnValue {
			get { return checkAccessReturnValue; }
			set { checkAccessReturnValue = value; }
		}
		
		public object Invoke(Delegate method, params object[] args)
		{
			methodInvoked = method;
			methodInvokedArgs = args;
			return null;
		}
		
		public Delegate MethodInvoked {
			get { return methodInvoked; }
			set { methodInvoked = null; }
		}
		
		public object[] MethodInvokedArgs {
			get { return methodInvokedArgs; }
			set { methodInvokedArgs = value; }
		}
	}
}
