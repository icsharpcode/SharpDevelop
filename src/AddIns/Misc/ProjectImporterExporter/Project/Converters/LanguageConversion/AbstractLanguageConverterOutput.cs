// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	/// <summary>
	/// Description of AbstractLanguageConverterOutput.
	/// </summary>
	public abstract class AbstractLanguageConverterOutput : AbstractOutputConverter
	{

		protected abstract string Extension {
			get;
		}
		public override void ConvertCombine(IProgressMonitor progressMonitor,string inputCombine, string outputPath)
		{
			string inputPath = Path.GetFullPath(Path.GetDirectoryName(inputCombine));
			
			Combine combine = new Combine();
			combine.LoadCombine(inputCombine);
			
			Combine outputCombine = new Combine();
			ArrayList projects = Combine.GetAllProjects(combine);
			if (progressMonitor != null) {
				progressMonitor.BeginTask("Convert", projects.Count + 1);
			}
			foreach (ProjectCombineEntry project in projects) {
				string projectFileName  = Path.GetFullPath(Path.Combine(inputPath, project.Filename));
				string relativeFileName = projectFileName.Substring(inputPath.Length + 1);
				
				string output = TranslateProject(null, projectFileName, Path.Combine(outputPath, Path.GetDirectoryName(relativeFileName)));
				outputCombine.AddEntry(output);
				if (progressMonitor != null) {
					progressMonitor.Worked(1);
				}
			}
			
			outputCombine.SaveCombine(Path.Combine(outputPath, Path.GetFileName(inputCombine)));
			if (progressMonitor != null) {
				progressMonitor.Done();;
			}
		}
		
		public override void ConvertProject(IProgressMonitor progressMonitor,string inputProject, string outputPath)
		{
			TranslateProject(progressMonitor, inputProject, outputPath);
		}
		
		
		protected virtual IProject CreateProject(string outputPath, IProject originalProject)
		{
			return CreateProject(outputPath, originalProject, originalProject.ProjectType);
		}
		
		protected IProject CreateProject(string outputPath, IProject originalProject, string targetLanguage)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)ServiceManager.Services.GetService(typeof(LanguageBindingService));
			ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(targetLanguage);
			
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.CombinePath = outputPath;
			info.ProjectBasePath = outputPath;
			info.ProjectName = originalProject.Name + " converted";
			
			return binding.CreateProject(info, null);
		}
		
		bool CopyFile(string original, string newFile)
		{
			try {
				File.Copy(original, newFile);
			} catch(IOException) {
				return false;
			}
			return true;
		}
		
		bool SaveFile(string fileName, string content)
		{
			try {
				if (!Directory.Exists(Path.GetDirectoryName(fileName))) {
					Directory.CreateDirectory(Path.GetDirectoryName(fileName));
				}
				
				StreamWriter sw = new StreamWriter(fileName);
				sw.Write(content);
				sw.Close();
			} catch (Exception e) {
				Console.WriteLine("Error while saving file : " + e);
				return false;
			}
			return true;
		}
		
		
		protected abstract string ConvertFile(string fileName);
		
		string TranslateProject(IProgressMonitor progressMonitor, string inputProject, string outputPath)
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.ServiceManager.Services.GetService(typeof(IProjectService));
			IProject inProject = projectService.LoadProject(inputProject);
			int len = inProject.BaseDirectory.Length;
			
			IProject project      = CreateProject(outputPath, inProject);
			if (progressMonitor != null) {
				progressMonitor.BeginTask("Convert", inProject.ProjectFiles.Count + 1);
			}
			
			foreach (ProjectFile file in inProject.ProjectFiles) {
				
				if (file.BuildAction == BuildAction.EmbedAsResource) {
					string outFile;
					
					// resource files can be outside of the project path
					if(file.Name.StartsWith(outputPath)) {
						// Path.GetFilename can't be used because the filename can be
						// a relative path that shouldn't get lost
						outFile = Path.Combine(outputPath, file.Name.Substring(len + 1));
					} else {
						outFile = Path.Combine(outputPath, Path.GetFileName(file.Name));
					}
					
					if (CopyFile(file.Name, outFile)) {
						ProjectFile pf = new ProjectFile(outFile);
						pf.BuildAction = BuildAction.EmbedAsResource;
						project.ProjectFiles.Add(pf);
					}
				} else if(file.Subtype != Subtype.Directory && File.Exists(file.Name)) {
					string outPut;
					try {
						outPut = ConvertFile(file.Name);
					} catch (Exception e) {
						outPut = "Conversion Error : " + e.ToString();
					}
					
					// Path.GetFilename can't be used because the filename can be
					// a relative path that shouldn't get lost
					string outFile = Path.Combine(outputPath, file.Name.Substring(len + 1));
					outFile = Path.ChangeExtension(outFile, Extension);
					
					if (SaveFile(outFile, outPut)) {
						project.ProjectFiles.Add(new ProjectFile(outFile));
					}
				}
				if (progressMonitor != null) {
					progressMonitor.Worked(1);
				}

			}
			string output = Path.Combine(outputPath, project.Name + ".prjx");
			try {
				project.SaveProject(output);
			} catch (Exception e) {
				Console.WriteLine("Error while saving project : " + e);
				return null;
			}
			if (progressMonitor != null) {
				progressMonitor.Done();;
			}
			return output;
		}
	}
}
