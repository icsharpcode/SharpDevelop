// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matt Everson" email="ti.just.me@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace MattEverson.SourceAnalysis
{
	/// <summary>
	/// Option panel to choose the StyleCop path from.
	/// </summary>
	public class AnalysisIdeOptionsPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("MattEverson.SourceAnalysis.Resources.AnalysisIdeOptionPanel.xfrm"));
			
			ShowStatus();
			Get<Button>("FindStyleCopPath").Click += FindStyleCopPathClick;
			Get<Button>("ModifyStyleCopSettings").Click += ModifyStyleCopSettingsClick;
		}
		
		void ShowStatus()
		{
			string path = StyleCopWrapper.FindStyleCopPath();
			if (path == null) {
				Get<Label>("status").Text = StringParser.Parse("StyleCop not found in the given path.");
			} else {
				Get<Label>("status").Text = StringParser.Parse("StyleCop was found in: ")
					+ Environment.NewLine + path;
			}
		}
		
		void FindStyleCopPathClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.DefaultExt = "dll";
				dlg.Filter = StringParser.Parse("StyleCop|Microsoft.SourceAnalysis.dll|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				if (dlg.ShowDialog() == DialogResult.OK) {
					string path = Path.GetDirectoryName(dlg.FileName);
					if (StyleCopWrapper.IsStyleCopPath(path)) {
						StyleCopPath = path;
					} else {
						MessageService.ShowError("Directory does not contain StyleCop.");
					}
				}
			}
			ShowStatus();
		}
		
		void ModifyStyleCopSettingsClick(object sender, EventArgs e)
		{
		    var executable = "\"" + StyleCopWrapper.FindStyleCopPath() + "\\SourceAnalysisSettingsEditor.exe\"";
		    var parameters = AnalysisProjectOptionsPanel.MasterSettingsFileName;
		    using(Process p = Process.Start(executable, parameters))
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
