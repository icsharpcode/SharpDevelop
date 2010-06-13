// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockNUnitTestRunnerContext
	{
		MockProcessRunner processRunner = new MockProcessRunner();
		UnitTestingOptions options = new UnitTestingOptions(new Properties());
		MockTestResultsMonitor testResultsMonitor = new MockTestResultsMonitor();

		public MockNUnitTestRunnerContext() 
		{
			testResultsMonitor.FileName = @"c:\temp\tmp66.tmp";
		}
		
		public MockProcessRunner MockProcessRunner {
			get { return processRunner; }
		}
		
		public MockTestResultsMonitor MockTestResultsMonitor {
			get { return testResultsMonitor; }
		}
		
		public NUnitTestRunner CreateNUnitTestRunner()
		{
			return new NUnitTestRunner(processRunner, testResultsMonitor, options);
		}
	}
}
