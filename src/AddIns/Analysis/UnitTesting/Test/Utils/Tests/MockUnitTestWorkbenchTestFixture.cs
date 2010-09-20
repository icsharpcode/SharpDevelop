// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
