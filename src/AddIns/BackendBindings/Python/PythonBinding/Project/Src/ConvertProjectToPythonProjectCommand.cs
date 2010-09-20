// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Converts a C# or VB.NET project to Python.
	/// </summary>
	public class ConvertProjectToPythonProjectCommand : LanguageConverter
	{
		public override string TargetLanguageName {
			get { return PythonProjectBinding.LanguageName; }
		}
		
		/// <summary>
		/// Creates an PythonProject.
		/// </summary>
		protected override IProject CreateProject(string targetProjectDirectory, IProject sourceProject)
		{
			// Add IronPython reference.
			PythonProject targetProject = (PythonProject)base.CreateProject(targetProjectDirectory, sourceProject);
			IProjectItemListProvider targetProjectItems = targetProject as IProjectItemListProvider;
			targetProjectItems.AddProjectItem(CreateIronPythonReference(targetProject));			
			return targetProject;
		}
		
		/// <summary>
		/// Converts C# and VB.NET files to Python and saves the files.
		/// </summary>
		protected override void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			NRefactoryToPythonConverter converter = NRefactoryToPythonConverter.Create(sourceItem.Include);
			if (converter != null) {
				targetItem.Include = ChangeFileExtensionToPythonFileExtension(sourceItem.Include);

				string code = GetParseableFileContent(sourceItem.FileName);
				string pythonCode = converter.Convert(code);
				
				PythonProject pythonTargetProject = (PythonProject)targetItem.Project;
				if ((converter.EntryPointMethods.Count > 0) && !pythonTargetProject.HasMainFile) {
					pythonTargetProject.AddMainFile(targetItem.Include);
					
					// Add code to call main method at the end of the file.
					pythonCode += "\r\n\r\n" + converter.GenerateMainMethodCall(converter.EntryPointMethods[0]);
				}
				
				SaveFile(targetItem.FileName, pythonCode, GetDefaultFileEncoding());
			} else {
				LanguageConverterConvertFile(sourceItem, targetItem);
			}
		}

		/// <summary>
		/// Adds the MainFile property since adding it in the CreateProject method would mean
		/// it gets removed via the base class CopyProperties method.
		/// </summary>
		protected override void CopyProperties(IProject sourceProject, IProject targetProject)
		{
			base.CopyProperties(sourceProject, targetProject);
			AddMainFile(sourceProject, (PythonProject)targetProject);
		}
		
		/// <summary>
		/// Calls the LanguageConverter class method ConvertFile which copies the source file to the target
		/// file without any modifications.
		/// </summary>
		protected virtual void LanguageConverterConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			base.ConvertFile(sourceItem, targetItem);
		}
		
		/// <summary>
		/// Writes the specified file to disk.
		/// </summary>
		protected virtual void SaveFile(string fileName, string content, Encoding encoding)
		{
			File.WriteAllText(fileName, content, encoding);
		}
		
		protected virtual Encoding GetDefaultFileEncoding()
		{
			return FileService.DefaultFileEncoding.GetEncoding();
		}
		
		/// <summary>
		/// Gets the content of the file from the parser service.
		/// </summary>
		protected virtual string GetParseableFileContent(string fileName)
		{
			return ParserService.GetParseableFileContent(fileName).Text;
		}
		
		/// <summary>
		/// Gets the project content for the specified project.
		/// </summary>
		protected virtual IProjectContent GetProjectContent(IProject project)
		{
			return ParserService.GetProjectContent(project);
		}
		
		ReferenceProjectItem CreateIronPythonReference(IProject project)
		{
			ReferenceProjectItem reference = new ReferenceProjectItem(project, "IronPython");
			reference.SetMetadata("HintPath", @"$(PythonBinPath)\IronPython.dll");
			return reference;
		}
				
		/// <summary>
		/// Adds a MainFile if the source project has a StartupObject.
		/// </summary>
		void AddMainFile(IProject sourceProject, PythonProject targetProject)
		{
			string startupObject = GetStartupObject(sourceProject);
			if (startupObject != null) {
				IClass c = FindClass(sourceProject, startupObject);
				if (c != null) {
					string fileName = FileUtility.GetRelativePath(sourceProject.Directory, c.CompilationUnit.FileName);
					targetProject.AddMainFile(ChangeFileExtensionToPythonFileExtension(fileName));
				}
			}			
		}
		
		string GetStartupObject(IProject project)
		{
			MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
			if (msbuildProject != null) {
				return msbuildProject.GetProperty(null, null, "StartupObject");
			}
			return null;
		}
		
		IClass FindClass(IProject project, string name)
		{
			return GetProjectContent(project).GetClass(name, 0);
		}
				
		/// <summary>
		/// Changes the extension to ".py"
		/// </summary>
		static string ChangeFileExtensionToPythonFileExtension(string fileName)
		{
			return Path.ChangeExtension(fileName, ".py");	
		}
	}
}
