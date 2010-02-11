// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.BuildEngine;
using System.IO;

namespace FSharpBinding
{
	//Global todos
	//TODO add "compiling" dialog to output the correct directory
	//TODO copy all config to the correct locations
	//TODO add directory structure
	
	public class FSharpProject : CompilableProject
	{
		public FSharpProject(IMSBuildEngineProvider engineProvider, string fileName, string projectName) : base(engineProvider)
		{
			this.Name = projectName;
			this.LoadProject(fileName);
		}
		
		public FSharpProject(ProjectCreateInformation info) : base(info.Solution)
		{
			this.Create(info);
			try {
				this.AddImport(@"$(MSBuildExtensionsPath)\FSharp\1.0\Microsoft.FSharp.Targets", null);
			} catch (InvalidProjectFileException ex) {
				throw new ProjectLoadException("Please ensure that the F# compiler is installed on your computer.\n\n" + ex.Message, ex);
			}
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (string.Equals(".fs", Path.GetExtension(fileName), StringComparison.InvariantCultureIgnoreCase)) {
				return ItemType.Compile;
			} else if (string.Equals(".fsi", Path.GetExtension(fileName), StringComparison.InvariantCultureIgnoreCase)) {
				return ItemType.Compile;
			} else {
				return base.GetDefaultItemType(fileName);
			}
		}
		
		public override string Language {
			get {
				return "F#";
			}
		}
		
		public override LanguageProperties LanguageProperties {
			get {
				return LanguageProperties.None;
			}
		}
	}
}
