// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	public class TestProcessRunnerBaseContext
	{
		TestExecutionOptions executionOptions;
		IProcessRunner processRunner;
		ITestResultsReader testResultsReader;
		IFileSystem fileSystem;
		IMessageService messageService;
		
		public TestProcessRunnerBaseContext(TestExecutionOptions executionOptions)
			: this(executionOptions,
				executionOptions.ProcessRunner ?? new ProcessRunner(),
				new TestResultsReader(),
				SD.FileSystem,
				SD.MessageService)
		{
		}
		
		public TestProcessRunnerBaseContext(TestExecutionOptions executionOptions,
			IProcessRunner processRunner,
			ITestResultsReader testResultsMonitor,
			IFileSystem fileSystem,
			IMessageService messageService)
		{
			this.executionOptions = executionOptions;
			this.processRunner = processRunner;
			this.testResultsReader = testResultsMonitor;
			this.fileSystem = fileSystem;
			this.messageService = messageService;
		}
		
		public TestExecutionOptions ExecutionOptions {
			get { return executionOptions; }
		}
		
		public IProcessRunner TestProcessRunner {
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
