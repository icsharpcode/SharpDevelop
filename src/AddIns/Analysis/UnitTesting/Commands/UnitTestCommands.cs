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
		IFileService fileService;
		
		public GotoDefinitionCommand()
			: this(SD.FileService)
		{
		}
		
		public GotoDefinitionCommand(IFileService fileService)
		{
			this.fileService = fileService;
		}
		
		public override void Run()
		{
			ITestTreeView treeView = Owner as ITestTreeView;
			if (treeView != null) {
				IMethod method = treeView.SelectedMethod.Resolve();
				ITypeDefinition c = treeView.SelectedClass;
				if (method != null) {
					GotoMember(method);
				} else if (c != null) {
					GotoClass(c);
				}
			}
		}
		
		void GotoMember(IMember member)
		{
			GotoFilePosition(member.Region);
		}
		
		void GotoClass(ITypeDefinition c)
		{
			GotoFilePosition(c.Region);
		}
		
		void GotoFilePosition(DomRegion filePosition)
		{
			if (filePosition.IsEmpty) {
				fileService.OpenFile(new FileName(filePosition.FileName));
			} else {
				fileService.JumpToFilePosition(new FileName(filePosition.FileName), filePosition.BeginLine, filePosition.BeginColumn);
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
			NRefactory.Utils.TreeTraversal.PreOrder(treeView.Root, n => n.Children).ForEach(n => n.IsExpanded = false);
			
			if (treeView.Root.Children.Count > 0) {
				treeView.Root.IsExpanded = true;
			}
		}
	}
}
