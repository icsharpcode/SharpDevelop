// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class BuildEvents : AbstractXmlFormsProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.BuildEvents.xfrm");
			
			InitializeHelper();
			baseDirectory = Path.GetDirectoryName(project.OutputAssemblyFullPath);
			
			ConfigurationGuiBinding b;
			
			b = helper.BindString("preBuildEventTextBox", "PreBuildEvent", TextBoxEditMode.EditRawProperty);
			b.CreateLocationButton("preBuildEventTextBox");
			
			b = helper.BindString("postBuildEventTextBox", "PostBuildEvent", TextBoxEditMode.EditRawProperty);
			b.CreateLocationButton("postBuildEventTextBox");
			
			b = helper.BindEnum<RunPostBuildEvent>("runPostBuildEventComboBox", "RunPostBuildEvent");
			b.CreateLocationButton("runPostBuildEventComboBox");
			
			helper.AddConfigurationSelector(this);
		}
	}
}
