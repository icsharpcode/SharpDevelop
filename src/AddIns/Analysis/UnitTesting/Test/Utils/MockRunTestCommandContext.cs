// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockRunTestCommandContext : IRunTestCommandContext
	{
		UnitTestingOptions options = new UnitTestingOptions(new Properties());
		MockRegisteredTestFrameworks testFrameworks = new MockRegisteredTestFrameworks();
		MockTestResultsMonitor testResultsMonitor = new MockTestResultsMonitor();
		MockTaskService taskService = new MockTaskService();
		MockUnitTestWorkbench workbench = new MockUnitTestWorkbench();
		MockBuildProjectFactory buildProjectFactory = new MockBuildProjectFactory();
		MockBuildOptions buildOptions = new MockBuildOptions();
		MessageViewCategory unitTestCategory = new MessageViewCategory("Unit Tests");
		MockUnitTestsPad unitTestsPad = new MockUnitTestsPad();
		MockMessageService messageService = new MockMessageService();
		MockSaveAllFilesCommand saveAllFilesCommand = new MockSaveAllFilesCommand();
		
		public UnitTestingOptions UnitTestingOptions {
			get { return options; }
		}
		
		public IRegisteredTestFrameworks RegisteredTestFrameworks {
			get { return testFrameworks; }
		}
		
		public MockRegisteredTestFrameworks MockRegisteredTestFrameworks {
			get { return testFrameworks; }
		}
		
		public MockTestResultsMonitor MockTestResultsMonitor {
			get { return testResultsMonitor; }
		}
		
		public IUnitTestTaskService TaskService {
			get { return taskService; }
		}
		
		public MockTaskService MockTaskService {
			get { return taskService; }
		}
		
		public IUnitTestWorkbench Workbench {
			get { return workbench; }
		}
		
		public MockUnitTestWorkbench MockUnitTestWorkbench {
			get { return workbench; }
		}
		
		public IBuildProjectFactory BuildProjectFactory {
			get { return buildProjectFactory; }
		}
		
		public MockBuildProjectFactory MockBuildProjectFactory {
			get { return buildProjectFactory; }
		}
		
		public IBuildOptions BuildOptions {
			get { return buildOptions; }
		}
		
		public MockBuildOptions MockBuildOptions {
			get { return buildOptions; }
		}
		
		public MessageViewCategory UnitTestCategory {
			get { return unitTestCategory; }
		}
		
		public IUnitTestsPad OpenUnitTestsPad {
			get { return unitTestsPad; }
		}
		
		public MockUnitTestsPad MockUnitTestsPad {
			get { return unitTestsPad; }
			set { unitTestsPad = value; }
		}
		
		public IUnitTestMessageService MessageService {
			get { return messageService; }
		}
		
		public MockMessageService MockMessageService {
			get { return messageService; }
		}
		
		public IUnitTestSaveAllFilesCommand SaveAllFilesCommand {
			get { return saveAllFilesCommand; }
		}
		
		public MockSaveAllFilesCommand MockSaveAllFilesCommand {
			get { return saveAllFilesCommand; }
		}
	}
}
