/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.07.2012
 * Time: 21:27
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.CodeAnalysis
{
	/// <summary>
	/// Interaction logic for AnalysisIdeOptionsPanelXaml.xaml
	/// </summary>
	public partial class AnalysisIdeOptionsPanel : OptionPanel
	{
		public AnalysisIdeOptionsPanel()
		{
			InitializeComponent();
			ShowStatus();
		}
		
		
		private void ShowStatus()
		{
			string path = FxCopWrapper.FindFxCopPath();
			if (path == null) {
				status.Text = StringParser.Parse("${res:ICSharpCode.CodeAnalysis.IdeOptions.FxCopNotFound}");
			} else {
				status.Text = StringParser.Parse("${res:ICSharpCode.CodeAnalysis.IdeOptions.FxCopFoundInPath}")
					+ Environment.NewLine + path;
			}
		}
		
		
		private void FindFxCopPath_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = "exe";
			dlg.Filter = StringParser.Parse("FxCop|fxcop.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			if (dlg.ShowDialog() == true) {
				string path = Path.GetDirectoryName(dlg.FileName);
				if (FxCopWrapper.IsFxCopPath(path)) {
					FxCopPath = path;
				} else {
					MessageService.ShowError("${res:ICSharpCode.CodeAnalysis.IdeOptions.DirectoryDoesNotContainFxCop}");
				}
			}
			ShowStatus();
		}
		
		
		public static string FxCopPath {
			get {
				return PropertyService.Get("CodeAnalysis.FxCopPath");
			}
			set {
				PropertyService.Set("CodeAnalysis.FxCopPath", value);
			}
		}
	}
}