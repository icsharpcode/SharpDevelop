using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
	
namespace VBNetBinding.OptionPanels
{
	public class ReferencePaths : AbstractOptionPanel
	{
		VBNetProject project;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ReferencePaths.xfrm"));
			this.project = (VBNetProject)((Properties)CustomizationObject).Get("Project");
		}
		
		public override bool StorePanelContents()
		{
			// TODO
			return true;
		}
	}
}
