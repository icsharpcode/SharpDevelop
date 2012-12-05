/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.07.2012
 * Time: 18:51
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SourceAnalysis
{
	/// <summary>
	/// Interaction logic for AnalysisIdeOptionsPanelXaml.xaml
	/// </summary>
	public partial class AnalysisProjectOptionsPanel : ProjectOptionPanel
	{
		public AnalysisProjectOptionsPanel()
		{
			InitializeComponent();
		}
		
		
		public ProjectProperty<string> SourceAnalysisOverrideSettingsFile {
			get { return GetProperty("SourceAnalysisOverrideSettingsFile", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			if (String.IsNullOrEmpty(SourceAnalysisOverrideSettingsFile.Value)) {
				SourceAnalysisOverrideSettingsFile.Value = StyleCopWrapper.GetMasterSettingsFile();
				SourceAnalysisOverrideSettingsFile.Location =  PropertyStorageLocations.Base;
				IsDirty = false;
			}
		}
		
		
		void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			BrowseForFile(SourceAnalysisOverrideSettingsFile, "");
		}
		
		
		void ModifyStyleCopSettings_Click(object sender, RoutedEventArgs e)
		{

			var settingsFile = SourceAnalysisOverrideSettingsFile.Value;
			
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
				executable = Path.Combine(Path.GetDirectoryName(styleCopPath), "StyleCopSettingsEditor.exe");
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
		
		
		private bool ConfirmReplaceMissingFile()
		{
			return  MessageService.AskQuestion("A settings file is not present. Would you like to copy the master into the " +
			                             "project folder?",
			                             "Missing Settings File");
		}
		
		
		private bool ConfirmSwitchFromMaster()
		{
			return MessageService.AskQuestion("You are currently using the master settings file. Do you want to make a "
			                                  + "copy in the project folder instead?",
			                                  "Using Master Settings File");
		}
		
		
		private string CopyFromMaster()
		{
			var newSettingsFile = Project.Directory + "\\Settings.SourceAnalysis";
			System.IO.File.Copy(
				StyleCopWrapper.GetMasterSettingsFile(),
				newSettingsFile,
				true
			);
			SourceAnalysisOverrideSettingsFile.Value = newSettingsFile;
			SourceAnalysisOverrideSettingsFile.Location = PropertyStorageLocations.Base;
			return newSettingsFile;
		}
	}
}