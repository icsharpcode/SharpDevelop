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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockUnitTestWorkbenchTestFixture
	{
		MockUnitTestWorkbench workbench;
		TestResult resultShown;
		bool testAsyncMethodCalled;
		
		[SetUp]
		public void Init()
		{
			workbench = new MockUnitTestWorkbench();
		}
		
		[Test]
		public void GetPadReturnsNullForUnknownType()
		{
			Assert.IsNull(workbench.GetPad(typeof(String)));
		}
		
		[Test]
		public void NoActionsPassedToSafeThreadAsyncCallMethod()
		{
			Assert.AreEqual(0, workbench.SafeThreadAsyncMethodCalls.Count);
		}
		
		[Test]
		public void MethodStoredInSafeThreadAsyncMethodCallsCollectionAfterSafeThreadAsyncCallMethodCalled()
		{
			workbench.SafeThreadAsyncCall(this.MethodStoredInSafeThreadAsyncMethodCallsCollectionAfterSafeThreadAsyncCallMethodCalled);
			
			Action action = this.MethodStoredInSafeThreadAsyncMethodCallsCollectionAfterSafeThreadAsyncCallMethodCalled;
			Assert.AreEqual(action, workbench.SafeThreadAsyncMethodCalls[0]);
		}
		
		[Test]
		public void MethodWithParameterStoredInSafeThreadAsyncMethodCallsCollectionAfterSafeThreadAsyncCallMethodCalled()
		{
			TestResult result = new TestResult("abc");
			workbench.SafeThreadAsyncCall(this.ShowResults, result);
			
			ActionArguments<TestResult> actionArgs = new ActionArguments<TestResult>();
			actionArgs.Action = this.ShowResults;
			actionArgs.Arg = result;
			
			Assert.AreEqual(actionArgs, workbench.SafeThreadAsyncMethodCallsWithArguments[0]);
		}
		
		void ShowResults(TestResult result)
		{
			resultShown = result;
		}
		
		[Test]
		public void SafeThreadAsyncCallWillCallActualMethodIfMakeMethodCallsSetToTrue()
		{
			TestResult result = new TestResult("abc");
			workbench.MakeSafeThreadAsyncMethodCallsWithArguments = true;
			workbench.SafeThreadAsyncCall(this.ShowResults, result);
			
			Assert.AreEqual(result, resultShown);
		}
		
		[Test]
		public void NonGenericSafeThreadAsyncCallWillCallActualMethodIfMakeMethodCallsSetToTrue()
		{
			workbench.MakeNonGenericSafeThreadAsyncMethodCalls = true;
			workbench.SafeThreadAsyncCall(this.TestAsyncCallMethod);
			
			Assert.IsTrue(testAsyncMethodCalled);
		}
		
		void TestAsyncCallMethod()
		{
			testAsyncMethodCalled = true;
		}
		
		[Test]
		public void MakeMethodCallsIsFalseByDefault()
		{
			Assert.IsFalse(workbench.MakeSafeThreadAsyncMethodCallsWithArguments);
		}
		
		[Test]
		public void TypesPassedToGetPadMethodReturnsEmptyCollectionByDefault()
		{
			Assert.AreEqual(0, workbench.TypesPassedToGetPadMethod.Count);
		}
		
		[Test]
		public void ErrorListTypeStoredInTypesPassedToGetPadMethod()
		{
			workbench.GetPad(typeof(ErrorListPad));
			Type[] expectedTypes = new Type[] { typeof(ErrorListPad) };
			
			Assert.AreEqual(expectedTypes, workbench.TypesPassedToGetPadMethod.ToArray());
		}
		
		[Test]
		public void CompilerMessageViewPadExistsInWorkbench()
		{
			PadDescriptor actualPadDescriptor = workbench.GetPad(typeof(CompilerMessageView));
			Assert.IsNotNull(actualPadDescriptor);
		}
		
		[Test]
		public void ErrorListPadExistsInWorkbench()
		{
			PadDescriptor actualPadDescriptor = workbench.GetPad(typeof(ErrorListPad));
			Assert.IsNotNull(actualPadDescriptor);
		}
		
		[Test]
		public void ErrorListPadDescriptorPropertyReturnsErrorListPad()
		{
			string expectedTypeName = typeof(ErrorListPad).FullName;
			string typeName =  workbench.ErrorListPadDescriptor.Class;
			Assert.AreEqual(expectedTypeName, typeName);
		}
		
		[Test]
		public void CompilerMessageViewPadDescriptorPropertyReturnsCompilerMessageViewPad()
		{
			string expectedTypeName = typeof(CompilerMessageView).FullName;
			string typeName =  workbench.CompilerMessageViewPadDescriptor.Class;
			Assert.AreEqual(expectedTypeName, typeName);
		}
	}
}
