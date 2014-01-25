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
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Interaction logic for PreprocessorOptionsXaml.xaml
	/// </summary>
	public partial class PreprocessorOptions : ProjectOptionPanel
	{
		private const string metaElement ="ClCompile";
	
		
		public PreprocessorOptions()
		{
			InitializeComponent();
			this.HeaderVisibility = Visibility.Collapsed;
		}
		
		#region Properties
		
		public ProjectProperty<string> IncludePath {
			get { return GetProperty("IncludePath", "", TextBoxEditMode.EditRawProperty); }
		}
		
		bool checkBoxChecked;
		
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
		
		#region overrides
		
		protected override void Initialize()
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(base.Project, base.Project.ActiveConfiguration);
			
			this.defineTextBox.Text = GetElementMetaData(group,"PreprocessorDefinitions");
			
			this.undefineTextBox.Text =  GetElementMetaData(group,"UndefinePreprocessorDefinitions" );
			
			string defs = GetElementMetaData(group,"UndefineAllPreprocessorDefinitions");
			
			bool check;
			if (bool.TryParse(defs, out check))
			{
				this.CheckBoxChecked = check;
				this.undefineAllCheckBox.IsChecked = check;
			}
			IsDirty = false;
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, project.ActiveConfiguration);
			
			SetElementMetaData(group,"PreprocessorDefinitions",this.defineTextBox.Text);
			
			SetElementMetaData(group,"UndefinePreprocessorDefinitions",this.undefineTextBox.Text);
			
			string check = "false";
			if ((bool)this.undefineAllCheckBox.IsChecked) {
				check = "true";
			}
			SetElementMetaData(group,"UndefineAllPreprocessorDefinitions",check);
					
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
		
		
		private void IncludePathButton_Click(object sender, RoutedEventArgs e)
		{
			LinkerOptions.PopulateStringListEditor(StringParser.Parse("${res:Global.Folder}:"),
			                                           StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Preprocessor.Includes}:"),
			                                           this.includePathTextBox,
			                                           true);
		}
		
		
		private void DefinePathButton_Click(object sender, RoutedEventArgs e)
		{
			LinkerOptions.PopulateStringListEditor(StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                                           StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Preprocessor.Definitions}:"),
			                                           this.defineTextBox,
			                                           false);
		}
		
		
		private void UndefineButton_Click(object sender, RoutedEventArgs e)
		{
			LinkerOptions.PopulateStringListEditor(StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                                           StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                                           this.undefineTextBox,
			                                           false);
		}
		
		
		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IsDirty = true;
		}
	}
}
