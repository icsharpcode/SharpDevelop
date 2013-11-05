// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class AddNUnitReferenceCommand : AbstractMenuCommand
	{
		public void Run(IProject project)
		{
			if (project != null) {
				ReferenceProjectItem nunitRef = new ReferenceProjectItem(project, "nunit.framework");
				ProjectService.AddProjectItem(project, nunitRef);
				project.Save();
			}
		}
		
		public override void Run()
		{
			Run(ProjectService.CurrentProject);
		}
	}
	
	public class GotoDefinitionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestTreeView treeView = Owner as ITestTreeView;
			if (treeView != null) {
				ITest test = treeView.SelectedTests.FirstOrDefault();
				if (test != null && test.GoToDefinition.CanExecute(null))
					test.GoToDefinition.Execute(null);
			}
		}
	}
	
	public class CollapseAllTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!(this.Owner is SharpTreeView))
				return;
			
			var treeView = (SharpTreeView)this.Owner;
			if (treeView.Root != null) {
				foreach (var n in treeView.Root.Descendants())
					n.IsExpanded = false;
			}
		}
	}
}
