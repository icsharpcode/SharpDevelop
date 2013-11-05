// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Win32;

namespace ICSharpCode.CodeAnalysis
{
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
			get { return PropertyService.Get("CodeAnalysis.FxCopPath", String.Empty); }
			set { PropertyService.Set("CodeAnalysis.FxCopPath", value); }
		}
	}
}
