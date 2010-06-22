// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils.Tests
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
