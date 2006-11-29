// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Hornung.ResourceToolkit.Gui;
using Hornung.ResourceToolkit.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;
using SearchAndReplace;

namespace Hornung.ResourceToolkit.Commands
{
	/// <summary>
	/// Find missing resource keys in the whole solution.
	/// </summary>
	public class FindMissingResourceKeysCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			// Allow the menu to close
			Application.DoEvents();
			using(AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:Hornung.ResourceToolkit.FindMissingResourceKeys}")) {
				FindReferencesAndRenameHelper.ShowAsSearchResults(StringParser.Parse("${res:Hornung.ResourceToolkit.ReferencesToMissingKeys}"),
				                                                  ResourceRefactoringService.FindReferencesToMissingKeys(monitor));
			}
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
