// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

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
		UnitTestMessageService messageService = new UnitTestMessageService();
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
		
		public IUnitTestMessageService MessageService {
			get { return messageService; }
		}
		
		public IUnitTestSaveAllFilesCommand SaveAllFilesCommand {
			get { return saveAllFilesCommand; }
		}
		
		public IStatusBarService StatusBarService {
			get { return statusBarService; }
		}
		
		public Solution OpenSolution {
			get { return ProjectService.OpenSolution; }
		}
	}
}
