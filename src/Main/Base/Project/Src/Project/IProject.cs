// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum OutputType {
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe}")]
		Exe,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe}")]
		WinExe,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Library}")]
		Library,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Module}")]
		Module
	}
	
	public interface IProject : ISolutionFolder, IDisposable, IMementoCapable
	{
		List<ProjectItem> Items {
			get;
		}
		
		/// <summary>
		/// Gets a list of property sections stored in the solution file.
		/// </summary>
		List<ProjectSection> ProjectSections {
			get;
		}
		
		/// <summary>
		/// Marks a project for needing recompilation.
		/// </summary>
		bool IsDirty {
			get;
			set;
		}
		string Language {
			get;
		}
		
		ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get;
		}
		
		ICSharpCode.Core.IAmbience Ambience {
			get;
		}
		
		string FileName {
			get;
			set;
		}
		string Directory {
			get;
		}
		
		string Configuration {
			get;
			set;
		}
		string Platform {
			get;
			set;
		}
		
		string AssemblyName {
			get;
			set;
		}
		
		string DocumentationFileName {
			get;
		}
		
		string OutputAssemblyFullPath {
			get;
		}
		
		string IntermediateOutputFullPath {
			get;
		}
		
		OutputType OutputType {
			get;
			set;
		}
		
		string RootNamespace {
			get;
			set;
		}
		
		string AppDesignerFolder {
			get;
			set;
		}
		
		List<string> GetConfigurationNames();
		List<string> GetPlatformNames();
		
		bool CanCompile(string fileName);
		
		void Save();
		void Save(string fileName);
		
		/// <summary>
		/// Returns true, if a specific file (given by it's name)
		/// is inside this project.
		/// </summary>
		bool IsFileInProject(string fileName);
		
		/// <summary>
		/// Returns the file content as a string which can be parsed by the parser.
		/// The fileName must be a file name in the project. This is used for files
		/// 'behind' other files or zipped file contents etc.
		/// </summary>
		string GetParseableFileContent(string fileName);
		
		bool IsStartable { get; }
		
		void Start(bool withDebugging);
		
		/// <summary>
		/// Creates a new project content for this project.
		/// This method should only be called by ParserService.LoadSolutionProjectsInternal()!
		/// </summary>
		ParseProjectContent CreateProjectContent();
		
		/// <summary>
		/// Creates a new projectItem for the passed itemType
		/// This method should only be called by ProjectItemFactory.CreateProjectItem()!
		/// </summary>
		ProjectItem CreateProjectItem(string itemType);

		void Build(MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties);
		void Rebuild(MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties);
		void Clean(MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties);
		void Publish(MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties);
	}
}
