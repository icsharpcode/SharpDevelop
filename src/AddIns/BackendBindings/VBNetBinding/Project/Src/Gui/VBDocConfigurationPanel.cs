// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace VBBinding
{
	public class VBDOCConfigurationPanel  : AbstractOptionPanel
	{
		VBCompilerParameters compilerParameters = null;
		VBProject project = null;
		
		static 
		static 
		
		///<summary>
		/// Returns if the filename will be parsed when running VB.DOC.
		/// </summary>
		public static bool IsFileIncluded(string filename, VBProject project)
		{
			VBCompilerParameters compilerparameters = (VBCompilerParameters)project.ActiveConfiguration;
			return Array.IndexOf(compilerparameters.VBDOCFiles, filename) == -1;
		}
		
		
		public VBDOCConfigurationPanel() : base(PropertyService.DataDirectory + @"\resources\panels\ProjectOptions\VBDOCConfigurationPanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
			ControlDictionary["BrowseOutputFileButton"].Click += new EventHandler(BrowseOutputFileButton_Click);
		}
		
		private void BrowseOutputFileButton_Click(object sender, EventArgs e) {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "XML files (*.xml)|*.xml";
			if(dialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
				((TextBox)ControlDictionary["OutputFileTextBox"]).Text = dialog.FileName;
			}
		}
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null) {
					return true;
				}
				
				compilerParameters.VBDOCOutputFile = ((TextBox)ControlDictionary["OutputFileTextBox"]).Text;
				compilerParameters.VBDOCCommentPrefix = ((TextBox)ControlDictionary["CommentPrefixTextBox"]).Text;
				
				string[] files = new string[((CheckedListBox)ControlDictionary["FileListBox"]).Items.Count - ((CheckedListBox)ControlDictionary["FileListBox"]).CheckedIndices.Count];
				int count = 0;
				for(int index = 0; index < ((CheckedListBox)ControlDictionary["FileListBox"]).Items.Count; index++) {
					if(((CheckedListBox)ControlDictionary["FileListBox"]).GetItemChecked(index) == false) {
						files[count] = (string)((CheckedListBox)ControlDictionary["FileListBox"]).Items[index];
						count++;
					}
				}
				compilerParameters.VBDOCFiles = files;
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (VBCompilerParameters)((Properties)CustomizationObject).Get("Config");
			project = (VBProject)((Properties)CustomizationObject).Get("Project");
			
			((TextBox)ControlDictionary["OutputFileTextBox"]).Text = compilerParameters.VBDOCOutputFile;
			((TextBox)ControlDictionary["CommentPrefixTextBox"]). Text = compilerParameters.VBDOCCommentPrefix;
			
			foreach(ProjectFile pfile in project.ProjectFiles) {
				bool included = IsFileIncluded(pfile.Name, project);
				((CheckedListBox)ControlDictionary["FileListBox"]).Items.Add(pfile.Name, included);
			}
		}
	}
}
