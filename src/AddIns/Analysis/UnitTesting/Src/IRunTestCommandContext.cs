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
	}
}
