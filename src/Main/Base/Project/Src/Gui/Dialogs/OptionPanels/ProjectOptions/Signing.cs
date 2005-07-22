// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	public class Signing : AbstractOptionPanel
	{
		AdvancedMSBuildProject project;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ProjectOptions.Signing.xfrm"));
			this.project = (AdvancedMSBuildProject)((Properties)CustomizationObject).Get("Project");
			
			Get<CheckBox>("signAssembly").Checked = project.SignAssembly;
			Get<CheckBox>("signAssembly").CheckedChanged += new EventHandler(UpdateEnabledStates);
			Get<CheckBox>("signAssembly").CheckedChanged += new EventHandler(Save);
			
			Get<RadioButton>("useKeyFile").Checked         = project.AssemblyOriginatorKeyMode == AssemblyOriginatorKeyMode.File;
			Get<RadioButton>("useKeyFile").CheckedChanged += new EventHandler(UpdateEnabledStates);
			Get<RadioButton>("useKeyFile").CheckedChanged += new EventHandler(Save);
			
			
			Get<RadioButton>("useKeyProvider").Checked = project.AssemblyOriginatorKeyMode == AssemblyOriginatorKeyMode.Provider;
			Get<RadioButton>("useKeyProvider").CheckedChanged += new EventHandler(UpdateEnabledStates);
			Get<RadioButton>("useKeyProvider").CheckedChanged += new EventHandler(Save);
			
			Get<ComboBox>("keyFile").Text = project.AssemblyOriginatorKeyFile;
			Get<ComboBox>("keyFile").TextChanged += new EventHandler(Save);
			
			Get<ComboBox>("providerName").Text = project.AssemblyKeyProviderName;
			Get<ComboBox>("providerName").Items.Add("TODO: GetKeyProviders()");
			Get<ComboBox>("providerName").TextChanged += new EventHandler(Save);
			
			Get<ComboBox>("container").Text    = "TODO";
			Get<ComboBox>("container").TextChanged += new EventHandler(Save);
			
			Get<CheckBox>("delaySignOnly").Checked = project.DelaySign;
			Get<CheckBox>("delaySignOnly").CheckedChanged += new EventHandler(Save);
			
			UpdateEnabledStates(this, EventArgs.Empty);
		}
		
		
		void Save(object sender, EventArgs e)
		{
			StorePanelContents();
		}
		
		
		void UpdateEnabledStates(object sender, EventArgs e)
		{
			Get<Button>("changePassword").Enabled = false;
			
			Get<ComboBox>("providerName").Enabled = Get<ComboBox>("container").Enabled = Get<RadioButton>("useKeyProvider").Checked;
			Get<ComboBox>("keyFile").Enabled = Get<RadioButton>("useKeyFile").Checked;
			Get<GroupBox>("signing").Enabled = Get<CheckBox>("signAssembly").Checked;
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
			project.Save();
			
			return true;
		}
	}
}
