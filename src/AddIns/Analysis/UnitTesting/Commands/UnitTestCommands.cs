// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class StopTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractRunTestCommand runTestCommand = AbstractRunTestCommand.RunningTestCommand;
			if (runTestCommand != null) {
				runTestCommand.Stop();
			}
		}
	}
	
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
				var member = treeView.SelectedMember;
				var c = treeView.SelectedClass;
				IEntity entity;
				if (member != null) {
					entity = member.Resolve(treeView.SelectedProject);
				} else if (c != null) {
					entity = c.Resolve(treeView.SelectedProject);
				} else {
					entity = null;
				}
				if (entity != null) {
					NavigationService.NavigateTo(entity);
				}
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
