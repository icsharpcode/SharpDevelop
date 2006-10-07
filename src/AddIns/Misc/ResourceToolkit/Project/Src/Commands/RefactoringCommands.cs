// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

using Hornung.ResourceToolkit.Gui;
using Hornung.ResourceToolkit.Refactoring;

namespace Hornung.ResourceToolkit.Commands
{
	/// <summary>
	/// Find missing resource keys in the whole solution.
	/// </summary>
	public class FindMissingResourceKeysCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindReferencesAndRenameHelper.ShowAsSearchResults(StringParser.Parse("${res:Hornung.ResourceToolkit.ReferencesToMissingKeys}"),
			                                                  ResourceRefactoringService.FindReferencesToMissingKeys());
		}
	}
	
	/// <summary>
	/// Find unused resource keys in the whole solution.
	/// </summary>
	public class FindUnusedResourceKeysCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ICollection<ResourceItem> unusedKeys = ResourceRefactoringService.FindUnusedKeys();
			
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
