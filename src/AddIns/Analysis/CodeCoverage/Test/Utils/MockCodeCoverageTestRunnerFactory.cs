// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Utils
{
	public class MockCodeCoverageTestRunnerFactory : ICodeCoverageTestRunnerFactory
	{
		public MockProcessRunner ProcessRunner;
		public MockTestResultsMonitor TestResultsMonitor;
		public UnitTestingOptions Options;
		public CodeCoverageTestRunner TestRunner;
		public MockFileSystem FileSystem;
		public MockMessageService MessageService;
		
		public MockCodeCoverageTestRunnerFactory()
		{
			ProcessRunner = new MockProcessRunner();
			TestResultsMonitor = new MockTestResultsMonitor();
			Options = new UnitTestingOptions(new Properties());
			FileSystem = new MockFileSystem();
			CodeCoverageTestRunnerContext context = new CodeCoverageTestRunnerContext(ProcessRunner, 
				TestResultsMonitor, 
				FileSystem, 
				MessageService,
				Options);
			TestRunner = new CodeCoverageTestRunner(context);
		}
		
		public CodeCoverageTestRunner CreateCodeCoverageTestRunner()
		{
			return TestRunner;
		}
	}
}
