// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using Rhino.Mocks;

namespace UnitTesting.Tests.Utils
{
	public class MockNUnitTestRunnerContext : TestProcessRunnerBaseContext
	{
		public UnitTestingOptions options = new UnitTestingOptions(new Properties());
		
		public MockProcessRunner MockProcessRunner {
			get { return base.TestProcessRunner as MockProcessRunner; }
		}
		
		public MockTestResultsMonitor MockTestResultsMonitor {
			get { return base.TestResultsMonitor as MockTestResultsMonitor; }
		}
		
		public MockFileService MockFileService {
			get { return base.FileSystem as MockFileService; }
		}
		
		public MockNUnitTestRunnerContext()
			: base(new MockProcessRunner(),
				new MockTestResultsMonitor(), 
				new MockFileService(), 
				MockRepository.GenerateStrictMock<IMessageService>())
		{
			MockTestResultsMonitor.FileName = @"c:\temp\tmp66.tmp";
		}
		
		public NUnitTestRunner CreateNUnitTestRunner()
		{
			return new NUnitTestRunner(this, options);
		}
	}
}
