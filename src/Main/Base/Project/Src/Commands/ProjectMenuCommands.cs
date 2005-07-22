// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class ViewProjectOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			IProject selectedProject = ProjectService.CurrentProject;
			if (selectedProject == null) {
				return;
			}
			
			try {
				AddInTreeNode projectOptionsNode = AddInTree.GetTreeNode("/SharpDevelop/BackendBindings/ProjectOptions/" + selectedProject.Language);
				Properties newProperties = new Properties();
				newProperties.Set("Project", selectedProject);
				
				ProjectOptionsView projectOptions = new ProjectOptionsView(projectOptionsNode, selectedProject);
				WorkbenchSingleton.Workbench.ShowView(projectOptions);
			} catch (TreePathNotFoundException) {
				// TODO: Translate me!
				MessageService.ShowError("No installed project options panels were found.");
			}
		}
	}
	
//	public class GenerateProjectDocumentation : AbstractMenuCommand
//	{
//		public override void Run()
//		{
//			try {
//				
//				
//				if (ProjectService.CurrentProject != null) {
//					LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(LanguageBindingService));
//					ILanguageBinding csc = languageBindingService.GetBindingPerLanguageName(ProjectService.CurrentProject.ProjectType);
//					
//					string assembly    = csc.GetCompiledOutputName(ProjectService.CurrentProject);
//					string projectFile = Path.ChangeExtension(assembly, ".ndoc");
//					if (!File.Exists(projectFile)) {
//						StreamWriter sw = File.CreateText(projectFile);
//						sw.WriteLine("<project>");
//						sw.WriteLine("    <assemblies>");
//						sw.WriteLine("        <assembly location=\""+ assembly +"\" documentation=\"" + Path.ChangeExtension(assembly, ".xml") + "\" />");
//						sw.WriteLine("    </assemblies>");
//						/*
//						sw.WriteLine("    				    <documenters>");
//						sw.WriteLine("    				        <documenter name=\"JavaDoc\">");
//						sw.WriteLine("    				            <property name=\"Title\" value=\"NDoc\" />");
//						sw.WriteLine("    				            <property name=\"OutputDirectory\" value=\".\\docs\\JavaDoc\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingReturns\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
//						sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
//						sw.WriteLine("    				            <property name=\"CopyrightHref\" value=\"\" />");
//						sw.WriteLine("    				        </documenter>");
//						sw.WriteLine("    				        <documenter name=\"MSDN\">");
//						sw.WriteLine("    				            <property name=\"OutputDirectory\" value=\".\\docs\\MSDN\" />");
//						sw.WriteLine("    				            <property name=\"HtmlHelpName\" value=\"NDoc\" />");
//						sw.WriteLine("    				            <property name=\"HtmlHelpCompilerFilename\" value=\"C:\\Program Files\\HTML Help Workshop\\hhc.exe\" />");
//						sw.WriteLine("    				            <property name=\"IncludeFavorites\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"Title\" value=\"An NDoc Documented Class Library\" />");
//						sw.WriteLine("    				            <property name=\"SplitTOCs\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DefaulTOC\" value=\"\" />");
//						sw.WriteLine("    				            <property name=\"ShowVisualBasic\" value=\"True\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
//						sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
//						sw.WriteLine("                <property name=\"CopyrightHref\" value=\"\" />");
//						sw.WriteLine("            </documenter>");
//						sw.WriteLine("    				        <documenter name=\"XML\">");
//						sw.WriteLine("    				            <property name=\"OutputFile\" value=\".\\docs\\doc.xml\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingReturns\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
//						sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
//						sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
//						sw.WriteLine("    				            <property name=\"CopyrightHref\" value=\"\" />");
//						sw.WriteLine("    				        </documenter>");
//						sw.WriteLine("    				    </documenters>");*/
//						sw.WriteLine("    				</project>");
//						sw.Close();
//					}
//					
//					string command = FileUtility.SharpDevelopRootPath +
//					Path.DirectorySeparatorChar + "bin" +
//					Path.DirectorySeparatorChar + "ndoc" +
//					Path.DirectorySeparatorChar + "NDocGui.exe";
//					string args    = '"' + projectFile + '"';
//					
//					ProcessStartInfo psi = new ProcessStartInfo(command, args);
//					psi.WorkingDirectory = FileUtility.SharpDevelopRootPath +
//					Path.DirectorySeparatorChar + "bin" +
//					Path.DirectorySeparatorChar + "ndoc";
//					psi.UseShellExecute = false;
//					Process p = new Process();
//					p.StartInfo = psi;
//					p.Start();
//				}
//			} catch (Exception) {
//				MessageBox.Show("You need to compile the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
//			}
//		}
//	}
}
