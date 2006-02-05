// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class ReloadCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadContent.Instance.ReloadAssemblyList(null);
		}
	}
	
	public class UnloadCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadContent.Instance.UnloadTests();
		}
	}
	
	public class RunTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadContent.Instance.RunTests();
		}
	}
	
	public class StopTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadContent.Instance.StopTests();
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
	
	public class AddMbUnitReferenceCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.CurrentProject != null) {
				ProjectService.AddProjectItem(ProjectService.CurrentProject, new ReferenceProjectItem(ProjectService.CurrentProject, "MbUnit.Framework"));
				ProjectService.CurrentProject.Save();
			}
		}
	}
	
	public class GotoDefinitionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadContent.Instance.TreeView.GotoDefinition();
		}
	}
	
	public class ExpandAllCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadContent.Instance.TreeView.ExpandAll();
		}
	}
	
	public class CollapseAllCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadContent.Instance.TreeView.CollapseAll();
		}
	}
}
