// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.BuildEngine;

namespace ICSharpCode.RubyBinding
{
	public class RubyProject : CompilableProject
	{
		public RubyProject(IMSBuildEngineProvider provider, string fileName, string projectName)
			: base(provider)
		{
			Name = projectName;
			LoadProject(fileName);
		}
		
		public RubyProject(ProjectCreateInformation info) : base(info.Solution)
		{
			Create(info);
		}
		
		/// <summary>
		/// Gets the language associated with the project.
		/// </summary>
		public override string Language {
			get { return RubyLanguageBinding.LanguageName; }
		}
		
		/// <summary>
		/// Gets the language properties associated with this project.
		/// </summary>
		public override LanguageProperties LanguageProperties {
			get { return RubyLanguageProperties.Default; }
		}
		
		/// <summary>
		/// Returns ItemType.Compile if the filename has a Ruby extension (.rb).
		/// </summary>
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (fileName != null) {
				string extension = Path.GetExtension(fileName);
				if (extension.ToLowerInvariant() == ".rb") {
					return ItemType.Compile;
				}
			}
			return base.GetDefaultItemType(fileName);
		}
		
		public void AddMainFile(string fileName)
		{
			SetProperty(null, null, "MainFile", fileName, PropertyStorageLocations.Base, true);
		}
		
		/// <summary>
		/// Returns true if a main file is already defined for this project.
		/// </summary>
		public bool HasMainFile {
			get { return GetProperty(null, null, "MainFile") != null; }
		}
		
		protected override void Create(ProjectCreateInformation information)
		{
			base.Create(information);
			MSBuildProject.Targets.AddNewTarget("Build");
		}		
	}
}
