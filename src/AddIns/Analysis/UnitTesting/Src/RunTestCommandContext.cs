// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class RunTestCommandContext : IRunTestCommandContext
	{
		UnitTestingOptions options = new UnitTestingOptions();
		IRegisteredTestFrameworks testFrameworks = TestService.RegisteredTestFrameworks;
		UnitTestTaskService taskService = new UnitTestTaskService();
		UnitTestWorkbench workbench = new UnitTestWorkbench();
		UnitTestBuildProjectFactory buildProjectFactory = new UnitTestBuildProjectFactory();
		UnitTestBuildOptions buildOptions = new UnitTestBuildOptions();
		MessageViewCategory unitTestCategory = TestService.UnitTestMessageView;
		IMessageService messageService = ServiceManager.Instance.MessageService;
		UnitTestSaveAllFilesCommand saveAllFilesCommand = new UnitTestSaveAllFilesCommand();
		IStatusBarService statusBarService = WorkbenchSingleton.StatusBar;
		
		public IRegisteredTestFrameworks RegisteredTestFrameworks {
			get { return testFrameworks; }
		}
		
		public IUnitTestTaskService TaskService {
			get { return taskService; }
		}
		
		public IUnitTestWorkbench Workbench {
			get { return workbench; }
		}
		
		public IBuildProjectFactory BuildProjectFactory {
			get { return buildProjectFactory; }
		}
		
		public IBuildOptions BuildOptions {
			get { return buildOptions; }
		}
		
		public MessageViewCategory UnitTestCategory {
			get { return unitTestCategory; }
		}
				
		public IUnitTestsPad OpenUnitTestsPad {
			get { return UnitTestsPad.Instance; }
		}
		
		public IMessageService MessageService {
			get { return messageService; }
		}
		
		public IUnitTestSaveAllFilesCommand SaveAllFilesCommand {
			get { return saveAllFilesCommand; }
		}
		
		public IStatusBarService StatusBarService {
			get { return statusBarService; }
		}
	}
}
