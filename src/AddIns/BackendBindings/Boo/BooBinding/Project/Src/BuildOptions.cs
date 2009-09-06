// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

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
			InitTargetFramework();
			
			ConfigurationGuiBinding b;
			b = helper.BindString("conditionalSymbolsTextBox", "DefineConstants", TextBoxEditMode.EditRawProperty);
			b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
			b.CreateLocationButton("conditionalSymbolsTextBox");
			
			helper.BindBoolean("noCorlibCheckBox", "NoStdLib", false).CreateLocationButton("noCorlibCheckBox");
			helper.BindBoolean("duckyCheckBox", "Ducky", false).CreateLocationButton("duckyCheckBox");
			helper.BindBoolean("checkForOverflowCheckBox", "CheckForOverflowUnderflow", true).CreateLocationButton("checkForOverflowCheckBox");
			helper.BindBoolean("useStrictMode", "Strict", false).CreateLocationButton("useStrictMode");
			helper.BindBoolean("allowUnsafeCodeCheckBox", "AllowUnsafeBlocks", false).CreateLocationButton("allowUnsafeCodeCheckBox");
			
			helper.BindString("pipelineTextBox", "Pipeline", TextBoxEditMode.EditEvaluatedProperty).CreateLocationButton("pipelineLabel");
			
			CreatePlatformTarget().CreateLocationButton("targetCpuLabel");
			
			InitWarnings();
			
			//
			InitBaseIntermediateOutputPath();
			InitIntermediateOutputPath();
			//
			
			helper.AddConfigurationSelector(this);
		}
	}
}
