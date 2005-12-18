// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class BuildEvents : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.BuildEvents.xfrm");
			
			InitializeHelper();
			
			ConnectBrowseButton("preBuildEventBrowseButton",
			                    "preBuildEventTextBox",
			                    "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			ConnectBrowseButton("postBuildEventBrowseButton",
			                    "postBuildEventTextBox",
			                    "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			
			ConfigurationGuiBinding b;
			
			b = helper.BindString("preBuildEventTextBox", "PreBuildEvent");
			b.CreateLocationButton("preBuildEventTextBox");
			
			b = helper.BindString("postBuildEventTextBox", "PostBuildEvent");
			b.CreateLocationButton("postBuildEventTextBox");
			
			b = helper.BindEnum<RunPostBuildEvent>("runPostBuildEventComboBox", "RunPostBuildEvent");
			b.CreateLocationButton("runPostBuildEventComboBox");
			
			helper.AddConfigurationSelector(this);
		}
	}
}
