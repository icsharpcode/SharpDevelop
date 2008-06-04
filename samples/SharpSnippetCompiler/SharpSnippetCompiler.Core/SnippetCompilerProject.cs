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

		SnippetCompilerProject() : base(new Solution())
		{
			Create();			
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
		
		public const string DefaultTargetsFile = @"$(MSBuildBinPath)\Microsoft.CSharp.Targets";

		protected override void Create(ProjectCreateInformation information)
		{
			this.AddImport(DefaultTargetsFile, null);
			
			// Add import before base.Create call - base.Create will call AddOrRemoveExtensions, which
			// needs to change the import when the compact framework is targeted.
			base.Create(information);
			
			SetProperty("Debug", null, "CheckForOverflowUnderflow", "True",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "CheckForOverflowUnderflow", "False",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			
			SetProperty("Debug", null, "DefineConstants", "DEBUG;TRACE",
			            PropertyStorageLocations.ConfigurationSpecific, false);
			SetProperty("Release", null, "DefineConstants", "TRACE",
			            PropertyStorageLocations.ConfigurationSpecific, false);
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
		
		void Create()
		{
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.Solution = new Solution();
			info.OutputProjectFileName = Path.Combine(PropertyService.ConfigDirectory, "SharpSnippet.exe");
			info.ProjectName = "SharpSnippet";
			Create(info);			
			this.Parent = info.Solution;
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
