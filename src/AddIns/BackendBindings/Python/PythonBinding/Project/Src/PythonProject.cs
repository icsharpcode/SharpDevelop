// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents an MSBuild project that can build python code.
	/// </summary>
	public class PythonProject : CompilableProject
	{
		public const string DefaultTargetsFile = @"$(PythonBinPath)\SharpDevelop.Build.Python.targets";
		
		public PythonProject(ProjectLoadInformation info)
			: base(info)
		{
		}
		
		public PythonProject(ProjectCreateInformation info)
			: base(info)
		{
			AddImport(DefaultTargetsFile, null);
		}
		
		/// <summary>
		/// Gets the language associated with the project.
		/// </summary>
		public override string Language {
			get { return PythonProjectBinding.LanguageName; }
		}
		
		/// <summary>
		/// Gets the language properties associated with this project.
		/// </summary>
		public override LanguageProperties LanguageProperties {
			get { return PythonLanguageProperties.Default; }
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
			return new PythonProjectBehavior(this, base.CreateDefaultBehavior());
		}
	}
	
	public class PythonProjectBehavior : ProjectBehavior
	{
		public PythonProjectBehavior(PythonProject project, ProjectBehavior next = null)
			: base(project, next)
		{
			
		}
		
		/// <summary>
		/// Returns ItemType.Compile if the filename has a
		/// python extension (.py).
		/// </summary>
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (fileName != null) {
				string extension = Path.GetExtension(fileName);
				if (extension.ToLowerInvariant() == ".py") {
					return ItemType.Compile;
				}
			}
			return base.GetDefaultItemType(fileName);
		}
	}
}
