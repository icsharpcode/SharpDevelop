using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
	
namespace CSharpBinding.OptionPanels
{
	public class DebugOptions : AbstractOptionPanel
	{
		CSharpProject project;
		
		string Config   = "Debug";
		string Platform = "AnyCPU";
		
		public DebugOptions()
		{
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.DebugOptions.xfrm"));
			ConnectBrowseButton("startExternalProgramBrowseButton", "startExternalProgramTextBox", "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			ConnectBrowseFolder("workingDirectoryBrowseButton", "workingDirectoryTextBox");
			this.project = (CSharpProject)((Properties)CustomizationObject).Get("Project");
			
			StartAction startAction = project.GetStartAction(Config, Platform);
			
			Get<RadioButton>("startProject").Checked         = startAction == StartAction.Project;
			Get<RadioButton>("startProject").CheckedChanged += new EventHandler(UpdateEnabledStates);
			Get<RadioButton>("startProject").CheckedChanged += new EventHandler(Save);
			
			Get<RadioButton>("startExternalProgram").Checked = startAction == StartAction.Program;
			Get<RadioButton>("startExternalProgram").CheckedChanged += new EventHandler(UpdateEnabledStates);
			Get<RadioButton>("startExternalProgram").CheckedChanged += new EventHandler(Save);
			
			Get<RadioButton>("startBrowserInURL").Checked    = startAction == StartAction.StartURL;
			Get<RadioButton>("startBrowserInURL").CheckedChanged += new EventHandler(UpdateEnabledStates);
			Get<RadioButton>("startBrowserInURL").CheckedChanged += new EventHandler(Save);
			
			Get<TextBox>("startExternalProgram").Text = project.GetStartProgram(Config, Platform);
			Get<TextBox>("startExternalProgram").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("startBrowserInURL").Text    = project.GetStartURL(Config, Platform);
			Get<TextBox>("startBrowserInURL").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("commandLineArguments").Text = project.GetStartArguments(Config, Platform);
			Get<TextBox>("commandLineArguments").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("workingDirectory").Text     = project.GetStartWorkingDirectory(Config, Platform);
			Get<TextBox>("workingDirectory").TextChanged += new EventHandler(Save);
			
			UpdateEnabledStates(this, EventArgs.Empty);
		}
		
		void Save(object sender, EventArgs e) 
		{
			StorePanelContents();
		}
		
		void UpdateEnabledStates(object sender, EventArgs e)
		{
			Get<TextBox>("startExternalProgram").Enabled = Get<Button>("startExternalProgramBrowse").Enabled = Get<RadioButton>("startExternalProgram").Checked;
			Get<TextBox>("startBrowserInURL").Enabled    = Get<RadioButton>("startBrowserInURL").Checked;
		}
		
		public override bool StorePanelContents()
		{
			project.SetStartProgram(Config, Platform, Get<TextBox>("startExternalProgram").Text);
			project.SetStartURL(Config, Platform, Get<TextBox>("startBrowserInURL").Text);
			project.SetStartArguments(Config, Platform, Get<TextBox>("commandLineArguments").Text);
			project.SetStartWorkingDirectory(Config, Platform, Get<TextBox>("workingDirectory").Text);
			
			if (Get<RadioButton>("startProject").Checked) {
				project.SetStartAction(Config, Platform, StartAction.Project);
			} else if (Get<RadioButton>("startExternalProgram").Checked) {
				project.SetStartAction(Config, Platform, StartAction.Program);
			} else {
				project.SetStartAction(Config, Platform, StartAction.StartURL);
			}
			project.Save();
			
			return true;
		}
	}
}
