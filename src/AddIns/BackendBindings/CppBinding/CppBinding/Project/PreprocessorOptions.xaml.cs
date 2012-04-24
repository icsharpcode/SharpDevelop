/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 14.04.2012
 * Time: 17:53
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
	public partial class PreprocessorOptions : ProjectOptionPanel,INotifyPropertyChanged
	{
		private const string metaElement ="ClCompile";
		
		private MSBuildBasedProject project;
		
		public PreprocessorOptions()
		{
			InitializeComponent();
		}
		
		
		private void Initialize()
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project,
			                                                                  project.ActiveConfiguration, project.ActivePlatform);
			
			this.defineTextBox.Text = group.GetElementMetadata(metaElement,"PreprocessorDefinitions");
			
			this.undefineTextBox.Text =  group.GetElementMetadata(metaElement,"UndefinePreprocessorDefinitions" );
			
			var defs = group.GetElementMetadata(metaElement,"UndefineAllPreprocessorDefinitions");
			
			bool check;
			if (bool.TryParse(defs, out check))
			{
				this.CheckBoxChecked = check;
				this.undefineAllCheckBox.IsChecked = check;
			}
			IsDirty = false;
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
				if (PropertyChanged != null)
					PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("UnCheck"));
				IsDirty = true;
			}
		}
		
		#endregion
		
		
		#region overrides
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			HideHeader();
		}
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			this.project = project;
			Initialize();
		}
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project,
			                                                                  project.ActiveConfiguration, project.ActivePlatform);
			
			group.SetElementMetadata(metaElement,"PreprocessorDefinitions",this.defineTextBox.Text);
			
			group.SetElementMetadata(metaElement,"UndefinePreprocessorDefinitions",this.undefineTextBox.Text);
			
			string check = "false";
			if ((bool)this.undefineAllCheckBox.IsChecked) {
				check = "true";
			}
			group.SetElementMetadata(metaElement,"UndefineAllPreprocessorDefinitions",check);
					
			return base.Save(project, configuration, platform);
		}
		#endregion
		
		
		void IncludePathButton_Click(object sender, RoutedEventArgs e)
		{
			StringListEditorDialogXaml dlg = new StringListEditorDialogXaml();
			dlg.TitleText = StringParser.Parse("${res:Global.Folder}:");
			dlg.ListCaption = StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Preprocessor.Includes}:");
			dlg.BrowseForDirectory = true;
			string[] strings = this.includePathTextBox.Text.Split(';');
			dlg.LoadList (strings);
			dlg.ShowDialog();
			
			if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
			{
				this.includePathTextBox.Text =  String.Join(";", dlg.GetList());
			}
			
		}
		
		void DefinePathButton_Click(object sender, RoutedEventArgs e)
		{
			StringListEditorDialogXaml dlg = new StringListEditorDialogXaml();
			dlg.TitleText = StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:");
			dlg.ListCaption = StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Preprocessor.Definitions}:");
			
			string[] strings = this.defineTextBox.Text.Split(';');
			dlg.LoadList (strings);
			dlg.ShowDialog();
			
			if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
			{
				this.defineTextBox.Text = String.Join(";",dlg.GetList());
			}
		}
		
		
		void UndefineButton_Click(object sender, RoutedEventArgs e)
		{
			StringListEditorDialogXaml dlg = new StringListEditorDialogXaml();
			dlg.TitleText = StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:");
			dlg.ListCaption = StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:");
			string[] strings = this.undefineTextBox.Text.Split(';');
			dlg.LoadList (strings);
			dlg.BrowseForDirectory = false;
			dlg.ShowDialog();
			
			if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
			{
				this.undefineTextBox.Text = String.Join(";",dlg.GetList());
			}
		}
		
		
		void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IsDirty = true;
		}
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	}
}