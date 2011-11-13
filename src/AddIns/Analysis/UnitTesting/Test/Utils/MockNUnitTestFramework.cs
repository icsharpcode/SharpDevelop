// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
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
		IUnitTestMessageService messageService;
		IUnitTestFileService fileService = new MockFileService();

		public MockNUnitTestFramework(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor, 
			UnitTestingOptions options)
			: this(null, processRunner, testResultsMonitor, options, null)
		{
		}
		
		public MockNUnitTestFramework(IUnitTestDebuggerService debuggerService,
			ITestResultsMonitor testResultsMonitor, 
			UnitTestingOptions options,
			IUnitTestMessageService messageService)
			: this(debuggerService, null, testResultsMonitor, options, messageService)
		{
		}
		
		public MockNUnitTestFramework(IUnitTestDebuggerService debuggerService,
			IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor, 
			UnitTestingOptions options,
			IUnitTestMessageService messageService)
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
		
		public IEnumerable<TestMember> GetTestMembersFor(IClass @class) {
			throw new NotImplementedException();
		}
		
		public bool IsTestClass(IClass c)
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
