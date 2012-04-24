/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.04.2012
 * Time: 20:14
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Interaction logic for LinkerOptionsXaml.xaml
	/// </summary>
	public partial class LinkerOptionsXaml :  ProjectOptionPanel,INotifyPropertyChanged
	{
		private const string metaElement ="Link";
		private MSBuildBasedProject project;
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public LinkerOptionsXaml()
		{
			InitializeComponent();
		}
		
		void Initialize()
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project,
			                                                                  project.ActiveConfiguration, project.ActivePlatform);
			
			this.additionalLibsTextBox.Text = group.GetElementMetadata(metaElement,"AdditionalDependencies");
			
			this.addModuleTextBox.Text = group.GetElementMetadata(metaElement,"AddModuleNamesToAssembly");
			
			this.resourceFileTextBox.Text = group.GetElementMetadata(metaElement,"EmbedManagedResourceFile");
			
			this.additionalOptionsTextBox.Text = group.GetElementMetadata(metaElement,"AdditionalOptions");
			
			var def = group.GetElementMetadata(metaElement,"GenerateDebugInformation");
			bool check;
			if (bool.TryParse(def, out check))
			{
				this.CheckBoxChecked = check;
				this.debugInfoCheckBox.IsChecked = check;
			}
			
			IsDirty = false;
		}
		
		#region Properties
		
		public ProjectProperty<string> LibraryPath {
			get { return GetProperty("LibraryPath", "", TextBoxEditMode.EditRawProperty); }
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
		
		#region Save/Load
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
			
			
			group.SetElementMetadata(metaElement,"AdditionalDependencies",this.additionalLibsTextBox.Text);
			group.SetElementMetadata(metaElement,"AddModuleNamesToAssembly",this.addModuleTextBox.Text);
			group.SetElementMetadata(metaElement,"EmbedManagedResourceFile",this.resourceFileTextBox.Text);
			group.SetElementMetadata(metaElement,"AdditionalOptions",this.additionalOptionsTextBox.Text);
			
			string check = "false";
			if ((bool)this.debugInfoCheckBox.IsChecked) {
				check = "true";
			}
			
			group.SetElementMetadata(metaElement,"GenerateDebugInformation",check);
			
			return base.Save(project, configuration, platform);
		}
		
		
		#endregion
		
		void LibraryPathButton_Click(object sender, RoutedEventArgs e)
		{
			var dlg = InitStringListEditor(StringParser.Parse("${res:Global.Folder}:"),
			                               StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Linker.Library}:"),true);

			string[] strings = this.libraryPathTextBox.Text.Split(';');
			dlg.LoadList (strings);
			dlg.ShowDialog();
			
			if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
			{
				this.libraryPathTextBox.Text =  String.Join(";", dlg.GetList());
			}
		}
		
		
		void AdditionalLibsButton_Click(object sender, RoutedEventArgs e)
		{
			var dlg = InitStringListEditor(StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                               StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Linker.AdditionalLibs}:"),false);
			string[] strings = this.additionalLibsTextBox.Text.Split(';');
			dlg.LoadList (strings);
			dlg.ShowDialog();
			
			if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
			{
				this.additionalLibsTextBox.Text = String.Join(";",dlg.GetList());
			}
		}

		
		void AddModuleButton_Click(object sender, RoutedEventArgs e)
		{
			var dlg = InitStringListEditor(StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                               StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Linker.AddModule}"),false);
			
			string[] strings = this.addModuleTextBox.Text.Split(';');
			dlg.LoadList (strings);
			dlg.ShowDialog();
			
			if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
			{
				this.addModuleTextBox.Text = String.Join(";",dlg.GetList());
			}
		}
		
		
		void ResourceFileButton_Click(object sender, RoutedEventArgs e)
		{
			var dlg = InitStringListEditor(StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:"),
			                               StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.Linker.ManagedResourceFile}"),false);
			string[] strings = this.resourceFileTextBox.Text.Split(';');
			dlg.LoadList (strings);
			dlg.ShowDialog();
			
			if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
			{
				this.resourceFileTextBox.Text = String.Join(";",dlg.GetList());
			}
		}
		
		
		static StringListEditorDialogXaml InitStringListEditor(string title, string listCaption,bool browseForDirectoty)
		{
			StringListEditorDialogXaml dlg = new StringListEditorDialogXaml();
			dlg.TitleText = title;
			dlg.ListCaption = listCaption;
			dlg.BrowseForDirectory = browseForDirectoty;
			return dlg;
		}
		
		
	}
}