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

namespace ICSharpCode.PythonBinding
{
	public class PythonTestRunner : TestProcessRunnerBase
	{
		PythonAddInOptions options;
		PythonStandardLibraryPath pythonStandardLibraryPath;
		IPythonFileService fileService;
		PythonTestRunnerApplication testRunnerApplication;
		
		public PythonTestRunner()
			: this(new UnitTestProcessRunner(),
				new TestResultsMonitor(),
				new PythonAddInOptions(),
				new PythonStandardLibraryPath(),
				new PythonFileService())
		{
		}
		
		public PythonTestRunner(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			PythonAddInOptions options,
			PythonStandardLibraryPath pythonStandardLibraryPath,
			IPythonFileService fileService)
			: base(processRunner, testResultsMonitor)
		{
			this.options = options;
			this.pythonStandardLibraryPath = pythonStandardLibraryPath;
			this.fileService = fileService;
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			CreateTestRunnerApplication();
			testRunnerApplication.CreateResponseFile(selectedTests);
			base.Start(selectedTests);
		}
		
		void CreateTestRunnerApplication()
		{
			testRunnerApplication = new PythonTestRunnerApplication(base.TestResultsMonitor.FileName, options, pythonStandardLibraryPath, fileService);
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
			return new PythonTestResult(testResult);
		}
	}
}
