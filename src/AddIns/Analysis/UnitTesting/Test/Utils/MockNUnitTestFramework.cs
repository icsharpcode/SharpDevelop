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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockNUnitTestFramework : ITestFramework
	{
		List<NUnitTestRunner> testRunnersCreated = new List<NUnitTestRunner>();
		List<NUnitTestDebugger> testDebuggersCreated = new List<NUnitTestDebugger>();
		IUnitTestProcessRunner processRunner;
		ITestResultsMonitor testResultsMonitor;
		UnitTestingOptions options;
		IUnitTestDebuggerService debuggerService;
		IMessageService messageService;
		IFileSystem fileService = new MockFileService();

		public MockNUnitTestFramework(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor, 
			UnitTestingOptions options)
			: this(null, processRunner, testResultsMonitor, options, null)
		{
		}
		
		public MockNUnitTestFramework(IUnitTestDebuggerService debuggerService,
			ITestResultsMonitor testResultsMonitor, 
			UnitTestingOptions options,
			IMessageService messageService)
			: this(debuggerService, null, testResultsMonitor, options, messageService)
		{
		}
		
		public MockNUnitTestFramework(IUnitTestDebuggerService debuggerService,
			IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor, 
			UnitTestingOptions options,
			IMessageService messageService)
		{
			this.debuggerService = debuggerService;
			this.processRunner = processRunner;
			this.testResultsMonitor = testResultsMonitor;
			this.options = options;
			this.messageService = messageService;
		}
		
		public bool IsTestMember(IMember member)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(ITypeDefinition @class) {
			throw new NotImplementedException();
		}
		
		public bool IsTestClass(ITypeDefinition c)
		{
			throw new NotImplementedException();
		}
		
		public bool IsTestProject(IProject project)
		{
			throw new NotImplementedException();
		}
		
		public ITestRunner CreateTestRunner()
		{
			TestProcessRunnerBaseContext context = new TestProcessRunnerBaseContext(processRunner, 
				testResultsMonitor, 
				fileService,
				messageService);
			NUnitTestRunner testRunner = new NUnitTestRunner(context, options);
			testRunnersCreated.Add(testRunner);
			return testRunner;
		}
		
		public List<NUnitTestRunner> NUnitTestRunnersCreated {
			get { return testRunnersCreated; }
		}
		
		public ITestRunner CreateTestDebugger()
		{
			NUnitTestDebugger testDebugger = 
				new NUnitTestDebugger(debuggerService, messageService, testResultsMonitor, options);
			testDebuggersCreated.Add(testDebugger);
			return testDebugger;
		}
		
		public List<NUnitTestDebugger> NUnitTestDebuggersCreated {
			get { return testDebuggersCreated; }
		}
		
		public bool IsBuildNeededBeforeTestRun {
			get { return true; }
		}
	}
}
