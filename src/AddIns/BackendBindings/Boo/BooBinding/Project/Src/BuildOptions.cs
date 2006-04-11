// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

using StringPair = System.Collections.Generic.KeyValuePair<string, string>;

namespace Grunwald.BooBinding
{
	public sealed class BuildOptions : AbstractBuildOptions
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Resources.BuildOptions.xfrm"));
			InitializeHelper();
			
			InitOutputPath();
			//InitXmlDoc();
			InitDebugInfo();
			
			//ConfigurationGuiBinding b;
			
			//b = helper.BindString("conditionalSymbolsTextBox", "DefineConstants");
			//b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			//b.CreateLocationButton("conditionalSymbolsTextBox");
			
			helper.BindBoolean("noCorlibCheckBox", "NoStdLib", false).CreateLocationButton("noCorlibCheckBox");
			helper.BindBoolean("duckyCheckBox", "Ducky", false).CreateLocationButton("duckyCheckBox");
			
			helper.BindString("pipelineTextBox", "Pipeline").CreateLocationButton("pipelineLabel");
			
			//InitWarnings();
			
			//
			helper.BindString("baseIntermediateOutputPathTextBox", "BaseIntermediateOutputPath").CreateLocationButton("baseIntermediateOutputPathTextBox");
			ConnectBrowseFolder("baseIntermediateOutputPathBrowseButton", "baseIntermediateOutputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
			helper.BindString("intermediateOutputPathTextBox", "IntermediateOutputPath").CreateLocationButton("intermediateOutputPathTextBox");
			ConnectBrowseFolder("intermediateOutputPathBrowseButton", "intermediateOutputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
			//
			
			helper.AddConfigurationSelector(this);
		}
	}
}
