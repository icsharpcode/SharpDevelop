// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class AddReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.CurrentProject == null) {
				return;
			}
			using (SelectReferenceDialog selDialog = new SelectReferenceDialog(ProjectService.CurrentProject)) {
				if (selDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					foreach (ReferenceProjectItem reference in selDialog.ReferenceInformations) {
						ProjectService.AddReference(ProjectService.CurrentProject, reference);
					}
					ProjectService.SaveSolution();
				}
			}
		}
	}
	
	/*
	public class RefreshWebReference : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			ReferenceNode   node    = browser.SelectedNode as ReferenceNode;
			if (node != null) {				
				IProject project = node.Project;  //((ProjectBrowserNode)node.Parent.Parent).Project;
				ParserService ParserService = (ParserService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(ParserService));
				
				ProjectReference refInfo = (ProjectWebReference)node.UserData;
				WebReference.GenerateWebProxy(project, refInfo.HRef);				
				ParserService.AddReferenceToCompletionLookup(project, refInfo);							
			}
		}
	}*/
	
	public class AddWebReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
//			ProjectBrowserView browser = (ProjectBrowserView)Owner;
//			AbstractBrowserNode node   = browser.SelectedNode as AbstractBrowserNode;
//			AbstractBrowserNode projectNode = DefaultDotNetNodeBuilder.GetProjectNode(node);
//			bool bInitReferences = false;
//			
//			if (node != null) {
//				IProject project = ((ProjectBrowserNode)node.Parent).Project;
//				
//				
//				ParserService ParserService = (ParserService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(ParserService));					
//			
//				using (AddWebReferenceDialog refDialog = new AddWebReferenceDialog(project)) {
//					if (refDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {						
//						foreach(object objReference in refDialog.ReferenceInformations) {
//							if(objReference is ProjectReference) {
//								ProjectReference refInfo = (ProjectReference)objReference;
//								project.ProjectReferences.Add(refInfo);
//								if(refInfo.ReferenceType == ReferenceType.Assembly) {
//									ParserService.AddReferenceToCompletionLookup(project, refInfo);
//									bInitReferences = true;
//								}
//							} else if(objReference is ProjectFile) {
//								ProjectFile projectFile = (ProjectFile) objReference;
//								//HACK: fix later
//								if(projectFile.Subtype == Subtype.WebReferences || projectFile.Subtype == Subtype.Directory) {																		
//									AbstractBrowserNode checkNode = DefaultDotNetNodeBuilder.GetPath(FileUtility.GetRelativePath(project.BaseDirectory,projectFile.Name + Path.DirectorySeparatorChar), projectNode, false);
//									if(checkNode != null) {
//										continue;
//									}
//								}																																	
//								// add to the project browser
//								DefaultDotNetNodeBuilder.AddProjectFileNode(project, projectNode, projectFile);
//									
//								// add to the project
//								ProjectService.AddFileToProject(project, projectFile);
//								
//								// add to code completion
//								if(projectFile.Subtype == Subtype.Code ) {
//									ParserService.ParseFile(projectFile.Name);
//								}
//								
//							}							
//						}
//						if(bInitReferences) {
//							DefaultDotNetNodeBuilder.InitializeReferences(node, project);						
//						}
//						ProjectService.SaveCombine();						
//					}
//				}				
//			}
		}
	}
}
