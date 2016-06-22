// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		public bool IsShowConsolePadCalled;
		
		public void ShowConsolePad()
		{
			IsShowConsolePadCalled = true;
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
		
		public bool IsSafeThreadFunctionCallMade;
		
		public R SafeThreadFunction<R>(Func<R> method)
		{
			IsSafeThreadFunctionCallMade = true;
			return method();
		}
	}
}
