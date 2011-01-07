// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class WebProjectOptionsPanel : UserControl
	{
		private readonly DebugOptions parentPanel;
		
		public WebProjectOptionsPanel(DebugOptions parentPanel)
		{
			InitializeComponent();
			
			this.parentPanel = parentPanel;
			
			if (CurrentProjectDebugData == null)
				CurrentProjectDebugData = new WebProjectDebugData();
			
			Loaded += delegate(object sender, RoutedEventArgs e) {
				switch (CurrentProjectDebugData.WebServer)
				{
					case WebServer.IISExpress:
						UseIISExpress.IsChecked = true;
						UseIISExpress_Click(null, null);
						break;
					case WebServer.IIS:
						UseLocalIIS.IsChecked = true;
						ProjectUrl.Text = CurrentProjectDebugData.ProjectUrl ?? string.Empty;
						UseLocalIIS_Click(null, null);
						break;
					default:
						// do nothing
						break;
				}
			};
		}
		
		WebProjectDebugData CurrentProjectDebugData {
			get {
				var data = WebProjectsOptions.Instance.GetWebProjectOptions(ProjectService.CurrentProject.Name);
				if (data != null)
					return data.Data;
				
				return null;
			}
			set {
				WebProjectOptions data;
				if (value != null)
				{
					data = new WebProjectOptions() {
						ProjectName = ProjectService.CurrentProject.Name,
						Data = value
					};
				}
				else
					data = new WebProjectOptions();
				
				WebProjectsOptions.Instance.SetWebProjectOptions(ProjectService.CurrentProject.Name, data);				
			}
		}
		
		void CreateVirtualDirectory_Click(object sender, RoutedEventArgs e)
		{
			string error = WebProjectService.CreateVirtualDirectory(ProjectService.CurrentProject.Name,
			                                                        Path.GetDirectoryName(ProjectService.CurrentProject.FileName));
			
			if (!string.IsNullOrEmpty(error))
				MessageService.ShowError(error);
			else {
				MessageService.ShowMessage(ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.VirtualDirCreated"));
			}
		}
		
		void UseIISExpress_Click(object sender, RoutedEventArgs e)
		{
			WebProjectDebugData data = new WebProjectDebugData();
			data.WebServer = WebServer.IISExpress;
			
			if (ProjectService.CurrentProject is CompilableProject) {
				((CompilableProject)ProjectService.CurrentProject).StartAction = StartAction.Program;
				parentPanel.SetStartAction(StartAction.Program);
			}
			
			LocalIISGroup.IsEnabled = false;
			CurrentProjectDebugData = data;
		}
		
		void UseLocalIIS_Click(object sender, RoutedEventArgs e)
		{
			WebProjectDebugData data = new WebProjectDebugData();
			data.WebServer = WebServer.IIS;
			
			if (!WebProjectService.IsIISInstalled) {
				StatusLabel.Text = ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.IISNotFound");
				ProjectUrl.Text = string.Empty;
			}
			else {
				StatusLabel.Text = string.Empty;
				ProjectUrl.Text = @"http://localhost/" + ProjectService.CurrentProject.Name;
			}
			
			data.ProjectUrl = ProjectUrl.Text;
			CreateVirtualDirectoryButton.IsEnabled = WebProjectService.IsIISInstalled;
			ProjectUrl.IsEnabled = WebProjectService.IsIISInstalled;
			
			if (ProjectService.CurrentProject is CompilableProject) {
				((CompilableProject)ProjectService.CurrentProject).StartAction = StartAction.Project;
				parentPanel.SetStartAction(StartAction.Project);
			}
			
			LocalIISGroup.IsEnabled = true;
			CurrentProjectDebugData = data;
		}
		
		void ProjectUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			WebProjectDebugData data = new WebProjectDebugData();
			data.WebServer = WebServer.IIS;
			data.ProjectUrl = ProjectUrl.Text;
			CurrentProjectDebugData = data;
		}
	}
}