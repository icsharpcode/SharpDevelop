//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.IO;
//using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
//using System.Windows.Forms;
//
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//
//namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
//{
//	public class GeneralProjectOptions : AbstractOptionPanel
//	{
//		IProject project;
//		static 
//		
//		public override void LoadPanelContents()
//		{
//			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.GeneralProjectOptions.xfrm"));
//			
//			((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).CheckedChanged += new EventHandler(AutoLoadCheckBoxCheckedChangeEvent);
//			(ControlDictionary["BrowseButton"]).Click += new EventHandler(BrowseFileEvent);
//			
//			this.project = (IProject)((Properties)CustomizationObject).Get("Project");
//			
//			ControlDictionary["ProjectNameTextBox"].Text        = project.Name;
//			ControlDictionary["ProjectNamespaceTextBox"].Text   = project.StandardNamespace;
//			ControlDictionary["ProjectDescriptionTextBox"].Text = project.Description;
//			
//			((CheckBox)ControlDictionary["EnableViewStateCheckBox"]).Checked = project.EnableViewState;
//			
//			switch (project.NewFileSearch) {
//				case NewFileSearch.None:
//					((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked = ((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Checked = false;
//					break;
//				case NewFileSearch.OnLoad:
//					((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked = true;
//					((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Checked = false;
//					break;
//				default:
//					((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked = ((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Checked = true;
//					break;
//			}
//			AutoLoadCheckBoxCheckedChangeEvent(null, null);
//		}
//		
//		public override bool StorePanelContents()
//		{
//			project.Name              = ControlDictionary["ProjectNameTextBox"].Text;
//			project.StandardNamespace = ControlDictionary["ProjectNamespaceTextBox"].Text;
//			project.Description       = ControlDictionary["ProjectDescriptionTextBox"].Text;
//			
//			project.EnableViewState = ((CheckBox)ControlDictionary["EnableViewStateCheckBox"]).Checked;
//			
//			if (((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked) {
//				project.NewFileSearch = ((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Checked ?  NewFileSearch.OnLoadAutoInsert : NewFileSearch.OnLoad;
//			} else {
//				project.NewFileSearch = NewFileSearch.None;
//			}
//			return true;
//		}
//		
//		void AutoLoadCheckBoxCheckedChangeEvent(object sender, EventArgs e)
//		{
//			((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Enabled = ((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked;
//		}
//		
//		void BrowseFileEvent(object sender, EventArgs e)
//		{
//			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
//				fdiag.CheckFileExists = true;
//				
//				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
//					ControlDictionary["ProjectDocumentationLocationTextBox"].Text = fdiag.FileName;
//				}
//			}
//		}
//	}
//}
