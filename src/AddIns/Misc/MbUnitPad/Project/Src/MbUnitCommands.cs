// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using MbUnit.Forms;

namespace ICSharpCode.MbUnitPad
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
	
	public class RunTestInPadCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IMember m = MbUnitTestableCondition.GetMember(Owner);
			IClass c = (m != null) ? m.DeclaringType : MbUnitTestableCondition.GetClass(Owner);
			MessageService.ShowMessage("Not implemented");
		}
	}
	
	public class RunTestWithDebuggerCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!DebuggerService.IsDebuggerLoaded || DebuggerService.CurrentDebugger.IsDebugging) {
				MessageService.ShowMessage("The debugger is currently busy.");
				return;
			}
			IMember m = MbUnitTestableCondition.GetMember(Owner);
			IClass c = (m != null) ? m.DeclaringType : MbUnitTestableCondition.GetClass(Owner);
			if (m != null) {
				MessageService.ShowMessage("Running single tests is not implemented, run the test fixture instead.");
				return;
			}
			IProject project = c.ProjectContent.Project;
			if (project.Build().Errors.Count > 0) {
				return;
			}
			string mbUnitDir = Path.GetDirectoryName(typeof(ReflectorTreeView).Assembly.Location);
			ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(mbUnitDir, "MbUnit.Cons.exe"));
			string assemblyPath = project.OutputAssemblyFullPath;
			StringBuilder sb = new StringBuilder();
			sb.Append("\"/filter-type:" + c.FullyQualifiedName + "\"");
			sb.Append(" \"/assembly-path:" + Path.GetDirectoryName(assemblyPath) + "\"");
			sb.Append(" \"" + assemblyPath + "\"");
			startInfo.Arguments = sb.ToString();
			startInfo.WorkingDirectory = mbUnitDir;
			LoggingService.Info("Run " + startInfo.FileName + " " + startInfo.Arguments);
			DebuggerService.CurrentDebugger.Start(startInfo);
		}
	}
}
