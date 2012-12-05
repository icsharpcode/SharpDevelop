/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.07.2012
 * Time: 20:13
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.SourceAnalysis
{
	/// <summary>
	/// Interaction logic for AnalysisIdeOptionsPanelXaml.xaml
	/// </summary>
	public partial class AnalysisIdeOptionsPanel : OptionPanel
	{
		private bool enableModifyStyleCopSettings;
		
		public AnalysisIdeOptionsPanel()
		{
			InitializeComponent();
			DataContext = this;
			ShowStatus();
		}
		
		
		private void ShowStatus()
		{
			string path = StyleCopWrapper.FindStyleCopPath();
			if (path == null) {
				status.Text = StringParser.Parse("StyleCop not found in the given path.");
				EnableModifyStyleCopSettings = false;
			} else {
				status.Text = StringParser.Parse("StyleCop was found in: ") + Environment.NewLine + path;
				EnableModifyStyleCopSettings = true;
			}
		}
		
		
		public bool EnableModifyStyleCopSettings {
			get { return enableModifyStyleCopSettings; }
			set { enableModifyStyleCopSettings = value;
				base.RaisePropertyChanged("EnableModifyStyleCopSettings");
			}
		}
		
		
		private void FindStyleCopPath_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = "dll";
			dlg.Filter = StringParser.Parse("StyleCop|*" + StyleCopWrapper.STYLE_COP_FILE + "|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			if (dlg.ShowDialog() == true) {
				string path = dlg.FileName;
				if (StyleCopWrapper.IsStyleCopPath(path)) {
					StyleCopPath = path;
				} else {
					MessageService.ShowError(string.Format("Directory does not contain StyleCop (*{0}).", StyleCopWrapper.STYLE_COP_FILE));
				}
			}
			ShowStatus();
		}
		
		
		void ModifyStyleCopSettings_Click(object sender, RoutedEventArgs e)
		{
			var executable = Path.Combine(Path.GetDirectoryName(StyleCopWrapper.FindStyleCopPath()), "StyleCopSettingsEditor.exe");
			var parameters = "\"" + StyleCopWrapper.GetMasterSettingsFile() + "\"";

			if (!File.Exists(executable)) {
				LoggingService.Debug("StyleCopSettingsEditor.exe: " + executable);
				MessageService.ShowWarning("Unable to find the StyleCop Settings editor. Please specify the StyleCop location in Tools Options.");
				return;
			}
			
			using(Process p = Process.Start("\"" + executable + "\"", parameters))
			{
				// No need to wait for the settings dialog to close - we can leave it open.
			}
		}
		
		
		public static string StyleCopPath {
			get {
				return PropertyService.Get("SourceAnalysis.StyleCopPath");
			}
			set {
				PropertyService.Set("SourceAnalysis.StyleCopPath", value);
			}
		}
	}
}