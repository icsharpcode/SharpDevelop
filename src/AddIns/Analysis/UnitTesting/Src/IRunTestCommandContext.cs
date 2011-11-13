// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public interface IRunTestCommandContext
	{
		IRegisteredTestFrameworks RegisteredTestFrameworks { get; }
		IUnitTestTaskService TaskService { get; }
		IUnitTestWorkbench Workbench { get; }
		IBuildProjectFactory BuildProjectFactory { get; }
		IBuildOptions BuildOptions { get; }
		MessageViewCategory UnitTestCategory { get; }
		IUnitTestsPad OpenUnitTestsPad { get; }
		IUnitTestMessageService MessageService { get; }
		IUnitTestSaveAllFilesCommand SaveAllFilesCommand { get; }
		IStatusBarService StatusBarService { get; }
		Solution OpenSolution { get; }
	}
}
