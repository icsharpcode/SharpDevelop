// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Converters;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs
{
	public class ChooseProjectLocationDialog : BaseSharpDevelopForm
	{
		public string FileName {
			get {
				return ControlDictionary["fileNameTextBox"].Text;
			}
			set {
				ControlDictionary["fileNameTextBox"].Text = value;
			}
		}
		
		public ChooseProjectLocationDialog()
		{
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("ChooseProjectLocationDialog.xfrm"));
			Icon = null;
			ControlDictionary["okButton"].Click += new EventHandler(OkButtonClick);
			ControlDictionary["browseButton"].Click += new EventHandler(BrowseProject);
			
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			if (!FileUtilityService.IsValidFileName(FileName)) {
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.ProjectImportExporter.Dialogs.ImportProjectDialog.InputFileInvalidError}");
				return;
			}
			
			if (FileUtilityService.TestFileExists(FileName)) {
				DialogResult = DialogResult.OK;
			}
		}
		
		void BrowseProject(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.Filter          = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					ControlDictionary["fileNameTextBox"].Text = fdiag.FileName;
				}
			}
		}
	}
}
