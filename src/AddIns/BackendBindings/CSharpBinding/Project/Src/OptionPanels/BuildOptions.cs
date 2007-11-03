// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using StringPair = System.Collections.Generic.KeyValuePair<System.String, System.String>;

namespace CSharpBinding.OptionPanels
{
	public class BuildOptions : AbstractBuildOptions
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("BuildOptions.xfrm");
			InitializeHelper();
			
			InitBaseIntermediateOutputPath();
			InitIntermediateOutputPath();
			InitOutputPath();
			InitXmlDoc();
			InitTargetFramework();
			
			ConfigurationGuiBinding b;
			
			b = helper.BindString("conditionalSymbolsTextBox", "DefineConstants", TextBoxEditMode.EditRawProperty);
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			b.CreateLocationButton("conditionalSymbolsTextBox");
			
			b = helper.BindBoolean("optimizeCodeCheckBox", "Optimize", false);
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			b.CreateLocationButton("optimizeCodeCheckBox");
			
			b = helper.BindBoolean("allowUnsafeCodeCheckBox", "AllowUnsafeBlocks", false);
			b.CreateLocationButton("allowUnsafeCodeCheckBox");
			
			b = helper.BindBoolean("checkForOverflowCheckBox", "CheckForOverflowUnderflow", false);
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			b.CreateLocationButton("checkForOverflowCheckBox");
			
			b = helper.BindBoolean("noCorlibCheckBox", "NoStdLib", false);
			b.CreateLocationButton("noCorlibCheckBox");
			
			InitDebugInfo();
			InitAdvanced();
			b = helper.BindStringEnum("fileAlignmentComboBox", "FileAlignment",
			                          "4096",
			                          new StringPair("512", "512"),
			                          new StringPair("1024", "1024"),
			                          new StringPair("2048", "2048"),
			                          new StringPair("4096", "4096"),
			                          new StringPair("8192", "8192"));
			b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
			b.RegisterLocationButton(advancedLocationButton);
			
			InitWarnings();
			
			helper.AddConfigurationSelector(this);
		}
	}
}
