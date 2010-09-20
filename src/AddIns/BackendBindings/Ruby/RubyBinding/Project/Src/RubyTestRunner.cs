// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestRunner : TestProcessRunnerBase
	{
		RubyAddInOptions options;
		IScriptingFileService fileService;
		RubyTestRunnerApplication testRunnerApplication;
		
		public RubyTestRunner()
			: this(new RubyTestRunnerContext())
		{
		}
		
		public RubyTestRunner(RubyTestRunnerContext context)
			: base(context)
		{
			this.options = context.Options;
			this.fileService = context.ScriptingFileService;
			context.TestResultsMonitor.InitialFilePosition = 0;
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			CreateTestRunnerApplication();
			testRunnerApplication.CreateResponseFile(selectedTests);
			base.Start(selectedTests);
		}
		
		void CreateTestRunnerApplication()
		{
			testRunnerApplication = new RubyTestRunnerApplication(base.TestResultsMonitor.FileName, options, fileService);
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			return testRunnerApplication.CreateProcessStartInfo(selectedTests);
		}
		
		public override void Dispose()
		{
			testRunnerApplication.Dispose();
			base.Dispose();
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return new RubyTestResult(testResult);
		}
	}
}
