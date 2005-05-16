using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum OutputType {
		Exe, 
		WinExe, 
		Library,
		Module
	};
	
	public interface IProject : ISolutionFolder, IDisposable, IMementoCapable
	{
		PropertyGroup BaseConfiguration {
			get;
		}
		
		List<ProjectItem> Items {
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
		}
		string Directory {
			get;
		}
		
		string Configuration {
			get;
		}
		
		string Platform {
			get;
		}
		
		string OutputAssemblyFullPath {
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
		
		PropertyGroup GetConfiguration(string configurationName);
		PropertyGroup GetConfiguration(string configurationName, string platform);
		
		PropertyGroup GetUserConfiguration(string configurationName);
		PropertyGroup GetUserConfiguration(string configurationName, string platform);
		
		
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
		
		
		void Start(bool withDebugging);
		
		CompilerResults Build();
		CompilerResults Rebuild();
		CompilerResults Clean();
		CompilerResults Publish();
	}
}
