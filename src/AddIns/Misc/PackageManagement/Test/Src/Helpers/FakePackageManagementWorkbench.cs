// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementWorkbench : IPackageManagementWorkbench
	{
		public bool IsCreateConsolePadCalled;
		
		public void CreateConsolePad()
		{
			IsCreateConsolePadCalled = true;
		}
		
		public bool InvokeRequiredReturnValue;
		
		public bool InvokeRequired {
			get { return InvokeRequiredReturnValue; }
		}
		
		public bool IsSafeThreadAsyncCallMade;
		public object Arg1PassedToSafeThreadAsyncCall;
		public object Arg2PassedToSafeThreadAsyncCall;
		
		public void SafeThreadAsyncCall<A>(Action<A> method, A arg1)
		{
			IsSafeThreadAsyncCallMade = true;
			Arg1PassedToSafeThreadAsyncCall = arg1;
		}
		
		public void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			IsSafeThreadAsyncCallMade = true;
			Arg1PassedToSafeThreadAsyncCall = arg1;
			Arg2PassedToSafeThreadAsyncCall = arg2;
		}
	}
}
