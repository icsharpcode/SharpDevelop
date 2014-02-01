// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
