// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class TestProcessRunnerBaseContext
	{		
		IUnitTestProcessRunner processRunner;
		ITestResultsReader testResultsReader;
		IFileSystem fileSystem;
		IMessageService messageService;
		
		public TestProcessRunnerBaseContext()
			: this(new UnitTestProcessRunner(),
				new TestResultsReader(),
				new UnitTestFileService(),
				SD.MessageService)
		{
		}
				
		public TestProcessRunnerBaseContext(IUnitTestProcessRunner processRunner,
			ITestResultsReader testResultsMonitor,
			IFileSystem fileSystem,
			IMessageService messageService)
		{
			this.processRunner = processRunner;
			this.testResultsReader = testResultsMonitor;
			this.fileSystem = fileSystem;
			this.messageService = messageService;
		}
		
		public IUnitTestProcessRunner TestProcessRunner {
			get { return processRunner; }
		}
		
		public ITestResultsReader TestResultsReader {
			get { return testResultsReader; }
		}
		
		public IFileSystem FileSystem {
			get { return fileSystem; }
		}
		
		public IMessageService MessageService {
			get { return messageService; }
		}
	}
}
