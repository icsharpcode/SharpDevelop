// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Web.Services.Description;
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
			
			Loaded += OnLoaded;
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (!WebProjectService.IsIISInstalled) {
				StatusLabel.Text = ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.IISNotFound");
				return;
			}
			
			switch (CurrentProjectDebugData.WebServer)
			{
				case WebServer.IISExpress:
					if (WebProjectService.IISVersion == IISVersion.IISExpress) {
						UseIISExpress.IsChecked = true;
						PortTextBox.Text = CurrentProjectDebugData.Port ?? "8080";
						
						SelectIISExpress();
					}
					break;
				case WebServer.IIS:
					if (WebProjectService.IISVersion == IISVersion.IIS5 ||
					    WebProjectService.IISVersion == IISVersion.IIS6 ||
					    WebProjectService.IISVersion == IISVersion.IIS7 ||
					    WebProjectService.IISVersion == IISVersion.IIS_Future) {
						UseLocalIIS.IsChecked = true;
						ProjectUrl.Text = CurrentProjectDebugData.ProjectUrl ?? string.Empty;
						
						SelectLocalIIS();
					}
					break;
				default:
					// do nothing
					break;
			}
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
			SelectIISExpress();
		}
		
		void SelectIISExpress()
		{
			WebProjectDebugData data = new WebProjectDebugData();
			data.WebServer = WebServer.IISExpress;
			data.Port = PortTextBox.Text;
			data.ProjectUrl = string.Format(@"http://localhost:{0}/" + ProjectService.CurrentProject.Name, PortTextBox.Text);
			bool isIISExpressInstalled = WebProjectService.IISVersion == IISVersion.IISExpress;
			
			if (!isIISExpressInstalled) {
				UseIISExpress.IsChecked = false;
				data.WebServer = WebServer.None;
				StatusLabel.Text = ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.IISNotFound");
				data.ProjectUrl = string.Empty;
			}
			else
				StatusLabel.Text = string.Empty;
			
			IISExpressGroup.IsEnabled = CreateVirtualDirectoryButton.IsEnabled = isIISExpressInstalled;
			LocalIISGroup.IsEnabled = false;
			CurrentProjectDebugData = data;
		}
		
		void UseLocalIIS_Click(object sender, RoutedEventArgs e)
		{
			SelectLocalIIS();
		}
		
		void SelectLocalIIS()
		{
			WebProjectDebugData data = new WebProjectDebugData();
			data.WebServer = WebServer.IIS;
			data.Port = string.Empty;
			bool isIISInstalled = WebProjectService.IISVersion == IISVersion.IIS5 ||
				WebProjectService.IISVersion == IISVersion.IIS6 ||
				WebProjectService.IISVersion == IISVersion.IIS7;
			
			if (!isIISInstalled) {
				StatusLabel.Text = ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.IISNotFound");
				ProjectUrl.Text = string.Empty;
				data.WebServer = WebServer.None;
				UseLocalIIS.IsChecked = false;
			}
			else {
				StatusLabel.Text = string.Empty;
				ProjectUrl.Text = @"http://localhost/" + ProjectService.CurrentProject.Name;
			}
			
			data.ProjectUrl = ProjectUrl.Text;
			LocalIISGroup.IsEnabled = CreateVirtualDirectoryButton.IsEnabled = isIISInstalled;
			IISExpressGroup.IsEnabled = false;
			CurrentProjectDebugData = data;
		}
		
		void ProjectUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			WebProjectDebugData data = new WebProjectDebugData();
			data.WebServer = WebServer.IIS;
			data.ProjectUrl = ProjectUrl.Text;
			CurrentProjectDebugData = data;
		}
		
		void ClearWebServerButton_Click(object sender, RoutedEventArgs e)
		{
			UseIISExpress.IsChecked = false;
			UseLocalIIS.IsChecked = false;
			CreateVirtualDirectoryButton.IsEnabled = false;
			ProjectUrl.Text = string.Empty;
			LocalIISGroup.IsEnabled = false;
			IISExpressGroup.IsEnabled = false;
			
			WebProjectDebugData data = new WebProjectDebugData();
			data.WebServer = WebServer.None;
			data.ProjectUrl = string.Empty;
			
			CurrentProjectDebugData = data;
		}
		
		bool AreAllValidNumericChars(string str)
		{
			foreach(char c in str)
			{
				if(!Char.IsNumber(c)) return false;
			}

			return true;
		}
		
		void PortTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = !AreAllValidNumericChars(e.Text);
			base.OnPreviewTextInput(e);
		}
		
		void PortTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			WebProjectDebugData data = new WebProjectDebugData();
			data.WebServer = WebServer.IISExpress;
			data.Port = PortTextBox.Text;
			data.ProjectUrl = string.Format(@"http://localhost:{0}/" + ProjectService.CurrentProject.Name, PortTextBox.Text);
			CurrentProjectDebugData = data;
		}
	}
}