using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
	
namespace CSharpBinding.OptionPanels
{
	public class BuildEvents : AbstractOptionPanel
	{
		CSharpProject project;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.BuildEvents.xfrm"));
			ConnectBrowseButton("preBuildEventBrowseButton", 
			                    "preBuildEventTextBox", 
			                    "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			ConnectBrowseButton("postBuildEventBrowseButton", 
			                    "postBuildEventTextBox", 
			                    "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			
			this.project = (CSharpProject)((Properties)CustomizationObject).Get("Project");
			
			Get<TextBox>("preBuildEvent").Text  = project.PreBuildEvent; 
			Get<TextBox>("preBuildEvent").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("postBuildEvent").Text = project.PostBuildEvent;
			Get<TextBox>("postBuildEvent").TextChanged += new EventHandler(Save);
			
			Get<ComboBox>("runPostBuildEvent").Items.Add("Always");
			Get<ComboBox>("runPostBuildEvent").Items.Add("On successful build");
			Get<ComboBox>("runPostBuildEvent").Items.Add("When the build updates the project output");
			
			Get<ComboBox>("runPostBuildEvent").SelectedIndex = (int)project.RunPostBuildEvent;
			Get<ComboBox>("runPostBuildEvent").SelectedIndexChanged += new EventHandler(Save);
			
		}
		
		void Save(object sender, EventArgs e) 
		{
			StorePanelContents();
		}
		
		public override bool StorePanelContents()
		{
			project.PreBuildEvent  = Get<TextBox>("preBuildEvent").Text; 
			project.PostBuildEvent = Get<TextBox>("postBuildEvent").Text; 
			project.RunPostBuildEvent = (RunPostBuildEvent)Get<ComboBox>("runPostBuildEvent").SelectedIndex;
			project.Save();
			return true;
		}
	}
}
