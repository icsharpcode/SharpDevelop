// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class ProjectAndSolutionOptions : OptionPanel
	{
		public ProjectAndSolutionOptions()
		{
			InitializeComponent();
			
			FillComboBoxWithEnumValues(typeof(Project.BuildOnExecuteSetting), onExecuteComboBox);
			FillComboBoxWithEnumValues(typeof(Project.BuildOutputVerbosity), verbosityComboBox);
		}
	
		void FillComboBoxWithEnumValues(Type type, ComboBox comboBox)
		{
			foreach (Project.BuildOnExecuteSetting element in Enum.GetValues(type)) {
				object[] attr = type.GetField(Enum.GetName(type, element)).GetCustomAttributes(typeof(DescriptionAttribute), false);
				string description;
				if (attr.Length > 0) {
					description = StringParser.Parse((attr[0] as DescriptionAttribute).Description);
				} else {
					description = Enum.GetName(type, element);
				}
				comboBox.Items.Add(description);
			}
		}
		
		void defaultProjectLocationButtonClick(object sender, RoutedEventArgs e)
		{
			using (var fdiag = FileService.CreateFolderBrowserDialog("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.SelectDefaultProjectLocationDialog.Title}", defaultProjectLocationTextBox.Text)) {
				if (fdiag.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					defaultProjectLocationTextBox.Text = fdiag.SelectedPath;
				}
			}
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			parallelBuildCount.Value = Project.BuildOptions.DefaultParallelProjectCount;
			onExecuteComboBox.SelectedIndex = (int)Project.BuildModifiedProjectsOnlyService.Setting;
			verbosityComboBox.SelectedIndex = (int)Project.BuildOptions.DefaultBuildOutputVerbosity;
		}
		
		public override bool SaveOptions()
		{
			// check for correct settings
			string projectPath = defaultProjectLocationTextBox.Text;
			if (projectPath.Length > 0) {
				if (!FileUtility.IsValidPath(projectPath)) {
					MessageService.ShowError(StringParser.Parse("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.InvalidProjectPathSpecified}"));
					return false;
				}
			}
			Project.BuildOptions.DefaultParallelProjectCount = (int)parallelBuildCount.Value;
			Project.BuildModifiedProjectsOnlyService.Setting = (Project.BuildOnExecuteSetting)onExecuteComboBox.SelectedIndex;
			Project.BuildOptions.DefaultBuildOutputVerbosity = (Project.BuildOutputVerbosity)verbosityComboBox.SelectedIndex;
			return base.SaveOptions();
		}
		
		public static string DefaultProjectCreationPath {
			get {
				return PropertyService.Get("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath",
				                           Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
				                                        "SharpDevelop Projects"));
			}
			set {
				PropertyService.Set("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", value);
			}
		}
		
		public static bool LoadPrevProjectAtStartup {
			get {
				return PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false);
			}
			set {
				PropertyService.Set("SharpDevelop.LoadPrevProjectOnStartup", value);
			}
		}
	}
}
