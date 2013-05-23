// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestDebugger : TestDebuggerBase
	{
		UnitTestingOptions options;
		
		public NUnitTestDebugger()
			: this(new UnitTestDebuggerService(),
				SD.MessageService,
				new TestResultsReader(),
				UnitTestingOptions.Instance.Clone())
		{
		}
		
		public NUnitTestDebugger(IUnitTestDebuggerService debuggerService,
			IMessageService messageService,
			ITestResultsReader testResultsReader,
			UnitTestingOptions options)
			: base(debuggerService, messageService, testResultsReader)
		{
			this.options = options;
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests, options);
			app.ResultsPipe = base.TestResultsReader.PipeName;
			return app.GetProcessStartInfo();
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return new NUnitTestResult(testResult);
		}
		
		public override int GetExpectedNumberOfTestResults(IEnumerable<ITest> selectedTests)
		{
			return NUnitTestRunner.GetNumberOfTestMethods(selectedTests);
		}
	}
}
