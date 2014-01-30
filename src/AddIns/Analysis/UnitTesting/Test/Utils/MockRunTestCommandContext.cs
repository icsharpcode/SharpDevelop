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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using Rhino.Mocks;

namespace UnitTesting.Tests.Utils
{
	public class MockRunTestCommandContext : IRunTestCommandContext
	{
		public UnitTestingOptions UnitTestingOptions = new UnitTestingOptions(new Properties());
		public MockRegisteredTestFrameworks MockRegisteredTestFrameworks = new MockRegisteredTestFrameworks();
		public MockTestResultsMonitor MockTestResultsMonitor = new MockTestResultsMonitor();
		public MockTaskService MockTaskService = new MockTaskService();
		public MockUnitTestWorkbench MockUnitTestWorkbench = new MockUnitTestWorkbench();
		public MockBuildProjectFactory MockBuildProjectFactory = new MockBuildProjectFactory();
		public MockBuildOptions MockBuildOptions = new MockBuildOptions();
		public MockUnitTestsPad MockUnitTestsPad = new MockUnitTestsPad();
		public IMessageService MockMessageService = MockRepository.GenerateStrictMock<IMessageService>();
		public MockSaveAllFilesCommand MockSaveAllFilesCommand = new MockSaveAllFilesCommand();
		public MockStatusBarService MockStatusBarService = new MockStatusBarService();
		public MessageViewCategory UnitTestMessageViewCategory = new MessageViewCategory("Unit Tests");
		public Solution Solution;
		
		public IRegisteredTestFrameworks RegisteredTestFrameworks {
			get { return MockRegisteredTestFrameworks; }
		}

		public IUnitTestTaskService TaskService {
			get { return MockTaskService; }
		}
		
		public IUnitTestWorkbench Workbench {
			get { return MockUnitTestWorkbench; }
		}
		
		public IBuildProjectFactory BuildProjectFactory {
			get { return MockBuildProjectFactory; }
		}

		public IBuildOptions BuildOptions {
			get { return MockBuildOptions; }
		}
		
		public MessageViewCategory UnitTestCategory {
			get { return UnitTestMessageViewCategory; }
		}
		
		public IUnitTestsPad OpenUnitTestsPad {
			get { return MockUnitTestsPad; }
		}
		
		public IMessageService MessageService {
			get { return MockMessageService; }
		}

		public IUnitTestSaveAllFilesCommand SaveAllFilesCommand {
			get { return MockSaveAllFilesCommand; }
		}
		
		public IStatusBarService StatusBarService {
			get { return MockStatusBarService; }
		}
		
		public Solution OpenSolution {
			get { return Solution; }
		}
	}
}
