// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class ApplicationSettingsPanel : AbstractProjectOptionPanel
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
