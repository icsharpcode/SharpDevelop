// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Hornung.ResourceToolkit.Gui;
using Hornung.ResourceToolkit.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace Hornung.ResourceToolkit.Commands
{
	public static class FindMissingResourceKeysHelper
	{
		public static void Run(SearchScope scope) {
			// Allow the menu to close
			Application.DoEvents();
			using(AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:Hornung.ResourceToolkit.FindMissingResourceKeys}")) {
				FindReferencesAndRenameHelper.ShowAsSearchResults(StringParser.Parse("${res:Hornung.ResourceToolkit.ReferencesToMissingKeys}"),
				                                                  ResourceRefactoringService.FindReferencesToMissingKeys(monitor, scope));
			}
		}
	}
	
	/// <summary>
	/// Find missing resource keys in the whole solution.
	/// </summary>
	public class FindMissingResourceKeysWholeSolutionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindMissingResourceKeysHelper.Run(SearchScope.WholeSolution);
		}
	}
	
	/// <summary>
	/// Find missing resource keys in the current project.
	/// </summary>
	public class FindMissingResourceKeysCurrentProjectCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindMissingResourceKeysHelper.Run(SearchScope.CurrentProject);
		}
	}
	
	/// <summary>
	/// Find missing resource keys in the current file.
	/// </summary>
	public class FindMissingResourceKeysCurrentFileCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindMissingResourceKeysHelper.Run(SearchScope.CurrentFile);
		}
	}
	
	/// <summary>
	/// Find missing resource keys in all open files.
	/// </summary>
	public class FindMissingResourceKeysOpenFilesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindMissingResourceKeysHelper.Run(SearchScope.OpenFiles);
		}
	}
	
	/// <summary>
	/// Find unused resource keys in the whole solution.
	/// </summary>
	public class FindUnusedResourceKeysCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ICollection<ResourceItem> unusedKeys;
			
			// Allow the menu to close
			Application.DoEvents();
			using(AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:Hornung.ResourceToolkit.FindUnusedResourceKeys}")) {
				unusedKeys = ResourceRefactoringService.FindUnusedKeys(monitor);
			}
			
			if (unusedKeys == null) {
				return;
			}
			
			if (unusedKeys.Count == 0) {
				MessageService.ShowMessage("${res:Hornung.ResourceToolkit.UnusedResourceKeys.NotFound}");
				return;
			}
			
			IWorkbench workbench = WorkbenchSingleton.Workbench;
			if (workbench != null) {
				UnusedResourceKeysViewContent vc = new UnusedResourceKeysViewContent(unusedKeys);
				workbench.ShowView(vc);
			}
		}
	}
}
