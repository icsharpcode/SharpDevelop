// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using StringPair = System.Collections.Generic.KeyValuePair<System.String, System.String>;

namespace ICSharpCode.WixBinding
{
	public class CompilerParametersPanel : AbstractProjectOptionPanel
	{	
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.WixBinding.Resources.CompilerParametersPanel.xfrm"));
			InitializeHelper();
			
			ConfigurationGuiBinding b;
			b = helper.BindString("outputPathTextBox", "OutputPath");
			b.CreateLocationButton("outputPathTextBox");
			ConnectBrowseFolder("outputPathBrowseButton", "outputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");

			b = helper.BindString("baseOutputPathTextBox", "BaseOutputPath");
			b.CreateLocationButton("baseOutputPathTextBox");
			ConnectBrowseFolder("baseOutputPathBrowseButton", "baseOutputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
			
			b = helper.BindString("intermediateOutputPathTextBox", "IntermediateOutputPath");
			b.CreateLocationButton("intermediateOutputPathTextBox");
			ConnectBrowseFolder("intermediateOutputPathBrowseButton", "intermediateOutputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
			
			b = helper.BindString("wixToolPathTextBox", "WixToolPath");
			ConnectBrowseFolder("wixToolPathBrowseButton", "wixToolPathTextBox", String.Empty);
			
			b = helper.BindString("wixMSBuildExtensionsPathTextBox", "WixMSBuildExtensionsPath");
			ConnectBrowseFolder("wixMSBuildExtensionsPathBrowseButton", "wixMSBuildExtensionsPathTextBox", String.Empty);

			// Add the extension picker in manually since the anchoring does not
			// work if we add the picker into the XML of the CompilerParametersPanel.xfrm file.
			WixCompilerExtensionPicker extensionPicker = new WixCompilerExtensionPicker();
			extensionPicker.Dock = DockStyle.Fill;
			ControlDictionary["compilerExtensionsGroupBox"].Controls.Add(extensionPicker);
			extensionPicker.ExtensionsChanged += CompilerExtensionsChanged;

			b = new WixCompilerExtensionBinding(extensionPicker);
			helper.AddBinding("CompileExtension", b);

			InitWarnings();

			helper.AddConfigurationSelector(this);
		}
		
		void InitWarnings()
		{
			ConfigurationGuiBinding b;
			b = helper.BindStringEnum("warningLevelComboBox", "WarningLevel",
			                          "4",
			                          new StringPair("0", "0"),
			                          new StringPair("1", "1"),
			                          new StringPair("2", "2"),
			                          new StringPair("3", "3"));
			ChooseStorageLocationButton locationButton = b.CreateLocationButtonInPanel("errorsAndWarningsGroupBox");
			b = helper.BindString("suppressWarningsTextBox", "NoWarn");
			b.RegisterLocationButton(locationButton);
			
			b = helper.BindBoolean("treatWarningsAsErrorsCheckBox", "TreatWarningsAsErrors", false);
			b.RegisterLocationButton(locationButton);
		}
		
		void CompilerExtensionsChanged(object source, EventArgs e)
		{
			IsDirty = true;
		}
	}
}
