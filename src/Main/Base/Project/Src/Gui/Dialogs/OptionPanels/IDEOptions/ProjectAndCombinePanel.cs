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
//using System.Windows.Forms;
//
//using ICSharpCode.SharpDevelop.Internal.ExternalTool;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//
//namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
//{
//	public class ProjectAndCombinePanel : AbstractOptionPanel
//	{
//		public override void LoadPanelContents()
//		{
//			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ProjectAndCombineOptions.xfrm"));
//			
//			// read properties
//			ControlDictionary["projectLocationTextBox"].Text = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.NewProjectDialog.DefaultPath", Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
//			                                                                                                                                                                 "SharpDevelop Projects")).ToString();
//			
//			BeforeCompileAction action = (BeforeCompileAction)PropertyService.Get("SharpDevelop.Services.ParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
//			
//			((RadioButton)ControlDictionary["saveChangesRadioButton"]).Checked = action == BeforeCompileAction.SaveAllFiles;
//			((RadioButton)ControlDictionary["promptChangesRadioButton"]).Checked = action == BeforeCompileAction.PromptForSave;
//			((RadioButton)ControlDictionary["noSaveRadioButton"]).Checked = action == BeforeCompileAction.Nothing;
//			
//			((CheckBox)ControlDictionary["loadPrevProjectCheckBox"]).Checked = (bool)PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false);
//
//			((CheckBox)ControlDictionary["showTaskListCheckBox"]).Checked = (bool)PropertyService.Get("SharpDevelop.ShowTaskListAfterBuild", true);
//			((CheckBox)ControlDictionary["showOutputCheckBox"]).Checked = (bool)PropertyService.Get("SharpDevelop.ShowOutputWindowAtBuild", true);
//			
//			((Button)ControlDictionary["selectProjectLocationButton"]).Click += new EventHandler(SelectProjectLocationButtonClicked);
//		}
//		
//		public override bool StorePanelContents()
//		{
//			// check for correct settings
//			string projectPath = ControlDictionary["projectLocationTextBox"].Text;
//			if (projectPath.Length > 0) {
//				if (!FileUtility.IsValidFileName(projectPath)) {
//					MessageService.ShowError("Invalid project path specified");
//					return false;
//				}
//			}
//			
//			// set properties
//			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.NewProjectDialog.DefaultPath", projectPath);
//			
//			if (((RadioButton)ControlDictionary["saveChangesRadioButton"]).Checked) {
//				PropertyService.Set("SharpDevelop.Services.ParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
//			} else if (((RadioButton)ControlDictionary["promptChangesRadioButton"]).Checked) {
//				PropertyService.Set("SharpDevelop.Services.ParserService.BeforeCompileAction", BeforeCompileAction.PromptForSave);
//			} else if (((RadioButton)ControlDictionary["noSaveRadioButton"]).Checked) {
//				PropertyService.Set("SharpDevelop.Services.ParserService.BeforeCompileAction", BeforeCompileAction.Nothing);
//			}
//			
//			PropertyService.Set("SharpDevelop.LoadPrevProjectOnStartup", ((CheckBox)ControlDictionary["loadPrevProjectCheckBox"]).Checked);
//			
//			PropertyService.Set("SharpDevelop.ShowTaskListAfterBuild", ((CheckBox)ControlDictionary["showTaskListCheckBox"]).Checked);
//			PropertyService.Set("SharpDevelop.ShowOutputWindowAtBuild", ((CheckBox)ControlDictionary["showOutputCheckBox"]).Checked);
//			
//			return true;
//		}
//		
//		void SelectProjectLocationButtonClicked(object sender, EventArgs e)
//		{
//			FolderDialog fdiag = new  FolderDialog();
//			if (fdiag.DisplayDialog("Select default combile location") == DialogResult.OK) {
//				ControlDictionary["projectLocationTextBox"].Text = fdiag.Path;
//			}
//		}
//	}
//}
