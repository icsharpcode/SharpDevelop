// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using ICSharpCode.Scripting;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestRunner : TestProcessRunnerBase
	{
		PythonAddInOptions options;
		PythonStandardLibraryPath pythonStandardLibraryPath;
		IScriptingFileService fileService;
		PythonTestRunnerApplication testRunnerApplication;
		
		public PythonTestRunner()
			: this(new PythonTestRunnerContext())
		{
		}
		
		public PythonTestRunner(PythonTestRunnerContext context)
			: base(context)
		{
			this.options = context.Options;
			this.pythonStandardLibraryPath = context.PythonStandardLibraryPath;
			this.fileService = context.ScriptingFileService;
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
