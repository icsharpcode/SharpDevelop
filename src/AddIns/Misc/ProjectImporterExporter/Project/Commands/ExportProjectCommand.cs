// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Xml.Xsl;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

using MSjogren.GacTool.FusionNative;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Converters;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Commands
{
	public class ExportProjectCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (ExportProjectDialog exportProjectDialog = new ExportProjectDialog("Visual Studio.NET 2003 Solutions")) {
				exportProjectDialog.Owner = (Form)WorkbenchSingleton.Workbench;
				exportProjectDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
	
	public class ExportProjectToCSharpCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(IProjectService));
			
			using (ExportProjectDialog exportProjectDialog = new ExportProjectDialog("Convert VB.NET to C#", projectService.CurrentSelectedProject != null ? projectService.CurrentSelectedProject.Name : null)) {
				exportProjectDialog.Owner = (Form)WorkbenchSingleton.Workbench;
				exportProjectDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
	
	public class ExportProjectToVBNetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(IProjectService));
			
			using (ExportProjectDialog exportProjectDialog = new ExportProjectDialog("Convert C# to VB.NET", projectService.CurrentSelectedProject != null ? projectService.CurrentSelectedProject.Name : null)) {
				exportProjectDialog.Owner = (Form)WorkbenchSingleton.Workbench;
				exportProjectDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
}
