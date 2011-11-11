// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

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
		IUnitTestFileService fileService;
		
		public GotoDefinitionCommand()
			: this(new UnitTestFileService())
		{
		}
		
		public GotoDefinitionCommand(IUnitTestFileService fileService)
		{
			this.fileService = fileService;
		}
		
		public override void Run()
		{
			ITestTreeView treeView = Owner as ITestTreeView;
			if (treeView != null) {
				IMember member = GetMember(treeView);
				IClass c = treeView.SelectedClass;
				if (member != null) {
					GotoMember(member);
				} else if (c != null) {
					GotoClass(c);
				}
			}
		}
		
		IMember GetMember(ITestTreeView treeView)
		{
			IMember member = treeView.SelectedMember;
			if (member != null) {
				BaseTestMember baseTestMethod = member as BaseTestMember;
				if (baseTestMethod != null) {
					return baseTestMethod.Member;
				}
			}
			return member;
		}
		
		void GotoMember(IMember member)
		{
			MemberResolveResult resolveResult = new MemberResolveResult(null, null, member);
			GotoFilePosition(resolveResult.GetDefinitionPosition());
		}
		
		void GotoClass(IClass c)
		{
			TypeResolveResult resolveResult = new TypeResolveResult(null, null, c);
			GotoFilePosition(resolveResult.GetDefinitionPosition());
		}
		
		void GotoFilePosition(FilePosition filePosition)
		{
			if (filePosition.Position.IsEmpty) {
				fileService.OpenFile(filePosition.FileName);
			} else {
				fileService.JumpToFilePosition(filePosition.FileName, filePosition.Line - 1, filePosition.Column - 1);
			}
		}
	}
	
	public class CollapseAllTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!(this.Owner is TreeView))
				return;
			
			var treeView = (TreeView)this.Owner;
			treeView.CollapseAll();
			
			if (treeView.Nodes.Count > 0) {
				treeView.Nodes[0].Expand();
			}
		}
	}
}
