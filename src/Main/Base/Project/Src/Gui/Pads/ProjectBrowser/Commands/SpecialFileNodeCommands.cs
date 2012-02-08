// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class ViewInBrowser : AbstractMenuCommand
	{
		public override void Run()
		{
			var node  = ProjectBrowserPad.Instance.SelectedNode as FileNode;
			if (node == null) {
				return;
			}
			
			var project = ProjectService.CurrentProject as CompilableProject;
			if (project == null) {
				return;
			}
			
			if (!project.IsWebProject) {
				MessageService.ShowError("${res:ProjectComponent.ContextMenu.NotAWebProject}");
				return;
			}
			
			if (!WebProjectService.IsIISOrIISExpressInstalled) {
				MessageService.ShowError("${res:ICSharpCode.WepProjectOptionsPanel.IISNotFound}");
				return;
			}
			
			string fileName = node.FileName;
			
			// set project options
			project.StartAction = StartAction.StartURL;
			string directoryName = Path.GetDirectoryName(project.FileName) + "\\";
			project.StartUrl = fileName.Replace(directoryName, "/").Replace("\\", "/");
			
			// set web server options
			string projectName = project.Name;
			WebProjectOptions existingOptions = WebProjectsOptions.Instance.GetWebProjectOptions(projectName);
				
			var options = new WebProjectOptions {
				Data = new WebProjectDebugData {
					WebServer = WebProjectService.IsIISExpressInstalled ? WebServer.IISExpress : WebServer.IIS,
					Port = (existingOptions != null && existingOptions.Data != null) ? existingOptions.Data.Port : "8080", //TODO: port collision detection
					ProjectUrl = string.Format("{0}/{1}", CompilableProject.LocalHost, project.Name)				
				},
				ProjectName = projectName
			};
			
			if (options.Data.WebServer == WebServer.IISExpress) {
				options.Data.ProjectUrl = string.Format(
					@"{0}:{1}/{2}", CompilableProject.LocalHost, options.Data.Port, projectName);
			}
			
			WebProjectsOptions.Instance.SetWebProjectOptions(projectName, options);
			
			// create virtual directory
			string error = WebProjectService.CreateVirtualDirectory(
				options.Data.WebServer,
				projectName,
				Path.GetDirectoryName(ProjectService.CurrentProject.FileName));
			LoggingService.Info(error ?? string.Empty);
			
			// RunProject
			new RunProject().Run();
		}
	}
}
