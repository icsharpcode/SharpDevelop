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

namespace VBNetBinding.OptionPanels
{
	public class BuildOptions : AbstractBuildOptions
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("BuildOptions.xfrm");
			InitializeHelper();
			
			ConfigurationGuiBinding b;
			
			b = helper.BindString("conditionalSymbolsTextBox", "DefineConstants");
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
			
			InitBaseIntermediateOutputPath();
			InitIntermediateOutputPath();
			InitOutputPath();
			InitXmlDoc();
			InitTargetFramework(VBNetProject.DefaultTargetsFile,
			   @"$(SharpDevelopBinPath)\SharpDevelop.Build.VisualBasic.targets");

			InitAdvanced();
			InitWarnings();
			
			helper.AddConfigurationSelector(this);
		}
	}
}
