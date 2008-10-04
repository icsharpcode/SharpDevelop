// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Project;
using RadioBinding = System.Collections.Generic.KeyValuePair<ICSharpCode.SharpDevelop.Project.StartAction, System.Windows.Forms.RadioButton>;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class DebugOptions : AbstractProjectOptionPanel
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
		}
		
		void UpdateEnabledStates(object sender, EventArgs e)
		{
			Get<TextBox>("startExternalProgram").Enabled = Get<Button>("startExternalProgramBrowse").Enabled = Get<RadioButton>("startExternalProgram").Checked;
			Get<TextBox>("startBrowserInURL").Enabled    = Get<RadioButton>("startBrowserInURL").Checked;
		}
	}
}
