//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.IO;
//using System.Threading;
//using System.Drawing;
//using System.Drawing.Printing;
//using System.Collections;
//using System.ComponentModel;
//using System.Windows.Forms;
//using System.Diagnostics;
//
//using ICSharpCode.Core;
//
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//
//using ICSharpCode.Core;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Gui.ProjectBrowser;
//
//namespace ICSharpCode.SharpDevelop.Commands.ProjectBrowser
//{
//	public class AddResourceToProject : AbstractMenuCommand
//	{
//		public override void Run()
//		{
//			ProjectBrowserView browser = (ProjectBrowserView)Owner;
//			FolderNode         node    = browser.SelectedNode as FolderNode;
//			
//			if (node != null) {
//				using (OpenFileDialog fdiag  = new OpenFileDialog()) {
//					fdiag.AddExtension    = true;
//					
//					
//					fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.ResourceFiles}|*.resources;*.resx|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
//					fdiag.Multiselect     = true;
//					fdiag.CheckFileExists = false; //// Alex: allows creation of new file
//					
//					if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
//						IProject project = ((ProjectBrowserNode)node.Parent).Project;
//						
//						
//						
//						foreach (string fileName in fdiag.FileNames) {
////// Alex: allows creation of new resource file when it doesn't exist
//							if (!File.Exists(fileName)) {
//								FileStream fs=File.Create(fileName);
//								fs.Close();
//							}
//							ProjectFile fileInformation = ProjectService.AddFileToProject(project, fileName, BuildAction.EmbedAsResource);
//							
//							AbstractBrowserNode newResNode = new FileNode(fileInformation);
//							newResNode.IconImage = ResourceService.GetBitmap("Icons.16x16.ResourceFileIcon");
//							node.Nodes.Add(newResNode);
//						}
//						node.Expand();
//						ProjectService.SaveCombine();
//					}
//				}
//			}
//		}
//	}
//	
//}
