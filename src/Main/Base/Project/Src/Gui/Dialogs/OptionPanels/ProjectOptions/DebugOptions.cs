// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Project;
using RadioBinding = System.Collections.Generic.KeyValuePair<ICSharpCode.SharpDevelop.Project.StartAction, System.Windows.Forms.RadioButton>;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class DebugOptions : AbstractXmlFormsProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.DebugOptions.xfrm");
			ConnectBrowseButton("startExternalProgramBrowseButton", "startExternalProgramTextBox",
			                    "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe;*.com;*.pif;*.bat;*.cmd",
			                    TextBoxEditMode.EditRawProperty);
			ConnectBrowseFolder("workingDirectoryBrowseButton", "workingDirectoryTextBox",
			                    TextBoxEditMode.EditRawProperty);
			
			InitializeHelper();
			
			ConfigurationGuiBinding b;
			ChooseStorageLocationButton locationButton;
			
			b = helper.BindRadioEnum("StartAction",
			                         new RadioBinding(StartAction.Project, Get<RadioButton>("startProject")),
			                         new RadioBinding(StartAction.Program, Get<RadioButton>("startExternalProgram")),
			                         new RadioBinding(StartAction.StartURL, Get<RadioButton>("startBrowserInURL")));
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			locationButton = b.CreateLocationButtonInPanel("startActionGroupBox");
			
			b = helper.BindString("startExternalProgramTextBox", "StartProgram", TextBoxEditMode.EditRawProperty);
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			b.RegisterLocationButton(locationButton);
			
			b = helper.BindString("startBrowserInURLTextBox", "StartURL", TextBoxEditMode.EditRawProperty);
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			b.RegisterLocationButton(locationButton);
			
			Get<RadioButton>("startExternalProgram").CheckedChanged += UpdateEnabledStates;
			Get<RadioButton>("startBrowserInURL").CheckedChanged += UpdateEnabledStates;
			
			b = helper.BindString("commandLineArgumentsTextBox", "StartArguments", TextBoxEditMode.EditRawProperty);
			locationButton = b.CreateLocationButtonInPanel("startOptionsGroupBox");
			b = helper.BindString("workingDirectoryTextBox", "StartWorkingDirectory", TextBoxEditMode.EditRawProperty);
			b.RegisterLocationButton(locationButton);
			
			UpdateEnabledStates(this, EventArgs.Empty);
			
			helper.AddConfigurationSelector(this);
			
			// add server for web projects
			if (ProjectService.CurrentProject is CompilableProject) {
				bool isWebProject = ((CompilableProject)ProjectService.CurrentProject).IsWebProject;
				if (isWebProject) {
					ElementHost host = new ElementHost();
					host.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
					host.Height = 175;
					host.Width = 550;
					host.Top = 240;
					host.Left = 8;					
					host.Child = new WebProjectOptionsPanel(this);
					Controls.Add(host);
				}
			}		
		}
		
		void UpdateEnabledStates(object sender, EventArgs e)
		{
			Get<TextBox>("startExternalProgram").Enabled = Get<Button>("startExternalProgramBrowse").Enabled = Get<RadioButton>("startExternalProgram").Checked;
			Get<TextBox>("startBrowserInURL").Enabled    = Get<RadioButton>("startBrowserInURL").Checked;
		}
		
		public void SetStartAction(StartAction action) 
		{
			switch(action) {
				case StartAction.Project:
					Get<RadioButton>("startProject").Checked = true;
					break;
				case StartAction.Program:
					Get<RadioButton>("startExternalProgram").Checked = true;
					break;
				case StartAction.StartURL:
					Get<RadioButton>("startBrowserInURL").Checked = true;
					break;
				default:
					throw new NotSupportedException("Unknown action!");
			}
			
			UpdateEnabledStates(null, EventArgs.Empty);
		}
		
		public void SetExternalProgram(string externalProgram)
		{
			if (externalProgram == null)
				return;
			
			Get<TextBox>("startExternalProgram").Text = externalProgram;
		}
	}
}
