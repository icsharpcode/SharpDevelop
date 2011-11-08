// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

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
		public MockMessageService MockMessageService = new MockMessageService();
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
		
		public IUnitTestMessageService MessageService {
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
