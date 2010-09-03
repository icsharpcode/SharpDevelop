// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using StringPair = System.Collections.Generic.KeyValuePair<System.String, System.String>;

namespace ICSharpCode.VBNetBinding.OptionPanels
{
	public class BuildOptions : AbstractBuildOptions
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("BuildOptions.xfrm");
			InitializeHelper();
			
			ConfigurationGuiBinding b;
			
			b = helper.BindString("conditionalSymbolsTextBox", "DefineConstants", TextBoxEditMode.EditRawProperty);
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			b.CreateLocationButton("conditionalSymbolsTextBox");
			
			b = helper.BindBoolean("optimizeCodeCheckBox", "Optimize", false);
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			b.CreateLocationButton("optimizeCodeCheckBox");
			
			b = helper.BindBoolean("removeOverflowCheckBox", "RemoveIntegerChecks", false);
			b.CreateLocationButton("removeOverflowCheckBox");
			
			ChooseStorageLocationButton locationButton;
			b = helper.BindStringEnum("optionExplicitComboBox", "OptionExplicit", "On",
			                          new StringPair("Off", "Explicit Off"),
			                          new StringPair("On", "Explicit On"));
			locationButton = b.CreateLocationButton("optionExplicitComboBox");
			b = helper.BindStringEnum("optionStrictComboBox", "OptionStrict", "Off",
			                          new StringPair("Off", "Strict Off"),
			                          new StringPair("On", "Strict On"));
			b.RegisterLocationButton(locationButton);
			b = helper.BindStringEnum("optionCompareComboBox", "OptionCompare", "Binary",
			                          new StringPair("Binary", "Compare Binary"),
			                          new StringPair("Text", "Compare Text"));
			b.RegisterLocationButton(locationButton);
			b = helper.BindStringEnum("optionInferComboBox", "OptionInfer", "Off",
			                          new StringPair("Off", "Infer Off"),
			                          new StringPair("On", "Infer On"));
			b.RegisterLocationButton(locationButton);
			
			InitBaseIntermediateOutputPath();
			InitIntermediateOutputPath();
			InitOutputPath();
			InitXmlDoc();
			InitTargetFramework();

			InitDebugInfo();
			InitAdvanced();
			InitWarnings();
			
			helper.AddConfigurationSelector(this);
		}
	}
}
