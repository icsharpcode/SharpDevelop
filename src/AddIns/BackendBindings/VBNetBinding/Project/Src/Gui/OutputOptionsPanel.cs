// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace VBBinding
{
	public class OutputOptionsPanel : AbstractOptionPanel
	{
		VBCompilerParameters compilerParameters;
		static 
		
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null) {
					return true;
				}
				
				
				if (!FileUtility.IsValidFileName(ControlDictionary["assemblyNameTextBox"].Text)) {
					MessageBox.Show("Invalid assembly name specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				if (!FileUtility.IsValidFileName(ControlDictionary["outputDirectoryTextBox"].Text)) {
					MessageBox.Show("Invalid output directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				
				if (ControlDictionary["win32IconTextBox"].Text.Length > 0) {
					if (!FileUtility.IsValidFileName(ControlDictionary["win32IconTextBox"].Text)) {
						MessageBox.Show("Invalid Win32Icon specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return false;
					}
					if (!File.Exists(ControlDictionary["win32IconTextBox"].Text)) {
						MessageBox.Show("Win32Icon doesn't exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return false;
					}
				}
				
				compilerParameters.CompileTarget               = (CompileTarget)((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex;
				compilerParameters.OutputAssembly              = ControlDictionary["assemblyNameTextBox"].Text;
				compilerParameters.OutputDirectory             = ControlDictionary["outputDirectoryTextBox"].Text;
				compilerParameters.CommandLineParameters       = ControlDictionary["parametersTextBox"].Text;
				compilerParameters.ExecuteBeforeBuild          = ControlDictionary["executeBeforeTextBox"].Text;
				compilerParameters.ExecuteAfterBuild           = ControlDictionary["executeAfterTextBox"].Text;
				compilerParameters.ExecuteScript               = ControlDictionary["executeScriptTextBox"].Text;
				compilerParameters.Win32Icon                   = ControlDictionary["win32IconTextBox"].Text;
				compilerParameters.ExecuteBeforeBuildArguments = ControlDictionary["executeBeforeArgumentsTextBox"].Text;
				compilerParameters.ExecuteAfterBuildArguments  = ControlDictionary["executeAfterArgumentsTextBox"].Text;
				
				compilerParameters.PauseConsoleOutput = ((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked;
			}
			return true;
		}
	
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (VBCompilerParameters)((Properties)CustomizationObject).Get("Config");
			
			((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex = (int)compilerParameters.CompileTarget;
			ControlDictionary["win32IconTextBox"].Text              = compilerParameters.Win32Icon;
			ControlDictionary["assemblyNameTextBox"].Text           = compilerParameters.OutputAssembly;
			ControlDictionary["outputDirectoryTextBox"].Text        = compilerParameters.OutputDirectory;
			ControlDictionary["parametersTextBox"].Text             = compilerParameters.CommandLineParameters;
			ControlDictionary["executeScriptTextBox"].Text          = compilerParameters.ExecuteScript;
			ControlDictionary["executeBeforeTextBox"].Text          = compilerParameters.ExecuteBeforeBuild;
			ControlDictionary["executeAfterTextBox"].Text           = compilerParameters.ExecuteAfterBuild;
			ControlDictionary["executeBeforeArgumentsTextBox"].Text = compilerParameters.ExecuteBeforeBuildArguments;
			ControlDictionary["executeAfterArgumentsTextBox"].Text  = compilerParameters.ExecuteAfterBuildArguments;
			
			((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked = compilerParameters.PauseConsoleOutput;
		}
		
		void SelectFolder(object sender, EventArgs e)
		{
			FolderDialog fdiag = new  FolderDialog();
			
			if (fdiag.DisplayDialog("${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}") == DialogResult.OK) {
				ControlDictionary["outputDirectoryTextBox"].Text = fdiag.Path;
			}
		}
		
		void SelectFile2(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
				ControlDictionary["executeBeforeTextBox"].Text = fdiag.FileName;
			}
		}
		
		void SelectFile3(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
				ControlDictionary["executeAfterTextBox"].Text = fdiag.FileName;
			}
		}
		void SelectFile4(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
				ControlDictionary["executeScriptTextBox"].Text = fdiag.FileName;
			}
		}
		void SelectWin32Icon(object sender, EventArgs e) 
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.Icons}|*.ico|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					ControlDictionary["win32IconTextBox"].Text = fdiag.FileName;
				}
			}
		}
		
		static 
		public OutputOptionsPanel() : base(PropertyService.DataDirectory + @"\resources\panels\ProjectOptions\OutputPanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
			ControlDictionary["browseButton"].Click += new EventHandler(SelectFolder);
			ControlDictionary["browseButton2"].Click += new EventHandler(SelectFile2);
			ControlDictionary["browseButton3"].Click += new EventHandler(SelectFile3);
			ControlDictionary["browseButton4"].Click += new EventHandler(SelectFile4);
			ControlDictionary["browseWin32IconButton"].Click += new EventHandler(SelectWin32Icon);
			
			
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Library"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Module"));
			
		}
	}

}
