//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
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
//using ICSharpCode.Core;
//
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//
//namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
//{
//	/// <summary>
//	/// Summary description for Form1.
//	/// </summary>
//	public class CompileFileProjectOptions : AbstractOptionPanel
//	{
//		IProject project;
//		
//		public override void LoadPanelContents()
//		{
//			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.CompileFileProjectOptions.xfrm"));
//			
//			this.project = (IProject)((Properties)CustomizationObject).Get("Project");
//			
//			foreach (ProjectFile info in project.ProjectFiles) {
//				if (info.BuildAction == BuildAction.Nothing || info.BuildAction == BuildAction.Compile) {
//					((CheckedListBox)ControlDictionary["IncludeFilesCheckedListBox"]).Items.Add(FileUtility.GetRelativePath(project.BaseDirectory, info.Name).Substring(2), info.BuildAction == BuildAction.Compile ? CheckState.Checked : CheckState.Unchecked);
//				}
//			}
//		}
//		
//		public override bool StorePanelContents()
//		{
//			
//			for (int i = 0; i < ((CheckedListBox)ControlDictionary["IncludeFilesCheckedListBox"]).Items.Count; ++i) {
//				string name = FileUtility.RelativeToAbsolutePath(project.BaseDirectory, "." + Path.DirectorySeparatorChar + ((CheckedListBox)ControlDictionary["IncludeFilesCheckedListBox"]).Items[i].ToString());
//
//				int j = 0;
//				while (j < project.ProjectFiles.Count && Path.GetFullPath(project.ProjectFiles[j].Name).ToLower() != Path.GetFullPath(name).ToLower()) {
//					++j;
//				}
//				
//				if (j < project.ProjectFiles.Count) {
//					project.ProjectFiles[j].BuildAction = ((CheckedListBox)ControlDictionary["IncludeFilesCheckedListBox"]).GetItemChecked(i) ? BuildAction.Compile : BuildAction.Nothing;
//				} else {
//					//// if file not found - we have to remove it from compiled ones for future
//					MessageService.ShowError("File " + name + " not found in " + project.Name+ Environment.NewLine + "File will be ignored");
//					project.ProjectFiles[i].BuildAction = BuildAction.Nothing;
//				}
//			}
//			
//			return true;
//		}
//	}
//}
