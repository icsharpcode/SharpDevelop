// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class TestProcessRunnerBaseContext
	{		
		IUnitTestProcessRunner processRunner;
		ITestResultsMonitor testResultsMonitor;
		IFileSystem fileSystem;
		IUnitTestMessageService messageService;
		
		public TestProcessRunnerBaseContext()
			: this(new UnitTestProcessRunner(),
				new TestResultsMonitor(),
				new UnitTestFileService(),
				new UnitTestMessageService())
		{
		}
				
		public TestProcessRunnerBaseContext(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			IFileSystem fileSystem,
			IUnitTestMessageService messageService)
		{
			this.processRunner = processRunner;
			this.testResultsMonitor = testResultsMonitor;
			this.fileSystem = fileSystem;
			this.messageService = messageService;
		}
		
		public IUnitTestProcessRunner TestProcessRunner {
			get { return processRunner; }
		}
		
		public ITestResultsMonitor TestResultsMonitor {
			get { return testResultsMonitor; }
		}
		
		public IFileSystem FileSystem {
			get { return fileSystem; }
		}
		
		public IUnitTestMessageService MessageService {
			get { return messageService; }
		}
	}
}
