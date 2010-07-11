// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestRunner : TestProcessRunnerBase
	{
		RubyAddInOptions options;
		IRubyFileService fileService;
		RubyTestRunnerApplication testRunnerApplication;
		
		public RubyTestRunner()
			: this(new UnitTestProcessRunner(),
				new TestResultsMonitor(),
				new RubyAddInOptions(),
				new RubyFileService())
		{
		}
		
		public RubyTestRunner(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			RubyAddInOptions options,
			IRubyFileService fileService)
			: base(processRunner, testResultsMonitor)
		{
			this.options = options;
			this.fileService = fileService;
			testResultsMonitor.InitialFilePosition = 0;
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
