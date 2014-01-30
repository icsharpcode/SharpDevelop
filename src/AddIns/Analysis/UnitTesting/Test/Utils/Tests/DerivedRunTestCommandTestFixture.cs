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
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class DerivedRunTestCommandTestFixture
	{
		DerivedRunTestCommand runTestCommand;
		
		[SetUp]
		public void Init()
		{
			MockRunTestCommandContext context = new MockRunTestCommandContext();
			runTestCommand = new DerivedRunTestCommand(context);
		}
		
		[TearDown]
		public void TearDown()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
		}
		
		[Test]
		public void IsOnBeforeRunTestsMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
		
		[Test]
		public void IsOnBeforeRunTestsMethodCalledReturnsTrueAfterOnBeforeRunMethodCalled()
		{
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsTrue(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
		
		[Test]
		public void IsRunningTestPropertyWhenOnBeforeRunCalledReturnsFalseByDefault()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsFalse(runTestCommand.IsRunningTestWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void IsRunningTestPropertyWhenOnBeforeRunCalledReturnsTrueWhenRunningTestCommandIsNonNullWhenOnBeforeRunMethodCalled()
		{
			AbstractRunTestCommand.RunningTestCommand = runTestCommand;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsTrue(runTestCommand.IsRunningTestWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void RunningTestCommandPropertyWhenOnBeforeRunCalledReturnsNullByDefault()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.IsNull(runTestCommand.RunningTestCommandPropertyWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void RunningTestCommandPropertyWhenOnBeforeRunCalledReturnsNonNullWhenRunningTestCommandIsNonNullWhenOnBeforeRunMethodCalled()
		{
			AbstractRunTestCommand.RunningTestCommand = runTestCommand;
			runTestCommand.CallOnBeforeBuildMethod();
			Assert.AreEqual(runTestCommand, runTestCommand.RunningTestCommandPropertyWhenOnBeforeBuildCalled);
		}
		
		[Test]
		public void IsOnAfterRunTestsMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(runTestCommand.IsOnAfterRunTestsMethodCalled);
		}
		
		[Test]
		public void IsOnAfterRunTestsMethodCalledReturnsTrueAfterOnBeforeRunMethodCalled()
		{
			runTestCommand.CallOnAfterRunTestsMethod();
			Assert.IsTrue(runTestCommand.IsOnAfterRunTestsMethodCalled);
		}
		
		[Test]
		public void TestRunnersCreatedAreSaved()
		{
			MockTestRunner testRunner = runTestCommand.CallCreateTestRunner(null) as MockTestRunner;
			MockTestRunner[] expectedTestRunners = new MockTestRunner[] { testRunner };
			Assert.AreEqual(expectedTestRunners, runTestCommand.TestRunnersCreated.ToArray());
		}
	}
}
