// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
		public override void Run()
		{
			if (ProjectService.CurrentProject != null) {
				ProjectService.AddProjectItem(ProjectService.CurrentProject, new ReferenceProjectItem(ProjectService.CurrentProject, "nunit.framework"));
				ProjectService.CurrentProject.Save();
			}
		}
	}
	
	public class GotoDefinitionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestTreeView treeView = Owner as ITestTreeView;
			if (treeView != null) {
				IMember member = treeView.SelectedMethod;
				IClass c = treeView.SelectedClass;
				if (member != null) {
					BaseTestMethod baseTestMethod = member as BaseTestMethod;
					if (baseTestMethod != null) {
						member = baseTestMethod.Method;
					}
					GotoMember(member);
				} else if (c != null) {
					GotoClass(c);
				}
			}
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
				FileService.OpenFile(filePosition.FileName);
			} else {
				FileService.JumpToFilePosition(filePosition.FileName, filePosition.Line - 1, filePosition.Column - 1);
			}
		}
	}
}
