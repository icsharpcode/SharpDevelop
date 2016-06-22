// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
// </file>

using System;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public class SnippetCompilerProject : CompilableProject
	{
		static readonly string DefaultSnippetSource = "using System;\r\n\r\n" +
			"public class Program\r\n" +
			"{\r\n" +
			"\t[STAThread]\r\n" +
			"\tstatic void Main(string[] args)\r\n" +
			"\t{\r\n" +
			"\t}\r\n" +
			"}";
		
		public const string DefaultTargetsFile = @"$(MSBuildToolsPath)\Microsoft.CSharp.targets";
		
		public SnippetCompilerProject(ProjectLoadInformation loadInformation)
			: base(loadInformation)
		{
		}
		
		SnippetCompilerProject()
			: this(GetProjectCreateInfo())
		{
		}
		
		SnippetCompilerProject(ProjectCreateInformation createInfo)
			: base(createInfo)
		{
			this.Parent = createInfo.Solution;
			this.AddImport(DefaultTargetsFile, null);
			
			SetProperty("Debug", null, "CheckForOverflowUnderflow", "True", PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "CheckForOverflowUnderflow", "False", PropertyStorageLocations.ConfigurationSpecific, true);
			
			SetProperty("Debug", null, "DefineConstants", "DEBUG;TRACE", PropertyStorageLocations.ConfigurationSpecific, false);
			SetProperty("Release", null, "DefineConstants", "TRACE", PropertyStorageLocations.ConfigurationSpecific, false);
		}

		public static string SnippetFileName {
			get { return GetFullFileName("Snippet.cs"); }
		}
		
		public static string SnippetProjectFileName {
			get { return GetFullFileName("Snippet.csproj"); }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return LanguageProperties.None; }
		}
		
		public override string Language {
			get { return "C#"; }
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (string.Equals(Path.GetExtension(fileName), ".cs", StringComparison.OrdinalIgnoreCase)) {
				return ItemType.Compile;
			} else {
				return base.GetDefaultItemType(fileName);
			}
		}
		
		public static void Load()
		{
			CreateSnippetProject();
			CreateSnippetFile();
			ProjectService.LoadProject(SnippetProjectFileName);
		}
		
		static ProjectCreateInformation GetProjectCreateInfo()
		{
			return new ProjectCreateInformation {
				Solution = new Solution(new ProjectChangeWatcher(String.Empty)),
				OutputProjectFileName = Path.Combine(PropertyService.ConfigDirectory, "SharpSnippet.exe"),
				ProjectName = "SharpSnippet",
				Platform = "x86"
			};
		}
		
		/// <summary>
		/// Loads the snippet project or creates one if it does not already exist.
		/// </summary>
		static void CreateSnippetProject()
		{
			string fileName = SnippetProjectFileName;
			if (!File.Exists(fileName)) {
				
				// Add single snippet file to project.
				SnippetCompilerProject project = new SnippetCompilerProject();
				FileProjectItem item = new FileProjectItem(project, ItemType.Compile, "Snippet.cs");
				ProjectService.AddProjectItem(project, item);
				
				project.Save(fileName);
			}
		}
	
		/// <summary>
		/// Loads the snippet file or creates one if it does not already exist. 
		/// </summary>
		static void CreateSnippetFile()
		{
			string fileName = SnippetFileName;
			if (!File.Exists(fileName)) {
				LoggingService.Info("Creating Snippet.cs file: " + fileName);
				using (StreamWriter snippetFile = File.CreateText(fileName)) {
					snippetFile.Write(DefaultSnippetSource);
				}
			}
		}
		
		/// <summary>
		/// All snippet compiler files are stored loaded from the config directory so this
		/// method prefixes the filename with this path.
		/// </summary>
		public static string GetFullFileName(string fileName)
		{
			return Path.Combine(PropertyService.ConfigDirectory, fileName);
		}
	}
}
