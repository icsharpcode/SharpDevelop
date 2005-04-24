using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class Publish : AbstractOptionPanel
	{
		AdvancedMSBuildProject project;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ProjectOptions.Publish.xfrm"));
			this.project = (AdvancedMSBuildProject)((Properties)CustomizationObject).Get("Project");
		}
		
		
		public override bool StorePanelContents()
		{
			// TODO
			return true;
		}
	}
}
