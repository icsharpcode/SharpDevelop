// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matt Everson" email="ti.just.me@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace MattEverson.SourceAnalysis {
	public class AnalysisProjectOptionsPanel : AbstractProjectOptionPanel {
		
		public override void LoadPanelContents() {
			InitializeHelper();

			var masterSettingsFile = helper.GetProperty<string>("SourceAnalysisOverrideSettingsFile", "", true);
			if (masterSettingsFile.Length == 0) {

				helper.SetProperty<string>("SourceAnalysisOverrideSettingsFile",
				                           StyleCopWrapper.GetMasterSettingsFile(),
				                           true,
				                           PropertyStorageLocations.Base);
			}

			AnalysisProjectOptions po = new AnalysisProjectOptions();
			po.Dock = DockStyle.Fill;
			Controls.Add(po);

			ChooseStorageLocationButton btnEnable;
			ChooseStorageLocationButton btnFileLocation;
			btnEnable = helper.BindBoolean(po.EnableCheckBox, "RunSourceAnalysis", false).CreateLocationButton(po.EnableCheckBox);
			btnFileLocation = helper.BindString(po.SettingsFileTextBox, "SourceAnalysisOverrideSettingsFile", TextBoxEditMode.EditRawProperty).CreateLocationButton(po.SettingsFileTextBox);
			ConfigurationGuiBinding binding = po.CreateBinding();
			binding.RegisterLocationButton(btnEnable);
			binding.RegisterLocationButton(btnFileLocation);

			helper.AddConfigurationSelector(this);

			po.ModifyStyleCopSettingsButton.Click += ModifyStyleCopSettingsClick;
		}

		void ModifyStyleCopSettingsClick(object sender, EventArgs e) {
			var settingsFile = helper.GetProperty<string>("SourceAnalysisOverrideSettingsFile", "", true);
			
			if (settingsFile == StyleCopWrapper.GetMasterSettingsFile()) {
				if (ConfirmSwitchFromMaster()) {
					settingsFile = CopyFromMaster();
				}
			}

			if (!System.IO.File.Exists(settingsFile)) {
				if (ConfirmReplaceMissingFile()) {
					settingsFile = CopyFromMaster();
				}
				else {
					MessageService.ShowWarning("No settings file found to modify.");
					return;
				}
			}

			string styleCopPath = StyleCopWrapper.FindStyleCopPath();
			string executable;
			if (styleCopPath != null)
				executable = Path.Combine(styleCopPath, "StyleCopSettingsEditor.exe");
			else
				executable = null;
			string parameters = "\"" + settingsFile + "\"";
			if (!File.Exists(executable)) {
				LoggingService.Debug("StyleCopSettingsEditor.exe: " + executable);
				MessageService.ShowWarning("Unable to find the StyleCop Settings editor. Please specify the StyleCop location in Tools Options.");
				return;
			}
			
			using(Process p = Process.Start("\"" + executable + "\"", parameters)) {
				// No need to wait for the settings dialog to close - we can leave it open.
			}
		}

		private bool ConfirmReplaceMissingFile() {
			var result = MessageBox.Show("A settings file is not present. Would you like to copy the master into the " +
			                             "project folder?",
			                             "Missing Settings File",
			                             MessageBoxButtons.YesNo,
			                             MessageBoxIcon.Exclamation,
			                             MessageBoxDefaultButton.Button1
			                            );
			if (result == DialogResult.Yes) {
				return true;
			} else {
				return false;
			}
		}
		
		private bool ConfirmSwitchFromMaster()
		{
			var result = MessageBox.Show("You are currently using the master settings file. Do you want to make a "
			                             + "copy in the project folder instead?",
			                             "Using Master Settings File",
			                             MessageBoxButtons.YesNo,
			                             MessageBoxIcon.Exclamation,
			                             MessageBoxDefaultButton.Button1
			                            );
			if (result == DialogResult.Yes)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private string CopyFromMaster() {
			var newSettingsFile = helper.Project.Directory + "\\Settings.SourceAnalysis";
			System.IO.File.Copy(
				StyleCopWrapper.GetMasterSettingsFile(),
				newSettingsFile,
				true
			);
			helper.SetProperty<string>("SourceAnalysisOverrideSettingsFile",
			                           newSettingsFile,
			                           true,
			                           PropertyStorageLocations.Base
			                          );
			return newSettingsFile;
		}
	}
}
