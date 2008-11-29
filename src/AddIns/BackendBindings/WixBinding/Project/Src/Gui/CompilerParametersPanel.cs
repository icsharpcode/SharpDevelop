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
			b = helper.BindString("outputPathTextBox", "OutputPath", TextBoxEditMode.EditRawProperty);
			b.CreateLocationButton("outputPathTextBox");
			ConnectBrowseFolder("outputPathBrowseButton", "outputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}", TextBoxEditMode.EditRawProperty);

			b = helper.BindString("baseOutputPathTextBox", "BaseOutputPath", TextBoxEditMode.EditRawProperty);
			b.CreateLocationButton("baseOutputPathTextBox");
			ConnectBrowseFolder("baseOutputPathBrowseButton", "baseOutputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}", TextBoxEditMode.EditRawProperty);
			
			b = helper.BindString("intermediateOutputPathTextBox", "IntermediateOutputPath", TextBoxEditMode.EditRawProperty);
			b.CreateLocationButton("intermediateOutputPathTextBox");
			ConnectBrowseFolder("intermediateOutputPathBrowseButton", "intermediateOutputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}", TextBoxEditMode.EditRawProperty);
			
			b = helper.BindString("wixToolPathTextBox", "WixToolPath", TextBoxEditMode.EditRawProperty);
			ConnectBrowseFolder("wixToolPathBrowseButton", "wixToolPathTextBox", String.Empty, TextBoxEditMode.EditRawProperty);
			
			b = helper.BindString("wixTargetsPathTextBox", "WixTargetsPath", TextBoxEditMode.EditRawProperty);
			ConnectBrowseButton("wixTargetsPathBrowseButton", "wixTargetsPathTextBox", "${res:ICSharpCode.WixBinding.WixTargetsFileFilterName} (wix.targets)|wix.targets|${res:SharpDevelop.FileFilter.AllFiles}|*.*", TextBoxEditMode.EditRawProperty);

			b = helper.BindString("wixTasksPathTextBox", "WixTasksPath", TextBoxEditMode.EditRawProperty);
			ConnectBrowseButton("wixTasksPathBrowseButton", "wixTasksPathTextBox", "${res:ICSharpCode.WixBinding.WixTasksFileFilterName} (wixtasks.dll)|wixtasks.dll|${res:SharpDevelop.FileFilter.AllFiles}|*.*", TextBoxEditMode.EditRawProperty);

			b = helper.BindString("suppressWarningsTextBox", "SuppressIces", TextBoxEditMode.EditRawProperty);
			b = helper.BindString("wixVariablesTextBox", "WixVariables", TextBoxEditMode.EditRawProperty);
			b = helper.BindString("culturesTextBox", "Cultures", TextBoxEditMode.EditRawProperty);

			helper.AddConfigurationSelector(this);
		}
		
		void CompilerExtensionsChanged(object source, EventArgs e)
		{
			IsDirty = true;
		}
	}
}
