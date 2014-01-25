// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Web.Services.Description;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public partial class WebProjectOptionsPanel : ProjectOptionPanel
	{
		WebProject webProject;
		WebProjectProperties properties;
		
		public WebProjectOptionsPanel()
		{
			InitializeComponent();
		}
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			CreateWebProject(project);
			if (!WebProjectService.IsIISOrIISExpressInstalled) {
				StatusLabel.Text = ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound");
			}
			
			if (properties.UseIISExpress) {
				UseIISExpress.IsChecked = true;
				PortTextBox.Text = properties.DevelopmentServerPort.ToString();
				ProjectUrl.Text = String.Empty;
				SelectIISExpress();
			} else if (properties.UseIIS) {
				if (WebProjectService.IISVersion != IISVersion.None) {
					UseLocalIIS.IsChecked = true;
					ProjectUrl.Text = properties.IISUrl;
					SelectLocalIIS();
				}
			}
		}
	
		void CreateWebProject(MSBuildBasedProject project)
		{
			webProject = new WebProject(project);
			properties = webProject.GetWebProjectProperties();
		}
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			if (IsDirty) {
				webProject.UpdateWebProjectProperties(properties);
			}
			IsDirty = false;
			return true;
		}
		
		void CreateVirtualDirectory_Click(object sender, RoutedEventArgs e)
		{
			try {
				IISAdministrator administrator = IISAdministrator.CreateAdministrator(properties);
				if (!administrator.IsIISInstalled()) {
					ShowIISNotFoundMessage();
					return;
				}
				
				administrator.CreateVirtualDirectory(webProject);
				MessageService.ShowMessage(ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.VirtualDirCreated"));
			} catch (Exception ex) {
				MessageService.ShowError(ex.Message);
			}
		}
		
		void ShowIISNotFoundMessage()
		{
			MessageService.ShowWarning(ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound"));
		}
		
		void UseIISExpress_Click(object sender, RoutedEventArgs e)
		{
			ProjectUrl.Text = String.Empty;
			properties.IISUrl = String.Format(@"http://localhost:{0}/", PortTextBox.Text);
			SelectIISExpress();
			OnWebProjectPropertiesChanged();
		}
		
		void SelectIISExpress()
		{
			properties.UseIISExpress = true;
			properties.UseIIS = false;
			properties.DevelopmentServerPort = Int32.Parse(PortTextBox.Text);
			bool isIISExpressInstalled = WebProjectService.IsIISExpressInstalled;
			
			if (!isIISExpressInstalled) {
				StatusLabel.Text = ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound");
			} else {
				StatusLabel.Text = String.Empty;
			}
			IISExpressGroup.IsEnabled = true;
			CreateVirtualDirectoryButton.IsEnabled = isIISExpressInstalled;
			LocalIISGroup.IsEnabled = false;
		}
		
		void UseLocalIIS_Click(object sender, RoutedEventArgs e)
		{
			properties.IISUrl = String.Format("{0}/{1}", WebBehavior.LocalHost, webProject.Name);
			SelectLocalIIS();
			OnWebProjectPropertiesChanged();
		}
		
		void SelectLocalIIS()
		{
			properties.UseIISExpress = false;
			properties.UseIIS = true;
			bool isIISInstalled = WebProjectService.IISVersion != IISVersion.None;
			
			if (!isIISInstalled) {
				StatusLabel.Text = ResourceService.GetString("ICSharpCode.WebProjectOptionsPanel.IISNotFound");
			} else {
				StatusLabel.Text = String.Empty;
				ProjectUrl.Text = properties.IISUrl;
			}
			LocalIISGroup.IsEnabled = true;
			CreateVirtualDirectoryButton.IsEnabled = isIISInstalled;
			IISExpressGroup.IsEnabled = false;
		}
		
		void ProjectUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			properties.IISUrl = ProjectUrl.Text;
			OnWebProjectPropertiesChanged();
		}
		
		void OnWebProjectPropertiesChanged()
		{
			IsDirty = true;
		}
		
		void ClearWebServerButton_Click(object sender, RoutedEventArgs e)
		{
			UseIISExpress.IsChecked = false;
			UseLocalIIS.IsChecked = false;
			CreateVirtualDirectoryButton.IsEnabled = false;
			ProjectUrl.Text = String.Empty;
			LocalIISGroup.IsEnabled = false;
			IISExpressGroup.IsEnabled = false;
			
			properties.UseIISExpress = false;
			properties.UseIIS = false;
			OnWebProjectPropertiesChanged();
		}
		
		bool AreAllValidNumericChars(string str)
		{
			foreach(char c in str) {
				if (!Char.IsNumber(c)) return false;
			}
			return true;
		}
		
		void PortTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (AreAllValidNumericChars(e.Text)) {
				OnWebProjectPropertiesChanged();
			} else {
				e.Handled = true;
			}
			base.OnPreviewTextInput(e);
		}
		
		void PortTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			int port;
			if (int.TryParse(PortTextBox.Text, out port))
				properties.DevelopmentServerPort = port;
			properties.IISUrl = String.Format(@"{0}:{1}/", WebBehavior.LocalHost, PortTextBox.Text);
		}
	}
}
