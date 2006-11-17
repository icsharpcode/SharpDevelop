// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class ViewProjectOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			ShowProjectOptions(ProjectService.CurrentProject);
		}
		
		public static void ShowProjectOptions(IProject project)
		{
			if (project == null) {
				return;
			}
			foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
				ProjectOptionsView projectOptions = viewContent as ProjectOptionsView;
				if (projectOptions != null && projectOptions.Project == project) {
					projectOptions.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			try {
				AddInTreeNode projectOptionsNode = AddInTree.GetTreeNode("/SharpDevelop/BackendBindings/ProjectOptions/" + project.Language);
				ProjectOptionsView projectOptions = new ProjectOptionsView(projectOptionsNode, project);
				WorkbenchSingleton.Workbench.ShowView(projectOptions);
			} catch (TreePathNotFoundException) {
				MessageService.ShowError("${res:Dialog.ProjectOptions.NoPanelsInstalledForProject}");
			}
		}
	}
	
	public class GenerateProjectDocumentation : AbstractMenuCommand
	{
		public override void Run()
		{
			CompilableProject project = ProjectService.CurrentProject as CompilableProject;
			if (project == null) {
				return;
			}
			string assembly = project.OutputAssemblyFullPath;
			if (!File.Exists(assembly)) {
				MessageService.ShowMessage("${res:ProjectComponent.ContextMenu.GenerateDocumentation.ProjectNeedsToBeCompiled}");
				return;
			}
			string xmlDocFile = project.DocumentationFileFullPath;
			if (xmlDocFile == null) {
				MessageService.ShowMessage("${res:ProjectComponent.ContextMenu.GenerateDocumentation.NeedToEditBuildOptions}");
				return;
			}
			if (!File.Exists(xmlDocFile)) {
				MessageService.ShowMessage("${res:ProjectComponent.ContextMenu.GenerateDocumentation.ProjectNeedsToBeCompiled2}");
				return;
			}
			string nDocProjectFile = Path.ChangeExtension(assembly, ".ndoc");
			if (!File.Exists(nDocProjectFile)) {
				using (StreamWriter sw = File.CreateText(nDocProjectFile)) {
					sw.WriteLine("<project>");
					sw.WriteLine("    <assemblies>");
					sw.WriteLine("        <assembly location=\""+ assembly +"\" documentation=\"" + xmlDocFile + "\" />");
					sw.WriteLine("    </assemblies>");
					sw.WriteLine("</project>");
				}
			}
			
			string nDocDir = Path.Combine(FileUtility.ApplicationRootPath, "bin/Tools/NDoc");
			
			ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(nDocDir, "NDocGui.exe"), '"' + nDocProjectFile + '"');
			psi.WorkingDirectory = nDocDir;
			psi.UseShellExecute = false;
			Process.Start(psi);
		}
	}
}
