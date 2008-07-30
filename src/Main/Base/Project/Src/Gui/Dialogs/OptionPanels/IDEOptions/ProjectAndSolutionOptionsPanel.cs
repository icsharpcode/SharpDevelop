// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class ProjectAndSolutionOptionsPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ProjectAndSolutionOptionsPanel.xfrm"));
			
			// read properties
			ControlDictionary["projectLocationTextBox"].Text = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath",
			                                                                       Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
			                                                                                    "SharpDevelop Projects")).ToString();
			
			((CheckBox)ControlDictionary["loadPrevProjectCheckBox"]).Checked = PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false);
			((CheckBox)ControlDictionary["showErrorListCheckBox"]).Checked = Project.BuildOptions.ShowErrorListAfterBuild;
			((NumericUpDown)ControlDictionary["parallelBuildNumericUpDown"]).Value = Project.BuildOptions.DefaultParallelProjectCount;
			
			((Button)ControlDictionary["selectProjectLocationButton"]).Click += new EventHandler(SelectProjectLocationButtonClicked);
			
			ComboBox onExecuteComboBox = Get<ComboBox>("onExecute");
			Type type = typeof(Project.BuildOnExecuteSetting);
			foreach (Project.BuildOnExecuteSetting element in Enum.GetValues(type)) {
				object[] attr = type.GetField(Enum.GetName(type, element)).GetCustomAttributes(typeof(DescriptionAttribute), false);
				string description;
				if (attr.Length > 0) {
					description = StringParser.Parse((attr[0] as DescriptionAttribute).Description);
				} else {
					description = Enum.GetName(type, element);
				}
				onExecuteComboBox.Items.Add(description);
			}
			onExecuteComboBox.SelectedIndex = (int)Project.BuildModifiedProjectsOnlyService.Setting;
		}
		
		public override bool StorePanelContents()
		{
			// check for correct settings
			string projectPath = ControlDictionary["projectLocationTextBox"].Text;
			if (projectPath.Length > 0) {
				if (!FileUtility.IsValidPath(projectPath)) {
					MessageService.ShowError(StringParser.Parse("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.InvalidProjectPathSpecified}"));
					return false;
				}
			}
			
			// set properties
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", projectPath);
			PropertyService.Set("SharpDevelop.LoadPrevProjectOnStartup", ((CheckBox)ControlDictionary["loadPrevProjectCheckBox"]).Checked);
			Project.BuildOptions.ShowErrorListAfterBuild = ((CheckBox)ControlDictionary["showErrorListCheckBox"]).Checked;
			Project.BuildOptions.DefaultParallelProjectCount = (int)((NumericUpDown)ControlDictionary["parallelBuildNumericUpDown"]).Value;
			
			Project.BuildModifiedProjectsOnlyService.Setting = (Project.BuildOnExecuteSetting)Get<ComboBox>("onExecute").SelectedIndex;
			
			return true;
		}
		
		void SelectProjectLocationButtonClicked(object sender, EventArgs e)
		{
			TextBox projectLocationTextBox = (TextBox)ControlDictionary["projectLocationTextBox"];
			using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.SelectDefaultProjectLocationDialog.Title}", projectLocationTextBox.Text)) {
				if (fdiag.ShowDialog() == DialogResult.OK) {
					projectLocationTextBox.Text = fdiag.SelectedPath;
				}
			}
		}
	}
}
