// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;

namespace ICSharpCode.RubyBinding
{
	public class RubyProject : CompilableProject
	{
		public RubyProject(ProjectLoadInformation info)
			: base(info)
		{
		}
		
		public RubyProject(ProjectCreateInformation info)
			: base(info)
		{
			base.MSBuildProjectFile.AddTarget("Build");
		}
		
		/// <summary>
		/// Gets the language associated with the project.
		/// </summary>
		public override string Language {
			get { return RubyProjectBinding.LanguageName; }
		}
		
		/// <summary>
		/// Gets the language properties associated with this project.
		/// </summary>
		public override LanguageProperties LanguageProperties {
			get { return RubyLanguageProperties.Default; }
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
		
		protected override ProjectBehavior CreateDefaultBehavior()
		{
			return new RubyProjectBehavior(this, base.CreateDefaultBehavior());
		}
	}
	
	public class RubyProjectBehavior : ProjectBehavior
	{
		public RubyProjectBehavior(RubyProject project, ProjectBehavior next = null)
			: base(project, next)
		{
			
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
	}
}
