using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
	
namespace CSharpBinding.OptionPanels
{
	public class Publish : AbstractOptionPanel
	{
		CSharpProject project;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Publish.xfrm"));
			this.project = (CSharpProject)((Properties)CustomizationObject).Get("Project");
		}
		
	
		public override bool StorePanelContents()
		{
			// TODO
			return true;
		}
	}
}
