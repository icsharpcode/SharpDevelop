using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace CSharpBinding.OptionPanels
{
	public class BuildOptions : AbstractOptionPanel
	{
		CSharpProject project;
		
		public BuildOptions()
		{
		}
		
		void ShowAdvancedOptions(object sender, EventArgs e)
		{
			using (AdvancedBuildOptionsDialog advancedBuildOptionsDialog = new AdvancedBuildOptionsDialog(project)) {
				advancedBuildOptionsDialog.ShowDialog(WorkbenchSingleton.MainForm);
			}
		}
		string Config   = "Debug";
		string Platform = "AnyCPU";
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.BuildOptions.xfrm"));
			this.project = (CSharpProject)((Properties)CustomizationObject).Get("Project");
			
			ConnectBrowseFolder("outputPathBrowseButton", "outputPathTextBox", "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
			Get<Button>("advancedOptions").Click += new EventHandler(ShowAdvancedOptions);
			
			Get<TextBox>("conditionalSymbols").Text = project.GetDefineConstants(Config, Platform);
			Get<TextBox>("conditionalSymbols").TextChanged += new EventHandler(Save);
			
			
			Get<CheckBox>("optimizeCode").Checked = project.GetOptimize(Config, Platform);
			Get<CheckBox>("optimizeCode").CheckedChanged += new EventHandler(Save);

			Get<CheckBox>("allowUnsafeCode").Checked = project.GetAllowUnsafeBlocks(Config, Platform);
			Get<CheckBox>("allowUnsafeCode").CheckedChanged += new EventHandler(Save);
			
			Get<ComboBox>("warningLevel").Items.AddRange( new object[] {0, 1, 2, 3, 4});
			Get<ComboBox>("warningLevel").Text = project.WarningLevel.ToString();
			Get<ComboBox>("warningLevel").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("suppressWarnings").Text = project.GetNoWarn(Config, Platform);
			Get<TextBox>("suppressWarnings").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("specificWarnings").Text = project.GetWarningsAsErrors(Config, Platform);
			Get<TextBox>("specificWarnings").TextChanged += new EventHandler(Save);
			
			
			WarningsAsErrors warningsAsErrors = project.GetTreatWarningsAsErrors(Config, Platform);
			Get<RadioButton>("none").Checked             = warningsAsErrors == WarningsAsErrors.None;
			Get<RadioButton>("none").CheckedChanged += new EventHandler(Save);
			
			Get<RadioButton>("specificWarnings").Checked = warningsAsErrors == WarningsAsErrors.Specific;
			Get<RadioButton>("specificWarnings").CheckedChanged  += new EventHandler(UpdateEnabledStates);
			Get<RadioButton>("specificWarnings").CheckedChanged += new EventHandler(Save);
			
			Get<RadioButton>("all").Checked              = warningsAsErrors == WarningsAsErrors.All;
			Get<RadioButton>("all").CheckedChanged += new EventHandler(Save);
			
			Get<TextBox>("outputPath").Text = project.GetOutputPath(Config, Platform);
			Get<TextBox>("outputPath").TextChanged += new EventHandler(Save);
			
			Get<TextBox>("xmlDocumentation").Text = project.GetDocumentationFile(Config, Platform);
			Get<TextBox>("xmlDocumentation").TextChanged += new EventHandler(Save);
			
			Get<CheckBox>("xmlDocumentation").Checked = Get<TextBox>("xmlDocumentation").Text.Length > 0;
			Get<CheckBox>("xmlDocumentation").CheckedChanged  += new EventHandler(UpdateEnabledStates);
			
			Get<CheckBox>("registerCOMInterop").Checked = project.GetRegisterForComInterop(Config, Platform);
			Get<CheckBox>("registerCOMInterop").CheckedChanged += new EventHandler(Save);
			
			
			UpdateEnabledStates(this, EventArgs.Empty);
		}
		
		void Save(object sender, EventArgs e) 
		{
			StorePanelContents();
		}
		
		void UpdateEnabledStates(object sender, EventArgs e)
		{
			Get<TextBox>("specificWarnings").Enabled = Get<RadioButton>("specificWarnings").Checked;
			Get<TextBox>("xmlDocumentation").Enabled = Get<CheckBox>("xmlDocumentation").Checked;
		}
		public override bool StorePanelContents()
		{
			project.SetDefineConstants(Config, Platform, Get<TextBox>("conditionalSymbols").Text);
			project.SetOptimize(Config, Platform, Get<CheckBox>("optimizeCode").Checked);
			project.SetAllowUnsafeBlocks(Config, Platform, Get<CheckBox>("allowUnsafeCode").Checked);
			project.WarningLevel = Int32.Parse(Get<ComboBox>("warningLevel").Text);
			project.SetNoWarn(Config, Platform, Get<TextBox>("suppressWarnings").Text);
			project.SetWarningsAsErrors(Config, Platform, Get<TextBox>("specificWarnings").Text);
			
			if (Get<RadioButton>("none").Checked) {
				project.SetTreatWarningsAsErrors(Config, Platform, WarningsAsErrors.None);
			} else if (Get<RadioButton>("specificWarnings").Checked) {
				project.SetTreatWarningsAsErrors(Config, Platform, WarningsAsErrors.Specific);
			} else {
				project.SetTreatWarningsAsErrors(Config, Platform, WarningsAsErrors.All);
			}
			project.SetOutputPath(Config, Platform, Get<TextBox>("outputPath").Text);
			
			if (Get<CheckBox>("xmlDocumentation").Checked) {
				project.SetDocumentationFile(Config, Platform, Get<TextBox>("xmlDocumentation").Text);
			} else {
				project.SetDocumentationFile(Config, Platform, "");
			}
			
			project.SetRegisterForComInterop(Config, Platform, Get<CheckBox>("registerCOMInterop").Checked);
			project.Save();
			
			return true;
		}
	}

	public class AdvancedBuildOptionsDialog : BaseSharpDevelopForm
	{
		string Config   = "Debug";
		string Platform = "AnyCPU";
		CSharpProject project;
		
		public AdvancedBuildOptionsDialog(CSharpProject project)
		{
			this.project = project;
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.AdvancedBuildOptionsDialog.xfrm"));
			
			Get<ComboBox>("languageVersion").Text = project.GetLangVersion(Config, Platform);
			Get<ComboBox>("languageVersion").Items.AddRange(new string[] { "default", "ISO-1"});
			
			Get<ComboBox>("reportCompilerError").Text = project.GetErrorReport(Config, Platform);
			Get<ComboBox>("reportCompilerError").Items.AddRange(new string[] { "none", "prompt", "send"});
			
			
			Get<CheckBox>("checkOverUnderflow").Checked = project.GetCheckForOverflowUnderflow(Config, Platform);
			Get<CheckBox>("noMsCorLibReference").Checked = project.GetNoStdLib(Config, Platform);
			
			Get<ComboBox>("debugInfo").Text = project.GetDebugType(Config, Platform);
			Get<ComboBox>("debugInfo").Items.AddRange(new string[] { "none", "full", "pdb-only"});
			Get<ComboBox>("fileAlignment").Text = project.GetFileAlignment(Config, Platform).ToString();
			Get<ComboBox>("fileAlignment").Items.AddRange(new string[] { "512", "1024", "2048", "4096", "8192"});
			
			
			Get<ComboBox>("DLLBaseAddress").Text = project.GetBaseAddress(Config, Platform).ToString();
			
			Get<Button>("ok").Click += new EventHandler(OkButtonClick);
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			if (Get<ComboBox>("languageVersion").Text != "default") {
				project.SetLangVersion(Config, Platform, Get<ComboBox>("languageVersion").Text);
			} else {
				project.SetLangVersion(Config, Platform, "");
			}
			
			project.SetErrorReport(Config, Platform, Get<ComboBox>("reportCompilerError").Text);
			project.SetCheckForOverflowUnderflow(Config, Platform, Get<CheckBox>("checkOverUnderflow").Checked);
			project.SetNoStdLib(Config, Platform, Get<CheckBox>("noMsCorLibReference").Checked);
			project.SetDebugType(Config, Platform, Get<ComboBox>("debugInfo").Text);
			project.SetFileAlignment(Config, Platform, Int32.Parse(Get<ComboBox>("fileAlignment").Text));
			project.SetBaseAddress(Config, Platform, Int32.Parse(Get<ComboBox>("DLLBaseAddress").Text));
		}
	}
}
