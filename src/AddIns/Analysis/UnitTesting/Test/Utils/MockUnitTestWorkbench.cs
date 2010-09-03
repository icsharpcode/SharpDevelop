// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils
{
	public struct ActionArguments<T> {
		public Action<T> Action;
		public T Arg;
	}
	
	public class MockUnitTestWorkbench : IUnitTestWorkbench
	{
		public List<Action> SafeThreadAsyncMethodCalls = new List<Action>();
		public List<object> SafeThreadAsyncMethodCallsWithArguments = 
			new List<object>();
		public bool MakeSafeThreadAsyncMethodCallsWithArguments;
		public bool MakeNonGenericSafeThreadAsyncMethodCalls;
		public List<Type> TypesPassedToGetPadMethod = new List<Type>();
		public PadDescriptor CompilerMessageViewPadDescriptor;
		public PadDescriptor ErrorListPadDescriptor;
		
		List<PadDescriptor> padDescriptors = new List<PadDescriptor>();
		
		public MockUnitTestWorkbench()
		{
			CompilerMessageViewPadDescriptor = new PadDescriptor(typeof(CompilerMessageView), "Output", String.Empty);
			AddPadDescriptor(CompilerMessageViewPadDescriptor);
			
			ErrorListPadDescriptor = new PadDescriptor(typeof(ErrorListPad), "Errors", String.Empty);
			AddPadDescriptor(ErrorListPadDescriptor);
		}

		public void AddPadDescriptor(PadDescriptor padDescriptor)
		{
			padDescriptors.Add(padDescriptor);
		}
		
		public PadDescriptor GetPad(Type type)
		{
			TypesPassedToGetPadMethod.Add(type);
			
			foreach (PadDescriptor padDescriptor in padDescriptors) {
				if (padDescriptor.Class == type.FullName) {
					return padDescriptor;
				}
			}
			return null;
		}
		
		public void SafeThreadAsyncCall(Action method)
		{
			SafeThreadAsyncMethodCalls.Add(method);
			
			if (MakeNonGenericSafeThreadAsyncMethodCalls) {
				method();
			}
		}
		
		public void SafeThreadAsyncCall<T>(Action<T> method, T arg)
		{
			ActionArguments<T> actionArgs = new ActionArguments<T>();
			actionArgs.Action = method;
			actionArgs.Arg = arg;
			
			SafeThreadAsyncMethodCallsWithArguments.Add(actionArgs);
			
			if (MakeSafeThreadAsyncMethodCallsWithArguments) {
				method(arg);
			}
		}
	}
}
