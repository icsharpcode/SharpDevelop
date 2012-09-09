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
				new TestResultsMonitor(),
				UnitTestingOptions.Instance.Clone())
		{
		}
		
		public NUnitTestDebugger(IUnitTestDebuggerService debuggerService,
			IMessageService messageService,
			ITestResultsMonitor testResultsMonitor,
			UnitTestingOptions options)
			: base(debuggerService, messageService, testResultsMonitor)
		{
			this.options = options;
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests, options);
			app.Results = base.TestResultsMonitor.FileName;
			return app.GetProcessStartInfo();
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return new NUnitTestResult(testResult);
		}
	}
}
