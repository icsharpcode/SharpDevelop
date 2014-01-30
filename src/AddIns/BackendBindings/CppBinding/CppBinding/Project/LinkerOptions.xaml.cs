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
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Interaction logic for LinkerOptionsXaml.xaml
	/// </summary>
	public partial class LinkerOptions :  ProjectOptionPanel
	{
		private const string metaElement ="Link";
		
		public LinkerOptions()
		{
			this.HeaderVisibility = Visibility.Collapsed;
			InitializeComponent();
		}
		
	
		#region Properties
		
		public ProjectProperty<string> LibraryPath {
			get { return GetProperty("LibraryPath", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		private bool checkBoxChecked;
		
		public bool CheckBoxChecked {
			get {return checkBoxChecked;}
			set
			{
				checkBoxChecked = value;
				IsDirty = true;
				base.RaisePropertyChanged(() => CheckBoxChecked);
			}
		}
		
		#endregion
		
		#region Save/Load
		
		protected override void Initialize()
		{
			var msDefGroup = new MSBuildItemDefinitionGroup(base.Project, base.Project.ActiveConfiguration);
			                                                                  
			this.additionalLibsTextBox.Text = GetElementMetaData(msDefGroup,"AdditionalDependencies");
			
			this.addModuleTextBox.Text = GetElementMetaData(msDefGroup,"AddModuleNamesToAssembly");
			
			this.resourceFileTextBox.Text = GetElementMetaData(msDefGroup,"EmbedManagedResourceFile");
			
			this.additionalOptionsTextBox.Text = GetElementMetaData(msDefGroup,"AdditionalOptions");
			
			string def = GetElementMetaData(msDefGroup,"GenerateDebugInformation");
			
			bool check;
			if (bool.TryParse(def, out check))
			{
				this.CheckBoxChecked = check;
				this.debugInfoCheckBox.IsChecked = check;
			}
			IsDirty = false;
		}
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, project.ActiveConfiguration);
			
			SetElementMetaData(group,"AdditionalDependencies",this.additionalLibsTextBox.Text);
			SetElementMetaData(group,"AddModuleNamesToAssembly",this.addModuleTextBox.Text);
			SetElementMetaData(group,"EmbedManagedResourceFile",this.resourceFileTextBox.Text);
			SetElementMetaData(group,"AdditionalOptions",this.additionalOptionsTextBox.Text);
			
			string check = "false";
			if ((bool)this.debugInfoCheckBox.IsChecked) {
				check = "true";
			}
			
			SetElementMetaData(group,"GenerateDebugInformation",check);
			
			return base.Save(project, configuration, platform);
		}
		
		#endregion
		
		#region MSBuildItemDefinitionGroup Set-Get
		
		private static string GetElementMetaData (MSBuildItemDefinitionGroup group,string name)
		{
			return group.GetElementMetadata(metaElement,name);
		}
		
		
		private  static void SetElementMetaData (MSBuildItemDefinitionGroup group,string name,string value)
		{
			group.SetElementMetadata(metaElement,name,value);
		}
		
		#endregion
		
		private void LibraryPathButton_Click(object sender, RoutedEventArgs e)
		{
			PopulateStringListEditor(StringParser.Parse("${res:Global.Folder}:"),
			                        StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Linker.Library}:"),
			                        this.libraryPathTextBox,
			                        true);
		}
		
		
		private void AdditionalLibsButton_Click(object sender, RoutedEventArgs e)
		{
			PopulateStringListEditor(StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                               StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Linker.AdditionalLibs}:"),
			                               this.additionalLibsTextBox,
			                               false);
		}

		
		private void AddModuleButton_Click(object sender, RoutedEventArgs e)
		{
			PopulateStringListEditor(StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                               StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Linker.AddModule}"),
			                               this.addModuleTextBox,
			                               false);
		}
		
		
		private void ResourceFileButton_Click(object sender, RoutedEventArgs e)
		{
			PopulateStringListEditor(StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                               StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Linker.ManagedResourceFile}"),
			                               this.resourceFileTextBox,
			                               false);
		}
		
		
		public static void PopulateStringListEditor(string title, string listCaption,TextBox textBox,bool browseForDirectoty)
		{
			var  stringListDialog = new StringListEditorDialog();
			stringListDialog.TitleText = title;
			stringListDialog.ListCaption = listCaption;
			stringListDialog.BrowseForDirectory = browseForDirectoty;
			string[] strings = textBox.Text.Split(';');
			stringListDialog.LoadList (strings);
			stringListDialog.ShowDialog();
			if (stringListDialog.DialogResult ?? false) {
				textBox.Text = String.Join(";",stringListDialog.GetList());
			}
		}
		

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IsDirty = true;
		}
		
	}
}
