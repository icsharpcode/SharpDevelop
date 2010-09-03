// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class ApplicationSettingsPanel : AbstractXmlFormsProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.WixBinding.Resources.ApplicationSettingsPanel.xfrm"));
			InitializeHelper();
			
			ConfigurationGuiBinding b;
			b = helper.BindString("outputNameTextBox", "OutputName", TextBoxEditMode.EditEvaluatedProperty);
			b.CreateLocationButton("outputNameTextBox");
			Get<TextBox>("outputName").TextChanged += RefreshOutputFileNameTextBox;
						
			b = helper.BindEnum<WixOutputType>("outputTypeComboBox", "OutputType");
			b.CreateLocationButton("outputTypeComboBox");
			Get<ComboBox>("outputType").SelectedIndexChanged += RefreshOutputFileNameTextBox;

			Get<TextBox>("projectFolder").Text = project.Directory;
			Get<TextBox>("projectFile").Text = Path.GetFileName(project.FileName);
			
			RefreshOutputFileNameTextBox(null, EventArgs.Empty);
			
			helper.AddConfigurationSelector(this);
		}
		
		void RefreshOutputFileNameTextBox(object sender, EventArgs e)
		{
			WixOutputType outputType = (WixOutputType)(Get<ComboBox>("outputType").SelectedIndex);
			Get<TextBox>("outputFileName").Text = Get<TextBox>("outputName").Text + WixProject.GetInstallerExtension(outputType.ToString());
		}
	}
}
