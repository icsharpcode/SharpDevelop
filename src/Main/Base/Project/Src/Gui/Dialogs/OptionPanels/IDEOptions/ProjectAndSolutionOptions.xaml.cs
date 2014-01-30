// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			
			FillComboBoxWithEnumValues(typeof(Project.BuildDetection), onExecuteComboBox);
			FillComboBoxWithEnumValues(typeof(Project.BuildOutputVerbosity), verbosityComboBox);
		}
		
		void FillComboBoxWithEnumValues(Type type, ComboBox comboBox)
		{
			foreach (Project.BuildDetection element in Enum.GetValues(type)) {
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
			string path = SD.FileService.BrowseForFolder(
				"${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.SelectDefaultProjectLocationDialog.Title}",
				defaultProjectLocationTextBox.Text);
			if (path != null)
				defaultProjectLocationTextBox.Text = path;
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			parallelBuildCount.Value = Project.BuildOptions.DefaultParallelProjectCount;
			onExecuteComboBox.SelectedIndex = (int)Project.BuildOptions.BuildOnExecute;
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
			Project.BuildOptions.BuildOnExecute = (Project.BuildDetection)onExecuteComboBox.SelectedIndex;
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
