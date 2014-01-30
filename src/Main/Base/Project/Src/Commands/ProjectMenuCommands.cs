// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class ViewProjectOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			ShowProjectOptions(ProjectService.CurrentProject);
		}
		
		public static void ShowProjectOptions(IProject project)
		{
			if (project == null) {
				return;
			}
			foreach (IViewContent viewContent in SD.Workbench.ViewContentCollection) {
				ProjectOptionsView projectOptions = viewContent as ProjectOptionsView;
				if (projectOptions != null && projectOptions.Project == project) {
					projectOptions.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			try {
				AddInTreeNode projectOptionsNode = AddInTree.GetTreeNode("/SharpDevelop/BackendBindings/ProjectOptions/" + project.Language);
				ProjectOptionsView projectOptions = new ProjectOptionsView(projectOptionsNode, project);
				SD.Workbench.ShowView(projectOptions);
			} catch (TreePathNotFoundException) {
				MessageService.ShowError("${res:Dialog.ProjectOptions.NoPanelsInstalledForProject}");
			}
		}
	}
	
	public class GenerateProjectDocumentation : AbstractMenuCommand
	{
		static string[] registryKeys = new string[] {
			@"HKEY_CLASSES_ROOT\Sandcastle Help File Builder Project\shell\open\command",
			@"HKEY_CLASSES_ROOT\SandcastleBuilder.shfbproj\shell\open\command"
		};
		
		static string FindSHFB()
		{
			string envVar = Environment.GetEnvironmentVariable("SHFBROOT");
			if (!string.IsNullOrEmpty(envVar)) {
				return Path.Combine(envVar, "SandcastleBuilderGUI.exe");
			}
			foreach (string registryKey in registryKeys) {
				string fileName = FindSHFB(registryKey);
				if (fileName != null) {
					return fileName;
				}
			}
			return null;
		}
		
		static string FindSHFB(string registryKey)
		{
			string command = Registry.GetValue(registryKey, null, string.Empty) as string;
			return ExtractExecutableFromCommand(command);
		}
		
		static string ExtractExecutableFromCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
				return null;
			command = command.Trim();
			if (string.IsNullOrEmpty(command))
				return null;
			if (command[0] == '"') {
				// "program" %1
				int pos = command.IndexOf('"', 1);
				if (pos < 0)
					return null;
				return command.Substring(1, pos - 1);
			} else {
				// program %1
				int pos = command.IndexOf(' ');
				if (pos < 0)
					return command;
				else
					return command.Substring(0, pos);
			}
		}
		
		public override void Run()
		{
			CompilableProject project = ProjectService.CurrentProject as CompilableProject;
			if (project == null) {
				return;
			}
			string sandcastleHelpFileBuilderPath = FindSHFB();
			if (sandcastleHelpFileBuilderPath == null || !File.Exists(sandcastleHelpFileBuilderPath)) {
				using (ToolNotFoundDialog dlg = new ToolNotFoundDialog(
					StringParser.Parse("${res:ProjectComponent.ContextMenu.GenerateDocumentation.SHFBNotFound}"),
					"http://www.codeplex.com/SHFB/", null))
				{
					dlg.ShowDialog(SD.WinForms.MainWin32Window);
				}
				return;
			}
			
			string assembly = project.OutputAssemblyFullPath;
			string xmlDocFile = project.DocumentationFileFullPath;
			if (xmlDocFile == null) {
				MessageService.ShowMessage("${res:ProjectComponent.ContextMenu.GenerateDocumentation.NeedToEditBuildOptions}");
				return;
			}
			if (!File.Exists(assembly)) {
				MessageService.ShowMessage("${res:ProjectComponent.ContextMenu.GenerateDocumentation.ProjectNeedsToBeCompiled}");
				return;
			}
			if (!File.Exists(xmlDocFile)) {
				MessageService.ShowMessage("${res:ProjectComponent.ContextMenu.GenerateDocumentation.ProjectNeedsToBeCompiled2}");
				return;
			}
			string sandcastleHelpFileBuilderProjectFile = Path.ChangeExtension(project.FileName, ".shfbproj");
			if (!File.Exists(sandcastleHelpFileBuilderProjectFile)) {
				using (XmlTextWriter w = new XmlTextWriter(sandcastleHelpFileBuilderProjectFile, Encoding.UTF8)) {
					w.Formatting = Formatting.Indented;
					const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";
					w.WriteStartElement("Project", ns);
					w.WriteAttributeString("DefaultTargets", "Build");
					w.WriteAttributeString("ToolsVersion", "3.5");
					
					w.WriteStartElement("PropertyGroup", ns);
					w.WriteComment("The configuration and platform will be used to determine which\n" +
					               "assemblies to include from solution and project documentation\n" +
					               "sources");
					w.WriteStartElement("Configuration", ns);
					w.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
					w.WriteValue("Debug");
					w.WriteEndElement();  // </Configuration>
					
					w.WriteStartElement("Platform", ns);
					w.WriteAttributeString("Condition", " '$(Platform)' == '' ");
					w.WriteValue("AnyCPU");
					w.WriteEndElement();  // </AnyCPU>
					
					w.WriteElementString("SchemaVersion", ns, "2.0");
					w.WriteElementString("ProjectGuid", ns, Guid.NewGuid().ToString("B"));
					w.WriteElementString("SHFBSchemaVersion", ns, "1.8.0.3");
					
					w.WriteElementString("AssemblyName", ns, "Documentation");
					w.WriteElementString("RootNamespace", ns, "Documentation");
					w.WriteElementString("Name", ns, "Documentation");
					
					w.WriteElementString("OutputPath", ns, @".\Help\");
					w.WriteElementString("HtmlHelpName", ns, "Documentation");
					
					w.WriteStartElement("DocumentationSources", ns);
					w.WriteStartElement("DocumentationSource", "");
					w.WriteAttributeString("sourceFile", FileUtility.GetRelativePath(Path.GetDirectoryName(sandcastleHelpFileBuilderProjectFile), project.FileName));
					w.WriteEndElement(); // </DocumentationSource>
					w.WriteEndElement(); // </DocumentationSources>
					
					w.WriteEndElement(); // </PropertyGrup>
					
					w.WriteComment("There are no properties for these groups.  AnyCPU needs to appear in\n" +
					               "order for Visual Studio to perform the build.  The others are optional\n" +
					               "common platform types that may appear.");
					string[] confPlatList = {
						"Debug|AnyCPU", "Release|AnyCPU", "Debug|x86", "Release|x86", "Debug|x64", "Release|x64", "Debug|Win32", "Release|Win32"
					};
					foreach (string confPlat in confPlatList) {
						w.WriteStartElement("PropertyGroup", ns);
						w.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == '" + confPlat + "' ");
						w.WriteEndElement(); // </PropertyGrup>
					}
					
					w.WriteComment("Import the SHFB build targets");
					w.WriteStartElement("Import", ns);
					w.WriteAttributeString("Project", @"$(SHFBROOT)\SandcastleHelpFileBuilder.targets");
					w.WriteEndElement(); // </Import>
					
					w.WriteEndElement(); // </Project>
				}
			}
			
			ProcessStartInfo psi = new ProcessStartInfo(sandcastleHelpFileBuilderPath, '"' + sandcastleHelpFileBuilderProjectFile + '"');
			psi.WorkingDirectory = Path.GetDirectoryName(sandcastleHelpFileBuilderPath);
			psi.UseShellExecute = false;
			Process.Start(psi);
		}
	}
	
	/// <summary>
	/// Opens the projects output folder in an explorer window.
	/// </summary>
	public class OpenProjectFolder : AbstractMenuCommand
	{
		public override void Run()
		{
			IProject project = ProjectService.CurrentProject;
			if (project == null) {
				return;
			}
			
			OpenFolder.OpenFolderInExplorer(project.Directory);
		}
	}
	
	/// <summary>
	/// Opens the projects output folder in an explorer window.
	/// </summary>
	public class OpenProjectOutputFolder : AbstractMenuCommand
	{
		public override void Run()
		{
			CompilableProject project = ProjectService.CurrentProject as CompilableProject;
			if (project == null) {
				return;
			}
			
			// Explorer does not handle relative paths as a command line argument properly
			string outputFolder =  project.OutputFullPath;
			if (!Directory.Exists(outputFolder)) {
				Directory.CreateDirectory(outputFolder);
			}
			
			OpenFolder.OpenFolderInExplorer(outputFolder);
		}
	}
}
