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
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	[Obsolete("TODO: rewrite me in WPF")]
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
