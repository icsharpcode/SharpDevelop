// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
