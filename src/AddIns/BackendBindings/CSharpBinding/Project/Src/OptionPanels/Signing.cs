using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
	
namespace CSharpBinding.OptionPanels
{
	public class Signing : AbstractOptionPanel
	{
		CSharpProject project;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.Signing.xfrm"));
			this.project = (CSharpProject)((Properties)CustomizationObject).Get("Project");
			
			Get<CheckBox>("signAssembly").Checked = project.SignAssembly;
			
			Get<RadioButton>("useKeyFile").Checked         = project.AssemblyOriginatorKeyMode == AssemblyOriginatorKeyMode.File;
			Get<RadioButton>("useKeyFile").CheckedChanged += new EventHandler(UpdateEnabledStates);
			
			Get<RadioButton>("useKeyProvider").Checked = project.AssemblyOriginatorKeyMode == AssemblyOriginatorKeyMode.Provider;
			Get<RadioButton>("useKeyProvider").CheckedChanged += new EventHandler(UpdateEnabledStates);
			
			Get<ComboBox>("keyFile").Text = project.AssemblyOriginatorKeyFile;
			
			Get<ComboBox>("providerName").Text = project.AssemblyKeyProviderName;
			Get<ComboBox>("providerName").Items.Add("TODO: GetKeyProviders()");
			Get<ComboBox>("container").Text    = "TODO";
			
			Get<CheckBox>("delaySignOnly").Checked = project.DelaySign;
			UpdateEnabledStates(this, EventArgs.Empty);
		}
		
		void UpdateEnabledStates(object sender, EventArgs e)
		{
			Get<Button>("changePassword").Enabled = false;
			
			Get<ComboBox>("providerName").Enabled = Get<ComboBox>("container").Enabled = Get<RadioButton>("useKeyProvider").Checked;
			Get<ComboBox>("keyFile").Enabled = Get<RadioButton>("useKeyFile").Checked;
			Get<CheckBox>("signAssembly").Enabled = Get<RadioButton>("useKeyFile").Checked;
		}
		
		public override bool StorePanelContents()
		{
			project.SignAssembly              = Get<CheckBox>("signAssembly").Checked;
			project.DelaySign                 = Get<CheckBox>("delaySignOnly").Checked;
			
			project.AssemblyOriginatorKeyFile = Get<ComboBox>("keyFile").Text;
			project.AssemblyKeyProviderName   = Get<ComboBox>("providerName").Text;
			// TODO : Container ????
			
			if (Get<RadioButton>("useKeyFile").Checked) {
				project.AssemblyOriginatorKeyMode = AssemblyOriginatorKeyMode.File;
			} else if (Get<RadioButton>("useKeyProvider").Checked) {
				project.AssemblyOriginatorKeyMode = AssemblyOriginatorKeyMode.Provider;
			}
			
			return true;
		}
	}
}
