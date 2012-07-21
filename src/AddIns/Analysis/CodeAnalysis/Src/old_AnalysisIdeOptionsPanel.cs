// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeAnalysis
{
	/// <summary>
	/// Option panel to choose the FxCop path from.
	/// </summary>
	public class AnalysisIdeOptionsPanel : XmlFormsOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.CodeAnalysis.Resources.AnalysisIdeOptionPanel.xfrm"));
			
			ShowStatus();
			Get<Button>("findFxCopPath").Click += FindFxCopPathClick;
		}
		
		void ShowStatus()
		{
			string path = FxCopWrapper.FindFxCopPath();
			if (path == null) {
				Get<Label>("status").Text = StringParser.Parse("${res:ICSharpCode.CodeAnalysis.IdeOptions.FxCopNotFound}");
			} else {
				Get<Label>("status").Text = StringParser.Parse("${res:ICSharpCode.CodeAnalysis.IdeOptions.FxCopFoundInPath}")
					+ Environment.NewLine + path;
			}
		}
		
		void FindFxCopPathClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.DefaultExt = "exe";
				dlg.Filter = StringParser.Parse("FxCop|fxcop.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				if (dlg.ShowDialog() == DialogResult.OK) {
					string path = Path.GetDirectoryName(dlg.FileName);
					if (FxCopWrapper.IsFxCopPath(path)) {
						FxCopPath = path;
					} else {
						MessageService.ShowError("${res:ICSharpCode.CodeAnalysis.IdeOptions.DirectoryDoesNotContainFxCop}");
					}
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
