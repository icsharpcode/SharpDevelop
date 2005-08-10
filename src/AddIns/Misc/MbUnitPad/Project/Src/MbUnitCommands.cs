// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using MbUnit.Forms;

namespace MbUnitPad
{
	public class ReloadCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			MbUnitPadContent.Instance.ReloadAssemblyList();
		}
	}
	
	public class UnloadCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			MbUnitPadContent.Instance.TreeView.RemoveAssemblies();
		}
	}
	
	public class RunTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			MbUnitPadContent.Instance.RunTests();
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
}
