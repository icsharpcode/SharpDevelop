// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		List<PadDescriptor> padDescriptors = new List<PadDescriptor>();
		List<Action> safeThreadAsyncMethodCalls = new List<Action>();
		List<object> safeThreadAsyncMethodCallsWithArguments = 
			new List<object>();
		bool makeSafeThreadAsyncMethodCallsWithArguments;
		bool makeNonGenericSafeThreadAsyncMethodCalls;
		List<Type> typesPassedToGetPadMethod = new List<Type>();
		PadDescriptor compilerMessageViewPadDescriptor;
		PadDescriptor errorListPadDescriptor;
		
		public MockUnitTestWorkbench()
		{
			compilerMessageViewPadDescriptor = new PadDescriptor(typeof(CompilerMessageView), "Output", String.Empty);
			AddPadDescriptor(compilerMessageViewPadDescriptor);
			
			errorListPadDescriptor = new PadDescriptor(typeof(ErrorListPad), "Errors", String.Empty);
			AddPadDescriptor(errorListPadDescriptor);
		}
		
		public PadDescriptor CompilerMessageViewPadDescriptor {
			get { return compilerMessageViewPadDescriptor; }
		}
		
		public PadDescriptor ErrorListPadDescriptor {
			get { return errorListPadDescriptor; }
		}
		
		public void AddPadDescriptor(PadDescriptor padDescriptor)
		{
			padDescriptors.Add(padDescriptor);
		}
		
		public PadDescriptor GetPad(Type type)
		{
			typesPassedToGetPadMethod.Add(type);
			
			foreach (PadDescriptor padDescriptor in padDescriptors) {
				if (padDescriptor.Class == type.FullName) {
					return padDescriptor;
				}
			}
			return null;
		}
		
		public List<Type> TypesPassedToGetPadMethod {
			get { return typesPassedToGetPadMethod; }
		}
		
		public List<Action> SafeThreadAsyncMethodCalls {
			get { return safeThreadAsyncMethodCalls; }
		}
		
		public bool MakeNonGenericSafeThreadAsyncMethodCalls {
			get { return makeNonGenericSafeThreadAsyncMethodCalls; }
			set { makeNonGenericSafeThreadAsyncMethodCalls = value; }
		}
		
		public void SafeThreadAsyncCall(Action method)
		{
			safeThreadAsyncMethodCalls.Add(method);
			
			if (makeNonGenericSafeThreadAsyncMethodCalls) {
				method();
			}
		}
		
		public bool MakeSafeThreadAsyncMethodCallsWithArguments {
			get { return makeSafeThreadAsyncMethodCallsWithArguments; }
			set { makeSafeThreadAsyncMethodCallsWithArguments = value; }
		}
		
		public void SafeThreadAsyncCall<T>(Action<T> method, T arg)
		{
			ActionArguments<T> actionArgs = new ActionArguments<T>();
			actionArgs.Action = method;
			actionArgs.Arg = arg;
			
			safeThreadAsyncMethodCallsWithArguments.Add(actionArgs);
			
			if (makeSafeThreadAsyncMethodCallsWithArguments) {
				method(arg);
			}
		}
		
		public List<object> SafeThreadAsyncMethodCallsWithArguments {
			get { return safeThreadAsyncMethodCallsWithArguments; }
		}
	}
}
